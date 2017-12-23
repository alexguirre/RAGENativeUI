using System.Collections.Generic;
using System.Drawing;
using Rage;
using Rage.Native;

namespace RAGENativeUI.Elements
{
    public class Container : Rectangle
    {
        private List<IElement> _mItems = new List<IElement>();
        public List<IElement> Items { get { return this._mItems; } set { this._mItems = value; } }

        public Container()
            : base()
        {

        }
        public Container(Point pos, Size size)
            : base(pos, size)
        {
        }
        public Container(Point pos, Size size, Color color)
            : base(pos, size, color)
        {
        }

        public override void Draw()
        {
            Draw(new Size());
        }

        public override void Draw(Size offset)
        {
            if (!this.Enabled) return;

            int screenw = Game.Resolution.Width;
            int screenh = Game.Resolution.Height;
            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            var width = height * ratio;

            float w = Size.Width / width;
            float h = Size.Height / height;
            float x = ((Position.X + offset.Width) / width) + w * 0.5f;
            float y = ((Position.Y + offset.Height) / height) + h * 0.5f;

            NativeFunction.Natives.DrawRect(x, y, w, h, (int)Color.R, (int)Color.G, (int)Color.B, (int)Color.A);

            foreach (IElement item in this.Items)
            {
                item.Draw((Size)(this.Position + offset));
            }
        }
    }
}
