namespace RAGENativeUI.Elements
{
    using System;
    using System.ComponentModel;
    using System.Drawing;

    // TimerBar Constants
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

        internal const float BarXOffset = 0.118f; // == (((((0.919f - 0.081f) + 0.028f) + 0.05f) - 0.001f) - 0.002f) - TimerBarManager.InitialX
        internal const float BarYOffset = ((((0.013f - 0.002f) + 0.001f) + 0.001f) - 0.001f);
        internal const float BarWidth = 0.069f;
        internal const float BarHeight = 0.011f;
    }

    public abstract class TimerBarBase
    {
        public static readonly TextStyle DefaultLabelStyle = TextStyle.Default.With(
            scale: TB.LabelSize,
            wrap: default((float, float)),
            justification: TextJustification.Right,
            color: HudColorWhite
        );

        public static readonly PointF DefaultLabelOffset = new PointF(0.0f, 0.0f);

        // TODO: HighlightColor property
        // TODO: public SmallHeight property
        internal bool SmallHeight => this is BarTimerBar;

        // TODO: remove this when we have some method for accesing HUD colors
        internal static Color HudColorWhite
        {
            get
            {
                N.GetHudColour(1, out int r, out int g, out int b, out int a);
                return Color.FromArgb(a, r, g, b);
            }
        }

        public string Label { get; set; }
        /// <summary>
        /// Gets the label style.
        /// <para>
        /// Note, if the property <see cref="TextStyle.Wrap"/> is set to <c>default((float, float))</c> its value is ignored
        /// and the appropriate wrap bounds are calculated when drawing the timer bar.
        /// </para>
        /// </summary>
        public TextStyle LabelStyle { get; set; } = DefaultLabelStyle;
        /// <summary>
        /// Gets the position offset of the label in relative coordinates.
        /// </summary>
        public PointF LabelOffset { get; set; } = DefaultLabelOffset;

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

        public virtual void Draw(float x, float y)
        {
            DrawBackground(x, y);
            DrawLabel(x, y);
        }

        private void DrawBackground(float x, float y)
        {
            x += TB.BgXOffset;
            y += SmallHeight ? TB.BgSmallYOffset : TB.BgDefaultYOffset;

            float w = TB.BgWidth;
            float h = SmallHeight ? TB.BgSmallHeight : TB.BgDefaultHeight;

            N.DrawSprite(TB.BgTextureDictionary, TB.BgTextureName, x, y, w, h, 0.0f, 255, 255, 255, 140);
        }

        private void DrawLabel(float x, float y)
        {
            LabelStyle.Apply();
            if (LabelStyle.Wrap == default)
            {
                float wrapEnd = TB.LabelInitialWrapEnd;
                if (!N.GetIsWidescreen())
                {
                    wrapEnd -= 0.02f;
                }
                wrapEnd = wrapEnd - (0.03f * WrapEndMultiplier);

                N.SetTextWrap(0.0f, wrapEnd);
            }

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

