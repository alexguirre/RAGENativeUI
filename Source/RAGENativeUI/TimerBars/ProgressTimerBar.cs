namespace RAGENativeUI.TimerBars
{
#if RPH1
    extern alias rph1;
    using Vector2 = rph1::Rage.Vector2;
#else
    /** REDACTED **/
#endif

    using System.Drawing;
    using System.Collections.Generic;

    public class ProgressTimerBar : LabeledTimerBar
    {
        private static readonly TextureDictionary MarkerTextureDictionary = "timerbar_lines";
        /// <summary>
        /// We use the texture with the marker at 50% because the game only has textures for every 10% interval,
        /// but here we support any percentage and the 50% texture is the easiest one to offset.
        /// </summary>
        private const string MarkerTextureName = "linemarker50_128";

        private float percentage;

        /// <summary>
        /// Gets or sets the percentage of the progress bar.
        /// The percentage is clamped to the range [0.0, 1.0].
        /// </summary>
        public float Percentage { get => percentage; set => percentage = RPH.MathHelper.Clamp(value, 0.0f, 1.0f); }
        public Color BackColor { get; set; } = HudColor.RedDark.GetColor();
        public Color ForeColor { get; set; } = HudColor.Red.GetColor();
        /// <summary>
        /// Gets the <see cref="List{T}"/> of <see cref="float"/> that contains the percentages at which a marker is drawn.
        /// The percentages are clamped to the range [0.0, 1.0].
        /// </summary>
        public List<float> Markers { get; } = new List<float>();
        public Color MarkersColor { get; set; } = HudColor.Black.GetColor();

        public ProgressTimerBar(string label, Color backColor, Color foreColor) : base(label)
        {
            BackColor = backColor;
            ForeColor = foreColor;
        }

        public ProgressTimerBar(string label) : base(label)
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
            
            float fillX = pos.X - Width * 0.5f + Width * 0.5f * percentage;
            N.DrawRect(fillX, pos.Y, Width * percentage, Height, ForeColor.R, ForeColor.G, ForeColor.B, ForeColor.A);
            
            if (Markers.Count > 0)
            {
                if (!MarkerTextureDictionary.IsLoaded) MarkerTextureDictionary.Load();

                float markerOrigX = pos.X - Width * 0.5f;
                foreach (float markerPercentage in Markers)
                {
                    float x = markerOrigX + Width * RPH.MathHelper.Clamp(markerPercentage, 0.0f, 1.0f);
                    N.DrawSprite(MarkerTextureDictionary, MarkerTextureName, x, pos.Y, Width, Height * 2.0f, 0.0f, 0, 0, 0, 255);
                }
            }
        }
    }
}

