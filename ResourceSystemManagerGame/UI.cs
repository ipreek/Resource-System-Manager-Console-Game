using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace RSM;

public abstract class UI
{
    public abstract void Render();
}

public class TextLabel : UI
{
    public string Text {get;set;}
    public string RenderedText{get;set;}
    public override void Render()
    {
        Console.WriteLine(this.RenderedText);
    }


    public TextLabel(string text)
    {
        this.Text = text;
        RenderedText = Text;
    }
}

public class Button :TextLabel
{
    public Button(string text, Action<Player> action) : base(text)
    {
        PlayerAction += action;
    }

    public Action<Player> PlayerAction {get;set;}
    public bool IsChoosed{get;set;} = false;

    public override void Render()
    {
        Console.Write(this.RenderedText);
    }

    public void Choose()
    {
        this.IsChoosed = true;
        this.RenderedText = String.Format("> {0}", this.Text);
    }
    public void UnChoose()
    {
        this.IsChoosed = false;
        this.RenderedText = Text;
    }
    public void OnClick(Player player)
    {
        if (this.IsChoosed){
        PlayerAction(player);
        }
    }
}

public class UiList<T>
{
    public static int LengthOfUiDimension;
    public T UiElement {get;set;}
    private List<List<TextLabel>> Dimensions {get;set;} = new List<List<TextLabel>>();

    public UiList(params TextLabel[] uiElements)
    {
        int index = 0;
        int dims = -1;
        int paramLength = uiElements.Count();

        int dimAmmount = paramLength/LengthOfUiDimension;

        for (int i = 0; i<=dimAmmount; i++){
            List<TextLabel> localList = new List<TextLabel>();
            this.Dimensions.Add(localList);
        }

        foreach(TextLabel element in uiElements)
        {
            if (index % LengthOfUiDimension != 0)
            {
                this.Dimensions[dims].Add(element);
            }
            else
            {
                dims++;
                this.Dimensions[dims].Add(element);
            }
            index++;
        }
    }

    public TextLabel GetElement(int dim, int element) => this.Dimensions[dim][element];

    public int GetIndex(int dim, TextLabel element) => this.Dimensions[dim].FindIndex(e => e.Equals(element));

    public int GetDimension(TextLabel element)
    {   int defaultDim = -1;
        foreach(List<TextLabel> localList in this.Dimensions)
        {
            if(localList.Contains(element))
                return this.Dimensions.FindIndex(l => l.Equals(localList));
        }
        return defaultDim;
    }

    public int GetIndex(TextLabel element)
    {
        int dim = GetDimension(element);
        return this.Dimensions[dim].FindIndex(e => e.Equals(element));
    }

    public void RenderAll()
    {
        int sum =0;
        foreach(List<TextLabel> list in Dimensions)
        {
            Console.Write("\n");
            foreach(TextLabel tl in list)
            {
                sum++;
                string newText = String.Format($"{sum}.{tl.Text} ");
                if ((tl as Button).IsChoosed == false){
                tl.RenderedText = newText;}
                tl.Render();
            }
        }
    }

    public void Move(Player player, out Button? button)
    {
        Pointer MovingPointer = new Pointer();
        Button? selectedButton = (Button)Dimensions[0][0];
        Button? lastButton = (Button)Dimensions[0][0];
        button = null;
        bool breakTheCycle = false;

        try{
        MovingPointer.SelectButton(Dimensions, ref selectedButton);
        }
        catch
        {
            button = null;
        }
        while(true){
            #if !DEBUG
            Console.Clear();
            #endif
            RenderAll();
            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.DownArrow:
                    if(MovingPointer.Y != (Dimensions.Count-1) && (Dimensions[MovingPointer.Y+1].Count-1 >= MovingPointer.X)){
                        MovingPointer.Y++;
                    }
                break;
                case ConsoleKey.RightArrow:
                    if(MovingPointer.X == (Dimensions[MovingPointer.Y].Count-1) && MovingPointer.Y != (Dimensions.Count-1))
                    {
                        MovingPointer.X=0;
                        MovingPointer.Y++;
                    }else if(MovingPointer.X == (Dimensions[MovingPointer.Y].Count-1) && MovingPointer.Y == (Dimensions.Count - 1))
                    {
                        MovingPointer.X=0;
                        MovingPointer.Y=0;
                    }
                    else
                    {
                        MovingPointer.X++;
                    }
                break;
                case ConsoleKey.LeftArrow:
                    if(MovingPointer.X==0 && MovingPointer.Y == 0)
                    {
                        MovingPointer.Y = Dimensions.Count-1;
                        MovingPointer.X = Dimensions[MovingPointer.Y].Count-1;
                    }else if(MovingPointer.X==0 && MovingPointer.Y != 0)
                    {
                        MovingPointer.Y--;
                        MovingPointer.X = Dimensions[MovingPointer.Y].Count-1;
                    }
                    else
                    {
                        MovingPointer.X--;
                    }
                break;
                case ConsoleKey.UpArrow:
                    if(MovingPointer.Y != 0)
                    {
                        MovingPointer.Y--;
                    }
                break;
                case ConsoleKey.Enter:
                    if(MovingPointer.SelectedButton != null)
                        {
                            button = MovingPointer.SelectedButton;
                            breakTheCycle = true;
                        }
                break; 
            }
            lastButton?.UnChoose();
            MovingPointer.SelectButton(Dimensions, ref selectedButton);
            selectedButton?.Choose();
            lastButton = (selectedButton != null) ? selectedButton : null;
            if(breakTheCycle == true)
            {
                break;
            }
            
        }
    }

}

public class Pointer
{
    public int Y {get;set;}
    public int X {get;set;}

    public Button SelectedButton {get;set;}

    public Pointer()
    {
        this.Y=0;
        this.X=0;
    }

    public void SelectButton(IList<List<TextLabel>> list, ref Button element)
    {
        SelectedButton = (Button)list[Y][X];
        SelectedButton.Choose();
        element = SelectedButton;
    }
}