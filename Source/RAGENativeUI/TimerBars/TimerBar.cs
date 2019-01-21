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
        private static readonly TextureDictionary BgTextureDictionary = "timerbars";
        private const string BgTextureName = "all_black_bg";
        private const string BgHighlightTextureName = "all_white_bg";

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

        public TimerBar()
        {
            TimerBarManager.AddTimerBar(this);
        }

        ~TimerBar()
        {
            Dispose(false);
        }

        public virtual void Draw(int index)
        {
            if (!IsVisible)
                return;

            if (!BgTextureDictionary.IsLoaded) BgTextureDictionary.Load();

            // Constants from the game scripts
            const float XOffset = 0.079f;
            const float YOffset = 0.008f;
            const float Width = 0.157f;
            const float Height = 0.036f;

            Vector2 pos = Position(index);
            pos.X += XOffset;
            pos.Y += YOffset;

            if (HighlightColor.HasValue)
            {
                N.DrawSprite(BgTextureDictionary, BgHighlightTextureName, pos.X, pos.Y, Width, Height, 0.0f, HighlightColor.Value.R, HighlightColor.Value.G, HighlightColor.Value.B, 140);
            }
            N.DrawSprite(BgTextureDictionary, BgTextureName, pos.X, pos.Y, Width, Height, 0.0f, 255, 255, 255, 140);
        }
        
        protected static Vector2 Position(int index)
        {
            // Constants from the game scripts
            const float InitialX = 0.795f;
            const float InitialY = 0.925f - 0.002f;
            const float HeightWithGap = 0.035f + 0.023f - 0.003f + 0.001f - 0.007f - 0.012f + 0.001f + 0.002f + 0.0003f;
            const float LoadingPromptYOffset = 0.036f;

            return new Vector2(InitialX, InitialY - HeightWithGap * index - (N.IsLoadingPromptBeingDisplayed() ? LoadingPromptYOffset : 0.0f));
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

