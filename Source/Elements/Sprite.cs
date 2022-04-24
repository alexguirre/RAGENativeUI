using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using Rage;
using Rage.Native;

using RAGENativeUI.Internals;

namespace RAGENativeUI.Elements
{
    public class Sprite
    {
        public Point Position;
        public Size Size;
        public Color Color;
        public bool Visible;
        public float Heading;

        [Obsolete("Use Sprite.TextureDictionary instead.")]
        public string TextureDict
        {
            get { return _textureDict; }
            set
            {
                _textureDict = value;
                if (!IsTextureDictionaryLoaded)
                    LoadTextureDictionary();
            }
        }

        public string TextureDictionary
        {
            get { return _textureDict; }
            set
            {
                _textureDict = value;
                if (!IsTextureDictionaryLoaded)
                    LoadTextureDictionary();
            }
        }

        public string TextureName;
        private string _textureDict;

        [Obsolete("Use Sprite.IsTextureDictionaryLoaded instead.")]
        public bool IsTextureDictLoaded
        {
            get { return IsTextureDictionaryLoaded; }
        }

        public bool IsTextureDictionaryLoaded
        {
            get { return HasTextureDictionaryLoaded(_textureDict); }
        }

        [Obsolete("Use Sprite.LoadTextureDictionary() instead.")]
        public void LoadTextureDict()
        {
            LoadTextureDictionary();
        }

        public void LoadTextureDictionary()
        {
            RequestTextureDictionary(_textureDict);
        }

        /// <summary>
        /// Creates a game sprite object from a texture dictionary and texture name.
        /// </summary>
        /// <param name="textureDict"></param>
        /// <param name="textureName"></param>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="heading"></param>
        /// <param name="color"></param>
        public Sprite(string textureDict, string textureName, Point position, Size size, float heading, Color color)
        {
            TextureDictionary = textureDict;
            TextureName = textureName;

            Position = position;
            Size = size;
            Heading = heading;
            Color = color;
            Visible = true;
        }

        /// <summary>
        /// Creates a game sprite object from a texture dictionary and texture name.
        /// </summary>
        /// <param name="textureDict"></param>
        /// <param name="textureName"></param>
        /// <param name="position"></param>
        /// <param name="size"></param>
        public Sprite(string textureDict, string textureName, Point position, Size size) : this(textureDict, textureName, position, size, 0f, Color.FromArgb(255, 255, 255, 255))
        {
        }


        /// <summary>
        /// Draws the sprite on a 1080-pixels height base.
        /// </summary>
        public void Draw()
        {
            if (!Visible) return;

            Draw(_textureDict, TextureName, Position, Size, Heading, Color, false);
        }

        private static void RequestTextureDictionary(string textureDictionary)
        {
            if (textureDictionary.StartsWith("embed:"))
            {
                // nothing, the user is in charge of loading the model
            }
            else
            {
                N.RequestStreamedTextureDict(textureDictionary);
            }
        }

        private static bool HasTextureDictionaryLoaded(string textureDictionary)
        {
            if (textureDictionary.StartsWith("embed:"))
            {
                // nothing, the user is in charge of loading the model, assume it is loaded
                return true;
            }
            else
            {
                return N.HasStreamedTextureDictLoaded(textureDictionary);
            }
        }

        public static void Draw(string textureDictionary, string textureName, Point position, Size size, float heading, Color color, bool loadTexture = true)
        {
            if (loadTexture)
            {
                RequestTextureDictionary(textureDictionary);
                if (!HasTextureDictionaryLoaded(textureDictionary))
                {
                    return;
                }
            }

            var res = Screen.ActualResolution;
            const float height = 1080f;
            float ratio = res.Width / res.Height;
            var width = height * ratio;


            float w = (size.Width / width);
            float h = (size.Height / height);
            float x = (position.X / width) + w * 0.5f;
            float y = (position.Y / height) + h * 0.5f;

            N.DrawSprite(textureDictionary, textureName, x, y, w, h, heading, color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// Draw a custom texture from a file on a 1080-pixels height base.
        /// </summary>
        /// <param name="texture">Your custom texture object.</param>
        /// <param name="position"></param>
        /// <param name="size"></param>
        [Obsolete("The Sprite.DrawTexture() overload that accepts a GraphicsEventArgs instance will be removed soon, use the Sprite.DrawTexture() overload that accepts a Graphics instance instead.")]
        public static void DrawTexture(Texture texture, Point position, Size size, GraphicsEventArgs canvas)
        {
            var origRes = Game.Resolution;
            float aspectRaidou = origRes.Width / (float)origRes.Height;
            PointF pos = new PointF(position.X / (1080 * aspectRaidou), position.Y / 1080f);
            SizeF siz = new SizeF(size.Width / (1080 * aspectRaidou), size.Height / 1080f);
            canvas.Graphics.DrawTexture(texture, pos.X * Game.Resolution.Width, pos.Y * Game.Resolution.Height, siz.Width * Game.Resolution.Width, siz.Height * Game.Resolution.Height);
        }

        /// <summary>
        /// Draws a custom texture from a file on a 1080-pixels height base.
        /// </summary>
        /// <param name="texture">Your custom texture object.</param>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="graphics"></param>
        public static void DrawTexture(Texture texture, Point position, Size size, Rage.Graphics graphics)
        {
            var origRes = Screen.ActualResolution;
            float aspectRatio = origRes.Width / (float)origRes.Height;
            PointF pos = new PointF(position.X / (1080 * aspectRatio), position.Y / 1080f);
            SizeF siz = new SizeF(size.Width / (1080 * aspectRatio), size.Height / 1080f);
            graphics.DrawTexture(texture, pos.X * origRes.Width, pos.Y * origRes.Height, siz.Width * origRes.Width, siz.Height * origRes.Height);
        }

        /// <summary>
        /// Save an embedded resource to a temporary file.
        /// </summary>
        /// <param name="yourAssembly">Your executing assembly.</param>
        /// <param name="fullResourceName">Resource name including your solution name. E.G MyMenuMod.banner.png</param>
        /// <returns>Absolute path to the written file.</returns>
        [Obsolete("Sprite.WriteFileFromResources() will be removed soon, use Common.WriteFileFromResources() instead.")]
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
        [Obsolete("Sprite.WriteFileFromResources() will be removed soon, use Common.WriteFileFromResources() instead.")]
        public static string WriteFileFromResources(Assembly yourAssembly, string fullResourceName, string savePath)
        {
            using (Stream stream = yourAssembly.GetManifestResourceStream(fullResourceName))
            {
                if (stream != null)
                {
                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, Convert.ToInt32(stream.Length));

                    using (FileStream fileStream = File.Create(savePath))
                    {
                        fileStream.Write(buffer, 0, Convert.ToInt32(stream.Length));
                        fileStream.Close();
                    }
                }
            }
            return Path.GetFullPath(savePath);
        }
    }
}

