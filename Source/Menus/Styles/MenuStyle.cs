namespace RAGENativeUI.Menus.Styles
{
    using System.IO;
    using System.Linq;
    using System.Drawing;
    using System.Reflection;

    using Rage;
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Rendering;
    using Font = RAGENativeUI.Rendering.Font;

    public class MenuStyle : IMenuStyle
    {
        private const float ItemsBorderSafezone = 8.25f;

        private Texture spriteSheet;
        private readonly Color itemSelectedTextColor = Color.FromArgb(225, 10, 10, 10),
                               itemDisabledTextColor = Color.FromArgb(163, 159, 148),
                               itemDefaultTextColor = Color.FromArgb(240, 240, 240, 240);

        public PointF InitialMenuLocation { get; set; }
        public float MenuWidth { get; set; }
        public float BannerHeight { get; set; }
        public float SubtitleHeight { get; set; }
        public float ItemHeight { get; set; }
        public float UpDownDisplayHeight{ get; set; }
        public Font TitleFont { get; set; }
        public Font SubtitleFont { get; set; }
        public Font ItemFont { get; set; }
        public Font DescriptionFont { get; set; }
        public Texture SpriteSheet { get => spriteSheet; set { Throw.IfNull(value, nameof(value)); spriteSheet = value; }  }

        public MenuStyle()
        {
            InitialMenuLocation = new PointF(30.0f, 23.0f);

            MenuWidth = 432.0f;

            BannerHeight = 109.0f;
            SubtitleHeight = 36.0f;
            ItemHeight = 37.0f;
            UpDownDisplayHeight = 38.0f;

            TitleFont = new Font("Arial", 35.0f);
            SubtitleFont = new Font("Arial", 20.0f);
            ItemFont = new Font("Arial", 20.0f);
            DescriptionFont = new Font("Arial", 20.0f);

            SpriteSheet = DefaultSpriteSheet;
        }


        public void DrawBackground(Graphics graphics, MenuBackground background, ref float x, ref float y)
        {
            Throw.IfNull(graphics, nameof(graphics));
            Throw.IfNull(background, nameof(background));

            if (background.Menu.IsAnyItemOnScreen)
            {
                float h = background.GetHeight();
                DrawBackgroundTexture(graphics, x, y, MenuWidth, h);
            }
        }

        public void DrawBanner(Graphics graphics, MenuBanner banner, ref float x, ref float y)
        {
            Throw.IfNull(graphics, nameof(graphics));
            Throw.IfNull(banner, nameof(banner));

            DrawBannerTexture(graphics, x, y, MenuWidth, BannerHeight);
            if (banner.Title != null)
            {
                DrawText(graphics, banner.Title, TitleFont, new RectangleF(x, y, MenuWidth, BannerHeight), Color.White, TextHorizontalAligment.Center, TextVerticalAligment.Center);
            }
            y += BannerHeight;
        }

        public void DrawDescription(Graphics graphics, MenuDescription description, ref float x, ref float y)
        {
            Throw.IfNull(graphics, nameof(graphics));
            Throw.IfNull(description, nameof(description));

            const float BorderSafezone = 8.5f;

            float h = description.GetHeight();

            if (description.FormattedText != null && h > 0f)
            {
                float drawY = y + 4.0f;
                
                // add 1 because there's a bug that offsets the rectangle by 1 on each axis
                // TODO: remove +1 from DrawRectangle calls once it's fixed
                graphics.DrawRectangle(new RectangleF(x + 1, drawY + 1, MenuWidth, 3.25f), Color.FromArgb(240, 0, 0, 0));
                graphics.DrawRectangle(new RectangleF(x + 1, drawY + 1, MenuWidth, h), Color.FromArgb(95, 0, 0, 0));

                DrawText(graphics, description.FormattedText, DescriptionFont, new RectangleF(x + BorderSafezone, drawY + BorderSafezone, MenuWidth - BorderSafezone * 2f, h), Color.White, TextHorizontalAligment.Left, TextVerticalAligment.Top);

                y += h;
            }
        }

        public void DrawSubtitle(Graphics graphics, MenuSubtitle subtitle, ref float x, ref float y)
        {
            Throw.IfNull(graphics, nameof(graphics));
            Throw.IfNull(subtitle, nameof(subtitle));

            const float BorderSafezone = 8.5f;
            
            graphics.DrawRectangle(new RectangleF(x + 1, y + 1, MenuWidth, SubtitleHeight), Color.Black);
            DrawText(graphics, subtitle.Text, SubtitleFont, new RectangleF(x + BorderSafezone, y, MenuWidth, SubtitleHeight), Color.White, TextHorizontalAligment.Left, TextVerticalAligment.Center);

            if (ShouldShowSubtitleItemsCounter(subtitle))
            {
                DrawText(graphics, subtitle.GetItemsCounterText(), SubtitleFont, new RectangleF(x, y, MenuWidth, SubtitleHeight), Color.White, TextHorizontalAligment.Right, TextVerticalAligment.Center);
            }

            y += SubtitleHeight;
        }

        public void DrawUpDownDisplay(Graphics graphics, MenuUpDownDisplay upDownDisplay, ref float x, ref float y)
        {
            Throw.IfNull(graphics, nameof(graphics));
            Throw.IfNull(upDownDisplay, nameof(upDownDisplay));

            if (ShouldUpDownDisplayBeVisible(upDownDisplay))
            {
                float arrowsSize = UpDownDisplayHeight;

                float drawY = y + 1f;
                DrawArrowsUpDownBackgroundTexture(graphics, x, drawY, MenuWidth, UpDownDisplayHeight);
                DrawArrowsUpDownTexture(graphics, x - arrowsSize / 2f + MenuWidth / 2f, drawY, arrowsSize, arrowsSize);

                y += UpDownDisplayHeight;
            }
        }

        public void DrawItem(Graphics graphics, MenuItem item, ref float x, ref float y)
        {
            Throw.IfNull(graphics, nameof(graphics));
            Throw.IfNull(item, nameof(item));

            switch (item)
            {
                case MenuItemCheckbox checkbox: DrawItemCheckbox(graphics, checkbox, ref x, ref y); break;
                case MenuItemScroller scroller: DrawItemScroller(graphics, scroller, ref x, ref y); break;
                default: DrawItemBase(graphics, item, ref x, ref y); break;
            }
        }

        private void DrawItemBase(Graphics graphics, MenuItem item, ref float x, ref float y)
        {
            bool selected = item.IsSelected, disabled = item.IsDisabled;

            Color textColor = GetItemTextColor(selected, disabled);

            if (selected)
            {
                DrawSelectedGradientTexture(graphics, x, y, MenuWidth, ItemHeight);
            }

            DrawText(graphics, item.Text, ItemFont, new RectangleF(x + ItemsBorderSafezone, y, MenuWidth, ItemHeight), textColor);

            y += ItemHeight;
        }

        private void DrawItemCheckbox(Graphics graphics, MenuItemCheckbox item, ref float x, ref float y)
        {
            float tempX = x, tempY = y;
            DrawItemBase(graphics, item, ref tempX, ref tempY);

            bool selected = item.IsSelected, disabled = item.IsDisabled, isChecked = item.IsChecked;

            if (disabled)
            {
                if (isChecked)
                {
                    DrawCheckboxTickDisabledTexture(graphics, x + MenuWidth - ItemHeight - ItemsBorderSafezone * 0.5f, y + ItemsBorderSafezone * 0.5f, ItemHeight - ItemsBorderSafezone, ItemHeight - ItemsBorderSafezone);
                }
                else
                {
                    DrawCheckboxEmptyDisabledTexture(graphics, x + MenuWidth - ItemHeight - ItemsBorderSafezone * 0.5f, y + ItemsBorderSafezone * 0.5f, ItemHeight - ItemsBorderSafezone, ItemHeight - ItemsBorderSafezone);
                }
            }
            else if (selected)
            {
                if (isChecked)
                {
                    DrawCheckboxTickBlackTexture(graphics, x + MenuWidth - ItemHeight - ItemsBorderSafezone * 0.5f, y + ItemsBorderSafezone * 0.5f, ItemHeight - ItemsBorderSafezone, ItemHeight - ItemsBorderSafezone);

                }
                else
                {
                    DrawCheckboxEmptyBlackTexture(graphics, x + MenuWidth - ItemHeight - ItemsBorderSafezone * 0.5f, y + ItemsBorderSafezone * 0.5f, ItemHeight - ItemsBorderSafezone, ItemHeight - ItemsBorderSafezone);

                }
            }
            else
            {
                if (isChecked)
                {
                    DrawCheckboxTickWhiteTexture(graphics, x + MenuWidth - ItemHeight - ItemsBorderSafezone * 0.5f, y + ItemsBorderSafezone * 0.5f, ItemHeight - ItemsBorderSafezone, ItemHeight - ItemsBorderSafezone);

                }
                else
                {
                    DrawCheckboxEmptyWhiteTexture(graphics, x + MenuWidth - ItemHeight - ItemsBorderSafezone * 0.5f, y + ItemsBorderSafezone * 0.5f, ItemHeight - ItemsBorderSafezone, ItemHeight - ItemsBorderSafezone);
                }
            }

            x = tempX;
            y = tempY;
        }

        private void DrawItemScroller(Graphics graphics, MenuItemScroller item, ref float x, ref float y)
        {
            float tempX = x, tempY = y;
            DrawItemBase(graphics, item, ref tempX, ref tempY);

            bool selected = item.IsSelected, disabled = item.IsDisabled;

            Color textColor = GetItemTextColor(selected, disabled);

            if (selected)
            {
                string selectedOptionText = item.GetSelectedOptionText();

                if (disabled)
                {
                    DrawArrowRightDisabledTexture(graphics, x + MenuWidth - ItemHeight, y + ItemsBorderSafezone * 0.5f, ItemHeight - ItemsBorderSafezone, ItemHeight - ItemsBorderSafezone);
                }
                else
                {
                    DrawArrowRightTexture(graphics, x + MenuWidth - ItemHeight, y + ItemsBorderSafezone * 0.5f, ItemHeight - ItemsBorderSafezone, ItemHeight - ItemsBorderSafezone);
                }

                DrawText(graphics, selectedOptionText, ItemFont, new RectangleF(x, y, MenuWidth - ItemHeight / 1.5f - ItemsBorderSafezone, ItemHeight), textColor, TextHorizontalAligment.Right);
                SizeF textSize = ItemFont.Measure(selectedOptionText);

                if (disabled)
                {
                    DrawArrowLeftDisabledTexture(graphics, x + MenuWidth - ItemHeight - textSize.Width - ItemsBorderSafezone * 2.5f, y + ItemsBorderSafezone * 0.5f, ItemHeight - ItemsBorderSafezone, ItemHeight - ItemsBorderSafezone);
                }
                else
                {
                    DrawArrowLeftTexture(graphics, x + MenuWidth - ItemHeight - textSize.Width - ItemsBorderSafezone * 2.5f, y + ItemsBorderSafezone * 0.5f, ItemHeight - ItemsBorderSafezone, ItemHeight - ItemsBorderSafezone);
                }
            }
            else
            {
                DrawText(graphics, item.GetSelectedOptionText(), ItemFont, new RectangleF(x, y, MenuWidth - ItemsBorderSafezone, ItemHeight), textColor, TextHorizontalAligment.Right);
            }

            x = tempX;
            y = tempY;
        }

        public virtual string FormatDescriptionText(MenuDescription description, string text, out SizeF textMeasurement)
        {
            Throw.IfNull(description, nameof(description));
            Throw.IfNull(text, nameof(text));

            const float BorderSafezone = 8.5f;

            string t = Common.WrapText(text.Replace('\n', ' '), DescriptionFont, MenuWidth - BorderSafezone * 2f);
            textMeasurement = DescriptionFont.Measure(t);
            textMeasurement.Height += BorderSafezone * 3.0f;
            return t;
        }

        private bool ShouldUpDownDisplayBeVisible(MenuUpDownDisplay display) => display.Menu.IsAnyItemOnScreen && display.Menu.GetOnScreenItemsCount() < display.Menu.Items.Sum(i => i.IsVisible ? 1 : 0);
        private bool ShouldShowSubtitleItemsCounter(MenuSubtitle subtitle) => subtitle.Menu.IsAnyItemOnScreen && subtitle.Menu.GetOnScreenItemsCount() < subtitle.Menu.Items.Sum(i => i.IsVisible ? 1 : 0);

        private Color GetItemTextColor(bool selected, bool disabled)
        {
            if (disabled)
            {
                return itemDisabledTextColor;
            }
            else if (selected)
            {
                return itemSelectedTextColor;
            }
            else
            {
                return itemDefaultTextColor;
            }
        }

        #region Draw Helper Methods
        private void DrawBannerTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, BannerCoords);
        private void DrawBackgroundTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, BackgroundCoords);
        private void DrawSelectedGradientTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, SelectedGradientCoords);
        private void DrawCheckboxEmptyWhiteTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, CheckboxEmptyWhiteCoords);
        private void DrawCheckboxEmptyBlackTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, CheckboxEmptyBlackCoords);
        private void DrawCheckboxEmptyDisabledTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, CheckboxEmptyDisabledCoords);
        private void DrawCheckboxTickDisabledTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, CheckboxTickDisabledCoords);
        private void DrawCheckboxTickWhiteTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, CheckboxTickWhiteCoords);
        private void DrawCheckboxTickBlackTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, CheckboxTickBlackCoords);
        private void DrawArrowLeftTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, ArrowLeftCoords);
        private void DrawArrowRightTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, ArrowRightCoords);
        private void DrawArrowsUpDownTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, ArrowsUpDownCoords);
        private void DrawArrowsUpDownBackgroundTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, ArrowsUpDownBackgroundCoords);
        private void DrawArrowLeftDisabledTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, ArrowLeftDisabledCoords);
        private void DrawArrowRightDisabledTexture(Graphics graphics, float x, float y, float w, float h) => DrawTexture(graphics, x, y, w, h, ArrowRightDisabledCoords);

        private void DrawTexture(Graphics graphics, float x, float y, float width, float height, UVCoords uv)
        {
            graphics.DrawTexture(SpriteSheet, new RectangleF(x, y, width, height), uv.U1, uv.V1, uv.U2, uv.V2);
        }

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
        #endregion // Draw Helper Methods


        public static MenuStyle Default => new MenuStyle();

        private static Texture defaultSpriteSheet;
        private static Texture DefaultSpriteSheet
        {
            get
            {
                if(defaultSpriteSheet == null)
                {
                    EnsureDefaultSpriteSheetFile();

                    defaultSpriteSheet = Game.CreateTextureFromFile(DefaultSpriteSheetPath);
                }

                return defaultSpriteSheet;
            }
        }

        private const string DefaultSpriteSheetPath = Common.ResourcesFolder + @"menu-default.png";
        private const string SpriteSheetResourceName = "RAGENativeUI.RAGENativeUI_Resources.menu-default.PNG";

        private static void EnsureDefaultSpriteSheetFile()
        {
            if (!File.Exists(DefaultSpriteSheetPath))
            {
                Common.EnsureResourcesFolder();

                using (Stream skinResource = Assembly.GetExecutingAssembly().GetManifestResourceStream(SpriteSheetResourceName))
                using (FileStream file = new FileStream(DefaultSpriteSheetPath, FileMode.Create))
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
        private static readonly UVCoords CheckboxEmptyDisabledCoords = new UVCoords(0.125f, 0.1875f, 0.1875f, 0.25f);
        private static readonly UVCoords CheckboxTickDisabledCoords = new UVCoords(0.1875f, 0.1875f, 0.25f, 0.25f);
        private static readonly UVCoords CheckboxTickWhiteCoords = new UVCoords(0.25f, 0.1875f, 0.3125f, 0.25f);
        private static readonly UVCoords CheckboxTickBlackCoords = new UVCoords(0.3125f, 0.1875f, 0.375f, 0.25f);
        private static readonly UVCoords ArrowLeftCoords = new UVCoords(0.375f, 0.1875f, 0.4375f, 0.25f);
        private static readonly UVCoords ArrowRightCoords = new UVCoords(0.4375f, 0.1875f, 0.5f, 0.25f);
        private static readonly UVCoords ArrowsUpDownCoords = new UVCoords(0f, 0.25f, 0.0625f, 0.3125f);
        private static readonly UVCoords ArrowsUpDownBackgroundCoords = new UVCoords(0.5f, 0.5f, 1f, 0.5625f);
        private static readonly UVCoords ArrowLeftDisabledCoords = new UVCoords(0.375f, 0.25f, 0.4375f, 0.3125f);
        private static readonly UVCoords ArrowRightDisabledCoords = new UVCoords(0.4375f, 0.25f, 0.5f, 0.3125f);
    }
}

