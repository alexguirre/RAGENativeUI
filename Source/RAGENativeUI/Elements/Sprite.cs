namespace RAGENativeUI.Elements
{
#if RPH1
    extern alias rph1;
    using Vector2 = rph1::Rage.Vector2;
#else
    /** REDACTED **/
#endif

    using System.Drawing;
    
    public class Sprite
    {
        public bool IsVisible { get; set; } = true;
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public Color Color { get; set; }
        public TextureDictionary TextureDictionary { get; set; }
        public string TextureName { get; set; }
        public float Rotation { get; set; }

        public Sprite(TextureDictionary textureDictionary, string textureName, Vector2 position, Vector2 size, Color color)
        {
            Throw.IfNull(textureName, nameof(textureName));

            TextureDictionary = textureDictionary;
            TextureName = textureName;
            Position = position;
            Size = size;
            Color = color;
        }

        public Sprite(TextureDictionary textureDictionary, string textureName, Vector2 position, Vector2 size) : this(textureDictionary, textureName, position, size, Color.White)
        {
        }


        public void Draw()
        {
            if (!IsVisible)
                return;

            Draw(TextureDictionary, TextureName, Position, Size, Rotation, Color);
        }

        public static void Draw(TextureDictionary textureDictionary, string textureName, Vector2 position, Vector2 size, float rotation, Color color)
        {
            Throw.IfNull(textureName, nameof(textureName));

            if (!textureDictionary.IsLoaded)
                textureDictionary.Load();

            N.DrawSprite(textureDictionary.Name, textureName, position.X, position.Y, size.X, size.Y, rotation, color.R, color.G, color.B, color.A);
        }
    }
}

