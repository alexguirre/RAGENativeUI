using System;
using System.Drawing;
using Rage.Native;

namespace RAGENativeUI.Elements
{

    public class Text : IElement
    {
        public float Scale { get; set; }
        public string Caption { get; set; }
        public bool Centered { get; set; }
        public Common.EFont FontEnum { get; set; }
        public Font Font { get { return new Font(FontEnum.ToString(), Scale); } }
        public virtual bool Enabled { get; set; }
        public virtual Point Position { get; set; }
        public virtual Color Color { get; set; }

        public Text(string caption, Point position, float scale)
	    {
		    this.Enabled = true;
		    this.Caption = caption;
		    this.Position = position;
		    this.Scale = scale;
		    this.Color = Color.WhiteSmoke;
            this.FontEnum = Common.EFont.ChaletLondon;
		    this.Centered = false;
	    }

        public Text(string caption, Point position, float scale, Color color)
	    {
            Enabled = true;
		    Caption = caption;
		    Position = position;
		    Scale = scale;
		    Color = color;
            FontEnum = Common.EFont.ChaletLondon;
		    Centered = false;
        }

        public Text(string caption, Point position, float scale, Color color, Common.EFont font, bool centered)
	    {
            Enabled = true;
		    Caption = caption;
		    Position = position;
		    Scale = scale;
		    Color = color;
		    FontEnum = font;
		    Centered = centered;
        }

        public virtual void Draw()
        {
            Draw(new Size());
        }

        public virtual void Draw(Size offset)
        {
            if (!this.Enabled) return;

            float x = (this.Position.X + offset.Width) / 1280;
            float y = (this.Position.Y + offset.Height) / 720;

            NativeFunction.CallByName<uint>("SET_TEXT_FONT", (int)this.FontEnum);
            NativeFunction.CallByName<uint>("SET_TEXT_SCALE", this.Scale, this.Scale);
            NativeFunction.CallByName<uint>("SET_TEXT_COLOUR", (int)this.Color.R, (int)this.Color.G, (int)this.Color.B, (int)this.Color.A);
            NativeFunction.CallByName<uint>("SET_TEXT_CENTRE", this.Centered);
            NativeFunction.CallByHash<uint>(0x25fbb336df1804cb, "STRING"); // SetTextEntry native
            NativeFunction.CallByHash<uint>(0x6c188be134e074aa, this.Caption); // AddTextComponentString native
            NativeFunction.CallByHash<uint>(0xcd015e5bb0d96a57, x, y); // DrawText native
        }
    }
}
