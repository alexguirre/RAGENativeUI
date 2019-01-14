namespace RAGENativeUI.Elements
{
#if RPH1
    extern alias rph1;
    using Vector3 = rph1::Rage.Vector3;
    using Vector2 = rph1::Rage.Vector2;
    using Matrix = rph1::Rage.Matrix;
    using Quaternion = rph1::Rage.Quaternion;
    using Debug = rph1::Rage.Debug;
#else
    /** REDACTED **/
#endif

    using System.Drawing;

    using Rage;
    
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
        public UVCoords UV { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the back-face should be drawn. If <c>true</c> both front and back faces will be drawn, 
        /// otherwise only the face that heads to (UpperRight-UpperLeft)x(UpperLeft-BottomLeft) is drawn.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the back-face should be drawn; otherwise, <c>false</c>.
        /// </value>
        public bool BackFace { get; set; }

        public Sprite3D(TextureDictionary textureDictionary, string textureName, Vector3 upperLeft, Vector3 upperRight, Vector3 bottomLeft, Vector3 bottomRight, Color color)
        {
            Throw.IfNull(textureName, nameof(textureName));

            TextureDictionary = textureDictionary;
            TextureName = textureName;
            UpperLeft = upperLeft;
            UpperRight = upperRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
            Color = color;
            UV = new UVCoords(0.0f, 0.0f, 1.0f, 1.0f);
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
        {
            SetTransform(Matrix.Scaling(new Vector3(size.X, 1.0f, size.Y)) * Matrix.RotationQuaternion(rotation) * Matrix.Translation(position));
        }

        public void SetTransform(Matrix transformationMatrix)
        {
            Vector3 v = new Vector3(-1.0f, 0.0f, 1.0f); // upper left
            Common.TransformCoordinate(ref v, ref transformationMatrix, out v);
            UpperLeft = v;

            v = new Vector3(1.0f, 0.0f, 1.0f); // upper right
            Common.TransformCoordinate(ref v, ref transformationMatrix, out v);
            UpperRight = v;

            v = new Vector3(-1.0f, 0.0f, -1.0f); // bottom left
            Common.TransformCoordinate(ref v, ref transformationMatrix, out v);
            BottomLeft = v;

            v = new Vector3(1.0f, 0.0f, -1.0f); // bottom right
            Common.TransformCoordinate(ref v, ref transformationMatrix, out v);
            BottomRight = v;
        }

        public void Draw()
        {
            if (!IsVisible)
                return;

            Draw(TextureDictionary, TextureName, UpperLeft, UpperRight, BottomLeft, BottomRight, Color, UV, BackFace);
        }

        public static void Draw(TextureDictionary textureDictionary, string textureName, Vector3 upperLeft, Vector3 upperRight, Vector3 bottomLeft, Vector3 bottomRight, Color color, UVCoords uv, bool backFace = false)
        {
            Throw.IfNull(textureName, nameof(textureName));

            if (!textureDictionary.IsLoaded)
                textureDictionary.Load();

            Vector3 ul = upperLeft, ur = upperRight, bl = bottomLeft, br = bottomRight;

            Debug.DrawLineDebug(ul, bl, Color.FromArgb(200, Color.Red));
            Debug.DrawLineDebug(bl, ur, Color.FromArgb(200, Color.Red));
            Debug.DrawLineDebug(ur, ul, Color.FromArgb(200, Color.Red));

            Debug.DrawLineDebug(bl, br, Color.FromArgb(200, Color.Blue));
            Debug.DrawLineDebug(br, ur, Color.FromArgb(200, Color.Blue));
            Debug.DrawLineDebug(ur, bl, Color.FromArgb(200, Color.Blue));

            Debug.DrawLineDebug(ul, ul + Vector3.Cross(ur - ul, ul - bl), Color.Green);

            N.DrawTriangle3D(ul, bl, ur, color.R, color.G, color.B, color.A, textureDictionary.Name, textureName, new Vector3(uv.U1, uv.V1, 1f), new Vector3(uv.U1, uv.V2, 1f), new Vector3(uv.U2, uv.V1, 1f));
            N.DrawTriangle3D(bl, br, ur, color.R, color.G, color.B, color.A, textureDictionary.Name, textureName, new Vector3(uv.U1, uv.V2, 1f), new Vector3(uv.U2, uv.V2, 1f), new Vector3(uv.U2, uv.V1, 1f));

            if (backFace)
            {
                // TODO: Sprite3D should back-face UV be flipped horizontally/vertically? or add the option via a property?
                //N.DrawTriangle3D(ur, bl, ul, color.R, color.G, color.B, color.A, textureDictionary.Name, textureName, new Vector3(-uv.U2, uv.V1, 1f), new Vector3(-uv.U1, uv.V2, 1f), new Vector3(-uv.U1, uv.V1, 1f));
                //N.DrawTriangle3D(ur, br, bl, color.R, color.G, color.B, color.A, textureDictionary.Name, textureName, new Vector3(-uv.U2, uv.V1, 1f), new Vector3(-uv.U2, uv.V2, 1f), new Vector3(-uv.U1, uv.V2, 1f));

                N.DrawTriangle3D(ur, bl, ul, color.R, color.G, color.B, color.A, textureDictionary.Name, textureName, new Vector3(uv.U2, uv.V1, 1f), new Vector3(uv.U1, uv.V2, 1f), new Vector3(uv.U1, uv.V1, 1f));
                N.DrawTriangle3D(ur, br, bl, color.R, color.G, color.B, color.A, textureDictionary.Name, textureName, new Vector3(uv.U2, uv.V1, 1f), new Vector3(uv.U2, uv.V2, 1f), new Vector3(uv.U1, uv.V2, 1f));
            }
        }
    }
}

