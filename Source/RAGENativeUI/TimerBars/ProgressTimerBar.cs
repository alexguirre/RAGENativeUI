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

            // TODO: search for the actual ProgressTimerBar constants value in the game scripts
            const float YOffset = 0.008f;
            const float Width = 0.059f;
            const float Height = 0.011f;

            Vector2 pos = Position(index);
            pos.X = (((((0.95f - 0.047f) + 0.001f) + 0.047f) - 0.002f) + 0.001f) - Width * 0.5f;
            pos.Y += YOffset;

            N.DrawRect(pos.X, pos.Y, Width, Height, BackColor.R, BackColor.G, BackColor.B, BackColor.A);

            pos.X = pos.X - Width * 0.5f + Width * 0.5f * percentage;
            N.DrawRect(pos.X, pos.Y, Width * percentage, Height, ForeColor.R, ForeColor.G, ForeColor.B, ForeColor.A);




            //Vector2 pos = Position;
            //Vector2 size = Size;

            //float barX = pos.X + 0.04f;
            //float barY = pos.Y;
            //float barW = size.X / 2.25f;
            //float barH = size.Y / 3f;
            //Rect.Draw((barX, barY).Rel(), (barW, barH).Rel(), BackColor);

            //float fillX = barX - barW * 0.5f + barW * 0.5f * percentage;
            //float fillW = barW * percentage;
            //Rect.Draw((fillX, barY).Rel(), (fillW, barH).Rel(), ForeColor);
        }
    }
}

