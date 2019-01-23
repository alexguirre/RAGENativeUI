namespace RAGENativeUI.TimerBars
{
#if RPH1
    extern alias rph1;
    using Vector2 = rph1::Rage.Vector2;
#else
    /** REDACTED **/
#endif

    using System.Drawing;

    // TODO: IconsTimerBar class
    // timerbar that inherits LabeledTimerBar and can have many icons

    public sealed class TimerBarIcon
    {
        internal const float XOffset = 0.145f + 0.001f;
        internal const float YOffset = 0.016f * 0.5f;
        internal const float DefaultWidth = 0.016f + 0.003f;
        internal const float DefaultHeight = 0.032f + 0.004f;
        internal const HudColor DefaultColor = HudColor.White;

        public TextureReference Texture { get; set; }
        public Vector2 Size { get; set; } = new Vector2(DefaultWidth, DefaultHeight);
        public Color Color { get; set; } = DefaultColor.GetColor();
    }
}

