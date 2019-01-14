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

    public class Rect3D
    {
        public bool IsVisible { get; set; } = true;
        public Color Color { get; set; }
        public Vector3 UpperLeft { get; set; }
        public Vector3 UpperRight { get; set; }
        public Vector3 BottomLeft { get; set; }
        public Vector3 BottomRight { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the back-face should be drawn. If <c>true</c> both front and back faces will be drawn, 
        /// otherwise only the face that heads to (UpperRight-UpperLeft)x(UpperLeft-BottomLeft) is drawn.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the back-face should be drawn; otherwise, <c>false</c>.
        /// </value>
        public bool BackFace { get; set; }

        public Rect3D(Vector3 upperLeft, Vector3 upperRight, Vector3 bottomLeft, Vector3 bottomRight, Color color)
        {
            UpperLeft = upperLeft;
            UpperRight = upperRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
            Color = color;
            BackFace = false;
        }

        public Rect3D(Vector3 upperLeft, Vector3 upperRight, Vector3 bottomLeft, Vector3 bottomRight)
            : this(upperLeft, upperRight, bottomLeft, bottomRight, Color.White)
        {
        }

        public Rect3D() : this(Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero, Color.White)
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

            Draw(UpperLeft, UpperRight, BottomLeft, BottomRight, Color, BackFace);
        }

        public static void Draw(Vector3 upperLeft, Vector3 upperRight, Vector3 bottomLeft, Vector3 bottomRight, Color color, bool backFace = false)
        {
            Vector3 ul = upperLeft, ur = upperRight, bl = bottomLeft, br = bottomRight;

            Debug.DrawLineDebug(ul, bl, Color.FromArgb(200, Color.Red));
            Debug.DrawLineDebug(bl, ur, Color.FromArgb(200, Color.Red));
            Debug.DrawLineDebug(ur, ul, Color.FromArgb(200, Color.Red));

            Debug.DrawLineDebug(bl, br, Color.FromArgb(200, Color.Blue));
            Debug.DrawLineDebug(br, ur, Color.FromArgb(200, Color.Blue));
            Debug.DrawLineDebug(ur, bl, Color.FromArgb(200, Color.Blue));

            Debug.DrawLineDebug(ul, ul + Vector3.Cross(ur - ul, ul - bl), Color.Green);

            N.DrawTriangle3D(ul, bl, ur, color.R, color.G, color.B, color.A);
            N.DrawTriangle3D(bl, br, ur, color.R, color.G, color.B, color.A);

            if (backFace)
            {
                N.DrawTriangle3D(ur, bl, ul, color.R, color.G, color.B, color.A);
                N.DrawTriangle3D(ur, br, bl, color.R, color.G, color.B, color.A);
            }
        }
    }
}

