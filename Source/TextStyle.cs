namespace RAGENativeUI
{
    using System;
    using System.Drawing;

    // TODO: decide if we keep Common.EFont or we make it obsolete and replace it with this enum
    public enum TextFont
    {
        ChaletLondon = 0,
        HouseScript = 1,
        Monospace = 2,
        ChaletComprimeCologne = 4,
        Pricedown = 7
    }

    public enum TextJustification
    {
        /// <summary>
        /// When used, the midpoint of the text is located at the drawing position.
        /// </summary>
        Center = 0,

        /// <summary>
        /// When used, the left side of the text is located at the drawing position.
        /// </summary>
        Left = 1,

        /// <summary>
        /// When used, the right side of the text is located at the border of the wrapping bounds.
        /// </summary>
        Right = 2,
    }

    /// <summary>
    /// Defines the appearance of text.
    /// </summary>
    public struct TextStyle : IEquatable<TextStyle>
    {
        private TextFont font;
        private float scale;
        private float? characterHeight;

        /// <summary>
        /// Gets or sets the font of the text.
        /// </summary>
        public TextFont Font
        {
            readonly get => font;
            set
            {
                if (value != font)
                {
                    font = value;
                    characterHeight = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Gets or sets the size of the text.
        /// </summary>
        public float Scale
        {
            readonly get => scale;
            set
            {
                if (value != scale)
                {
                    scale = value;
                    characterHeight = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets how the text is aligned.
        /// </summary>
        public TextJustification Justification { get; set; }

        /// <summary>
        /// Gets or sets the bounds where the text will be wrapped, in relative coordinates along the X-axis.
        /// </summary>
        public (float Start, float End) Wrap { get; set; }

        /// <summary>
        /// Gets or sets whether a drop shadow is applied to the text.
        /// </summary>
        public bool DropShadow { get; set; }

        /// <summary>
        /// Gets or sets whether an black outline is applied to the text.
        /// </summary>
        public bool Outline { get; set; }

        /// <summary>
        /// Gets the height of a character in relative coordinates. It depends on the <see cref="Font"/> and the <see cref="Scale"/>.
        /// </summary>
        public float CharacterHeight => characterHeight ?? (characterHeight = N.GetTextHeight(Scale, (int)Font)).Value;

        private TextStyle(TextFont font, Color color, float scale, TextJustification justification, 
                          (float Start, float End) wrap, bool dropShadow, bool outline, float? characterHeight = null)
            : this()
        {
            Font = font;
            Color = color;
            Scale = scale;
            Justification = justification;
            Wrap = wrap;
            DropShadow = dropShadow;
            Outline = outline;
            this.characterHeight = characterHeight;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextStyle"/> structure.
        /// </summary>
        /// <param name="font">Determines the font of the text.</param>
        /// <param name="color">Determines the color of the text.</param>
        /// <param name="scale">Determines the size of the text.</param>
        /// <param name="justification">Determines how the text is aligned.</param>
        /// <param name="wrapStart">Determines the start of the bounds where the text will be wrapped, in relative coordinates along the X-axis.</param>
        /// <param name="wrapEnd">Determines the end of the bounds where the text will be wrapped, in relative coordinates along the X-axis.</param>
        /// <param name="dropShadow">Determines whether a drop shadow is applied to the text.</param>
        /// <param name="outline">Determines whether a black outline is applied to the text.</param>
        /// <remarks>
        /// Note, for creating your own styles, the recommended way is to derive them from a base <see cref="TextStyle"/>, for example <see cref="Default"/>.
        /// With this method you only have to specify the properties you want to change.
        /// The following snippet shows how to give the <see cref="Default"/> style a dark red color and a drop shadow.
        /// <code lang="C#">
        /// TextStyle myStyle = TextStyle.Default.With(color: Color.FromArgb(120, 0, 0), dropShadow: true);
        /// </code>
        /// </remarks>
        public TextStyle(
            TextFont font,
            Color color,
            float scale = 1.12f,
            TextJustification justification = TextJustification.Left,
            float wrapStart = 0.0f, float wrapEnd = 1.0f,
            bool dropShadow = false,
            bool outline = false)
            : this(font, color, scale, justification, (wrapStart, wrapEnd), dropShadow, outline)
        {

        }

        /// <summary>
        /// Applies this style for the next text command.
        /// </summary>
        public readonly void Apply()
        {
            if (Internals.CTextStyle.ScriptStyleAvailable)
            {
                ref var s = ref Internals.CTextStyle.ScriptStyle;
                s.Font = (int)Font;
                s.Color = Color.ToArgb();
                s.Scale = Scale;
                s.Justification = (byte)Justification;
                s.Wrap = Wrap;
                s.Outline = Outline;
                s.DropShadow = DropShadow;
            }
            else
            {
                N.SetTextFont((int)Font);
                N.SetTextColour(Color.R, Color.G, Color.B, Color.A);
                N.SetTextScale(0.0f, Scale);
                N.SetTextJustification((int)Justification);
                N.SetTextWrap(Wrap.Start, Wrap.End);

                if (Outline)
                {
                    N.SetTextOutline();
                }

                if (DropShadow)
                {
                    N.SetTextDropShadow();
                }
            }
        }

        /// <summary>
        /// Creates a copy of this style with the specified properties changed.
        /// For each parameter, if it is <c>null</c> the copy keeps the value of the corresponding property of this style, otherwise, it uses the new value.
        /// <para>
        /// For creating your own styles, the recommended way is to derive them from a base <see cref="TextStyle"/>, for example <see cref="Default"/>.
        /// With this method you only have to specify the properties you want to change using named arguments.
        /// The following snippet shows how to give the <see cref="Default"/> style a dark red color and a drop shadow.
        /// <code lang="C#">
        /// TextStyle myStyle = TextStyle.Default.With(color: Color.FromArgb(120, 0, 0), dropShadow: true);
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="font">If specified, determines the font of the text.</param>
        /// <param name="color">If specified, determines the color of the text.</param>
        /// <param name="scale">If specified, determines the size of the text.</param>
        /// <param name="justification">If specified, determines how the text is aligned.</param>
        /// <param name="wrap">If specified, determines the bounds where the text will be wrapped, in relative coordinates along the X-axis.</param>
        /// <param name="dropShadow">If specified, determines whether a drop shadow is applied to the text.</param>
        /// <param name="outline">If specified, determines whether a black outline is applied to the text.</param>
        /// <returns>A copy of this <see cref="TextStyle"/> with the specified properties changed.</returns>
        public readonly TextStyle With(
            TextFont? font = null,
            Color? color = null,
            float? scale = null,
            TextJustification? justification = null,
            (float Start, float End)? wrap = null,
            bool? dropShadow = null,
            bool? outline = null)
        {
            TextFont newFont = font ?? Font;
            float newScale = scale ?? Scale;
            return new TextStyle(
                font: newFont,
                color: color ?? Color,
                scale: newScale,
                justification: justification ?? Justification,
                wrap: wrap ?? Wrap,
                dropShadow: dropShadow ?? DropShadow,
                outline: outline ?? Outline,
                characterHeight: (newFont != Font || newScale != Scale) ? null : characterHeight
            );
        }

        /// <inheritdoc/>
        public override readonly string ToString() => (Font, Color, Scale, Justification, Wrap, DropShadow, Outline).ToString();

        /// <inheritdoc/>
        public override readonly int GetHashCode() => (Font, Color, Scale, Justification, Wrap, DropShadow, Outline).GetHashCode();

        /// <inheritdoc/>
        public override readonly bool Equals(object other) => other is TextStyle s && Equals(s);

        /// <inheritdoc/>
        public readonly bool Equals(TextStyle other)
        {
            return Font == other.Font &&
                   Color == other.Color &&
                   Scale == other.Scale &&
                   Justification == other.Justification &&
                   Wrap == other.Wrap &&
                   DropShadow == other.DropShadow &&
                   Outline == other.Outline;
        }

        /// <summary>
        /// Represents the default text style.
        /// </summary>
        public static readonly TextStyle Default = new TextStyle(
            font: TextFont.ChaletLondon,
            color: Color.FromArgb(0xFF, 0xE1, 0xE1, 0xE1)
        );

        /// <summary>
        /// Gets or sets the text style currently applied. The setter is equivalent to <see cref="Apply"/>.
        /// </summary>
        public static TextStyle Current
        {
            get
            {
                if (Internals.CTextStyle.ScriptStyleAvailable)
                {
                    ref var s = ref Internals.CTextStyle.ScriptStyle;
                    return new TextStyle(
                        (TextFont)s.Font,
                        Color.FromArgb(s.Color),
                        s.Scale,
                        (TextJustification)s.Justification,
                        s.Wrap.Start,
                        s.Wrap.End,
                        s.DropShadow,
                        s.Outline
                        );
                }
                else
                {
                    return Default;
                }
            }
            set => value.Apply();
        }
    }
}
