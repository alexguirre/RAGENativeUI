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
            Draw(Size.Empty);
        }

        public virtual void Draw(Size offset)
        {
            if (!Enabled)
                return;

            Draw(Caption, new Point(Position.X + offset.Width, Position.Y + offset.Height), Scale, Color, FontEnum, Centered);
        }

        public static void Draw(string caption, Point position, float scale, Color color, Common.EFont font, bool centered)
        {
            float x = position.X / 1280.0f;
            float y = position.Y / 720.0f;

            NativeFunction.Natives.SetTextFont((int)font);
            NativeFunction.Natives.SetTextScale(scale, scale);
            NativeFunction.Natives.SetTextColour(color.R, color.G, color.B, color.A);
            NativeFunction.Natives.SetTextCentre(centered);
            NativeFunction.Natives.BeginTextCommandDisplayText("STRING");
            NativeFunction.Natives.AddTextComponentSubstringPlayerName(caption);
            NativeFunction.Natives.EndTextCommandDisplayText(x, y);
        }
    }
}