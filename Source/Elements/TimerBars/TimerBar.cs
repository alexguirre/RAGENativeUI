namespace RAGENativeUI.Elements.TimerBars
{
    using System.Drawing;
    
    using Rage.Native;

    public abstract class TimerBar : IScreenElement
    {
        public static readonly float DefaultWidth = 0.157f;
        public static readonly float DefaultHeight = 0.036f;


        private readonly TextureDictionary bgTextureDictionary = "timerbars";
        private readonly string bgTextureName = "all_black_bg";

        public bool IsVisible { get; set; } = true;
        public ScreenRectangle Rectangle { get; set; }
        public Color Color { get; set; }

        public TimerBar()
        {
            float x = 0.5f + (NativeFunction.Natives.GetSafeZoneSize<float>() / 2f);
            float y = x;
            Rectangle = ScreenRectangle.FromRelativeCoords(x - DefaultWidth * 0.5f, y - DefaultHeight * 0.5f, DefaultWidth, DefaultHeight);
            Color = Color.White;
        }

        public virtual void Draw()
        {
            if (!IsVisible)
                return;

            Sprite.Draw(bgTextureDictionary, bgTextureName, Rectangle, 0.0f, Color.FromArgb(140, 255, 255, 255));

            NativeFunction.Natives.HideHudComponentThisFrame(6); // VehicleName
            NativeFunction.Natives.HideHudComponentThisFrame(7); // AreaName
            NativeFunction.Natives.HideHudComponentThisFrame(9); // StreetName
        }
    }
}

