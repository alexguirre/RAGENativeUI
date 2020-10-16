namespace RAGENativeUI
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    using Rage;
    using RAGENativeUI.Internals;

    public enum Language
    {
        English = 0,
        French = 1,
        German = 2,
        Italian = 3,
        Spanish = 4,
        BrazilianPortuguese = 5,
        Polish = 6,
        Russian = 7,
        Korean = 8,
        TraditionalChinese = 9,
        Japanese = 10,
        MexicanSpanish = 11,
        SimplifiedChinese = 12,
    }

    public static class Localization
    {
        public static Language SystemLanguage => (Language)N.GetSystemLanguage();
        public static Language Language => (Language)N.GetCurrentLanguage();

        public static bool DoesTextExist(string labelId)
        {
            labelId = labelId ?? throw new ArgumentNullException(nameof(labelId));
            return CTextFile.Available ? DoesTextExist(Game.GetHashKey(labelId)) : N.DoesTextLabelExist(labelId);
        }

        public static bool DoesTextExist(uint labelIdHash)
            => !CTextFile.Available || CTextFile.Instance.GetStringByHash(labelIdHash) != IntPtr.Zero;

        public static string GetText(string labelId)
        {
            labelId = labelId ?? throw new ArgumentNullException(nameof(labelId));
            return CTextFile.Available ? GetText(Game.GetHashKey(labelId)) : FromUtf8(N.GetLabelText(labelId));
        }

        public static string GetText(uint labelIdHash)
        {
            const string Fallback = "NULL";
            if (!CTextFile.Available)
            {
                return Fallback;
            }

            var ptr = CTextFile.Instance.GetStringByHash(labelIdHash);
            return ptr != IntPtr.Zero ? FromUtf8(ptr) : Fallback;
        }

        public static unsafe void SetText(uint labelIdHash, string value)
        {
            if (!CTextFile.Available)
            {
                return;
            }

            // TODO: do this properly...
            ref var map = ref CTextFile.Instance.OverridesTextMap;
            map.IsSorted = true;
            map.Pairs.Count = 1;
            map.Pairs.Size = 10;
            map.Pairs.Items = (CTextFile.atBinaryMap.DataPair*)Marshal.AllocHGlobal(sizeof(CTextFile.atBinaryMap.DataPair) * 10);
            map.Pairs[0] = new CTextFile.atBinaryMap.DataPair { Key = labelIdHash, Value = Marshal.StringToHGlobalAnsi(value) };
        }

        private static unsafe string FromUtf8(IntPtr ptr)
        {
            var p = (byte*)ptr;
            return Encoding.UTF8.GetString(p, StrLen(p));

            static int StrLen(byte* str)
            {
                int len = 0;
                while (str[len] != 0)
                {
                    len++;
                }
                return len;
            }
        }
    }
}
