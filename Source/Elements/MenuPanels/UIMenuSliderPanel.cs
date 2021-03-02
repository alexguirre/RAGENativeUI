namespace RAGENativeUI.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Rage;

    public class UIMenuSliderPanel : UIMenuPanel
    {
        public delegate void ValueChangedEvent(UIMenuSliderPanel sender, float oldValue, float newValue);

        private static readonly TextStyle BaseLabelStyle = TextStyle.Default.With(font: TextFont.ChaletLondon, scale: 0.35f);
        private const float Height = 0.034722f * 2;
        private const float BorderMargin = 0.0046875f;

        private float value = 0.0f;
        private RectangleF barBounds;
        private bool mousePressed;

        /// <summary>
        /// Gets or sets the foreground color of the slider bar.
        /// </summary>
        /// <remarks>
        /// The default value is the color of <see cref="HudColor.White"/>.
        /// </remarks>
        public Color BarForegroundColor { get; set; }

        /// <summary>
        /// Gets or sets the background color of the slider bar.
        /// </summary>
        /// <remarks>
        /// The default value is the color of <see cref="HudColor.White"/>, with alpha 120.
        /// </remarks>
        public Color BarBackgroundColor { get; set; }

        public string Title { get; set; } = "Title";
        public TextStyle TitleStyle { get; set; } = BaseLabelStyle.With(justification: TextJustification.Center);
        public string LeftLabel { get; set; } = "Left (0.0)";
        public TextStyle LeftLabelStyle { get; set; } = BaseLabelStyle.With(justification: TextJustification.Left);
        public string RightLabel { get; set; } = "Right (1.0)";
        public TextStyle RightLabelStyle { get; set; } = BaseLabelStyle.With(justification: TextJustification.Right);

        public float Value
        {
            get => value;
            set
            {
                value = MathHelper.Clamp(value, 0.0f, 1.0f);
                if (this.value != value)
                {
                    var oldValue = this.value;
                    this.value = value;
                    OnValueChanged(oldValue, value);
                }
            }
        }

        public event ValueChangedEvent ValueChanged;

        public IList<Marker> Markers { get; }

        public UIMenuSliderPanel()
        {
            BarForegroundColor = HudColor.White.GetColor();
            BarBackgroundColor = Color.FromArgb(120, BarForegroundColor);
            Markers = new List<Marker>();
        }

        public override void Draw(float x, ref float y, float menuWidth)
        {
            DrawBackground(x, y, Height, menuWidth);

            barBounds = GetSliderBarBounds(x, y, menuWidth);
            DrawLabels(x, y, menuWidth);
            DrawSlider(x, y, menuWidth);

            y += Height;
        }

        private void DrawLabels(float x, float y, float menuWidth)
        {
            var titleCharHeight = TitleStyle.CharacterHeight;
            var leftCharHeight = LeftLabelStyle.CharacterHeight;
            var rightCharHeight = RightLabelStyle.CharacterHeight;

            var centerX = x + menuWidth * 0.5f;

            float wrapStart = x + BorderMargin, wrapEnd = x + menuWidth - BorderMargin;
            var titleStyle = TitleStyle.WithWrap(wrapStart, wrapEnd);
            var leftStyle = LeftLabelStyle.WithWrap(wrapStart, wrapEnd);
            var rightStyle = RightLabelStyle.WithWrap(wrapStart, wrapEnd);

            const float PaddingFromBar = BorderMargin * 2.5f;
            var r = barBounds;
            TextCommands.Display(Title, titleStyle, centerX, r.Y - PaddingFromBar - titleCharHeight);
            TextCommands.Display(LeftLabel, leftStyle, wrapStart, r.Y - PaddingFromBar - leftCharHeight);
            TextCommands.Display(RightLabel, rightStyle, wrapEnd, r.Y - PaddingFromBar - rightCharHeight);
        }

        private void DrawSlider(float x, float y, float menuWidth)
        {
            var r = barBounds;
            UIMenu.DrawRect(r.X + r.Width * 0.5f, r.Y + r.Height * 0.5f, r.Width, r.Height, BarBackgroundColor);

            float fillWidth = r.Width * Value;
            UIMenu.DrawRect(r.X + fillWidth * 0.5f, r.Y + r.Height * 0.5f, fillWidth, r.Height, BarForegroundColor);

            int markerCount = Markers.Count;
            if (markerCount > 0)
            {
                float markerY = r.Y + r.Height * 0.5f;
                const float markerW = 0.0015f;
                float markerH = Math.Min(Height - 0.00390625f, r.Height * 2.25f);

                for (int i = 0; i < markerCount; i++)
                {
                    var m = Markers[i];
                    float markerX = r.X + r.Width * m.Percentage;

                    UIMenu.DrawRect(markerX, markerY, markerW, markerH, m.Color);
                }
            }
        }

        public override bool ProcessControl()
        {
            // TODO
            return false;
        }

        public override bool ProcessMouse(float mouseX, float mouseY)
        {
            if (!mousePressed)
            {
                bool inBounds = barBounds.Contains(mouseX, mouseY);
                if (inBounds && Game.IsControlJustPressed(2, GameControl.CursorAccept))
                {
                    mousePressed = true;
                    return true;
                }
            }
            else
            {
                if (Game.IsControlJustReleased(2, GameControl.CursorAccept))
                {
                    mousePressed = false;
                }
                else
                {
                    Value = (mouseX - barBounds.X) / barBounds.Width;
                }

                return true;
            }

            return false;
        }

        private RectangleF GetSliderBarBounds(float x, float y, float menuWidth)
        {
            float barWidth = (menuWidth - BorderMargin * 2.0f);
            float barHeight = 0.00675f;
            float barX = x + BorderMargin;
            float barY = y + Height - BorderMargin * 5.0f - barHeight;

            return new RectangleF(barX, barY, barWidth, barHeight);
        }

        protected virtual void OnValueChanged(float oldValue, float newValue)
            => ValueChanged?.Invoke(this, oldValue, newValue);

        public struct Marker : IEquatable<Marker>
        {
            /// <summary>
            /// Gets the percentage at which the marker is placed. Its range is from <c>0.0f</c> to <c>1.0f</c>.
            /// </summary>
            public float Percentage { get; }

            /// <summary>
            /// Gets the color of the marker. If <c>null</c>, <see cref="UIMenuItem.CurrentForeColor"/> is used instead.
            /// </summary>
            public Color Color { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Marker"/> structure.
            /// </summary>
            /// <param name="percentage">
            /// The percentage at which the marker is placed.
            /// Valid range is from <c>0.0f</c> to <c>1.0f</c>, values outside this range are clamped.
            /// </param>
            /// <param name="color">The color of the marker. If <c>null</c>, <see cref="UIMenuItem.CurrentForeColor"/> is used instead.</param>
            public Marker(float percentage, Color color)
            {
                Percentage = MathHelper.Clamp(percentage, 0.0f, 1.0f);
                Color = color;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Marker"/> structure using <see cref="HudColor.Black"/> as its color.
            /// </summary>
            /// <param name="percentage">
            /// The percentage at which the marker is placed.
            /// Its valid range is from <c>0.0f</c> to <c>1.0f</c>, values outside this range are clamped.
            /// </param>
            public Marker(float percentage) : this(percentage, HudColor.Black.GetColor())
            {
            }

            /// <inheritdoc/>
            public override int GetHashCode() => (Percentage, Color).GetHashCode();

            /// <inheritdoc/>
            public override bool Equals(object other) => other is Marker m && Equals(m);

            /// <inheritdoc/>
            public bool Equals(Marker other)
            {
                return Percentage == other.Percentage &&
                       Color == other.Color;
            }
        }
    }
}

