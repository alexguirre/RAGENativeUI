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

        private UVCoords BannerCoords { get; } = new UVCoords(0f, 0f, 0.5f, 0.125f);
        private UVCoords BackgroundCoords { get; } = new UVCoords(0.5f, 0f, 1f, 0.5f);
        private UVCoords SelectedGradientCoords { get; } = new UVCoords(0f, 0.125f, 0.5f, 0.1875f);
        private UVCoords CheckboxEmptyWhiteCoords { get; } = new UVCoords(0f, 0.1875f, 0.0625f, 0.25f);
        private UVCoords CheckboxEmptyBlackCoords { get; } = new UVCoords(0.0625f, 0.1875f, 0.125f, 0.25f);
        private UVCoords CheckboxCrossWhiteCoords { get; } = new UVCoords(0.125f, 0.1875f, 0.1875f, 0.25f);
        private UVCoords CheckboxCrossBlackCoords { get; } = new UVCoords(0.1875f, 0.1875f, 0.25f, 0.25f);
        private UVCoords CheckboxTickWhiteCoords { get; } = new UVCoords(0.25f, 0.1875f, 0.3125f, 0.25f);
        private UVCoords CheckboxTickBlackCoords { get; } = new UVCoords(0.3125f, 0.1875f, 0.375f, 0.25f);
        private UVCoords ArrowLeftCoords { get; } = new UVCoords(0.375f, 0.1875f, 0.4375f, 0.25f);
        private UVCoords ArrowRightCoords { get; } = new UVCoords(0.4375f, 0.1875f, 0.5f, 0.25f);
        private UVCoords ArrowsUpDownCoords { get; } = new UVCoords(0f, 0.25f, 0.0625f, 0.3125f);
        private UVCoords ArrowsUpDownBackgroundCoords { get; } = new UVCoords(0.5f, 0.5f, 1f, 0.5625f);

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
                DrawBackgroundTexture(graphics, x, y - 1, s.Width, s.Height);
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
                y += 3.5f;

                graphics.DrawRectangle(new RectangleF(x, y, s.Width, 3.25f), Color.FromArgb(240, 0, 0, 0));
                graphics.DrawRectangle(new RectangleF(x, y, s.Width, s.Height), Color.FromArgb(95, 0, 0, 0));

                DrawText(graphics, description.FormattedText, DescriptionFont, new RectangleF(x + BorderSafezone, y + BorderSafezone, s.Width - BorderSafezone * 2f, s.Height), Color.White, TextHorizontalAligment.Left, TextVerticalAligment.Top);

            }
        }

        public void DrawSubtitle(Graphics graphics, MenuSubtitle subtitle, float x, float y)
        {
            const float BorderSafezone = 8.5f;

            SizeF s = subtitle.Size;
            graphics.DrawRectangle(new RectangleF(x, y, s.Width, s.Height), Color.Black);
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
                switch (item.State)
                {
                    case MenuItemCheckboxState.Empty:
                        DrawCheckboxEmptyBlackTexture(graphics, x + s.Width - s.Height - BorderSafezone * 0.5f, y + BorderSafezone * 0.5f, s.Height - BorderSafezone, s.Height - BorderSafezone);
                        break;
                    case MenuItemCheckboxState.Cross:
                        DrawCheckboxCrossBlackTexture(graphics, x + s.Width - s.Height - BorderSafezone * 0.5f, y + BorderSafezone * 0.5f, s.Height - BorderSafezone, s.Height - BorderSafezone);
                        break;
                    case MenuItemCheckboxState.Tick:
                        DrawCheckboxTickBlackTexture(graphics, x + s.Width - s.Height - BorderSafezone * 0.5f, y + BorderSafezone * 0.5f, s.Height - BorderSafezone, s.Height - BorderSafezone);
                        break;
                }
            }
            else
            {
                switch (item.State)
                {
                    case MenuItemCheckboxState.Empty:
                        DrawCheckboxEmptyWhiteTexture(graphics, x + s.Width - s.Height - BorderSafezone * 0.5f, y + BorderSafezone * 0.5f, s.Height - BorderSafezone, s.Height - BorderSafezone);
                        break;
                    case MenuItemCheckboxState.Cross:
                        DrawCheckboxCrossWhiteTexture(graphics, x + s.Width - s.Height - BorderSafezone * 0.5f, y + BorderSafezone * 0.5f, s.Height - BorderSafezone, s.Height - BorderSafezone);
                        break;
                    case MenuItemCheckboxState.Tick:
                        DrawCheckboxTickWhiteTexture(graphics, x + s.Width - s.Height - BorderSafezone * 0.5f, y + BorderSafezone * 0.5f, s.Height - BorderSafezone, s.Height - BorderSafezone);
                        break;
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
        private void DrawBannerTexture(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), BannerCoords.U1, BannerCoords.V1, BannerCoords.U2, BannerCoords.V2);
        }

        private void DrawBackgroundTexture(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), BackgroundCoords.U1, BackgroundCoords.V1, BackgroundCoords.U2, BackgroundCoords.V2);
        }

        private void DrawSelectedGradientTexture(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), SelectedGradientCoords.U1, SelectedGradientCoords.V1, SelectedGradientCoords.U2, SelectedGradientCoords.V2);
        }

        private void DrawCheckboxEmptyWhiteTexture(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), CheckboxEmptyWhiteCoords.U1, CheckboxEmptyWhiteCoords.V1, CheckboxEmptyWhiteCoords.U2, CheckboxEmptyWhiteCoords.V2);
        }

        private void DrawCheckboxEmptyBlackTexture(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), CheckboxEmptyBlackCoords.U1, CheckboxEmptyBlackCoords.V1, CheckboxEmptyBlackCoords.U2, CheckboxEmptyBlackCoords.V2);
        }

        private void DrawCheckboxCrossWhiteTexture(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), CheckboxCrossWhiteCoords.U1, CheckboxCrossWhiteCoords.V1, CheckboxCrossWhiteCoords.U2, CheckboxCrossWhiteCoords.V2);
        }

        private void DrawCheckboxCrossBlackTexture(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), CheckboxCrossBlackCoords.U1, CheckboxCrossBlackCoords.V1, CheckboxCrossBlackCoords.U2, CheckboxCrossBlackCoords.V2);
        }

        private void DrawCheckboxTickWhiteTexture(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), CheckboxTickWhiteCoords.U1, CheckboxTickWhiteCoords.V1, CheckboxTickWhiteCoords.U2, CheckboxTickWhiteCoords.V2);
        }

        private void DrawCheckboxTickBlackTexture(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), CheckboxTickBlackCoords.U1, CheckboxTickBlackCoords.V1, CheckboxTickBlackCoords.U2, CheckboxTickBlackCoords.V2);
        }

        private void DrawArrowLeftTexture(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), ArrowLeftCoords.U1, ArrowLeftCoords.V1, ArrowLeftCoords.U2, ArrowLeftCoords.V2);
        }

        private  void DrawArrowRightTexture(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), ArrowRightCoords.U1, ArrowRightCoords.V1, ArrowRightCoords.U2, ArrowRightCoords.V2);
        }

        private void DrawArrowsUpDownTexture(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), ArrowsUpDownCoords.U1, ArrowsUpDownCoords.V1, ArrowsUpDownCoords.U2, ArrowsUpDownCoords.V2);
        }

        private void DrawArrowsUpDownBackgroundTexture(Graphics graphics, float x, float y, float width, float height)
        {
            graphics.DrawTexture(Image, new RectangleF(x - 1, y, width, height), ArrowsUpDownBackgroundCoords.U1, ArrowsUpDownBackgroundCoords.V1, ArrowsUpDownBackgroundCoords.U2, ArrowsUpDownBackgroundCoords.V2);
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
    }
}

