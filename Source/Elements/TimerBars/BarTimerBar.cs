namespace RAGENativeUI.Elements
{
    using System;
    using System.ComponentModel;
    using System.Drawing;

    public class BarTimerBar : TimerBarBase
    {
        // TODO: markers (see timerbar_lines)

        /// <summary>
        /// Bar percentage. Goes from 0 to 1.
        /// </summary>
        public float Percentage { get; set; }

        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }

        public BarTimerBar(string label) : base(label)
        {
            N.GetHudColour(8, out int bR, out int bG, out int bB, out int bA); // HUD_COLOUR_REDDARK
            N.GetHudColour(6, out int fR, out int fG, out int fB, out int fA); // HUD_COLOUR_RED

            BackgroundColor = Color.FromArgb(bA, bR, bG, bB);
            ForegroundColor = Color.FromArgb(fA, fR, fG, fB);
        }

        [Obsolete("Use TimerBarBase.Draw(float, float) instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public override void Draw(int interval)
        {
            SizeF res = UIMenu.GetScreenResolutionMantainRatio();
            Point safe = UIMenu.GetSafezoneBounds();

            base.Draw(interval);

            var start = new Point((int)res.Width - safe.X - 160, (int)res.Height - safe.Y - (28 + (4 * interval)));

            ResRectangle.Draw(start, new Size(150, 15), BackgroundColor);
            ResRectangle.Draw(start, new Size((int)(150 * Percentage), 15), ForegroundColor);
        }

        public override void Draw(float x, float y)
        {
            base.Draw(x, y);

            x += TB.BarXOffset;
            y += TB.BarYOffset;

            float w = TB.BarWidth;
            float h = TB.BarHeight;

            N.DrawRect(x, y, w, h, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B, BackgroundColor.A);

            float fillW = w * Percentage;
            float fillX = x - w * 0.5f + fillW * 0.5f;
            N.DrawRect(fillX, y, fillW, h, ForegroundColor.R, ForegroundColor.G, ForegroundColor.B, ForegroundColor.A);
        }
    }
}

