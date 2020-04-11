namespace RAGENativeUI
{
    using System;
    using System.Text;

    /// <summary>
    /// Helper class for text command natives.
    /// </summary>
    internal static class TextCommands
    {
        private const string ShortStringFormat = "STRING";
        private const string LongStringFormat = "CELL_EMAIL_BCON";
        private const int MaxSubstringLength = 98;

        /// <summary>
        /// Gets the next substring of length <see cref="MaxSubstringLength"/> without splitting `~` tokens.
        /// </summary>
        private static unsafe bool NextSubstring(ref sbyte* src, sbyte* dest)
        {
            const char TokenAffix = '~';

            if (*src == 0)
            {
                return false;
            }

            int length = 0;
            int tokenStart = -1;
            sbyte* curr = src;
            sbyte* destCurr = dest;
            while (*curr != 0 && length < MaxSubstringLength)
            {
                sbyte c = *curr;

                if (c == TokenAffix)
                {
                    tokenStart = tokenStart  == -1 ? length : -1;
                }

                *(destCurr++) = c;
                curr++;
                length++;
            }
            *destCurr = 0;

            if (tokenStart != -1)
            {
                curr = &src[tokenStart];
                dest[tokenStart] = 0;
            }

            src = curr;

            return true;
        }

        private static unsafe void PushText(Action<string> beginCommand, string text)
        {
            if (Encoding.UTF8.GetByteCount(text) <= MaxSubstringLength)
            {
                beginCommand(ShortStringFormat);
                N.AddTextComponentSubstringPlayerName(text);
            }
            else
            {
                beginCommand(LongStringFormat);
                using (Rage.Native.NativeString s = new Rage.Native.NativeString(text))
                {
                    sbyte* src = s.Value;
                    sbyte* tmp = stackalloc sbyte[MaxSubstringLength + 1];

                    while (NextSubstring(ref src, tmp))
                    {
                        N.AddTextComponentSubstringPlayerName(new IntPtr(tmp));
                    }
                }
            }
        }

        public static float GetWidth(string text)
        {
            PushText(N.BeginTextCommandGetWidth, text);
            return N.EndTextCommandGetWidth(true);
        }

        public static void Display(string text, float x, float y)
        {
            PushText(N.BeginTextCommandDisplayText, text);
            N.EndTextCommandDisplayText(x, y);
        }

        public static int GetLineCount(string text, float x, float y)
        {
            PushText(N.BeginTextCommandGetLineCount, text);
            return N.EndTextCommandGetLineCount(x, y);
        }
    }
}
