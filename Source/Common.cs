using System.Windows.Forms;
using Rage;
using Rage.Native;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Drawing;

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


        /// <param name="min">Maximum value, inclusive</param>
        /// <param name="max">Maximum value, exclusive</param>
        internal static int Wrap(int value, int min, int max)
        {
            if (value < min)
                value = (max - (min - value) % (max - min)) % max;
            else
                value = min + (value - min) % (max - min);

            return value;
        }

        internal static RectangleF GetScriptGfxRect(RectangleF rect)
        {
            float x1 = rect.Left, y1 = rect.Top;
            float x2 = rect.Right, y2 = rect.Bottom;
            N.GetScriptGfxPosition(x1, y1, out x1, out y1);
            N.GetScriptGfxPosition(x2, y2, out x2, out y2);

            return new RectangleF(x1, y1, x2 - x1, y2 - y1);
        }

        internal static Vector3 TransformCoordinate(in Vector3 coordinate, in Matrix transform)
        {
            // From SlimMath's Vector3.TransformCoordinate: https://code.google.com/archive/p/slimmath/

            var x = (coordinate.X * transform.M11) + (coordinate.Y * transform.M21) + (coordinate.Z * transform.M31) + transform.M41;
            var y = (coordinate.X * transform.M12) + (coordinate.Y * transform.M22) + (coordinate.Z * transform.M32) + transform.M42;
            var z = (coordinate.X * transform.M13) + (coordinate.Y * transform.M23) + (coordinate.Z * transform.M33) + transform.M43;
            var w = 1f / ((coordinate.X * transform.M14) + (coordinate.Y * transform.M24) + (coordinate.Z * transform.M34) + transform.M44);

            return new(x * w, y * w, z * w);
        }
    }
}
