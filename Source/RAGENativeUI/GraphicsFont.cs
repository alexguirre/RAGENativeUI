namespace RAGENativeUI
{
#if RPH1
    extern alias rph1;
    using Graphics = rph1::Rage.Graphics;
#else
    /** REDACTED **/
#endif

    using System.Drawing;

    public struct GraphicsFont
    {
        private float height;

        public string Name { get; }
        public float Size { get; }
        public float Height
        {
            get
            {
                if (height == -1.0f)
                    height = Common.GetFontHeight(Name, Size);
                return height;
            }
        }

        public GraphicsFont(string name, float size)
        {
            Throw.IfNull(name, nameof(name));

            Name = name;
            Size = size;
            height = -1.0f;
        }

        public RectangleF Measure(string text)
        {
            Throw.IfNull(text, nameof(text));
#if RPH1
            return new RectangleF(PointF.Empty, Graphics.MeasureText(text, Name, Size));
#else
            /** REDACTED **/
#endif
        }
    }
}

