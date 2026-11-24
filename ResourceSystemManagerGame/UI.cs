using System;

namespace RSM;

public abstract class UI
{
    public abstract void Render();
}

public class TextLabel : UI
{
    public string Text {get;set;}
    public override void Render()
    {
        Console.WriteLine(this.Text);
    }


    public TextLabel(string text)
    {
        this.Text = text;
    }
}

public class Button :TextLabel
{
    public Button(string text) : base(text)
    {
    }

    public bool IsChoosed{get;set;} = false;

    public override void Render()
    {
        Console.Write(this.Text);
    }

    public void Choose()
    {
        this.IsChoosed = true;
        this.Text = String.Format("> {0}", this.Text);
    }
    public void UnChoose()
    {
        this.IsChoosed = false;
        this.Text = this.Text.Replace("> ", "");
    }

    public void OnClick(Player player, Action<Player> action)
    {
        if (this.IsChoosed){
        action(player);
        }
    }
}