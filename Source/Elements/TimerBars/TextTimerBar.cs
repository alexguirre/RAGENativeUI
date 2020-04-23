namespace RAGENativeUI.Elements
{
    using System;
    using System.ComponentModel;
    using System.Drawing;

    /// <summary>
    /// Represents a timer bar with text located at the right side.
    /// </summary>
    public class TextTimerBar : TimerBarBase
    {
        /// <summary>
        /// Represents the default value of <see cref="TextStyle"/>.
        /// </summary>
        public static readonly TextStyle DefaultTextStyle = TextStyle.Default.With(
            scale: TB.TextSize,
            wrap: (0.0f, TB.TextWrapEnd),
            justification: TextJustification.Right,
            color: HudColor.White.GetColor()
        );

        /// <summary>
        /// Represents the default value of <see cref="TextOffset"/>.
        /// </summary>
        public static readonly PointF DefaultTextOffset = new PointF(0.0f, TB.TextYOffset);

        private string text;

        /// <summary>
        /// Gets or sets the text. This is the <see cref="string"/> that appears at the right side of the timer bar.
        /// </summary>
        /// <exception cref="ArgumentNullException"><c>value</c> is <c>null</c>.</exception>
        public string Text
        {
            get => text;
            set => text = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets or sets the text style.
        /// </summary>
        public TextStyle TextStyle { get; set; } = DefaultTextStyle;

        /// <summary>
        /// Gets or sets the position offset of the text in relative coordinates.
        /// </summary>
        public PointF TextOffset { get; set; } = DefaultTextOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextTimerBar"/> class.
        /// </summary>
        /// <param name="label">A <see cref="string"/> that will appear at the left side of the timer bar.</param>
        /// <param name="text">A <see cref="string"/> that will appear at the right side of the timer bar.</param>
        /// <exception cref="ArgumentNullException"><paramref name="label"/> or <paramref name="text"/> are <c>null</c>.</exception>
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

        /// <inheritdoc/>
        public override void Draw(float x, float y)
        {
            base.Draw(x, y);

            TextStyle.Apply();
            TextCommands.Display(Text, x + TextOffset.X, y + TextOffset.Y);
        }
    }
}

