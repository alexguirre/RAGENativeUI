namespace RAGENativeUI
{
    using System;
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
        public static bool PrefersMetricMeasurements => N.ShouldUseMetricMeasurements();

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

        public static void SetText(string labelId, string value)
            => SetText(Game.GetHashKey(labelId ?? throw new ArgumentNullException(nameof(labelId))), value);

        public static void SetText(uint labelIdHash, string value)
        {
            if (!CTextFile.Available)
            {
                return;
            }

            // NOTE: entries only get freed when exiting the game or by calling ClearTextOverride
            CTextFile.CriticalSection.Enter();
            var oldValue = CTextFile.Instance.OverridesTextMap.AddOrSet(labelIdHash, ToUtf8(value));
            CTextFile.CriticalSection.Leave();

            if (oldValue != IntPtr.Zero)
            {
                unsafe { sysMemAllocator.TheAllocator.Free((void*)oldValue); }
            }
        }

        public static void ClearTextOverride(string labelId)
            => ClearTextOverride(Game.GetHashKey(labelId ?? throw new ArgumentNullException(nameof(labelId))));

        public static void ClearTextOverride(uint labelIdHash)
        {
            if (!CTextFile.Available)
            {
                return;
            }

            CTextFile.CriticalSection.Enter();
            var oldValue = CTextFile.Instance.OverridesTextMap.Remove(labelIdHash);
            CTextFile.CriticalSection.Leave();

            if (oldValue != IntPtr.Zero)
            {
                unsafe { sysMemAllocator.TheAllocator.Free((void*)oldValue); }
            }
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

        private static unsafe IntPtr ToUtf8(string str)
        {
            fixed (char* chars = str)
            {
                var size = Encoding.UTF8.GetByteCount(chars, str.Length) + 1; // str + null terminator
                var dest = (byte*)sysMemAllocator.TheAllocator.Allocate((ulong)size, 16, 0);
                Encoding.UTF8.GetBytes(chars, str.Length, dest, size - 1);
                dest[size - 1] = 0; // null terminator
                return (IntPtr)dest;
            }
        }
    }
}
