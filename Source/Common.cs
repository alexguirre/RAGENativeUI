using System;
using Rage;
using Rage.Native;

namespace RAGENativeUI
{
    public static class Common
    {
        public enum EFont
        {
            ChaletLondon = 0,
            HouseScript = 1,
            Monospace = 2,
            ChaletComprimeCologne = 4,
            Pricedown = 7
        }

        public enum MenuControls
        {
            Up,
            Down,
            Left,
            Right,
            Select,
            Back
        }


        public static void PlaySound(string soundFile, string soundSet)
        {
            NativeFunction.CallByName<uint>("PLAY_SOUND_FRONTEND", -1, soundFile, soundSet, false);
        }


        public static bool IsDisabledControlPressed(int index, GameControl control)
        {
            return NativeFunction.CallByName<bool>("IS_DISABLED_CONTROL_PRESSED", index, (int)control);
        }


        public static bool IsDisabledControlJustPressed(int index, GameControl control)
        {
            return NativeFunction.CallByName<bool>("IS_DISABLED_CONTROL_JUST_PRESSED", index, (int)control);
        }


        public static bool IsDisabledControlJustReleased(int index, GameControl control)
        {
            return NativeFunction.CallByName<bool>("IS_DISABLED_CONTROL_JUST_RELEASED", index, (int)control);
        }
    }
}

