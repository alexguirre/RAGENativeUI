using System.Drawing;
using Rage.Native;

namespace RAGENativeUI.Elements
{
    public class Rectangle : IElement
    {
        public Size Size { get; set; }
        public virtual bool Enabled { get; set; }
        public virtual Point Position { get; set; }
        public virtual Color Color { get; set; }
     
        public Rectangle()
        {
            this.Enabled = true;
            this.Position = new Point();
            this.Size = new Size(1280, 720);
            this.Color = Color.Transparent;
        }
        public Rectangle(Point position, Size size)
        {
            this.Enabled = true;
            this.Position = position;
            this.Size = size;
            this.Color = Color.Transparent;
        }
        public Rectangle(Point position, Size size, Color color)
        {
            this.Enabled = true;
            this.Position = position;
            this.Size = size;
            this.Color = color;
        }

        public virtual void Draw()
        {
            Draw(Size.Empty);
        }

        public virtual void Draw(Size offset)
        {
            if (!Enabled)
                return;

            Draw(new Point(Position.X + offset.Width, Position.Y + offset.Height), Size, Color);
        }

        public static void Draw(Point position, Size size, Color color)
        {
            float w = size.Width / 1280.0f;
            float h = size.Height / 720.0f;
            float x = (position.X / 1280.0f) + w * 0.5f;
            float y = (position.Y / 720.0f) + h * 0.5f;

            NativeFunction.Natives.DrawRect(x, y, w, h, color.R, color.G, color.B, color.A);
        }
    }
}
