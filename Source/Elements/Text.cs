namespace RAGENativeUI.Elements
{
    using System;
    using System.Drawing;
    using System.Collections.Generic;

    using Rage;
    using Rage.Native;

    using RAGENativeUI.Utility;

    public class Text
    {
        public bool IsVisible { get; set; }
        public float Scale { get; set; }
        private List<string> captionSplitted = new List<string>();
        private string caption;
        public string Caption
        {
            get { return caption; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException($"The text {nameof(Caption)} can't be null.");
                if (value == caption)
                    return;
                caption = value;

                captionSplitted.Clear();

                const int MaxStringLenth = 99;

                for (int i = 0; i < caption.Length; i += MaxStringLenth)
                {
                    string str = caption.Substring(i, Math.Min(MaxStringLenth, caption.Length - i));
                    captionSplitted.Add(str);
                }
            }
        }
        public Color Color { get; set; }
        public GameScreenPosition Position { get; set; }
        public TextFont Font { get; set; }
        public TextAlignment Alignment { get; set; } = TextAlignment.Left;
        public float WrapWidth { get; set; }
        public bool DropShadow { get; set; }
        public bool Outline { get; set; }

        public Text(string caption, GameScreenPosition position, float scale, Color color)
        {
            Caption = caption;
            Position = position;
            Scale = scale;
            Color = color;
        }

        public Text(string caption, GameScreenPosition position, float scale) : this(caption, position, scale, Color.White)
        {
        }

        public void Draw()
        {
            if (!IsVisible)
                return;

            Draw(Position, captionSplitted, Scale, Color, Font, Alignment, WrapWidth, DropShadow, Outline);
        }


        public static void Draw(GameScreenPosition position, string caption, float scale, Color color, TextFont font, TextAlignment alignment, float wrapWidth, bool dropShadow, bool outline)
        {
            const int MaxStringLenth = 99;
            List<string> captionSplitted = new List<string>((int)Math.Ceiling(caption.Length / (double)MaxStringLenth));

            for (int i = 0; i < caption.Length; i += MaxStringLenth)
            {
                string str = caption.Substring(i, Math.Min(MaxStringLenth, caption.Length - i));
                captionSplitted.Add(str);
            }

            Draw(position, captionSplitted, scale, color, font, alignment, wrapWidth, dropShadow, outline);
        }

        public static void Draw(GameScreenPosition position, List<string> captionSplitted, float scale, Color color, TextFont font, TextAlignment alignment, float wrapWidth, bool dropShadow, bool outline)
        {
            NativeFunction.Natives.SetTextFont((int)font);
            NativeFunction.Natives.SetTextScale(1.0f, scale);
            NativeFunction.Natives.SetTextColour(color.R, color.G, color.B, color.A);

            if (dropShadow)
            {
                NativeFunction.Natives.SetTextDropShadow();
            }

            if (outline)
            {
                NativeFunction.Natives.SetTextOutline();
            }

            NativeFunction.Natives.SetTextJustification((int)alignment);

            if (wrapWidth > 0.0f)
            {
                float w = wrapWidth;
                switch (alignment)
                {
                    case TextAlignment.Center:
                        NativeFunction.Natives.SetTextWrap(position.X - (w / 2.0f), position.X + (w  / 2.0f));
                        break;
                    case TextAlignment.Left:
                        NativeFunction.Natives.SetTextWrap(position.X, position.X + w);
                        break;
                    case TextAlignment.Right:
                        NativeFunction.Natives.SetTextWrap(position.X - w, position.X);
                        break;
                }
            }
            else if(alignment == TextAlignment.Right)
            {
                NativeFunction.Natives.SetTextWrap(0.0f, position.X);
            }

            NativeFunction.Natives.BeginTextCommandDisplayText("CELL_EMAIL_BCON");

            for (int i = 0; i < captionSplitted.Count; i++)
            {
                NativeFunction.Natives.AddTextComponentSubstringPlayerName(captionSplitted[i]);
            }

            NativeFunction.Natives.EndTextCommandDisplayText(position.X, position.Y);
        }
    }

    public enum TextAlignment
    {
        Center,
        Left,
        Right,
    }

    public enum TextFont
    {
        ChaletLondon = 0,
        HouseScript = 1,
        Monospace = 2,
        ChaletComprimeCologne = 4,
        Pricedown = 7
    }
}

