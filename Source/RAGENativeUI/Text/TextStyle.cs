namespace RAGENativeUI.Text
{
#if RPH1
    extern alias rph1;
    using Vector2 = rph1::Rage.Vector2;
#else
    /** REDACTED **/
#endif

    using System.Drawing;

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

    public class TextStyle
    {
        public Color Color { get; set; }
        public float Scale { get; set; }
        public TextFont Font { get; set; }
        public TextAlignment Alignment { get; set; }
        public bool DropShadow { get; set; }
        public bool Outline { get; set; }
        public float WrapWidth { get; set; }

        /// <summary>
        /// Gets the height of a line of text based on the <see cref="Scale"/> and the
        /// <see cref="Font"/>.
        /// </summary>
        public float Height => N.GetTextScaleHeight(Scale, (int)Font);

        public void Apply() => Apply(Vector2.Zero);
        public void Apply(Vector2 position)
        {
            N.SetTextFont((int)Font);
            N.SetTextScale(1.0f, Scale);
            N.SetTextColour(Color.R, Color.G, Color.B, Color.A);

            if (DropShadow)
            {
                N.SetTextDropShadow();
            }

            if (Outline)
            {
                N.SetTextOutline();
            }

            N.SetTextJustification((int)Alignment);

            if (WrapWidth > 0.0f)
            {
                float w = WrapWidth;
                switch (Alignment)
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
            else if (Alignment == TextAlignment.Right)
            {
                N.SetTextWrap(0.0f, position.X);
            }
        }
    }
}

