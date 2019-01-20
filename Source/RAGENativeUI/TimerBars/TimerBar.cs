namespace RAGENativeUI.TimerBars
{
#if RPH1
    extern alias rph1;
    using Vector2 = rph1::Rage.Vector2;
#else
    /** REDACTED **/
#endif

    using System.Drawing;

    public abstract class TimerBar
    {
        private static readonly TextureDictionary BgTextureDictionary = "timerbars";
        private const string BgTextureName = "all_black_bg";
        private const string BgHighlightTextureName = "all_white_bg";

        public bool IsVisible { get; set; } = true;
        public Color? HighlightColor { get; set; }

        public TimerBar()
        {
            TimerBarManager.AddTimerBar(this);
        }

        ~TimerBar()
        {
            TimerBarManager.RemoveTimerBar(this);
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
            const float InitialY = 0.925f - 0.002f;
            const float HeightWithGap = 0.035f + 0.023f - 0.003f + 0.001f - 0.007f - 0.012f + 0.001f + 0.002f + 0.0003f;

            return new Vector2(0.795f, InitialY - HeightWithGap * index);
        }
    }
}

