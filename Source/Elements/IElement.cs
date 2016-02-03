using System.Drawing;

namespace RAGENativeUI.Elements
{
    public interface IElement
    {
        void Draw();
        void Draw(Size offset);

        bool Enabled { get; set; }
        Point Position { get; set; }
        Color Color { get; set; }
    }
}