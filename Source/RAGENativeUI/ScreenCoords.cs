namespace RAGENativeUI
{
    using System.Drawing;

    using Rage;
    
    public struct ScreenRectangle
    {
        // relative coords
        public float X { get; }
        public float Y { get; }
        public float Width{ get; }
        public float Height { get; }

        private ScreenRectangle(RectangleF coords, bool absolute)
        {
            if (absolute)
            {
                int screenWidth = RPH.Game.Resolution.Width;
                int screenHeight = RPH.Game.Resolution.Height;
                const float height = 1080f;
                float ratio = (float)screenWidth / screenHeight;
                var width = height * ratio;

                float w = coords.Width / width;
                float h = coords.Height / height;
                float x = (coords.X / width) + w * 0.5f;
                float y = (coords.Y / height) + h * 0.5f;

                X = x;
                Y = y;
                Width = w;
                Height = h;
            }
            else
            {
                X = coords.X;
                Y = coords.Y;
                Width = coords.Width;
                Height = coords.Height;
            }
        }

        public override string ToString()
        {
            return $"X: {X} Y: {Y} Width: {Width} Height: {Height}";
        }

        public static ScreenRectangle FromAbsoluteCoords(float x, float y, float width, float height) => FromAbsoluteCoords(new RectangleF(x, y, width, height));
        public static ScreenRectangle FromAbsoluteCoords(PointF position, SizeF size) => FromAbsoluteCoords(new RectangleF(position.X, position.Y, size.Width, size.Height));
        public static ScreenRectangle FromAbsoluteCoords(RectangleF rectangle) => new ScreenRectangle(rectangle, true);

        public static ScreenRectangle FromRelativeCoords(float x, float y, float width, float height) => FromRelativeCoords(new RectangleF(x, y, width, height));
        public static ScreenRectangle FromRelativeCoords(PointF position, SizeF size) => FromRelativeCoords(new RectangleF(position.X, position.Y, size.Width, size.Height));
        public static ScreenRectangle FromRelativeCoords(RectangleF rectangle) => new ScreenRectangle(rectangle, false);
    }

    public struct ScreenPosition
    {
        // relative coords
        public float X { get; }
        public float Y { get; }

        private ScreenPosition(PointF coords, bool absolute)
        {
            if (absolute)
            {
                int screenWidth = RPH.Game.Resolution.Width;
                int screenHeight = RPH.Game.Resolution.Height;
                const float height = 1080f;
                float ratio = (float)screenWidth / screenHeight;
                var width = height * ratio;
                
                float x = (coords.X / width);
                float y = (coords.Y / height);

                X = x;
                Y = y;
            }
            else
            {
                X = coords.X;
                Y = coords.Y;
            }
        }

        public override string ToString()
        {
            return $"X: {X} Y: {Y}";
        }

        public static ScreenPosition FromAbsoluteCoords(float x, float y) => FromAbsoluteCoords(new PointF(x, y));
        public static ScreenPosition FromAbsoluteCoords(PointF position) => new ScreenPosition(position, true);

        public static ScreenPosition FromRelativeCoords(float x, float y) => FromRelativeCoords(new PointF(x, y));
        public static ScreenPosition FromRelativeCoords(PointF position) => new ScreenPosition(position, false);
    }
}

