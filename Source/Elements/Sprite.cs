namespace RAGENativeUI.Elements
{
    using System.Drawing;
    
    using Rage.Native;

    public class Sprite : IScreenElement
    {
        public bool IsVisible { get; set; } = true;
        public ScreenRectangle Rectangle { get; set; }
        public Color Color { get; set; }
        public TextureAsset Texture { get; set; }
        public float Rotation { get; set; }

        public Sprite(TextureDictionary textureDictionary, string textureName, ScreenRectangle rectangle, Color color)
        {
            Throw.IfNull(textureName, nameof(textureName));
            Throw.InvalidOperationIfNot(textureDictionary.IsValid, $"The texture dictionary '{textureDictionary.Name}' is invalid.");
            Throw.InvalidOperationIfNot(textureDictionary.Contains(textureName), $"The texture dictionary '{textureDictionary.Name}' does contain the texture '{textureName}'.");

            Texture = textureDictionary[textureName];
            Rectangle = rectangle;
            Color = color;
        }

        public Sprite(TextureDictionary textureDictionary, string textureName, ScreenRectangle rectangle) : this(textureDictionary, textureName, rectangle, Color.White)
        {
        }

        public Sprite(TextureAsset texture, ScreenRectangle rectangle, Color color)
        {
            Texture = texture;
            Rectangle = rectangle;
            Color = color;
        }

        public Sprite(TextureAsset texture, ScreenRectangle rectangle) : this(texture, rectangle, Color.White)
        {
        }


        public void Draw()
        {
            if (!IsVisible)
                return;

            Draw(Texture.Dictionary, Texture.Name, Rectangle, Rotation, Color);
        }


        public static void Draw(TextureDictionary textureDictionary, string textureName, ScreenRectangle rectangle, float rotation, Color color)
        {
            Throw.IfNull(textureName, nameof(textureName));

            if (!textureDictionary.IsLoaded)
                textureDictionary.Load();

            NativeFunction.Natives.DrawSprite(textureDictionary.Name, textureName, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, rotation, color.R, color.G, color.B, color.A);
        }
    }
}

