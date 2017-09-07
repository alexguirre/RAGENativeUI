namespace RAGENativeUI.Menus.Rendering
{
    using System.IO;
    using System.Drawing;
    using System.Reflection;

    using Rage;
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Rendering;
    using Font = RAGENativeUI.Rendering.Font;

    public class MenuSkin : IMenuSkin
    {
        private Texture Image { get; }

        private Font TitleFont { get; } = new Font("Arial", 35.0f);
        private Font SubtitleFont { get; } = new Font("Arial", 20.0f);
        private Font ItemTextFont { get; } = new Font("Arial", 20.0f);
        private Font DescriptionFont { get; } = new Font("Arial", 20.0f);

        public MenuSkin(string skinFileName)
        {
            if (!File.Exists(skinFileName))
                throw new FileNotFoundException($"The file '{skinFileName}' wasn't found, can't create menu skin.");

            Image = Game.CreateTextureFromFile(skinFileName);
        }

        #region IMenuSkin Implementation
        public void DrawBackground(Graphics graphics, MenuBackground background, float x, float y)
        {
            if (background.Menu.IsAnyItemOnScreen)
            {
                SizeF s = background.Size;
                DrawBackgroundTexture(graphics, x, y, s.Width, s.Height);
            }
        }

        public void DrawBanner(Graphics graphics, MenuBanner banner, float x, float y)
        {
            SizeF s = banner.Size;
            DrawBannerTexture(graphics, x, y, s.Width, s.Height);
            if (banner.Title != null)
            {
                DrawText(graphics, banner.Title, TitleFont, new RectangleF(x, y, s.Width, s.Height), Color.White, TextHorizontalAligment.Center, TextVerticalAligment.Center);
            }
        }

        public void DrawDescription(Graphics graphics, MenuDescription description, float x, float y)
        {
            const float BorderSafezone = 8.5f;

            SizeF s = description.Size;

            if (description.FormattedText != null && s.Height > 0f)
            {
                y += 4f;

                // add 1 because there's a bug that offsets the rectangle by 1 on each axis
                // TODO: remove +1 from DrawRectangle calls once it's fixed
                graphics.DrawRectangle(new RectangleF(x + 1, y + 1, s.Width, 3.25f), Color.FromArgb(240, 0, 0, 0));
                graphics.DrawRectangle(new RectangleF(x + 1, y + 1, s.Width, s.Height), Color.FromArgb(95, 0, 0, 0));

                DrawText(graphics, description.FormattedText, DescriptionFont, new RectangleF(x + BorderSafezone, y + BorderSafezone, s.Width - BorderSafezone * 2f, s.Height), Color.White, TextHorizontalAligment.Left, TextVerticalAligment.Top);

            }
        }

        public void DrawSubtitle(Graphics graphics, MenuSubtitle subtitle, float x, float y)
        {
            const float BorderSafezone = 8.5f;

            SizeF s = subtitle.Size;
            graphics.DrawRectangle(new RectangleF(x + 1, y + 1, s.Width, s.Height), Color.Black);
            DrawText(graphics, subtitle.Text, SubtitleFont, new RectangleF(x + BorderSafezone, y, s.Width, s.Height), Color.White, TextHorizontalAligment.Left, TextVerticalAligment.Center);

            if (subtitle.ShouldShowItemsCounter())
            {
                DrawText(graphics, subtitle.GetItemsCounterText(), SubtitleFont, new RectangleF(x, y, s.Width, s.Height), Color.White, TextHorizontalAligment.Right, TextVerticalAligment.Center);
            }
        }

        public void DrawUpDownDisplay(Graphics graphics, MenuUpDownDisplay upDownDisplay, float x, float y)
        {
            SizeF s = upDownDisplay.Size;
            float arrowsSize = s.Height;

            y += 1;
            DrawArrowsUpDownBackgroundTexture(graphics, x, y, s.Width, s.Height);
            DrawArrowsUpDownTexture(graphics, x - arrowsSize / 2f + s.Width / 2f, y, arrowsSize, arrowsSize);
        }

        public void DrawItem(Graphics graphics, MenuItem item, float x, float y, bool selected)
        {
            const float BorderSafezone = 8.25f;

            SizeF s = item.Size;
            if (selected)
            {
                DrawSelectedGradientTexture(graphics, x, y, s.Width, s.Height);
                DrawText(graphics, item.Text, ItemTextFont, new RectangleF(x + BorderSafezone, y, s.Width, s.Height), Color.FromArgb(225, 10, 10, 10));
            }
            else
            {
                DrawText(graphics, item.Text, ItemTextFont, new RectangleF(x + BorderSafezone, y, s.Width, s.Height), Color.FromArgb(240, 240, 240, 240));
            }
        }

        public void DrawItemCheckbox(Graphics graphics, MenuItemCheckbox item, float x, float y, bool selected)
        {
            const float BorderSafezone = 8.25f;

            DrawItem(graphics, item, x, y, selected);

            SizeF s = item.Size;
            if (selected)
            {
                if (item.IsChecked)
                {
                    DrawCheckboxTickBlackTexture(graphics, x + s.Width - s.Height - BorderSafezone * 0.5f, y + BorderSafezone * 0.5f, s.Height - BorderSafezone, s.Height - BorderSafezone);
                }
                else
                {
                    DrawCheckboxEmptyBlackTexture(graphics, x + s.Width - s.Height - BorderSafezone * 0.5f, y + BorderSafezone * 0.5f, s.Height - BorderSafezone, s.Height - BorderSafezone);
                }
            }
            else
            {
                if (item.IsChecked)
                {
                    DrawCheckboxTickWhiteTexture(graphics, x + s.Width - s.Height - BorderSafezone * 0.5f, y + BorderSafezone * 0.5f, s.Height - BorderSafezone, s.Height - BorderSafezone);
                }
                else
                {
                    DrawCheckboxEmptyWhiteTexture(graphics, x + s.Width - s.Height - BorderSafezone * 0.5f, y + BorderSafezone * 0.5f, s.Height - BorderSafezone, s.Height - BorderSafezone);
                }
            }
        }

        public void DrawItemScroller(Graphics graphics, MenuItemScroller item, float x, float y, bool selected)
        {
            const float BorderSafezone = 8.25f;

            DrawItem(graphics, item, x, y, selected);

            SizeF s = item.Size;
            if (selected)
            {
                string selectedOptionText = item.GetSelectedOptionText();

                DrawArrowRightTexture(graphics, x + s.Width - s.Height, y + BorderSafezone * 0.5f, s.Height - BorderSafezone, s.Height - BorderSafezone);
                DrawText(graphics, selectedOptionText, ItemTextFont, new RectangleF(x, y, s.Width - s.Height / 1.5f - BorderSafezone, s.Height), Color.FromArgb(225, 10, 10, 10), TextHorizontalAligment.Right);
                SizeF textSize = ItemTextFont.Measure(selectedOptionText);
                DrawArrowLeftTexture(graphics, x + s.Width - s.Height - textSize.Width - BorderSafezone * 2.5f, y + BorderSafezone * 0.5f, s.Height - BorderSafezone, s.Height - BorderSafezone);
            }
            else
            {
                DrawText(graphics, item.GetSelectedOptionText(), ItemTextFont, new RectangleF(x, y, s.Width - BorderSafezone, s.Height), Color.FromArgb(240, 240, 240, 240), TextHorizontalAligment.Right);
            }
        }

        public string FormatDescriptionText(MenuDescription description, string text, out SizeF textMeasurement)
        {
            const float BorderSafezone = 8.5f;

            string t = Common.WrapText(text.Replace('\n', ' '), DescriptionFont, description.Size.Width - BorderSafezone * 2f);
            textMeasurement = DescriptionFont.Measure(t);
            textMeasurement.Height += BorderSafezone * 3.0f;
            return t;
        }
        #endregion // IMenuSkin Implementation

        #region Draw Textures Helper Methods
        private void DrawBannerTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, BannerCoords);
        private void DrawBackgroundTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, BackgroundCoords);
        private void DrawSelectedGradientTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, SelectedGradientCoords);
        private void DrawCheckboxEmptyWhiteTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, CheckboxEmptyWhiteCoords);
        private void DrawCheckboxEmptyBlackTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, CheckboxEmptyBlackCoords);
        private void DrawCheckboxCrossWhiteTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, CheckboxCrossWhiteCoords);
        private void DrawCheckboxCrossBlackTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, CheckboxCrossBlackCoords);
        private void DrawCheckboxTickWhiteTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, CheckboxTickWhiteCoords);
        private void DrawCheckboxTickBlackTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, CheckboxTickBlackCoords);
        private void DrawArrowLeftTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, ArrowLeftCoords);
        private  void DrawArrowRightTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, ArrowRightCoords);
        private void DrawArrowsUpDownTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, ArrowsUpDownCoords);
        private void DrawArrowsUpDownBackgroundTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, ArrowsUpDownBackgroundCoords);

        private void DrawTexture(Graphics graphics, float x, float y, float width, float height, UVCoords uv)
        {
            graphics.DrawTexture(Image, new RectangleF(x, y, width, height), uv.U1, uv.V1, uv.U2, uv.V2);
        }
        #endregion // Draw Textures Helper Methods

        private void DrawText(Graphics graphics, string text, string fontName, float fontSize, RectangleF rectangle, Color color, TextHorizontalAligment horizontalAligment = TextHorizontalAligment.Left, TextVerticalAligment verticalAligment = TextVerticalAligment.Center)
        {
            DrawText(graphics, text, new Font(fontName, fontSize), rectangle, color, horizontalAligment, verticalAligment);
        }

        private void DrawText(Graphics graphics, string text, Font font, RectangleF rectangle, Color color, TextHorizontalAligment horizontalAligment = TextHorizontalAligment.Left, TextVerticalAligment verticalAligment = TextVerticalAligment.Center)
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


        private static readonly UVCoords BannerCoords = new UVCoords(0f, 0f, 0.5f, 0.125f);
        private static readonly UVCoords BackgroundCoords = new UVCoords(0.5f, 0f, 1f, 0.5f);
        private static readonly UVCoords SelectedGradientCoords = new UVCoords(0f, 0.125f, 0.5f, 0.1875f);
        private static readonly UVCoords CheckboxEmptyWhiteCoords = new UVCoords(0f, 0.1875f, 0.0625f, 0.25f);
        private static readonly UVCoords CheckboxEmptyBlackCoords = new UVCoords(0.0625f, 0.1875f, 0.125f, 0.25f);
        private static readonly UVCoords CheckboxCrossWhiteCoords = new UVCoords(0.125f, 0.1875f, 0.1875f, 0.25f);
        private static readonly UVCoords CheckboxCrossBlackCoords = new UVCoords(0.1875f, 0.1875f, 0.25f, 0.25f);
        private static readonly UVCoords CheckboxTickWhiteCoords = new UVCoords(0.25f, 0.1875f, 0.3125f, 0.25f);
        private static readonly UVCoords CheckboxTickBlackCoords = new UVCoords(0.3125f, 0.1875f, 0.375f, 0.25f);
        private static readonly UVCoords ArrowLeftCoords = new UVCoords(0.375f, 0.1875f, 0.4375f, 0.25f);
        private static readonly UVCoords ArrowRightCoords = new UVCoords(0.4375f, 0.1875f, 0.5f, 0.25f);
        private static readonly UVCoords ArrowsUpDownCoords = new UVCoords(0f, 0.25f, 0.0625f, 0.3125f);
        private static readonly UVCoords ArrowsUpDownBackgroundCoords = new UVCoords(0.5f, 0.5f, 1f, 0.5625f);
    }
}

