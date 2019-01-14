namespace RAGENativeUI
{
#if RPH1
    extern alias rph1;
    using GameControl = rph1::Rage.GameControl;
#else
    /** REDACTED **/
#endif

    using System.Windows.Forms;
    using System.Drawing;

    /// <summary>
    /// Compatibility layer between RPH1 and RPH2.
    /// </summary>
    internal static class RPH
    {
#if RPH1
        public const bool IsRPH1 = true;
#else
        /** REDACTED **/
#endif
        public static class Game
        {
            public static uint GameTime
            {
                get
                {
#if RPH1
                    return rph1::Rage.Game.GameTime;
#else
                    /** REDACTED **/
#endif
                }
            }

            public static Size Resolution
            {
                get
                {
#if RPH1
                    return rph1::Rage.Game.Resolution;
#else
                    /** REDACTED **/
#endif
                }
            }

            public static uint GetHashKey(string text)
            {
#if RPH1
                return rph1::Rage.Game.GetHashKey(text);
#else
                /** REDACTED **/
#endif
            }

            public static bool WasKeyJustPressed(Keys key)
            {
#if RPH1
                return rph1::Rage.Game.IsKeyDown(key);
#else
                /** REDACTED **/
#endif
            }

            public static bool IsKeyDown(Keys key)
            {
#if RPH1
                return rph1::Rage.Game.IsKeyDownRightNow(key);
#else
                /** REDACTED **/
#endif
            }

            public static bool WasControlActionJustPressed(int index, GameControl control, bool evenIfDisabled = false)
            {
#if RPH1
                return evenIfDisabled ? N.IsDisabledControlJustPressed(index, (int)control) : rph1::Rage.Game.IsControlJustPressed(index, control);
#else
                /** REDACTED **/
#endif
            }

            public static string GetLocalizedString(string localizationStringId)
            {
#if RPH1
                return rph1::Rage.Game.GetLocalizedString(localizationStringId);
#else
                /** REDACTED **/
#endif
            }

            public static System.IntPtr FindPattern(string pattern)
            {
#if RPH1
                return rph1::Rage.Game.FindPattern(pattern);
#else
                /** REDACTED **/
#endif
            }
        }

        public static class MathHelper
        {
            public static T Clamp<T>(T value, T minimum, T maximum) where T : System.IComparable<T>
            {
#if RPH1
                return rph1::Rage.MathHelper.Clamp(value, minimum, maximum);
#else
                /** REDACTED **/
#endif
            }

            public static T Max<T>(params T[] values)
            {
#if RPH1
                return rph1::Rage.MathHelper.Max(values);
#else
                /** REDACTED **/
#endif
            }
        }

        public static class GameFiber
        {
            public static void Yield()
            {
#if RPH1
                rph1::Rage.GameFiber.Yield();
#else
                /** REDACTED **/
#endif
            }
        }
    }
}

