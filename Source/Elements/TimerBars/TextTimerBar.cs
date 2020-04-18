namespace RAGENativeUI.Elements
{
    using System;
    using System.ComponentModel;
    using System.Drawing;

    public class TextTimerBar : TimerBarBase
    {
        public static readonly TextStyle DefaultTextStyle = TextStyle.Default.With(
            scale: TB.TextSize,
            wrap: (0.0f, TB.TextWrapEnd),
            justification: TextJustification.Right,
            color: HudColorWhite
        );

        public static readonly PointF DefaultTextOffset = new PointF(0.0f, TB.TextYOffset);

        public string Text { get; set; }
        /// <summary>
        /// Gets the text style.
        /// </summary>
        public TextStyle TextStyle { get; set; } = DefaultTextStyle;
        /// <summary>
        /// Gets the position offset of the text in relative coordinates.
        /// </summary>
        public PointF TextOffset { get; set; } = DefaultTextOffset;

        public TextTimerBar(string label, string text) : base(label)
        {
            Text = text;
        }

        [Obsolete("Use TimerBarBase.Draw(float, float) instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public override void Draw(int interval)
        {
            SizeF res = UIMenu.GetScreenResolutionMantainRatio();
            Point safe = UIMenu.GetSafezoneBounds();

            base.Draw(interval);
            ResText.Draw(Text, new Point((int)res.Width - safe.X - 10, (int)res.Height - safe.Y - (42 + (4 * interval))), 0.5f, Color.White, Common.EFont.ChaletLondon, ResText.Alignment.Right, false, false, Size.Empty);
        }

        public override void Draw(float x, float y)
        {
            base.Draw(x, y);

            TextStyle.Apply();
            TextCommands.Display(Text, x + TextOffset.X, y + TextOffset.Y);
        }
    }
}

