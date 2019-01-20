namespace RAGENativeUI.TimerBars
{
#if RPH1
    extern alias rph1;
    using Vector2 = rph1::Rage.Vector2;
#else
    /** REDACTED **/
#endif

    using System.Drawing;
    using RAGENativeUI.Drawing;

    public class ProgressTimerBar : LabeledTimerBar
    {
        private float percentage;

        public float Percentage { get => percentage; set => percentage = RPH.MathHelper.Clamp(value, 0.0f, 1.0f); }
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }

        public ProgressTimerBar(string label, Color backColor, Color foreColor) : base(label)
        {
            BackColor = backColor;
            ForeColor = foreColor;
        }

        public ProgressTimerBar(string label) : this(label, Color.DarkRed, Color.Red)
        {
        }

        public override void Draw(int index)
        {
            if (!IsVisible)
                return;

            base.Draw(index);

            // TODO: ProgressTimerBar background should be lower

            // Constants from the game scripts
            const float X = (((((0.919f - 0.081f) + 0.028f) + 0.05f) - 0.001f) - 0.002f);
            const float YOffset = ((((0.013f - 0.002f) + 0.001f) + 0.001f) - 0.001f);
            const float Width = 0.069f;
            const float Height = 0.011f;

            Vector2 pos = Position(index);
            pos.X = X;
            pos.Y += YOffset;

            N.DrawRect(pos.X, pos.Y, Width, Height, BackColor.R, BackColor.G, BackColor.B, BackColor.A);

            pos.X = pos.X - Width * 0.5f + Width * 0.5f * percentage;
            N.DrawRect(pos.X, pos.Y, Width * percentage, Height, ForeColor.R, ForeColor.G, ForeColor.B, ForeColor.A);
        }
    }
}

