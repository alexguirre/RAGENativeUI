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

        /// <summary>
        /// Gets whether the measurement system setting is assigned to metric instead of imperial.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the metric system is preferred;
        /// otherwise, <c>false</c> and the imperial system is preferred.
        /// </returns>
        public static bool PrefersMetricMeasurements => N.ShouldUseMetricMeasurements();

        /// <summary>
        /// Gets whether the specified text label is valid.
        /// </summary>
        /// <param name="labelId">The text label ID.</param>
        /// <returns><c>true</c> if the text label is valid; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="labelId"/> is <c>null</c>.</exception>
        public static bool DoesTextExist(string labelId)
        {
            labelId = labelId ?? throw new ArgumentNullException(nameof(labelId));
            return CTextFile.Available ? DoesTextExist(Game.GetHashKey(labelId)) : N.DoesTextLabelExist(labelId);
        }

        /// <summary>
        /// Gets whether the specified text label is valid.
        /// </summary>
        /// <param name="labelIdHash">The hash of the text label ID.</param>
        /// <returns><c>true</c> if the text label is valid; otherwise, <c>false</c>.</returns>
        public static bool DoesTextExist(uint labelIdHash)
            => !CTextFile.Available || CTextFile.Instance.GetStringByHash(labelIdHash) != IntPtr.Zero;

        /// <summary>
        /// Gets the <see cref="string"/> of a text label.
        /// </summary>
        /// <param name="labelId">The text label ID.</param>
        /// <returns>The <see cref="string"/> of the text label if it exists; otherwise, <c>"NULL"</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="labelId"/> is <c>null</c>.</exception>
        public static string GetText(string labelId)
        {
            labelId = labelId ?? throw new ArgumentNullException(nameof(labelId));
            return CTextFile.Available ? GetText(Game.GetHashKey(labelId)) : FromUtf8(N.GetLabelText(labelId));
        }

        /// <summary>
        /// Gets the <see cref="string"/> of a text label.
        /// </summary>
        /// <param name="labelIdHash">The hash of the text label ID.</param>
        /// <returns>The <see cref="string"/> of the text label if it exists; otherwise, <c>"NULL"</c>.</returns>
        public static string GetText(uint labelIdHash)
        {
            // same fallback as _GET_LABEL_TEXT (0x7B5280EBA9840C72)
            const string Fallback = "NULL";

            if (!CTextFile.Available)
            {
                return Fallback;
            }

            var ptr = CTextFile.Instance.GetStringByHash(labelIdHash);
            return ptr != IntPtr.Zero ? FromUtf8(ptr) : Fallback;
        }

        /// <summary>
        /// Sets the <see cref="string"/> of a text label.
        /// If the text label already exists, its value gets overwritten;
        /// otherwise, a new text label with the specified value is created.
        /// </summary>
        /// <param name="labelId">The text label ID.</param>
        /// <param name="value">The new <see cref="string"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="labelId"/> is <c>null</c>.</exception>
        public static void SetText(string labelId, string value)
            => SetText(Game.GetHashKey(labelId ?? throw new ArgumentNullException(nameof(labelId))), value);

        /// <summary>
        /// Sets the <see cref="string"/> of a text label.
        /// If the text label already exists, its value gets overwritten;
        /// otherwise, a new text label with the specified value is create.
        /// </summary>
        /// <param name="labelIdHash">The hash of the text label ID.</param>
        /// <param name="value">The new <see cref="string"/>.</param>
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

        /// <summary>
        /// Removes the text label value previously set with <see cref="SetText(string, string)"/> or <see cref="SetText(uint, string)"/>.
        /// If the text label was initially loaded from a .gxt2 file, its value is changed back to its original <see cref="string"/>;
        /// otherwise, the text label is deleted.
        /// </summary>
        /// <param name="labelId">The text label ID.</param>
        /// <exception cref="ArgumentNullException"><paramref name="labelId"/> is <c>null</c>.</exception>
        public static void ClearTextOverride(string labelId)
            => ClearTextOverride(Game.GetHashKey(labelId ?? throw new ArgumentNullException(nameof(labelId))));

        /// <summary>
        /// Removes the text label value previously set with <see cref="SetText(string, string)"/> or <see cref="SetText(uint, string)"/>.
        /// If the text label was initially loaded from a .gxt2 file, its value is changed back to its original <see cref="string"/>;
        /// otherwise, the text label is deleted.
        /// </summary>
        /// <param name="labelIdHash">The hash of the text label ID.</param>
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
