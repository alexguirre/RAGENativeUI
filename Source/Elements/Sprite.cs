namespace RAGENativeUI.Elements
{
    using System;
    using System.Drawing;

    using Rage;
    using Rage.Native;

    public class Sprite
    {
        private string textureName;

        public TextureDictionary TextureDictionary { get; set; }
        /// <exception cref="ArgumentNullException">When setting the property to a null value.</exception>
        public string TextureName { get { return textureName; } set { Throw.IfNull(value, nameof(value)); textureName = value; } }
        public SizeF Resolution
        {
            get
            {
                Vector3 tmp = NativeFunction.Natives.GetTextureResolution<Vector3>(TextureDictionary.Name, TextureName);
                return new SizeF(tmp.X, tmp.Y);
            }
        }
        public ScreenRectangle Rectangle { get; set; }
        public float Rotation { get; set; }
        public Color Color { get; set; }
        public bool IsVisible { get; set; } = true;

        public Sprite(TextureDictionary textureDictionary, string textureName, ScreenRectangle rectangle, Color color)
        {
            Throw.IfNull(textureName, nameof(textureName));

            TextureDictionary = textureDictionary;
            TextureName = textureName;
            Rectangle = rectangle;
            Color = color;
        }

        public Sprite(TextureDictionary textureDictionary, string textureName, ScreenRectangle rectangle) : this(textureDictionary, textureName, rectangle, Color.White)
        {
        }


        public void Draw()
        {
            if (!IsVisible)
                return;

            Draw(TextureDictionary, TextureName, Rectangle, Rotation, Color);
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

