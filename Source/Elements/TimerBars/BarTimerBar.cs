namespace RAGENativeUI.Elements
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;

    /// <summary>
    /// Represents a timer bar containing a progress bar.
    /// </summary>
    /// <seealso cref="TimerBarMarker"/>
    public class BarTimerBar : TimerBarBase
    {
        private float percentage;

        /// <summary>
        /// Gets or sets the progress percentage. It determines how filled the progress bar is and
        /// the valid range is from <c>0.0f</c> to <c>1.0f</c>, values outside this range are clamped.
        /// </summary>
        public float Percentage
        {
            get => percentage;
            set => percentage = Rage.MathHelper.Clamp(value, 0.0f, 1.0f);
        }

        /// <summary>
        /// Gets or sets the background color of the progress bar.
        /// </summary>
        public Color BackgroundColor { get; set; }
        /// <summary>
        /// Gets or sets the foreground color of the progress bar.
        /// </summary>
        public Color ForegroundColor { get; set; }

        /// <summary>
        /// Gets a list containing the <see cref="TimerBarMarker"/>s of the progress bar.
        /// </summary>
        public IList<TimerBarMarker> Markers { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BarTimerBar"/> class.
        /// </summary>
        /// <param name="label">A <see cref="string"/> that will appear at the left side of the timer bar.</param>
        public BarTimerBar(string label) : base(label)
        {
            Thin = true;

            N.GetHudColour(8, out int bR, out int bG, out int bB, out int bA); // HUD_COLOUR_REDDARK
            N.GetHudColour(6, out int fR, out int fG, out int fB, out int fA); // HUD_COLOUR_RED

            BackgroundColor = Color.FromArgb(bA, bR, bG, bB);
            ForegroundColor = Color.FromArgb(fA, fR, fG, fB);

            Markers = new List<TimerBarMarker>();
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

        /// <inheritdoc/>
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

            if (Markers.Count > 0)
            {
                N.RequestStreamedTextureDict(TB.MarkersTextureDictionary);
                if (!N.HasStreamedTextureDictLoaded(TB.MarkersTextureDictionary))
                {
                    return;
                }

                float markerOrigX = x - w * 0.5f;
                foreach (TimerBarMarker m in Markers)
                {
                    float markerX = markerOrigX + w * m.Percentage;

                    N.DrawSprite(TB.MarkersTextureDictionary, TB.MarkerTextureName, markerX, y, w * 1.1f, h * 1.9f, 0.0f, m.Color.R, m.Color.G, m.Color.B, m.Color.A);
                }
            }
        }
    }

    /// <summary>
    /// Defines a <see cref="BarTimerBar"/> marker. A marker is represented as a thin line over the progress bar at the specified <see cref="Percentage"/>.
    /// </summary>
    public struct TimerBarMarker : IEquatable<TimerBarMarker>
    {
        /// <summary>
        /// Gets the percentage at which the marker is placed. Its range is from <c>0.0f</c> to <c>1.0f</c>.
        /// </summary>
        public float Percentage { get; }

        /// <summary>
        /// Gets the color of the marker.
        /// </summary>
        public Color Color { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerBarMarker"/> structure.
        /// </summary>
        /// <param name="percentage">
        /// The percentage at which the marker is placed.
        /// Valid range is from <c>0.0f</c> to <c>1.0f</c>, values outside this range are clamped.
        /// </param>
        /// <param name="color">The color of the marker.</param>
        public TimerBarMarker(float percentage, Color color)
        {
            Percentage = Rage.MathHelper.Clamp(percentage, 0.0f, 1.0f);
            Color = color;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerBarMarker"/> structure with <see cref="Color"/> set to <see cref="Color.Black"/>.
        /// </summary>
        /// <param name="percentage">
        /// The percentage at which the marker is placed.
        /// Its valid range is from <c>0.0f</c> to <c>1.0f</c>, values outside this range are clamped.
        /// </param>
        public TimerBarMarker(float percentage) : this(percentage, Color.Black)
        {
        }

        /// <inheritdoc/>
        public override int GetHashCode() => (Percentage, Color).GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object other) => other is TimerBarMarker m && Equals(m);

        /// <inheritdoc/>
        public bool Equals(TimerBarMarker other)
        {
            return Percentage == other.Percentage &&
                   Color == other.Color;
        }
    }
}

