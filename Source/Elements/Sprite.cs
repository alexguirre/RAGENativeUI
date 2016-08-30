using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using Rage;
using Rage.Native;

namespace RAGENativeUI.Elements
{
    public class Sprite
    {
        public Point Position;
        public Size Size;
        public Color Color;
        public bool Visible;
        public float Heading;

        [Obsolete("Use Sprite.TextureDictionary instead")]
        public string TextureDict
        {
            get { return _textureDict; }
            set
            {
                _textureDict = value;
                if (!NativeFunction.CallByName<bool>("HAS_STREAMED_TEXTURE_DICT_LOADED", value))
                    NativeFunction.CallByName<uint>("REQUEST_STREAMED_TEXTURE_DICT", value, true);
            }
        }

        public string TextureDictionary
        {
            get { return _textureDict; }
            set
            {
                _textureDict = value;
                if (!NativeFunction.CallByName<bool>("HAS_STREAMED_TEXTURE_DICT_LOADED", value))
                    NativeFunction.CallByName<uint>("REQUEST_STREAMED_TEXTURE_DICT", value, true);
            }
        }

        public string TextureName;
        private string _textureDict;

        [Obsolete("Use Sprite.IsTextureDictionaryLoaded instead")]
        public bool IsTextureDictLoaded
        {
            get { return NativeFunction.CallByName<bool>("HAS_STREAMED_TEXTURE_DICT_LOADED", TextureDict); }
        }

        public bool IsTextureDictionaryLoaded
        {
            get { return NativeFunction.CallByName<bool>("HAS_STREAMED_TEXTURE_DICT_LOADED", TextureDictionary); }
        }

        [Obsolete("Use Sprite.LoadTextureDictionary() instead")]
        public void LoadTextureDict()
        {
            NativeFunction.CallByName<uint>("REQUEST_STREAMED_TEXTURE_DICT", TextureDict, true);
        }

        public void LoadTextureDictionary()
        {
            NativeFunction.CallByName<uint>("REQUEST_STREAMED_TEXTURE_DICT", TextureDictionary, true);
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
            if (!NativeFunction.CallByName<bool>("HAS_STREAMED_TEXTURE_DICT_LOADED", textureDict))
                NativeFunction.CallByName<uint>("REQUEST_STREAMED_TEXTURE_DICT", textureDict, true);
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
            int screenw = Game.Resolution.Width;
            int screenh = Game.Resolution.Height;
            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            var width = height * ratio;


            float w = (Size.Width / width);
            float h = (Size.Height / height);
            float x = (Position.X / width) + w * 0.5f;
            float y = (Position.Y / height) + h * 0.5f;

            NativeFunction.CallByName<uint>("DRAW_SPRITE", TextureDictionary, TextureName, x, y, w, h, Heading, Convert.ToInt32(Color.R), Convert.ToInt32(Color.G), Convert.ToInt32(Color.B), Convert.ToInt32(Color.A));
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
            var origRes = Game.Resolution;
            float aspectRaidou = origRes.Width / (float)origRes.Height;
            PointF pos = new PointF(position.X / (1080 * aspectRaidou), position.Y / 1080f);
            SizeF siz = new SizeF(size.Width / (1080 * aspectRaidou), size.Height / 1080f);
            graphics.DrawTexture(texture, pos.X * Game.Resolution.Width, pos.Y * Game.Resolution.Height, siz.Width * Game.Resolution.Width, siz.Height * Game.Resolution.Height);
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
