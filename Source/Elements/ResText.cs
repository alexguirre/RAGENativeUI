using System;
using System.Drawing;
using Rage;
using Rage.Native;

namespace RAGENativeUI.Elements
{
    /// <summary>
    /// A Text object in the 1080 pixels height base system.
    /// </summary>
    public class ResText : Text
    {
        public enum Alignment
        {
            Left,
            Centered,
            Right,
        }


        public Alignment TextAlignment { get; set; }
        public bool DropShadow { get; set; }
        public bool Outline { get; set; }
        public Size WordWrap { get; set; }

        public ResText(string caption, Point position, float scale) : base(caption, position, scale)
        {
            TextAlignment = Alignment.Left;
        }

        public ResText(string caption, Point position, float scale, Color color) : base(caption, position, scale, color)
        {
            TextAlignment = Alignment.Left;
        }

        public ResText(string caption, Point position, float scale, Color color, Common.EFont font, Alignment justify) : base(caption, position, scale, color, font, false)
        {
            TextAlignment = justify;
        }

        public override void Draw()
        {
            Draw(Size.Empty);
        }

        public override void Draw(Size offset)
        {
            if(!Enabled)
                return;

            Draw(Caption, new Point(Position.X + offset.Width, Position.Y + offset.Height), Scale, Color, FontEnum, TextAlignment, DropShadow, Outline, WordWrap);
        }

        public new static void Draw(string caption, Point position, float scale, Color color, Common.EFont font, bool centered)
        {
            int screenw = Game.Resolution.Width;
            int screenh = Game.Resolution.Height;

            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            var width = height * ratio;

            float x = (position.X) / width;
            float y = (position.Y) / height;

            NativeFunction.CallByName<uint>("SET_TEXT_FONT", (int)font);
            NativeFunction.CallByName<uint>("SET_TEXT_SCALE", 1.0f, scale);
            NativeFunction.CallByName<uint>("SET_TEXT_COLOUR", color.R, color.G, color.B, color.A);

            NativeFunction.CallByHash<uint>(0x25fbb336df1804cb, "jamyfafi");      // _SET_TEXT_ENTRY
            AddLongString(caption);


            NativeFunction.CallByHash<uint>(0xcd015e5bb0d96a57, x, y);     // _DRAW_TEXT
        }

        public static void Draw(string caption, Point position, float scale, Color color, Common.EFont font, Alignment textAlignment, bool dropShadow, bool outline, Size wordWrap)
        {
            int screenw = Game.Resolution.Width;
            int screenh = Game.Resolution.Height;

            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            var width = height * ratio;

            float x = (position.X) / width;
            float y = (position.Y) / height;

            NativeFunction.CallByName<uint>("SET_TEXT_FONT", (int)font);
            NativeFunction.CallByName<uint>("SET_TEXT_SCALE", 1.0f, scale);
            NativeFunction.CallByName<uint>("SET_TEXT_COLOUR", color.R, color.G, color.B, color.A);

            if (dropShadow)
            {
                NativeFunction.CallByName<uint>("SET_TEXT_DROP_SHADOW");
            }

            if (outline)
            {
                NativeFunction.CallByName<uint>("SET_TEXT_OUTLINE");
            }

            switch (textAlignment)
            {
                case Alignment.Centered:
                    NativeFunction.CallByName<uint>("SET_TEXT_CENTRE", true);
                    break;
                case Alignment.Right:
                    NativeFunction.CallByName<uint>("SET_TEXT_RIGHT_JUSTIFY", true);
                    NativeFunction.CallByName<uint>("SET_TEXT_WRAP", 0, x);
                    break;
            }

            if (wordWrap != new Size(0, 0))
            {
                float xsize = (position.X + wordWrap.Width) / width;
                NativeFunction.CallByName<uint>("SET_TEXT_WRAP", x, xsize);
            }

            NativeFunction.CallByHash<uint>(0x25fbb336df1804cb, "jamyfafi");      // _SET_TEXT_ENTRY
            AddLongString(caption);


            NativeFunction.CallByHash<uint>(0xcd015e5bb0d96a57, x, y);     // _DRAW_TEXT
        }


        public static float MeasureStringWidth(string str, Common.EFont font, float scale)
        {
            int screenw = Game.Resolution.Width;
            int screenh = Game.Resolution.Height;
            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            float width = height * ratio;
            return MeasureStringWidthNoConvert(str, font, scale) * width;
        }

        public static float MeasureStringWidthNoConvert(string str, Common.EFont font, float scale)
        {
            NativeFunction.CallByHash<ulong>(0x54ce8ac98e120cab, "STRING");
            AddLongString(str);
            return NativeFunction.CallByHash<float>(0x85f061da64ed2f67, (int)font) * scale;
        }

        /// <summary>
        /// Push a long string into the stack.
        /// </summary>
        /// <param name="str"></param>
        public static void AddLongString(string str)
        {
            const int strLen = 99;
            for (int i = 0; i < str.Length; i += strLen)
            {
                string substr = str.Substring(i, Math.Min(strLen, str.Length - i));
                NativeFunction.CallByHash<uint>(0x6c188be134e074aa, substr);      // _ADD_TEXT_COMPONENT_STRING
            }
        }
    }
}
