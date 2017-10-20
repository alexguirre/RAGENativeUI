namespace RAGENativeUI
{
    using System;
    using System.IO;
    using System.Text;

    using Rage;
    using Rage.Native;
    
    public delegate void TypedEventHandler<TSender, TArgs>(TSender sender, TArgs e) where TArgs : EventArgs;

    internal static class Common
    {
        public const string ResourcesFolder = @"RAGENativeUI Resources\";

        public const string ObjectDisposedExceptionMessage = "Cannot access a disposed object.";

        public static ObjectDisposedException NewDisposedException() => new ObjectDisposedException(null, ObjectDisposedExceptionMessage);

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

        public static string WrapText(string text, GraphicsFont font, float widthLimit) => WrapText(text, font.Name, font.Size, widthLimit);
        public static string WrapText(string text, string fontName, float fontSize, float widthLimit)
        {
            float GetTextWidth(string str) => Graphics.MeasureText(str, fontName, fontSize).Width;

            const char SplitChar = ' ';
            
            string[] words = text.Split(SplitChar);

            StringBuilder resultText = new StringBuilder();
            string currentLine = String.Empty;
            string lastWord = String.Empty;

            int index = 0;
            while (true)
            {
                string newString = (currentLine + SplitChar).TrimStart(SplitChar) + words[index];

                if (currentLine != String.Empty && GetTextWidth(newString) > widthLimit)
                {
                    // Word no longer fits in line.
                    resultText.Append(currentLine.TrimEnd() + "\n");
                    currentLine = String.Empty;
                }
                else
                {
                    lastWord = words[index];
                    index++;
                    currentLine = newString;
                }

                if (index == words.Length)
                {
                    resultText.Append(currentLine.TrimEnd() + "\n");
                    break;
                }
            }

            words = null;
            currentLine = null;
            lastWord = null;

            return resultText.ToString().TrimEnd();
        }

        public static void Log(object o) => Game.LogTrivial($"[RAGENativeUI] {o}");
        [System.Diagnostics.Conditional("DEBUG")]
        public static void LogDebug(object o) => Log(o);
    }
}

