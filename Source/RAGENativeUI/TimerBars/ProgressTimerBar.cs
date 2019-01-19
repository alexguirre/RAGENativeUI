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

        public override void Draw()
        {
            if (!IsVisible)
                return;

            base.Draw();

            Vector2 pos = Position;
            Vector2 size = Size;

            float barX = pos.X + 0.04f;
            float barY = pos.Y;
            float barW = size.X / 2.25f;
            float barH = size.Y / 3f;
            Rect.Draw((barX, barY).Rel(), (barW, barH).Rel(), BackColor);
            
            float fillX = barX - barW * 0.5f + barW * 0.5f * percentage;
            float fillW = barW * percentage;
            Rect.Draw((fillX, barY).Rel(), (fillW, barH).Rel(), ForeColor);
        }
    }
}

