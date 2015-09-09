// Added by alexguirre

using System.Drawing;
// using GTA;
// using GTA.Native;
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

        public override void Draw(Size offset)
        {
            if (!Enabled) return;
            int screenw = Game.Resolution.Width;
            int screenh = Game.Resolution.Height;
            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            var width = height * ratio;

            float w = Size.Width / width;
            float h = Size.Height / height;
            float x = ((Position.X + offset.Width) / width) + w * 0.5f;
            float y = ((Position.Y + offset.Height) / height) + h * 0.5f;

            NativeFunction.CallByName<uint>("DRAW_RECT", x, y, w, h, (int)Color.R, (int)Color.G, (int)Color.B, (int)Color.A);
        }
    }
}
