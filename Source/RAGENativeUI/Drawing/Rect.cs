namespace RAGENativeUI.Drawing
{
#if RPH1
    extern alias rph1;
    using Vector2 = rph1::Rage.Vector2;
#else
    /** REDACTED **/
#endif

    using System.Drawing;
    
    public class Rect
    {
        public bool IsVisible { get; set; } = true;
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public Color Color { get; set; }

        public Rect(Vector2 position, Vector2 size, Color color)
        {
            Position = position;
            Size = size;
            Color = color;
        }

        public void Draw()
        {
            if (!IsVisible)
                return;

            Draw(Position, Size, Color);
        }


        public static void Draw(Vector2 position, Vector2 size, Color color)
        {
            N.DrawRect(position.X, position.Y, size.X, size.Y, color.R, color.G, color.B, color.A);
        }
    }
}

