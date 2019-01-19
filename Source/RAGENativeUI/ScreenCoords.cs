namespace RAGENativeUI
{
#if RPH1
    extern alias rph1;
    using Vector2 = rph1::Rage.Vector2;
    using Vector4 = rph1::Rage.Vector4;
    using Matrix = rph1::Rage.Matrix;
#else
    /** REDACTED **/
#endif

    using System.Drawing;
    
    public static class ScreenCoordsExtension
    {
        // Example:
        // 
        // 0.5f.XRel() + 100.XPx() = 100 pixels to the right from the middle of the screen

        private static Size transformRes;
        private static Matrix pxToRelTransform;

        private static Vector2 PxToRel(float x, float y)
        {
            Size currentRes = RPH.Game.Resolution;
            if (transformRes != currentRes)
            {
                transformRes = currentRes;
                pxToRelTransform = Matrix.OrthoOffCenterRH(-transformRes.Width, transformRes.Width, -transformRes.Height, transformRes.Height, -1.0f, 1.0f);
            }

            Vector4 tmp = Vector4.Transform(new Vector4(x, y, 0.0f, 1.0f), pxToRelTransform);
            return new Vector2(tmp.X, tmp.Y);
        }

        public static Vector2 XRel(this float value) => new Vector2(value, 0.0f);
        public static Vector2 YRel(this float value) => new Vector2(0.0f, value);
        public static Vector2 Rel(this (float X, float Y) value) => new Vector2(value.X, value.Y);

        public static Vector2 XPx(this float value) => PxToRel(value, 0.0f);
        public static Vector2 YPx(this float value) => PxToRel(0.0f, value);
        public static Vector2 Px(this (float X, float Y) value)  => PxToRel(value.X, value.Y);

        public static Vector2 XPx(this int value) => XPx((float)value);
        public static Vector2 YPx(this int value) => YPx((float)value);
        public static Vector2 Px(this (int X, int Y) value) => Px(((float)value.X, (float)value.Y));
    }
}

