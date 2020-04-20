namespace RAGENativeUI.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// Represents a timer bar containing icons at the right side.
    /// </summary>
    /// <seealso cref="TimerBarIcon"/>
    public class IconsTimerBar : TimerBarBase
    {
        /// <summary>
        /// Gets the list containing the <see cref="TimerBarIcon"/>s of the timer bar.
        /// The icons are shown from right to left: the icons at index 0 is the right-most one
        /// and the last icon in the list is the left-most one.
        /// </summary>
        public IList<TimerBarIcon> Icons { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IconsTimerBar"/> class.
        /// </summary>
        /// <param name="label">A <see cref="string"/> that will appear at the left side of the timer bar.</param>
        public IconsTimerBar(string label) : base(label)
        {
            Icons = new List<TimerBarIcon>();
        }

        /// <inheritdoc/>
        public override void Draw(float x, float y)
        {
            base.Draw(x, y);

            if (Icons.Count > 0)
            {
                x += TB.IconXOffset;
                y += TB.IconYOffset;

                foreach (TimerBarIcon icon in Icons)
                {
                    if (icon != null)
                    {
                        N.RequestStreamedTextureDict(icon.TextureDictionary);
                        if (!N.HasStreamedTextureDictLoaded(icon.TextureDictionary))
                        {
                            continue;
                        }

                        Color c = icon.Color;

                        N.DrawSprite(icon.TextureDictionary, icon.TextureName, x, y, icon.Size.Width, icon.Size.Height, 0.0f, c.R, c.G, c.B, c.A);
                        x -= icon.Spacing;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Represents an icon of a <see cref="IconsTimerBar"/>.
    /// </summary>
    public class TimerBarIcon
    {
        /// <summary>
        /// Represents the default value of <see cref="Size"/>.
        /// </summary>
        public static readonly SizeF DefaultSize = new SizeF(TB.IconWidth, TB.IconHeight);

        /// <summary>
        /// Represents the default value of <see cref="Spacing"/>.
        /// </summary>
        public static readonly float DefaultSpacing = TB.IconWidth * 0.75f;

        /// <summary>
        /// Represents the default value of <see cref="Color"/>.
        /// </summary>
        public static readonly Color DefaultColor = Color.White;

        private string textureDictionary;
        private string textureName;

        /// <summary>
        /// Gets or sets the texture dictionary that contains the texture.
        /// </summary>
        /// <exception cref="ArgumentException">If <c>value</c> is <c>null</c>, empty or white-space.</exception>
        public string TextureDictionary
        {
            get => textureDictionary;
            set => textureDictionary = String.IsNullOrWhiteSpace(value) ? throw new ArgumentException($"{nameof(value)} is null, empty or white-space", nameof(value)) : value;
        }

        /// <summary>
        /// Gets or sets the name of the texture.
        /// </summary>
        /// <exception cref="ArgumentException">If <c>value</c> is <c>null</c>, empty or white-space.</exception>
        public string TextureName
        {
            get => textureName;
            set => textureName = String.IsNullOrWhiteSpace(value) ? throw new ArgumentException($"{nameof(value)} is null, empty or white-space", nameof(value)) : value;
        }

        /// <summary>
        /// Gets or sets the size of this icon, in relative coordinates.
        /// </summary>
        public SizeF Size { get; set; } = DefaultSize;

        /// <summary>
        /// Gets or sets how much space there is between this icon and the one to its left, in relative coordinates along the X-axis.
        /// </summary>
        public float Spacing { get; set; } = DefaultSpacing;

        /// <summary>
        /// Gets or sets the color of this icon.
        /// </summary>
        public Color Color { get; set; } = DefaultColor;

        /// <summary>
        /// Initializes a new instance of <see cref="TimerBarIcon"/> class.
        /// </summary>
        /// <param name="textureDictionary">The texture dictionary that contains the texture.</param>
        /// <param name="textureName">The name of the texture.</param>
        /// <exception cref="ArgumentException">If <paramref name="textureDictionary"/> or <paramref name="textureName"/> are <c>null</c>, empty or white-space.</exception>
        public TimerBarIcon(string textureDictionary, string textureName)
        {
            TextureDictionary = textureDictionary;
            TextureName = textureName;
        }

        // built-in icons
        private const string TBTxd = "timerbars";
        public static TimerBarIcon Rocket => new TimerBarIcon(TBTxd, "rockets");
        public static TimerBarIcon Spike => new TimerBarIcon(TBTxd, "spikes");
        public static TimerBarIcon Boost => new TimerBarIcon(TBTxd, "boost");

        private const string IconsTxd = "timerbar_icons";
        public static TimerBarIcon Beast => new TimerBarIcon(IconsTxd, "pickup_beast");
        public static TimerBarIcon Random => new TimerBarIcon(IconsTxd, "pickup_random");
        public static TimerBarIcon SlowTime => new TimerBarIcon(IconsTxd, "pickup_slow_time");
        public static TimerBarIcon Swap => new TimerBarIcon(IconsTxd, "pickup_swap");
        public static TimerBarIcon Testosterone => new TimerBarIcon(IconsTxd, "pickup_testosterone");
        public static TimerBarIcon Thermal => new TimerBarIcon(IconsTxd, "pickup_thermal");
        public static TimerBarIcon Weed => new TimerBarIcon(IconsTxd, "pickup_weed");
        public static TimerBarIcon Hidden => new TimerBarIcon(IconsTxd, "pickup_hidden");
        public static TimerBarIcon Time => new TimerBarIcon(IconsTxd, "pickup_b_time");
    }
}
