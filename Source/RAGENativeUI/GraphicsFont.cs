namespace RAGENativeUI
{
    using System.Drawing;
    
    using Graphics = Rage.Graphics;

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

        public /** REDACTED **/ Measure(string text)
        {
            Throw.IfNull(text, nameof(text));

            return /** REDACTED **/
        }
    }
}

