namespace RAGENativeUI.Elements.TimerBars
{
    using System.Drawing;

    using Rage;

    public class ProgressTimerBar : LabeledTimerBar
    {
        private float percentage;

        public float Percentage { get => percentage; set => percentage = MathHelper.Clamp(value, 0.0f, 1.0f); }
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

            ScreenRectangle rect = Rectangle;

            float barX = rect.X + 0.04f;
            float barY = rect.Y;
            float barW = rect.Width / 2.25f;
            float barH = rect.Height / 3f;
            Rect.Draw(ScreenRectangle.FromRelativeCoords(barX, barY, barW, barH), BackColor);
            
            float fillX = barX - barW * 0.5f + barW * 0.5f * percentage;
            float fillW = barW * percentage;
            Rect.Draw(ScreenRectangle.FromRelativeCoords(fillX, barY, fillW, barH), ForeColor);
        }
    }
}

