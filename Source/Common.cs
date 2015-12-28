using System;
using Rage;
using Rage.Native;

namespace RAGENativeUI
{
    public static class Common
    {
        /// <summary>
        /// Fonts used by GTA V
        /// </summary>
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

        /// <summary>
        /// Check if a Rage.GameControl is pressed while it's disabled
        /// </summary>
        /// <param name="index"></param>
        /// <param name="control"></param>
        /// <returns>true if a Rage.GameControl is pressed while it's disabled</returns>
        public static bool IsDisabledControlPressed(int index, GameControl control)
        {
            return NativeFunction.CallByName<bool>("IS_DISABLED_CONTROL_PRESSED", index, (int)control);
        }

        /// <summary>
        /// Check if a Rage.GameControl is just pressed while it's disabled
        /// </summary>
        /// <param name="index"></param>
        /// <param name="control"></param>
        /// <returns>true if a Rage.GameControl is just pressed while it's disabled</returns>
        public static bool IsDisabledControlJustPressed(int index, GameControl control)
        {
            return NativeFunction.CallByName<bool>("IS_DISABLED_CONTROL_JUST_PRESSED", index, (int)control);
        }

        /// <summary>
        /// Check if a Rage.GameControl is just released while it's disabled
        /// </summary>
        /// <param name="index"></param>
        /// <param name="control"></param>
        /// <returns>true if a Rage.GameControl is just released while it's disabled</returns>
        public static bool IsDisabledControlJustReleased(int index, GameControl control)
        {
            return NativeFunction.CallByName<bool>("IS_DISABLED_CONTROL_JUST_RELEASED", index, (int)control);
        }

        public static Keys GetKeyDown()
        {
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
                if (Game.IsKeyDown(key)) return key;
            return Keys.None;
        }

        public static Keys GetKeyDownRightNow()
        {
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
                if (Game.IsKeyDownRightNow(key)) return key;
            return Keys.None;
        }
    }
}
