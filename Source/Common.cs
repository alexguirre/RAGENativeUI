namespace RAGENativeUI
{
    using System.IO;

    internal static class Common
    {
        public const string ResourcesFolder = @"RAGENativeUI Resources\";

        public static void EnsureResourcesFolder()
        {
            if (!Directory.Exists(ResourcesFolder))
                Directory.CreateDirectory(ResourcesFolder);
        }
    }
}

