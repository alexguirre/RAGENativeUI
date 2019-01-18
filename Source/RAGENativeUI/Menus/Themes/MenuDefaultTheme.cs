namespace RAGENativeUI.Menus.Themes
{
#if RPH1
    extern alias rph1;
    using Graphics = rph1::Rage.Graphics;
#else
    /** REDACTED **/
#endif

    using System;
    using System.Drawing;
    using System.Linq;

    public class MenuDefaultTheme : MenuTheme
    {
        public TextureReference Banner { get; set; }
        public Color BannerColor { get; set; } = Color.White;
        public Color SubtitleBackColor { get; set; } = Color.Black;
        public Color SubtitleForeColor { get; set; } = Color.White;
        public TextureReference Nav { get; set; }
        public Color NavColor { get; set; } = Color.White;
        public TextureReference UpDownArrows { get; set; }
        public Color UpDownArrowsBackColor { get; set; } = Color.FromArgb(204, 0, 0, 0);
        public Color UpDownArrowsForeColor { get; set; } = Color.FromArgb(255, 255, 255);
        public TextureReference Background { get; set; }
        public Color BackgroundColor { get; set; } = Color.FromArgb(255, 255, 255);
        public TextureReference DescriptionBackground { get; set; }
        public Color DescriptionBackColor { get; set; } = Color.FromArgb(186, 0, 0, 0);
        public Color DescriptionForeColor { get; set; } = Color.FromArgb(240, 240, 240);
        public Color DescriptionSeparatorBarColor { get; set; } = Color.FromArgb(0, 0, 0);


        private float
            aspectRatio,
            menuWidth,
            itemHeight = 0.034722f;


        public MenuDefaultTheme(Menu menu) : base(menu)
        {
            TextureDictionary commonMenu = "CommonMenu";
            foreach (TextureReference t in commonMenu.GetTextures())
            {
                switch (t.Name.ToLower())
                {
                    case "interaction_bgd": Banner = t; break;
                    case "gradient_nav": Nav = t; break;
                    case "shop_arrows_upanddown": UpDownArrows = t; break;
                    case "gradient_bgd": Background = t; DescriptionBackground = t; break;
                }
            }
        }

        public override MenuTheme Clone(Menu menu)
        {
            return new MenuDefaultTheme(menu)
            {
                Banner = Banner,
                BannerColor = BannerColor,
                SubtitleBackColor = SubtitleBackColor,
                SubtitleForeColor = SubtitleForeColor,
                Nav = Nav,
                NavColor = NavColor,
                UpDownArrowsBackColor = UpDownArrowsBackColor,
                UpDownArrowsForeColor = UpDownArrowsForeColor,
                Background = Background,
                BackgroundColor = BackgroundColor,
                DescriptionBackground = DescriptionBackground,
                DescriptionBackColor = DescriptionBackColor,
                DescriptionForeColor = DescriptionForeColor,
                DescriptionSeparatorBarColor = DescriptionSeparatorBarColor,
            };
        }

        public override void Draw(Graphics g)
        {
            // Note: any magic constants found here were taken from the game scripts

            // TODO: resolution 1280x768  | 5:3   - the menu appears a bit to the left compared to game menus
            // TODO: resolution 1280x800  | 16:10 - the menu appears a bit to the left compared to game menus
            // TODO: resolution 1280x1024 | 5:4   - the menu appears a bit to the left (out of screen) compared to game menus
            // TODO: resolution 1440x990  | 16:11 - the menu appears a bit to the left compared to game menus
            // TODO: resolution 1600x1024 | 25:16 - the menu appears a bit to the left (out of screen) compared to game menus
            // TODO: resolution 1680x1050 | 16:10 - the menu appears a bit to the left compared to game menus

            aspectRatio = N.GetAspectRatio(false);
            menuWidth = 0.225f;
            if (aspectRatio < 1.77777f) // less than 16:9
            {
                menuWidth = (0.225f * ((16f / 9f) / aspectRatio));
            }

            N.SetScriptGfxAlign('L', 'T');
            N.SetScriptGfxAlignParams(0.0f, 0.0f, 0.0f, 0.0f);

            float x = 0.1125f;
            if (aspectRatio > 1.33f && aspectRatio < 1.34f) // == 4:3
            {
                x = 0.15f;
            }
            float y = 0.045f + (0.00277776f * 2.0f) - 0.001f - 0.05f;

            // testoffset
            //x += 0.02222f;


            // banner
            if (Banner != null)
            {
                if (!Banner.Dictionary.IsLoaded) Banner.Dictionary.Load();

                float bannerWidth = menuWidth;
                float bannerHeight = 0.1f;

                DrawSprite(Banner, x, y + bannerHeight * 0.5f, bannerWidth, bannerHeight, BannerColor);
                y += bannerHeight;
            }

            // subtitle
            {
                float subtitleWidth = menuWidth;
                float subtitleHeight = itemHeight;

                DrawRect(x, y + subtitleHeight * 0.5f, subtitleWidth, subtitleHeight, SubtitleBackColor);

                // subtitle text
                {
                    float subTextX = x - menuWidth * 0.5f + 0.00390625f;
                    float subTextY = y + 0.00416664f;

                    N.SetTextFont(0);
                    N.SetTextScale(0f, 0.35f);
                    N.SetTextColour(SubtitleForeColor.R, SubtitleForeColor.G, SubtitleForeColor.B, SubtitleForeColor.A);
                    N.SetTextWrap(x - menuWidth * 0.5f + 0.0046875f, x + menuWidth * 0.5f - 0.0046875f);
                    N.SetTextCentre(false);
                    N.SetTextDropshadow(0, 0, 0, 0, 0);
                    N.SetTextEdge(0, 0, 0, 0, 0);

                    N.BeginTextCommandDisplayText("STRING");
                    N.AddTextComponentSubstringPlayerName(Menu.Subtitle);
                    N.EndTextCommandDisplayText(subTextX, subTextY);
                }

                // counter
                {
                    int visibleItems = Menu.Items.Count(i => i.IsVisible);
                    if (visibleItems > Menu.MaxItemsOnScreen)
                    {
                        const string CounterFormat = "CM_ITEM_COUNT";


                        int pos = 0;
                        for (int i = 0; i < Menu.ItemsOnScreenEndIndex + 1; i++)
                        {
                            if (Menu.Items[i].IsVisible)
                            {
                                pos++;
                            }

                            if (i == Menu.SelectedIndex)
                            {
                                break;
                            }
                        }

                        void SetCounterTextOptions()
                        {
                            N.SetTextFont(0);
                            N.SetTextScale(0f, 0.35f);
                            N.SetTextColour(SubtitleForeColor.R, SubtitleForeColor.G, SubtitleForeColor.B, SubtitleForeColor.A);
                            N.SetTextWrap(x - menuWidth * 0.5f + 0.0046875f, x + menuWidth * 0.5f - 0.0046875f);
                            N.SetTextCentre(false);
                            N.SetTextDropshadow(0, 0, 0, 0, 0);
                            N.SetTextEdge(0, 0, 0, 0, 0);
                        }

                        void PushCounterComponents()
                        {
                            N.AddTextComponentInteger(pos);
                            N.AddTextComponentInteger(visibleItems);
                        }

                        SetCounterTextOptions();
                        N.BeginTextCommandGetWidth(CounterFormat);
                        PushCounterComponents();
                        float counterWidth = N.EndTextCommandGetWidth(true);

                        float counterX = x + menuWidth * 0.5f - 0.00390625f - counterWidth;
                        float counterY = y + 0.00416664f;

                        SetCounterTextOptions();
                        N.BeginTextCommandDisplayText(CounterFormat);
                        PushCounterComponents();
                        N.EndTextCommandDisplayText(counterX, counterY);

                    }
                }

                y += subtitleHeight;
            }

            float headerBottom = y;

            // background
            if (Background != null)
            {
                if (!Background.Dictionary.IsLoaded) Background.Dictionary.Load();

                float bgBottom = headerBottom;
                Menu.ForEachItemOnScreen((item, index) => bgBottom += itemHeight);

                float bgWidth = menuWidth;
                float bgHeight = bgBottom - headerBottom;

                DrawSprite(Background,
                           x,
                           headerBottom + bgHeight * 0.5f - 0.00138888f,
                           bgWidth,
                           bgHeight,
                           BackgroundColor);
            }

            // nav
            {
                float navWidth = menuWidth;
                float navHeight = itemHeight;

                int offset = Menu.SelectedIndex - Menu.ItemsOnScreenStartIndex;
                float navY = headerBottom + navHeight * 0.5f + (navHeight * offset);

                if (Nav != null)
                {
                    if (!Nav.Dictionary.IsLoaded) Nav.Dictionary.Load();

                    DrawSprite(Nav,
                               x, navY,
                               navWidth,
                               navHeight,
                               NavColor);
                }
                else
                {
                    DrawRect(x, navY,
                             navWidth,
                             navHeight,
                             NavColor);
                }
            }

            // items
            {
                Menu.ForEachItemOnScreen((item, index) =>
                {
                    void SetTextOptions(bool isSelected, bool isDisabled)
                    {
                        // TODO: add fields for item colors
                        if (!isDisabled)
                        {
                            if (isSelected)
                            {
                                N.SetTextColour(0, 0, 0, (int)(255f * 0.8f));
                            }
                            else
                            {
                                N.GetHudColour((int)HudColor.White, out int red, out int green, out int blue, out int alpha);
                                N.SetTextColour(red, green, blue, alpha);
                            }
                        }
                        else if (isSelected)
                        {
                            N.SetTextColour(155, 155, 155, 255);
                        }
                        else
                        {
                            N.SetTextColour(155, 155, 155, 255);
                        }
                        N.SetTextScale(0f, 0.35f);
                        N.SetTextJustification(1);
                        N.SetTextFont(0);
                        N.SetTextWrap(0f, 1f);
                        N.SetTextCentre(false);
                        N.SetTextDropshadow(0, 0, 0, 0, 0);
                        N.SetTextEdge(0, 0, 0, 0, 0);
                    }

                    SetTextOptions(item.IsSelected, item.IsDisabled);
                    N.BeginTextCommandDisplayText("STRING");
                    N.AddTextComponentSubstringPlayerName(item.Text);
                    N.EndTextCommandDisplayText(x - menuWidth * 0.5f + 0.0046875f, y + 0.00277776f);


                    // TODO: implement MenuItemCheckbox drawing
                    // TODO: implement MenuItem left/right badge drawing
                    switch (item)
                    {
                        case MenuItemScroller scroller:
                            {
                                if (scroller.IsSelected)
                                {
                                    SetTextOptions(item.IsSelected, item.IsDisabled);
                                    N.SetTextJustification(2);
                                    N.SetTextWrap(x - menuWidth * 0.5f + 0.0046875f, x + menuWidth * 0.5f - 0.0046875f);
                                    N.BeginTextCommandDisplayText("STRING");
                                    N.AddTextComponentSubstringPlayerName(scroller.GetSelectedOptionText());
                                    N.EndTextCommandDisplayText(0.0f, y + 0.00277776f);

                                    //TODO: scroller arrows
                                }
                                else
                                {
                                    SetTextOptions(item.IsSelected, item.IsDisabled);
                                    N.SetTextJustification(2);
                                    N.SetTextWrap(x - menuWidth * 0.5f + 0.0046875f, x + menuWidth * 0.5f - 0.0046875f);
                                    N.BeginTextCommandDisplayText("STRING");
                                    N.AddTextComponentSubstringPlayerName(scroller.GetSelectedOptionText());
                                    N.EndTextCommandDisplayText(0.0f, y + 0.00277776f);
                                }
                            }
                            break;
                        case MenuItemCheckbox checkbox:
                            {

                            }
                            break;
                    }

                    y += itemHeight;
                });
            }

            // up-down arrows
            if (UpDownArrows != null && Menu.Items.Count > Menu.MaxItemsOnScreen)
            {
                if (!UpDownArrows.Dictionary.IsLoaded) UpDownArrows.Dictionary.Load();

                float upDownRectWidth = menuWidth;
                float upDownRectHeight = itemHeight;

                y += 0.0001f;
                DrawRect(x, y + upDownRectHeight * 0.5f, upDownRectWidth, upDownRectHeight, UpDownArrowsBackColor);

                float fVar61 = 1.0f; // TODO: this may need to be calculated based on current resolution
                float upDownWidth = UpDownArrows.Width * (0.5f / fVar61);
                float upDownHeight = UpDownArrows.Height * (0.5f / fVar61);
                upDownWidth = upDownWidth / 1280.0f * fVar61;
                upDownHeight = upDownHeight / 720f * fVar61;
                DrawSprite(UpDownArrows, x, y + upDownRectHeight * 0.5f, upDownWidth, upDownHeight, UpDownArrowsForeColor);

                y += itemHeight;
            }

            // description
            if (!String.IsNullOrWhiteSpace(Menu.CurrentDescription))
            {
                y += 0.00277776f * 2f;

                float textX = x - menuWidth * 0.5f + 0.0046875f;  // x - menuWidth * 0.5f + 0.0046875f
                float textXEnd = x + menuWidth * 0.5f - 0.0046875f; // x - menuWidth * 0.5f + menuWidth - 0.0046875f

                void SetDescriptionTextOptions()
                {
                    N.SetTextFont(0);
                    N.SetTextScale(0f, 0.35f);
                    N.SetTextLeading(2);
                    N.SetTextColour(DescriptionForeColor.R, DescriptionForeColor.G, DescriptionForeColor.B, DescriptionForeColor.A);
                    N.SetTextWrap(textX, textXEnd);
                    N.SetTextCentre(false);
                    N.SetTextDropshadow(0, 0, 0, 0, 0);
                    N.SetTextEdge(0, 0, 0, 0, 0);
                }

                void PushDescriptionText()
                {
                    const int MaxSubstringLength = 99;
                    const int MaxSubstrings = 4;

                    string text = Menu.CurrentDescription;
                    for (int i = 0, c = 0; i < text.Length && c < MaxSubstrings; i += MaxSubstringLength, c++)
                    {
                        // TODO: get description substrings only when it changes using Menu.PropertyChanged event 
                        string str = text.Substring(i, Math.Min(MaxSubstringLength, text.Length - i));
                        N.AddTextComponentSubstringPlayerName(str);
                    }
                }

                const string DescFormat = "CELL_EMAIL_BCON";

                SetDescriptionTextOptions();
                N.BeginTextCommandGetLineCount(DescFormat);
                PushDescriptionText();
                int lineCount = N.EndTextCommandGetLineCount(textX, y + 0.00277776f);
                
                DrawRect(x, y - 0.00277776f * 0.5f, menuWidth, 0.00277776f, DescriptionSeparatorBarColor);

                float descHeight = (N.GetTextScaleHeight(0.35f, 0) * lineCount) + (0.00138888f * 13f) + (0.00138888f * 5f * (lineCount - 1));
                DrawSprite(DescriptionBackground,
                           x,
                           y + (descHeight * 0.5f) - 0.00138888f,
                           menuWidth,
                           descHeight,
                           DescriptionBackColor);

                SetDescriptionTextOptions();
                N.BeginTextCommandDisplayText(DescFormat);
                PushDescriptionText();
                N.EndTextCommandDisplayText(textX, y + 0.00277776f);

                y += descHeight;
            }

            N.ResetScriptGfxAlign();
        }


        private void DrawSprite(TextureReference tex, float x, float y, float w, float h, Color c)
            => N.DrawSprite(tex.Dictionary, tex.Name, x, y, w, h, 0.0f, c.R, c.G, c.B, c.A);

        private void DrawRect(float x, float y, float w, float h, Color c)
            => N.DrawRect(x, y, w, h, c.R, c.G, c.B, c.A);

        private bool IsWideScreen => aspectRatio > 1.5f; // equivalent to GET_IS_WIDESCREEN
        private bool IsUltraWideScreen => aspectRatio > 3.5f; // > 32:9
    }
}

