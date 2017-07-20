namespace RAGENativeUI.Menus.Rendering
{
    using System.IO;
    using System.Drawing;
    using System.Reflection;

    using Rage;
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Rendering;
    using RAGENativeUI.Utility;
    using Font = RAGENativeUI.Utility.Font;

    public class MenuSkin : IMenuSkin
    {
        public Texture Image { get; }

        public virtual Font TitleFont { get; } = new Font("Arial", 35.0f);
        public virtual Font SubtitleFont { get; } = new Font("Arial", 20.0f);
        public virtual Font ItemTextFont { get; } = new Font("Arial", 20.0f);

        public virtual UVCoords BannerCoords { get; } = new UVCoords(0f, 0f, 0.5f, 0.125f);
        public virtual UVCoords BackgroundCoords { get; } = new UVCoords(0.5f, 0f, 1f, 0.5f);
        public virtual UVCoords SelectedGradientCoords { get; } = new UVCoords(0f, 0.125f, 0.5f, 0.1875f);
        public virtual UVCoords CheckboxEmptyWhiteCoords { get; } = new UVCoords(0f, 0.1875f, 0.0625f, 0.25f);
        public virtual UVCoords CheckboxEmptyBlackCoords { get; } = new UVCoords(0.0625f, 0.1875f, 0.125f, 0.25f);
        public virtual UVCoords CheckboxCrossWhiteCoords { get; } = new UVCoords(0.125f, 0.1875f, 0.1875f, 0.25f);
        public virtual UVCoords CheckboxCrossBlackCoords { get; } = new UVCoords(0.1875f, 0.1875f, 0.25f, 0.25f);
        public virtual UVCoords CheckboxTickWhiteCoords { get; } = new UVCoords(0.25f, 0.1875f, 0.3125f, 0.25f);
        public virtual UVCoords CheckboxTickBlackCoords { get; } = new UVCoords(0.3125f, 0.1875f, 0.375f, 0.25f);
        public virtual UVCoords ArrowLeftCoords { get; } = new UVCoords(0.375f, 0.1875f, 0.4375f, 0.25f);
        public virtual UVCoords ArrowRightCoords { get; } = new UVCoords(0.4375f, 0.1875f, 0.5f, 0.25f);
        public virtual UVCoords ArrowsUpDownCoords { get; } = new UVCoords(0f, 0.25f, 0.0625f, 0.3125f);
        public virtual UVCoords ArrowsUpDownBackgroundCoords { get; } = new UVCoords(0.5f, 0.5f, 1f, 0.5625f);

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

        public virtual void DrawCheckboxEmptyWhite(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), CheckboxEmptyWhiteCoords.U1, CheckboxEmptyWhiteCoords.V1, CheckboxEmptyWhiteCoords.U2, CheckboxEmptyWhiteCoords.V2);
        }

        public virtual void DrawCheckboxEmptyBlack(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), CheckboxEmptyBlackCoords.U1, CheckboxEmptyBlackCoords.V1, CheckboxEmptyBlackCoords.U2, CheckboxEmptyBlackCoords.V2);
        }

        public virtual void DrawCheckboxCrossWhite(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), CheckboxCrossWhiteCoords.U1, CheckboxCrossWhiteCoords.V1, CheckboxCrossWhiteCoords.U2, CheckboxCrossWhiteCoords.V2);
        }

        public virtual void DrawCheckboxCrossBlack(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), CheckboxCrossBlackCoords.U1, CheckboxCrossBlackCoords.V1, CheckboxCrossBlackCoords.U2, CheckboxCrossBlackCoords.V2);
        }

        public virtual void DrawCheckboxTickWhite(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), CheckboxTickWhiteCoords.U1, CheckboxTickWhiteCoords.V1, CheckboxTickWhiteCoords.U2, CheckboxTickWhiteCoords.V2);
        }

        public virtual void DrawCheckboxTickBlack(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), CheckboxTickBlackCoords.U1, CheckboxTickBlackCoords.V1, CheckboxTickBlackCoords.U2, CheckboxTickBlackCoords.V2);
        }

        public virtual void DrawArrowLeft(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), ArrowLeftCoords.U1, ArrowLeftCoords.V1, ArrowLeftCoords.U2, ArrowLeftCoords.V2);
        }

        public virtual void DrawArrowRight(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), ArrowRightCoords.U1, ArrowRightCoords.V1, ArrowRightCoords.U2, ArrowRightCoords.V2);
        }

        public virtual void DrawArrowsUpDown(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), ArrowsUpDownCoords.U1, ArrowsUpDownCoords.V1, ArrowsUpDownCoords.U2, ArrowsUpDownCoords.V2);
        }

        public virtual void DrawArrowsUpDownBackground(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), ArrowsUpDownBackgroundCoords.U1, ArrowsUpDownBackgroundCoords.V1, ArrowsUpDownBackgroundCoords.U2, ArrowsUpDownBackgroundCoords.V2);
        }

        public virtual void DrawText(Graphics graphics, string text, string fontName, float fontSize, RectangleF rectangle, Color color, TextHorizontalAligment horizontalAligment = TextHorizontalAligment.Left, TextVerticalAligment verticalAligment = TextVerticalAligment.Center)
        {
            DrawText(graphics, text, new Font(fontName, fontSize), rectangle, color, horizontalAligment, verticalAligment);
        }

        public virtual void DrawText(Graphics graphics, string text, Font font, RectangleF rectangle, Color color, TextHorizontalAligment horizontalAligment = TextHorizontalAligment.Left, TextVerticalAligment verticalAligment = TextVerticalAligment.Center)
        {
            SizeF textSize = font.Measure(text);
            textSize.Height = font.Height;
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
                    x = rectangle.Right - textSize.Width - 2.0f;
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

            graphics.DrawText(text, font.Name, font.Size, new PointF(x, y), color, rectangle);
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

