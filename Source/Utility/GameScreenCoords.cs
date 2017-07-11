namespace RAGENativeUI.Utility
{
    using System.Drawing;

    using Rage;
    using Rage.Native;
    
    public struct GameScreenCoords
    {
        public RectangleF Relative { get; }

        private GameScreenCoords(RectangleF coords, bool absolute)
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

                Relative = new RectangleF(x, y, w, h);
            }
            else
            {
                Relative = coords;
            }
        }

        public static GameScreenCoords FromAbsoluteCoords(float x, float y, float width, float height) => FromAbsoluteCoords(new RectangleF(x, y, width, height));
        public static GameScreenCoords FromAbsoluteCoords(PointF position, SizeF size) => FromAbsoluteCoords(new RectangleF(position.X, position.Y, size.Width, size.Height));
        public static GameScreenCoords FromAbsoluteCoords(RectangleF rectangle) => new GameScreenCoords(rectangle, true);

        public static GameScreenCoords FromRelativeCoords(float x, float y, float width, float height) => FromRelativeCoords(new RectangleF(x, y, width, height));
        public static GameScreenCoords FromRelativeCoords(PointF position, SizeF size) => FromRelativeCoords(new RectangleF(position.X, position.Y, size.Width, size.Height));
        public static GameScreenCoords FromRelativeCoords(RectangleF rectangle) => new GameScreenCoords(rectangle, false);
    }
}

