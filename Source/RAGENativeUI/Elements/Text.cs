namespace RAGENativeUI.Elements
{
    using System;
    using System.Drawing;
    using System.Collections.Generic;
    
    public class Text
    {
        // "CELL_EMAIL_BCON" translates to "~a~~a~~a~~a~~a~~a~~a~~a~~a~~a~", 
        // even though it has 10 substring placeholders ('~a~') only 4 are used
        // (the classes that handle the DISPLAY_TEXT command are CDisplayTextZeroOrOneNumbers, 
        // CDisplayTextOneSubstring and CDisplayTextFourSubstringsThreeNumbers, so this limit is obvious).
        // Each substring max length is 99 + 1 (null terminating char), 
        // so the max string length that can be displayed in one command is 99 * 4 = 396 chars
        private const string DisplayTextFormat = "CELL_EMAIL_BCON";
        private const int MaxSubstringsCount = 4;
        private const int MaxSubstringLength = 99;

        private ScreenRectangle rectangle;
        private readonly List<string> captionSubstrings = new List<string>();
        private string caption;

        public bool IsVisible { get; set; } = true;
        // Width = wrapWidth, Height = ignored
        public ScreenRectangle Rectangle { get => rectangle; set => rectangle = value; }
        public ScreenPosition Position
        {
            get => ScreenPosition.FromRelativeCoords(rectangle.X, rectangle.Y);
            set => rectangle = ScreenRectangle.FromRelativeCoords(value.X, value.Y, rectangle.Width, rectangle.Height);
        }
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

                AddCaptionSubstrings(caption, captionSubstrings);
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

            Draw(Position, captionSubstrings, Scale, Color, Font, Alignment, WrapWidth, DropShadow, Outline);
        }


        public static void Draw(ScreenPosition position, string caption, float scale, Color color, TextFont font, TextAlignment alignment, float wrapWidth, bool dropShadow, bool outline)
        {
            Throw.IfNull(caption, nameof(caption));

            BeginDraw(position, scale, color, font, alignment, wrapWidth, dropShadow, outline);

            for (int i = 0, c = 0; i < caption.Length && c < MaxSubstringsCount; i += MaxSubstringLength, c++)
            {
                string str = caption.Substring(i, Math.Min(MaxSubstringLength, caption.Length - i));
                N.AddTextComponentSubstringPlayerName(str);
            }

            EndDraw(position);
        }

        public static void Draw(ScreenPosition position, List<string> captionSubstrings, float scale, Color color, TextFont font, TextAlignment alignment, float wrapWidth, bool dropShadow, bool outline)
        {
            Throw.IfNull(captionSubstrings, nameof(captionSubstrings));

            BeginDraw(position, scale, color, font, alignment, wrapWidth, dropShadow, outline);

            int c = Math.Min(captionSubstrings.Count, MaxSubstringsCount);
            for (int i = 0; i < c; i++)
            {
                N.AddTextComponentSubstringPlayerName(captionSubstrings[i]);
            }
            
            EndDraw(position);
        }

        private static void BeginDraw(ScreenPosition position, float scale, Color color, TextFont font, TextAlignment alignment, float wrapWidth, bool dropShadow, bool outline)
        {
            N.SetTextFont((int)font);
            N.SetTextScale(1.0f, scale);
            N.SetTextColour(color.R, color.G, color.B, color.A);

            if (dropShadow)
            {
                N.SetTextDropShadow();
            }

            if (outline)
            {
                N.SetTextOutline();
            }

            N.SetTextJustification((int)alignment);

            if (wrapWidth > 0.0f)
            {
                float w = wrapWidth;
                switch (alignment)
                {
                    case TextAlignment.Center:
                        N.SetTextWrap(position.X - (w * 0.5f), position.X + (w * 0.5f));
                        break;
                    case TextAlignment.Left:
                        N.SetTextWrap(position.X, position.X + w);
                        break;
                    case TextAlignment.Right:
                        N.SetTextWrap(position.X - w, position.X);
                        break;
                }
            }
            else if (alignment == TextAlignment.Right)
            {
                N.SetTextWrap(0.0f, position.X);
            }
            
            N.BeginTextCommandDisplayText(DisplayTextFormat);
        }

        private static void EndDraw(ScreenPosition position)
        {
            N.EndTextCommandDisplayText(position.X, position.Y);
        }
        
        private static void AddCaptionSubstrings(string caption, List<string> to)
        {
            to.Clear();
            for (int i = 0; i < caption.Length; i += MaxSubstringLength)
            {
                string str = caption.Substring(i, Math.Min(MaxSubstringLength, caption.Length - i));
                to.Add(str);
            }
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
        ChaletLondonFixedWidthNumbers = 5,
        Pricedown = 7
    }
}

