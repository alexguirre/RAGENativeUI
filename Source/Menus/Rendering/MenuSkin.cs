namespace RAGENativeUI.Menus.Rendering
{
    using System.IO;
    using System.Drawing;
    using System.Reflection;

    using Rage;
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Rendering;
    using RAGENativeUI.Utility;

    public class MenuSkin : ISkin
    {
        public Texture Image { get; }

        public virtual UVCoords BannerCoords { get; } = new UVCoords(0f, 0f, 0.5f, 0.125f);
        public virtual UVCoords BackgroundCoords { get; } = new UVCoords(0.5f, 0f, 1f, 0.5f);
        public virtual UVCoords SelectedGradientCoords { get; } = new UVCoords(0f, 0.125f, 0.5f, 0.1875f);

        public MenuSkin(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException($"The file '{fileName}' wasn't found, can't create menu skin.");

            Image = Game.CreateTextureFromFile(fileName);
        }

        public virtual void DrawBanner(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), BannerCoords.U1, BannerCoords.V1, BannerCoords.U2, BannerCoords.V2);
        }

        public virtual void DrawBackground(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), BackgroundCoords.U1, BackgroundCoords.V1, BackgroundCoords.U2, BackgroundCoords.V2);
        }

        public virtual void DrawSelectedGradient(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), SelectedGradientCoords.U1, SelectedGradientCoords.V1, SelectedGradientCoords.U2, SelectedGradientCoords.V2);
        }

        public virtual void DrawText(Graphics graphics, string text, string fontName, float fontSize, RectangleF rectangle, Color color, TextHorizontalAligment horizontalAligment = TextHorizontalAligment.Left, TextVerticalAligment verticalAligment = TextVerticalAligment.Center)
        {
            SizeF textSize = Graphics.MeasureText(text, fontName, fontSize);
            float x = 0.0f, y = 0.0f;

            switch (horizontalAligment)
            {
                case TextHorizontalAligment.Left:
                    x = rectangle.X;
                    break;
                case TextHorizontalAligment.Center:
                    x = rectangle.X + rectangle.Width * 0.5f - textSize.Width * 0.5f;
                    break;
                case TextHorizontalAligment.Right:
                    x = rectangle.Right - textSize.Width;
                    break;
            }

            switch (verticalAligment)
            {
                case TextVerticalAligment.Top:
                    y = rectangle.Y;
                    break;
                case TextVerticalAligment.Center:
                    y = rectangle.Y + rectangle.Height * 0.5f - textSize.Height * 0.8f;
                    break;
                case TextVerticalAligment.Down:
                    y = rectangle.Y + rectangle.Height - textSize.Height * 1.6f;
                    break;
            }

            graphics.DrawText(text, fontName, fontSize, new PointF(x, y), color, rectangle);
        }



        private static MenuSkin defaultSkin;
        public static MenuSkin DefaultSkin
        {
            get
            {
                if (defaultSkin == null)
                {
                    EnsureDefaultSkinFile();

                    defaultSkin = new MenuSkin(DefaultSkinPath);
                }

                return defaultSkin;
            }
        }

        internal const string DefaultSkinPath = Common.ResourcesFolder + @"menu-default-skin.PNG";
        internal const string SkinResourceName = "RAGENativeUI.RAGENativeUI_Resources.menu-default-skin.PNG";

        internal static void EnsureDefaultSkinFile()
        {
            if (!File.Exists(DefaultSkinPath))
            {
                Common.EnsureResourcesFolder();

                using (Stream skinResource = Assembly.GetExecutingAssembly().GetManifestResourceStream(SkinResourceName))
                using (FileStream file = new FileStream(DefaultSkinPath, FileMode.Create))
                {
                    skinResource.CopyTo(file);
                }
            }
        }
    }
}

