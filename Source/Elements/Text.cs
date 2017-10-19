namespace RAGENativeUI.Elements
{
    using System;
    using System.Drawing;
    using System.Collections.Generic;
    
    using Rage.Native;

    public class Text : IScreenElement
    {
        private ScreenRectangle rectangle;
        private List<string> splittedCaption = new List<string>();
        private string caption;

        public bool IsVisible { get; set; } = true;
        // Width = wrapWidth, Height = ignored
        public ScreenRectangle Rectangle { get => rectangle; set => rectangle = value; }
        public ScreenPosition Position { get => ScreenPosition.FromRelativeCoords(rectangle.X, rectangle.Y); set => rectangle = ScreenRectangle.FromRelativeCoords(value.X, value.Y, rectangle.Width, rectangle.Height); }
        public Color Color { get; set; }
        public float Scale { get; set; }
        public string Caption
        {
            get { return caption; }
            set
            {
                Throw.IfNull(value, nameof(value));

                if (value == caption)
                    return;
                caption = value;

                splittedCaption.Clear();

                foreach (string str in GetSplittedCaption(caption))
                {
                    splittedCaption.Add(str);
                }
            }
        }
        public TextFont Font { get; set; }
        public TextAlignment Alignment { get; set; }
        public bool DropShadow { get; set; }
        public bool Outline { get; set; }
        public float WrapWidth { get => rectangle.Width; set => rectangle = ScreenRectangle.FromRelativeCoords(rectangle.X, rectangle.Y, value, rectangle.Height); }

        public Text(string caption, ScreenPosition position, float scale, Color color)
        {
            Throw.IfNull(caption, nameof(caption));

            Caption = caption;
            Position = position;
            Scale = scale;
            Color = color;
        }

        public Text(string caption, ScreenPosition position, float scale) : this(caption, position, scale, Color.White)
        {
        }

        public void Draw()
        {
            if (!IsVisible)
                return;

            Draw(Position, splittedCaption, Scale, Color, Font, Alignment, WrapWidth, DropShadow, Outline);
        }


        public static void Draw(ScreenPosition position, string caption, float scale, Color color, TextFont font, TextAlignment alignment, float wrapWidth, bool dropShadow, bool outline)
        {
            Throw.IfNull(caption, nameof(caption));

            BeginDraw(position, scale, color, font, alignment, wrapWidth, dropShadow, outline);

            foreach(string str in GetSplittedCaption(caption))
            {
                NativeFunction.Natives.AddTextComponentSubstringPlayerName(str);
            }

            EndDraw(position);
        }

        public static void Draw(ScreenPosition position, List<string> splittedCaption, float scale, Color color, TextFont font, TextAlignment alignment, float wrapWidth, bool dropShadow, bool outline)
        {
            Throw.IfNull(splittedCaption, nameof(splittedCaption));

            BeginDraw(position, scale, color, font, alignment, wrapWidth, dropShadow, outline);

            for (int i = 0; i < splittedCaption.Count; i++)
            {
                NativeFunction.Natives.AddTextComponentSubstringPlayerName(splittedCaption[i]);
            }
            
            EndDraw(position);
        }

        private static void BeginDraw(ScreenPosition position, float scale, Color color, TextFont font, TextAlignment alignment, float wrapWidth, bool dropShadow, bool outline)
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
                        NativeFunction.Natives.SetTextWrap(position.X - (w / 2.0f), position.X + (w / 2.0f));
                        break;
                    case TextAlignment.Left:
                        NativeFunction.Natives.SetTextWrap(position.X, position.X + w);
                        break;
                    case TextAlignment.Right:
                        NativeFunction.Natives.SetTextWrap(position.X - w, position.X);
                        break;
                }
            }
            else if (alignment == TextAlignment.Right)
            {
                NativeFunction.Natives.SetTextWrap(0.0f, position.X);
            }

            NativeFunction.Natives.BeginTextCommandDisplayText("CELL_EMAIL_BCON");
        }

        private static void EndDraw(ScreenPosition position)
        {
            NativeFunction.Natives.EndTextCommandDisplayText(position.X, position.Y);
        }
        
        private static IEnumerable<string> GetSplittedCaption(string caption)
        {
            const int MaxStringLenth = 99;

            for (int i = 0; i < caption.Length; i += MaxStringLenth)
            {
                string str = caption.Substring(i, Math.Min(MaxStringLenth, caption.Length - i));
                yield return str;
            }
        }
    }

    public enum TextAlignment
    {
        Left,
        Center,
        Right,
    }

    public enum TextFont
    {
        ChaletLondon = 0,
        HouseScript = 1,
        Monospace = 2,
        ChaletComprimeCologne = 4,
        ChaletLondonFixedWidthNumbers = 5,
        Pricedown = 7
    }
}

