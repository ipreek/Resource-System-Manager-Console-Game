using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks.Dataflow;

namespace RSM;

public interface IRenderable
{
    public void Render();
}
public class TextLabel : IRenderable 
{
    public string Text {get;set;}
    public string RenderedText{get;set;}
    public List<object> paramList {get;set;} = new();
    public void Render()
    {
        foreach(object obj in paramList)
        {   
            RenderedText +=  obj;
        }
        Console.Write(this.RenderedText);
    }


    public TextLabel(string locDict, params object[] parametrs)
    {
        string getloc = Loc.Dict[locDict];
        foreach(object obj in parametrs)
        {
            paramList.Add(obj);
        }
        this.Text = getloc;
        RenderedText = Text;
    }
}

public delegate Player getPlayer();

public class Button :TextLabel, IRenderable
{
    public Button(string text, Action<object> action, params object[] listParams) : base(text,listParams)
    {
        PlayerAction += action;
    }
    public Button(string text, getPlayer action, params object[] listParams) : base(text,listParams)
    {
        PlayerFunc += action;
    }

    public Button(string text, Action action, params object[] listParams) : base(text, listParams)
    {
        anonymAction += action;
    }

    public Button(string text, params object[] listParams) : base(text,listParams){}
    public Action<object> PlayerAction {get;set;} = null;
    public getPlayer PlayerFunc {get;set;}  = null;
    public Action anonymAction {get;set;} = null;
    public bool IsChoosed{get;set;} = false;

    public void Render()
    {
        foreach(object obj in this.paramList)
        {
            RenderedText += String.Format(", {0}", obj);
        }
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
    public void OnClick(object obj)
    {
        if (this.IsChoosed){
            if(PlayerAction != null)
                PlayerAction(obj);
        }
    }

    public void OnClick(out Player player)
    {
        player = null;
        if(this.IsChoosed){
        if(PlayerFunc!=null)
            player = PlayerFunc();
        }
    }

    public void OnClick()
    {
        if (this.IsChoosed)
        {
            if(anonymAction != null)
                anonymAction();
        }
    }
}

public class UiList<T>
{
    public static int LengthOfUiDimension;
    public T UiElement {get;set;}
    private List<List<TextLabel>> Dimensions {get;set;} = new List<List<TextLabel>>();

    public UiList(bool verticalRender, params TextLabel[] uiElements)
    {
        int index = 0;
        int dims = 0;
        int paramLength = uiElements.Count();
        int dimAmmount = 0;
        if (verticalRender == false){
            dimAmmount = paramLength/LengthOfUiDimension;
        }
        else
        {
            dimAmmount = paramLength-1;
        }

        for (int i = 0; i<=dimAmmount; i++){
            List<TextLabel> localList = new List<TextLabel>();
            this.Dimensions.Add(localList);
        }

        foreach(TextLabel element in uiElements)
        {
            if (element is not Button)
            {
                this.Dimensions[0].Add(element);
            }else{
            if (index % LengthOfUiDimension != 0)
            {
                this.Dimensions[dims].Add(element);
            }
            else
            {
                dims++;
                this.Dimensions[dims].Add(element);
            }
            if(verticalRender==false){index++;}
            }
        }
            int currentLeng = Dimensions[0].Count;
            for(int i =0; i< currentLeng; i++)
            {
                if(Dimensions[0][i] is InputField inputField)
                {
                    Dimensions[0].RemoveAt(i);
                    Dimensions[0].Add(inputField);
                    break;
                }
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
            {   if(tl is Button){
                sum++;
                string newText = String.Format($"{sum}.{tl.Text} ");
                if ((tl as Button).IsChoosed == false){
                tl.RenderedText = newText;}
                tl.Render();
                
                }
                else
                {
                    tl.Render(); Console.Write("\n");
                }
            }
        }
    }

    public void Move(out Button? button)
    {
        Pointer MovingPointer = new Pointer();
        Button? selectedButton = (Button)Dimensions[1][0];
        Button? lastButton = (Button)Dimensions[1][0];
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
            if(Console.KeyAvailable){
            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.DownArrow:
                    if(MovingPointer.Y != (Dimensions.Count-1) && (Dimensions[MovingPointer.Y].Count-1 >= MovingPointer.X)){
                        MovingPointer.Y++;
                    }
                break;
                case ConsoleKey.RightArrow:
                    if(MovingPointer.X == (Dimensions[MovingPointer.Y].Count-1) && MovingPointer.Y != (Dimensions.Count-1))
                    {
                        MovingPointer.X=0;
                        MovingPointer.Y++;
                    }else if(MovingPointer.X == (Dimensions[MovingPointer.Y].Count-1) && MovingPointer.Y == (Dimensions.Count-1))
                    {
                        MovingPointer.X=0;
                        MovingPointer.Y=1;
                    }
                    else
                    {
                        MovingPointer.X++;
                    }
                break;
                case ConsoleKey.LeftArrow:
                    if(MovingPointer.X==0 && MovingPointer.Y == 1)
                    {
                        MovingPointer.Y = Dimensions.Count-1;
                        MovingPointer.X = Dimensions[MovingPointer.Y].Count-1;
                    }else if(MovingPointer.X==0 && MovingPointer.Y != 1)
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
                    if(MovingPointer.Y != 1)
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
            Thread.Sleep(50);
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
        this.Y=1;
        this.X=0;
    }

    public void SelectButton(IList<List<TextLabel>> list, ref Button element)
    {
        SelectedButton = (Button)list[Y][X];
        SelectedButton.Choose();
        element = SelectedButton;
    }
}

public class InputField : TextLabel, IRenderable
{

    public object Input {get;set;}
    public InputField (string text, out object input) : base(text)
    {
        input = null;
        this.Input = null;
        this.Text = text;
        this.RenderedText = text;
    }

    public void Render()
    {
        Console.WriteLine(RenderedText);
        this.Input = Console.ReadLine();
    }
}