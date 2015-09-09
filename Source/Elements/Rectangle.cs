using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
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
            Draw(new Size());
        }
        public virtual void Draw(Size offset)
        {
            if (!this.Enabled) return;

            float w = Size.Width / 1280;
            float h = Size.Height / 720;
            float x = ((Position.X + offset.Width) / 1280) + w * 0.5f;
            float y = ((Position.Y + offset.Height) / 720) + h * 0.5f;

            NativeFunction.CallByName<uint>("DRAW_RECT", x, y, w, h, (int)Color.R, (int)Color.G, (int)Color.B, (int)Color.A);
        }
    }
}
