namespace RAGENativeUI
{
    using System.IO;

    using Rage;
    using Rage.Native;

    internal static class Common
    {
        public const string ResourcesFolder = @"RAGENativeUI Resources\";

        public static void EnsureResourcesFolder()
        {
            if (!Directory.Exists(ResourcesFolder))
                Directory.CreateDirectory(ResourcesFolder);
        }

        public static float GetFontHeight(string fontName, float fontSize)
        {
            return Graphics.MeasureText("A", fontName, fontSize).Height;
        }

        public static void PlaySoundFrontend(string soundSet, string soundName)
        {
            NativeFunction.Natives.PlaySoundFrontend(-1, soundName, soundSet, false);
        }
    }
}

