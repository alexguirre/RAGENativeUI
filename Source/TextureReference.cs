namespace RAGENativeUI
{
    using Rage;

    public class TextureReference
    {
        public TextureDictionary Dictionary { get; }
        public string Name { get; }
        public int Width { get; }
        public int Height { get; }

        internal TextureReference(TextureDictionary dictionary, string name, int width, int height)
        {
            Dictionary = dictionary;
            Name = name;
            Width = width;
            Height = height;
        }
    }
}

