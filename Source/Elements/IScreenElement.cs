namespace RAGENativeUI.Elements
{
    using System.Drawing;

    public interface IScreenElement
    {
        bool IsVisible { get; set; }
        ScreenRectangle Rectangle { get; set; }
        Color Color { get; set; }

        void Draw();
    }
}

