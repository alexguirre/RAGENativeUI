namespace RAGENativeUI.Rendering
{
    using System.Drawing;
    
    using Graphics = Rage.Graphics;

    public struct Font
    {
        public string Name { get; }
        public float Size { get; }
        private float height;
        public float Height
        {
            get
            {
                if (height == -1.0f)
                    height = Common.GetFontHeight(Name, Size);
                return height;
            }
        }

        public Font(string name, float size)
        {
            Throw.IfNull(name, nameof(name));

            Name = name;
            Size = size;
            height = -1.0f;
        }

        public SizeF Measure(string text)
        {
            Throw.IfNull(text, nameof(text));

            return Graphics.MeasureText(text, Name, Size);
        }
    }
}

