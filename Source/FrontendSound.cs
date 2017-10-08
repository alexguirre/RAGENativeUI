namespace RAGENativeUI
{
    using System;

    public class FrontendSound
    {
        private string set;
        public string Set { get { return set; } set { Throw.IfNull(value, nameof(value)); set = value; } }
        private string name;
        public string Name { get { return name; } set { Throw.IfNull(value, nameof(value)); name = value; } }

        public FrontendSound(string set, string name)
        {
            Throw.IfNull(set, nameof(set));
            Throw.IfNull(name, nameof(name));

            Set = set;
            Name = name;
        }

        public void Play() => Common.PlaySoundFrontend(Set, Name);
    }
}

