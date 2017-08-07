namespace RAGENativeUI
{
    using System.Drawing;

    using Rage;
    
    public struct GameScreenRectangle
    {
        public float X { get; }
        public float Y { get; }
        public float Width{ get; }
        public float Height { get; }

        private GameScreenRectangle(RectangleF coords, bool absolute)
        {
            if (absolute)
            {
                int screenWidth = Game.Resolution.Width;
                int screenHeight = Game.Resolution.Height;
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

        public static GameScreenRectangle FromAbsoluteCoords(float x, float y, float width, float height) => FromAbsoluteCoords(new RectangleF(x, y, width, height));
        public static GameScreenRectangle FromAbsoluteCoords(PointF position, SizeF size) => FromAbsoluteCoords(new RectangleF(position.X, position.Y, size.Width, size.Height));
        public static GameScreenRectangle FromAbsoluteCoords(RectangleF rectangle) => new GameScreenRectangle(rectangle, true);

        public static GameScreenRectangle FromRelativeCoords(float x, float y, float width, float height) => FromRelativeCoords(new RectangleF(x, y, width, height));
        public static GameScreenRectangle FromRelativeCoords(PointF position, SizeF size) => FromRelativeCoords(new RectangleF(position.X, position.Y, size.Width, size.Height));
        public static GameScreenRectangle FromRelativeCoords(RectangleF rectangle) => new GameScreenRectangle(rectangle, false);
    }

    public struct GameScreenPosition
    {
        public float X { get; }
        public float Y { get; }

        private GameScreenPosition(PointF coords, bool absolute)
        {
            if (absolute)
            {
                int screenWidth = Game.Resolution.Width;
                int screenHeight = Game.Resolution.Height;
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

        public static GameScreenPosition FromAbsoluteCoords(float x, float y) => FromAbsoluteCoords(new PointF(x, y));
        public static GameScreenPosition FromAbsoluteCoords(PointF position) => new GameScreenPosition(position, true);

        public static GameScreenPosition FromRelativeCoords(float x, float y) => FromRelativeCoords(new PointF(x, y));
        public static GameScreenPosition FromRelativeCoords(PointF position) => new GameScreenPosition(position, false);
    }
}

