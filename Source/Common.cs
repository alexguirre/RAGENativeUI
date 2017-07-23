namespace RAGENativeUI
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using Rage;
    using Rage.Native;

    using RAGENativeUI.Utility;

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

        public static string WrapText(string text, Font font, float widthLimit) => WrapText(text, font.Name, font.Size, widthLimit);
        public static string WrapText(string text, string fontName, float fontSize, float widthLimit)
        {
            float GetTextWidth(string str)
            {
                return Graphics.MeasureText(str, fontName, fontSize).Width;
            }


            List<dynamic> words = new List<dynamic>(text.Split(' ').Select(x => new { SplitChar = " ", Word = x }));

            StringBuilder resultText = new StringBuilder();
            string currentLine = String.Empty;
            dynamic lastWord = new { Word = String.Empty, SplitChar = String.Empty };

            while (true)
            {
                string newString = (currentLine + lastWord.SplitChar).TrimStart(' ') + words[0].Word;

                if (currentLine != String.Empty && GetTextWidth(newString) > widthLimit)
                {
                    // Word no longer fits in line.
                    resultText.Append(currentLine.TrimEnd() + "\n");
                    currentLine = String.Empty;
                }
                else
                {
                    lastWord = words[0];
                    words.RemoveAt(0);
                    currentLine = newString;
                }

                if (words.Count == 0)
                {
                    resultText.Append(currentLine.TrimEnd() + "\n");
                    break;
                }
            }

            return resultText.ToString().TrimEnd();
        }
    }
}

