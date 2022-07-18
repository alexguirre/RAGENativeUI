namespace RAGENativeUI.Elements
{
    using Rage;

    using System;
    using System.Drawing;

    public class Sprite3D
    {
        public bool IsVisible { get; set; } = true;
        public Color Color { get; set; }
        public TextureDictionary TextureDictionary { get; set; }
        public string TextureName { get; set; }
        public Vector3 UpperLeft { get; set; }
        public Vector3 UpperRight { get; set; }
        public Vector3 BottomLeft { get; set; }
        public Vector3 BottomRight { get; set; }
        public RectangleF UV { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the back-face should be drawn. If <c>true</c> both front and back faces will be drawn, 
        /// otherwise only the face that heads to (UpperLeft-UpperRight)x(UpperRight-BottomRight) is drawn.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the back-face should be drawn; otherwise, <c>false</c>.
        /// </value>
        public bool BackFace { get; set; }

        public Sprite3D(TextureDictionary textureDictionary, string textureName, Vector3 upperLeft, Vector3 upperRight, Vector3 bottomLeft, Vector3 bottomRight, Color color)
        {
            if (string.IsNullOrEmpty(textureName))
            {
                throw new ArgumentException("Texture name cannot be empty", nameof(textureName));
            }

            TextureDictionary = textureDictionary;
            TextureName = textureName;
            UpperLeft = upperLeft;
            UpperRight = upperRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
            Color = color;
            UV = new RectangleF(0.0f, 0.0f, 1.0f, 1.0f);
            BackFace = false;
        }

        public Sprite3D(TextureDictionary textureDictionary, string textureName, Vector3 upperLeft, Vector3 upperRight, Vector3 bottomLeft, Vector3 bottomRight)
            : this(textureDictionary, textureName, upperLeft, upperRight, bottomLeft, bottomRight, Color.White)
        {
        }

        public Sprite3D(TextureDictionary textureDictionary, string textureName)
            : this(textureDictionary, textureName, Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero, Color.White)
        {
        }

        public void SetTransform(Vector3 position, Vector2 size, Quaternion rotation) 
            => SetTransform(CreateTransformMatrix(position, size, rotation));

        public void SetTransform(Matrix transformationMatrix)
            => (UpperLeft, UpperRight, BottomLeft, BottomRight) = TransformCorners(transformationMatrix);

        public void Draw()
        {
            if (!IsVisible)
                return;

            Draw(TextureDictionary, TextureName, UpperLeft, UpperRight, BottomLeft, BottomRight, Color, UV, BackFace);
        }

        public static void Draw(TextureDictionary textureDictionary, string textureName, Vector3 upperLeft, Vector3 upperRight, Vector3 bottomLeft, Vector3 bottomRight, Color color, RectangleF uv, bool backFace = false, bool loadTexture = true)
        {
            if (string.IsNullOrEmpty(textureName))
            {
                throw new ArgumentException("Texture name cannot be empty", nameof(textureName));
            }

            if (loadTexture)
            {
                textureDictionary.Load();
                if (!textureDictionary.IsLoaded)
                {
                    return;
                }
            }

            Vector3 ul = upperLeft, ur = upperRight, bl = bottomLeft, br = bottomRight;

            N.DrawSpritePoly(ur, bl, ul, color.R, color.G, color.B, color.A, textureDictionary.Name, textureName, new(uv.Left, uv.Top, 1f), new(uv.Right, uv.Bottom, 1f), new(uv.Right, uv.Top, 1f));
            N.DrawSpritePoly(ur, br, bl, color.R, color.G, color.B, color.A, textureDictionary.Name, textureName, new(uv.Left, uv.Top, 1f), new(uv.Left, uv.Bottom, 1f), new(uv.Right, uv.Bottom, 1f));

            if (backFace)
            {
                N.DrawSpritePoly(ul, bl, ur, color.R, color.G, color.B, color.A, textureDictionary.Name, textureName, new(uv.Right, uv.Top, 1f), new(uv.Right, uv.Bottom, 1f), new(uv.Left, uv.Top, 1f));
                N.DrawSpritePoly(bl, br, ur, color.R, color.G, color.B, color.A, textureDictionary.Name, textureName, new(uv.Right, uv.Bottom, 1f), new(uv.Left, uv.Bottom, 1f), new(uv.Left, uv.Top, 1f));
            }
        }

        public static void Draw(TextureDictionary textureDictionary, string textureName, Matrix transformation, Color color, RectangleF uv, bool backFace = false, bool loadTexture = true)
        {
            var corners = TransformCorners(transformation);
            Draw(textureDictionary, textureName, corners.UpperLeft, corners.UpperRight, corners.BottomLeft, corners.BottomRight, color, uv, backFace, loadTexture);
        }

        public static void Draw(TextureDictionary textureDictionary, string textureName, Vector3 position, Vector2 size, Quaternion rotation, Color color, RectangleF uv, bool backFace = false, bool loadTexture = true)
            => Draw(textureDictionary, textureName, CreateTransformMatrix(position, size, rotation), color, uv, backFace, loadTexture);

        private static (Vector3 UpperLeft, Vector3 UpperRight, Vector3 BottomLeft, Vector3 BottomRight) TransformCorners(Matrix transformationMatrix)
            => (UpperLeft:   Common.TransformCoordinate(new(-1.0f, 0.0f, 1.0f), transformationMatrix),
                UpperRight:  Common.TransformCoordinate(new(1.0f, 0.0f, 1.0f), transformationMatrix),
                BottomLeft:  Common.TransformCoordinate(new(-1.0f, 0.0f, -1.0f), transformationMatrix),
                BottomRight: Common.TransformCoordinate(new(1.0f, 0.0f, -1.0f), transformationMatrix));

        private static Matrix CreateTransformMatrix(Vector3 position, Vector2 size, Quaternion rotation)
            => Matrix.Scaling(new Vector3(size.X, 1.0f, size.Y)) * Matrix.RotationQuaternion(rotation) * Matrix.Translation(position);
    }
}
