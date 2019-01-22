namespace RAGENativeUI.TimerBars
{
#if RPH1
    extern alias rph1;
    using Vector2 = rph1::Rage.Vector2;
#else
    /** REDACTED **/
#endif

    using System;
    using System.Drawing;

    public abstract class TimerBar : IDisposable
    {
        // Constants from the game scripts
        private static readonly TextureDictionary BgTextureDictionary = "timerbars";
        private const string BgTextureName = "all_black_bg";
        private const string BgHighlightTextureName = "all_white_bg";
        private const float BgXOffset = 0.079f;
        private const float BgDefaultYOffset = 0.008f;
        private const float BgSmallYOffset = 0.012f;
        private const float BgWidth = 0.157f;
        private const float BgDefaultHeight = 0.036f;
        private const float BgSmallHeight = 0.028f;
        internal const float DefaultHeightWithGap = ((0.025f + 0.006f) + 0.0009f) + 0.008f;
        internal const float SmallHeightWithGap = ((0.025f + 0.006f) + 0.0009f);

        public const uint DefaultOrderPriority = uint.MaxValue;


        private bool isDisposed = false;
        private uint orderPriority = DefaultOrderPriority;

        public bool IsDisposed
        {
            get => isDisposed;
            private set
            {
                if (value != isDisposed)
                {
                    isDisposed = value;
                }
            }
        }

        public bool IsVisible { get; set; } = true;
        public Color? HighlightColor { get; set; }
        // TODO: decide how to finally implement TimerBar icons
        // maybe have an IconsTimerBar class which can have up to ~5 icons
        // and allow TextTimerBar to have 1 icon which will offset the text to the left
        public TimerBarIcon Icon { get; set; }
        /// <summary>
        /// Gets or sets the order priority. This value is used to determine the prefered position at which this <see cref="TimerBar"/>
        /// will be drawn.
        /// <para><see cref="TimerBar"/>s with a lower priority are drawn first, starting from the bottom.</para>
        /// </summary>
        /// <value>
        /// An unsigned integer that contains the order priority. The default value is <see cref="DefaultOrderPriority"/>.
        /// </value>
        public uint OrderPriority
        {
            get => orderPriority;
            set
            {
                if (value != orderPriority)
                {
                    orderPriority = value;
                    TimerBarManager.NotifyOrderPriorityChanged();
                }
            }
        }

        /// <summary>
        /// Gets whether the height of this <see cref="TimerBar"/> is small or default.
        /// </summary>
        protected internal virtual bool SmallHeight => false;

        public TimerBar()
        {
            TimerBarManager.AddTimerBar(this);
        }

        ~TimerBar()
        {
            Dispose(false);
        }

        public virtual void Draw(Vector2 position)
        {
            if (!IsVisible)
                return;

            if (!BgTextureDictionary.IsLoaded) BgTextureDictionary.Load();

            Vector2 bgPos = position;
            bgPos.X += BgXOffset;
            bgPos.Y += SmallHeight ? BgSmallYOffset : BgDefaultYOffset;
            float height = SmallHeight ? BgSmallHeight : BgDefaultHeight;

            if (HighlightColor.HasValue)
            {
                N.DrawSprite(BgTextureDictionary, BgHighlightTextureName, bgPos.X, bgPos.Y, BgWidth, height, 0.0f, HighlightColor.Value.R, HighlightColor.Value.G, HighlightColor.Value.B, 140);
            }
            N.DrawSprite(BgTextureDictionary, BgTextureName, bgPos.X, bgPos.Y, BgWidth, height, 0.0f, 255, 255, 255, 140);

            if (Icon != null)
            {
                if (!Icon.Texture.Dictionary.IsLoaded) Icon.Texture.Dictionary.Load();

                Vector2 iconPos = position + Icon.Offset;
                Color c = Icon.Color;
                N.DrawSprite(Icon.Texture.Dictionary, Icon.Texture.Name, iconPos.X, iconPos.Y, Icon.Size.X, Icon.Size.Y, 0.0f, c.R, c.G, c.B, c.A);
            }
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    TimerBarManager.RemoveTimerBar(this);
                }

                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}

