namespace RAGENativeUI.Elements.TimerBars
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
        public static readonly float DefaultWidth = 0.157f;
        public static readonly float DefaultHeight = 0.036f;


        private readonly TextureDictionary bgTextureDictionary = "timerbars";
        private readonly string bgTextureName = "all_black_bg";

        public bool IsVisible { get; set; } = true;
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public Color Color { get; set; }

        public TimerBar()
        {
            float x = 0.5f + (N.GetSafeZoneSize() / 2f);
            float y = x;
            Position = (x - DefaultWidth * 0.5f, y - DefaultHeight * 0.5f).Rel();
            Size = (DefaultWidth, DefaultHeight).Rel();
            Color = Color.White;
        }

        public virtual void Draw()
        {
            if (!IsVisible)
                return;

            Sprite.Draw(bgTextureDictionary, bgTextureName, Position, Size, 0.0f, Color.FromArgb(140, 255, 255, 255));

            N.HideHudComponentThisFrame(6); // VehicleName
            N.HideHudComponentThisFrame(7); // AreaName
            N.HideHudComponentThisFrame(9); // StreetName
        }
    }
}

