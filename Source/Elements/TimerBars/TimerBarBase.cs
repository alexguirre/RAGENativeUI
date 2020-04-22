namespace RAGENativeUI.Elements
{
    using System;
    using System.ComponentModel;
    using System.Drawing;

    /// <summary>
    /// Internal timer bars constants.
    /// </summary>
    internal static class TB
    {
        // Most constants are from the game scripts

        internal const float InitialX = 0.795f;
        internal const float InitialY = 0.925f - 0.002f;
        internal const float LoadingPromptYOffset = 0.036f;

        internal const string BgTextureDictionary = "timerbars";
        internal const string BgTextureName = "all_black_bg";
        internal const string BgHighlightTextureName = "all_white_bg";
        internal const float BgXOffset = 0.079f;
        internal const float BgDefaultYOffset = 0.008f;
        internal const float BgSmallYOffset = 0.012f;
        internal const float BgWidth = 0.157f;
        internal const float BgDefaultHeight = 0.036f;
        internal const float BgSmallHeight = 0.028f;
        internal const float DefaultHeightWithGap = ((0.025f + 0.006f) + 0.0009f) + 0.008f;
        internal const float SmallHeightWithGap = ((0.025f + 0.006f) + 0.0009f);

        internal const float LabelInitialWrapEnd = ((((0.88f - 0.062f) + 0.026f) + 0.027f) + 0.03f) - 0.034f;
        internal const float LabelScale = 0.202f;
        internal const float LabelSize = 0.288f;

        internal const float TextYOffset = ((((-0.01f - 0.005f) + 0.004f) - 0.001f) + 0.001f);
        internal const float TextWrapEnd = (((((0.95f - 0.047f) + 0.001f) + 0.047f) - 0.002f) + 0.001f);
        internal const float TextScale = 0.332f;
        internal const float TextSize = ((((((0.469f + 0.096f) - 0.017f) + 0.022f) - 0.062f) - 0.001f) - 0.013f);

        internal const float BarXOffset = 0.118f; // == (((((0.919f - 0.081f) + 0.028f) + 0.05f) - 0.001f) - 0.002f) - InitialX
        internal const float BarYOffset = ((((0.013f - 0.002f) + 0.001f) + 0.001f) - 0.001f);
        internal const float BarWidth = 0.069f;
        internal const float BarHeight = 0.011f;

        internal const float AccentXOffset = BgWidth - AccentWidth * 0.5f; // 0.951f;
        internal const float AccentWidth = 0.002f;

        internal const string MarkersTextureDictionary = "timerbar_lines";
        /// <summary>
        /// We use the texture with the marker at 50% because the game only has textures for every 10% interval,
        /// but here we support any percentage and the 50% texture is the easiest one to offset.
        /// </summary>
        internal const string MarkerTextureName = "linemarker50_128";

        internal const string CheckpointTextureDictionary = "timerbars";
        internal const string CheckpointTextureName = "circle_checkpoints";
        internal const string CrossTextureDictionary = "cross";
        internal const string CrossTextureName = "circle_checkpoints_cross";

        internal const float CheckpointXOffset = 0.1495f; // == ((((((((0.919f - 0.081f) + 0.004f) - 0.006f) + 0.05f) - 0.001f) - 0.005f) + 0.065f) - 0.0005f) - InitialX
        internal const float CheckpointYOffset = ((((0.013f - 0.002f) + 0.001f) + 0.001f) - 0.001f);
        internal const float CheckpointWidth = 0.012f;
        internal const float CheckpointHeight = 0.023f;
        internal const float CheckpointPadding = 0.0094f;

        internal const float IconXOffset = 0.145f + 0.001f;
        internal const float IconYOffset = 0.016f * 0.5f;
        internal const float IconWidth = 0.016f + 0.003f;
        internal const float IconHeight = 0.032f + 0.004f;
    }

    /// <summary>
    /// Defines the base class for timer bars.
    /// </summary>
    public abstract class TimerBarBase
    {
        /// <summary>
        /// Represents the default value of <see cref="LabelStyle"/>.
        /// </summary>
        public static readonly TextStyle DefaultLabelStyle = TextStyle.Default.With(
            scale: TB.LabelSize,
            wrap: (0.0f, TB.LabelInitialWrapEnd),
            justification: TextJustification.Right,
            color: HudColorWhite
        );

        /// <summary>
        /// Represents the default value of <see cref="LabelOffset"/>.
        /// </summary>
        public static readonly PointF DefaultLabelOffset = new PointF(0.0f, 0.0f);

        // TODO: remove this when we have some method for accesing HUD colors
        internal static Color HudColorWhite
        {
            get
            {
                N.GetHudColour(1, out int r, out int g, out int b, out int a);
                return Color.FromArgb(a, r, g, b);
            }
        }

        private string label;

        /// <summary>
        /// Gets or sets the label. This is the <see cref="string"/> that appears at the left side of the timer bar.
        /// </summary>
        /// <exception cref="ArgumentNullException"><c>value</c> is <c>null</c>.</exception>
        public string Label
        {
            get => label;
            set => label = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets or sets the label style.
        /// </summary>
        /// <remarks>
        /// Note, the <c>End</c> value of the property <see cref="TextStyle.Wrap"/> is slightly modified
        /// when applying the style based on the current aspect ratio.
        /// </remarks>
        public TextStyle LabelStyle { get; set; } = DefaultLabelStyle;

        /// <summary>
        /// Gets or sets the position offset of the label in relative coordinates.
        /// </summary>
        public PointF LabelOffset { get; set; } = DefaultLabelOffset;

        /// <summary>
        /// Gets or sets the current highlight color. If not <c>null</c>, the timer bar background is drawn with an overlay of the specified color.
        /// </summary>
        public Color? Highlight { get; set; }

        /// <summary>
        /// Gets or sets the current accent color. If not <c>null</c>, a thin line of the specified color is drawn at the right border of the timer bar.
        /// </summary>
        public Color? Accent { get; set; }

        /// <summary>
        /// Gets or sets the height of this timer bar.
        /// If <c>false</c>, it uses the default height (like <see cref="TextTimerBar"/>),
        /// otherwise, it uses a slightly smaller height (like <see cref="BarTimerBar"/>).
        /// </summary>
        protected internal bool Thin { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerBarBase"/> class.
        /// </summary>
        /// <param name="label">A <see cref="string"/> that will appear at the left side of the timer bar.</param>
        /// <exception cref="ArgumentNullException"><paramref name="label"/> is <c>null</c>.</exception>
        public TimerBarBase(string label)
        {
            Label = label;
        }

        [Obsolete("Use TimerBarBase.Draw(float, float) instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void Draw(int interval)
        {
            SizeF res = UIMenu.GetScreenResolutionMantainRatio();
            Point safe = UIMenu.GetSafezoneBounds();

            ResText.Draw(Label, new Point((int)res.Width - safe.X - 180, (int)res.Height - safe.Y - (30 + (4 * interval))), 0.3f, Color.White, Common.EFont.ChaletLondon, ResText.Alignment.Right, false, false, Size.Empty);
            Sprite.Draw("timerbars", "all_black_bg", new Point((int)res.Width - safe.X - 298, (int)res.Height - safe.Y - (40 + (4 * interval))), new Size(300, 37), 0f, Color.FromArgb(180, 255, 255, 255));
        }

        /// <summary>
        /// Draws this timer bar at the specified position.
        /// </summary>
        /// <param name="x">The X-position in relative coordinates.</param>
        /// <param name="y">The Y-position in relative coordinates.</param>
        public virtual void Draw(float x, float y)
        {
            DrawBackground(x, y);
            DrawLabel(x, y);
        }

        private void DrawBackground(float x, float y)
        {
            x += TB.BgXOffset;
            y += Thin ? TB.BgSmallYOffset : TB.BgDefaultYOffset;

            float w = TB.BgWidth;
            float h = Thin ? TB.BgSmallHeight : TB.BgDefaultHeight;
            /*
             0.028f -> 0.012f
             0.036f -> 0.008f
             */
            if (Highlight.HasValue)
            {
                Color c = Highlight.Value;
                N.DrawSprite(TB.BgTextureDictionary, TB.BgHighlightTextureName, x, y, w, h, 0.0f, c.R, c.G, c.B, c.A);
            }

            N.DrawSprite(TB.BgTextureDictionary, TB.BgTextureName, x, y, w, h, 0.0f, 255, 255, 255, 140);

            if (Accent.HasValue)
            {
                Color c = Accent.Value;
                N.DrawRect(x - TB.BgXOffset + TB.AccentXOffset, y, TB.AccentWidth, h, c.R, c.G, c.B, c.A);
            }
        }

        private void DrawLabel(float x, float y)
        {
            var wrap = LabelStyle.Wrap;
            if (!N.GetIsWidescreen())
            {
                wrap.End -= 0.02f;
            }
            wrap.End -= 0.03f * WrapEndMultiplier;

            LabelStyle.With(wrap: wrap).Apply();
            TextCommands.Display(Label, x + LabelOffset.X, y + LabelOffset.Y);
        }

        private static float WrapEndMultiplier
        {
            get
            {
                float aspectRatio = N.GetAspectRatio(false);
                N.GetActiveScreenResolution(out int screenWidth, out int screenHeight);
                float screenRatio = (float)screenWidth / screenHeight;
                aspectRatio = Math.Min(aspectRatio, screenRatio);
                if (screenRatio > 3.5f && aspectRatio > 1.7f)
                {
                    return 0.4f;
                }
                else if (aspectRatio > 1.7f)
                {
                    return 0.0f;
                }
                else if (aspectRatio > 1.5f)
                {
                    return 0.2f;
                }
                else if (aspectRatio > 1.3f)
                {
                    return 0.3f;
                }
                else
                {
                    return 0.4f;
                }
            }
        }
    }
}

