namespace RAGENativeUI
{
#if RPH1
    extern alias rph1;
    using Vector3 = rph1::Rage.Vector3;
    using Vector4 = rph1::Rage.Vector4;
    using Matrix = rph1::Rage.Matrix;
    using Graphics = rph1::Rage.Graphics;
    using Game = rph1::Rage.Game;
#else
    /** REDACTED **/
#endif

    using System;
    using System.IO;
    using System.Text;
    
    public delegate void TypedEventHandler<TSender, TArgs>(TSender sender, TArgs e) where TArgs : EventArgs;

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


        // https://code.google.com/archive/p/slimmath/
        public static void TransformCoordinate(ref Vector3 coordinate, ref Matrix transform, out Vector3 result)
        {
            Vector4 vector = new Vector4
            {
                X = (coordinate.X * transform.M11) + (coordinate.Y * transform.M21) + (coordinate.Z * transform.M31) + transform.M41,
                Y = (coordinate.X * transform.M12) + (coordinate.Y * transform.M22) + (coordinate.Z * transform.M32) + transform.M42,
                Z = (coordinate.X * transform.M13) + (coordinate.Y * transform.M23) + (coordinate.Z * transform.M33) + transform.M43,
                W = 1f / ((coordinate.X * transform.M14) + (coordinate.Y * transform.M24) + (coordinate.Z * transform.M34) + transform.M44),
            };
            result = new Vector3(vector.X * vector.W, vector.Y * vector.W, vector.Z * vector.W);
        }
    }
}

