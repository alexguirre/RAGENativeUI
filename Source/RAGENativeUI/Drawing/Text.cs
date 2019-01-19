namespace RAGENativeUI.Drawing
{
#if RPH1
    extern alias rph1;
    using Vector2 = rph1::Rage.Vector2;
#else
    /** REDACTED **/
#endif

    using System;
    using System.Drawing;
    using System.Collections.Generic;
    
    public class Text
    {
        private string label;

        public bool IsVisible { get; set; } = true;
        public Vector2 Position { get; set; }
        public Color Color { get; set; }
        public float Scale { get; set; }
        public string Label
        {
            get { return label; }
            set
            {
                Throw.IfNull(value, nameof(value));

                if (value == label)
                    return;
                label = value;
            }
        }
        public List<TextComponent> Components { get; } = new List<TextComponent>();
        public TextFont Font { get; set; }
        public TextAlignment Alignment { get; set; }
        public bool DropShadow { get; set; }
        public bool Outline { get; set; }
        public float WrapWidth { get; set; }

        public Text(string label, Vector2 position, float scale, Color color)
        {
            Throw.IfNull(label, nameof(label));

            Label = label;
            Position = position;
            Scale = scale;
            Color = color;
        }

        public Text(string label, Vector2 position, float scale) : this(label, position, scale, Color.White)
        {
        }

        public Text(Vector2 position, float scale) : this(String.Empty, position, scale, Color.White)
        {
        }

        public void SetText(string text)
        {
            Throw.IfNull(text, nameof(text));

            const string ShortStringFormat = "STRING";
            const string LongStringFormat = "CELL_EMAIL_BCON";
            const int MaxSubstringLength = 99;

            Components.Clear();
            if (text.Length <= MaxSubstringLength)
            {
                Label = ShortStringFormat;
                Components.Add(new TextComponentString { Value = text });
            }
            else
            {
                Label = LongStringFormat;
                for (int i = 0; i < text.Length; i += MaxSubstringLength)
                {
                    string str = text.Substring(i, Math.Min(MaxSubstringLength, text.Length - i));
                    Components.Add(new TextComponentString { Value = str });
                }
            }

        }

        public void Draw()
        {
            if (!IsVisible)
                return;

            Draw(Label, Components, Position, Scale, Color, Font, Alignment, WrapWidth, DropShadow, Outline);
        }


        public static void Draw(string label, Vector2 position, float scale, Color color, TextFont font, TextAlignment alignment, float wrapWidth, bool dropShadow, bool outline)
        {
            Throw.IfNull(label, nameof(label));
            Draw(label, null, position, scale, color, font, alignment, wrapWidth, dropShadow, outline);
        }

        public static void Draw(string label, List<TextComponent> components, Vector2 position, float scale, Color color, TextFont font, TextAlignment alignment, float wrapWidth, bool dropShadow, bool outline)
        {
            Throw.IfNull(label, nameof(label));

            BeginDraw(label, position, scale, color, font, alignment, wrapWidth, dropShadow, outline);
            
            if (components != null && components.Count > 0)
            {
                foreach (TextComponent comp in components)
                {
                    if (comp != null)
                    {
                        comp.Push();
                    }
                }
            }
            
            EndDraw(position);
        }

        private static void BeginDraw(string label, Vector2 position, float scale, Color color, TextFont font, TextAlignment alignment, float wrapWidth, bool dropShadow, bool outline)
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
            
            N.BeginTextCommandDisplayText(label);
        }

        private static void EndDraw(Vector2 position)
        {
            N.EndTextCommandDisplayText(position.X, position.Y);
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

