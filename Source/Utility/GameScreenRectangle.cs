namespace RAGENativeUI.Utility
{
    using System.Drawing;

    using Rage;
    using Rage.Native;
    
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
                int screenw = Game.Resolution.Width;
                int screenh = Game.Resolution.Height;
                const float height = 1080f;
                float ratio = (float)screenw / screenh;
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
}

