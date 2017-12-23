using System.Drawing;
using Rage;
using Rage.Native;

namespace RAGENativeUI.Elements
{
    /// <summary>
    /// A rectangle in 1080 pixels height system.
    /// </summary>
    public class ResRectangle : Rectangle
    {
        public ResRectangle()
        {
        }

        public ResRectangle(Point position, Size size) : base(position, size)
        { 
        }

        public ResRectangle(Point position, Size size, Color color) : base(position, size, color)
        { 
        }

        public override void Draw()
        {
            Draw(Size.Empty);
        }

        public override void Draw(Size offset)
        {
            if (!Enabled)
                return;

            Draw(new Point(Position.X + offset.Width, Position.Y + offset.Height), Size, Color);
        }

        public new static void Draw(Point position, Size size, Color color)
        {
            int screenw = Game.Resolution.Width;
            int screenh = Game.Resolution.Height;
            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            var width = height * ratio;

            float w = size.Width / width;
            float h = size.Height / height;
            float x = (position.X / width) + w * 0.5f;
            float y = (position.Y / height) + h * 0.5f;

            NativeFunction.Natives.DrawRect(x, y, w, h, color.R, color.G, color.B, color.A);
        }
    }
}
