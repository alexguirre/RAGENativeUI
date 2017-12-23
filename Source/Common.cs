using System.Windows.Forms;
using Rage;
using Rage.Native;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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

        internal static void DisableControl(int index, GameControl control)
        {
            Game.DisableControlAction(index, control, true);
        }

        internal static void EnableControl(int index, GameControl control)
        {
            NativeFunction.Natives.EnableControlAction(index, (int)control, true);
        }

        internal static float GetControlNormal(int index, GameControl control)
        {
            return NativeFunction.Natives.GetControlNormal<float>(index, (int)control);
        }


        internal static bool IsTextureDictionaryLoaded(string name)
        {
            return NativeFunction.Natives.HasStreamedTextureDictLoaded<bool>(name);
        }

        internal static void LoadTextureDictionary(string name)
        {
            NativeFunction.Natives.RequestStreamedTextureDict(name, true);
        }

        public static void PlaySound(string soundFile, string soundSet)
        {
            NativeFunction.Natives.PlaySoundFrontend(-1, soundFile, soundSet, false);
        }

        /// <summary>
        /// Check if a Rage.GameControl is pressed while it's disabled
        /// </summary>
        /// <param name="index"></param>
        /// <param name="control"></param>
        /// <returns>true if a Rage.GameControl is pressed while it's disabled</returns>
        public static bool IsDisabledControlPressed(int index, GameControl control)
        {
            return NativeFunction.Natives.IsDisabledControlPressed<bool>(index, (int)control);
        }

        /// <summary>
        /// Check if a Rage.GameControl is just pressed while it's disabled
        /// </summary>
        /// <param name="index"></param>
        /// <param name="control"></param>
        /// <returns>true if a Rage.GameControl is just pressed while it's disabled</returns>
        public static bool IsDisabledControlJustPressed(int index, GameControl control)
        {
            return NativeFunction.Natives.IsDisabledControlJustPressed<bool>(index, (int)control);
        }

        /// <summary>
        /// Check if a Rage.GameControl is just released while it's disabled
        /// </summary>
        /// <param name="index"></param>
        /// <param name="control"></param>
        /// <returns>true if a Rage.GameControl is just released while it's disabled</returns>
        public static bool IsDisabledControlJustReleased(int index, GameControl control)
        {
            return NativeFunction.Natives.IsDisabledControlJustReleased<bool>(index, (int)control);
        }

        /// <summary>
        /// Gets the current pressed keys.
        /// </summary>
        /// <returns>A <see cref="ICollection{T}"/> with the current pressed keys.</returns>
        public static ICollection<Keys> GetPressedKeys()
        {
            KeyboardState keyboard = Game.GetKeyboardState();

            return keyboard == null ? null : keyboard.PressedKeys;
        }

        /// <summary>
        /// Save an embedded resource to a temporary file.
        /// </summary>
        /// <param name="yourAssembly">Your executing assembly.</param>
        /// <param name="fullResourceName">Resource name including your solution name. E.G MyMenuMod.banner.png</param>
        /// <returns>Absolute path to the written file.</returns>
        public static string WriteFileFromResources(Assembly yourAssembly, string fullResourceName)
        {
            string tmpPath = Path.GetTempFileName();
            return WriteFileFromResources(yourAssembly, fullResourceName, tmpPath);
        }

        /// <summary>
        /// Save an embedded resource to a concrete path.
        /// </summary>
        /// <param name="yourAssembly">Your executing assembly.</param>
        /// <param name="fullResourceName">Resource name including your solution name. E.G MyMenuMod.banner.png</param>
        /// <param name="savePath">Path to where save the file, including the filename.</param>
        /// <returns>Absolute path to the written file.</returns>
        public static string WriteFileFromResources(Assembly yourAssembly, string fullResourceName, string savePath)
        {
            using (Stream stream = yourAssembly.GetManifestResourceStream(fullResourceName))
            {
                if (stream != null)
                {
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, System.Convert.ToInt32(stream.Length));

                    using (FileStream fileStream = File.Create(savePath))
                    {
                        fileStream.Write(buffer, 0, System.Convert.ToInt32(stream.Length));
                        fileStream.Close();
                    }
                }
            }
            return Path.GetFullPath(savePath);
        }
    }
}