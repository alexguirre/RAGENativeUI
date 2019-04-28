namespace RAGENativeUI.Menus.Themes
{
#if RPH1
    extern alias rph1;
    using Graphics = rph1::Rage.Graphics;
    using Vector2 = rph1::Rage.Vector2;
#else
    /** REDACTED **/
#endif

    using System;
    using System.Drawing;
    using System.Linq;

    public class MenuDefaultTheme : MenuTheme
    {
        public TextureReference Banner { get; set; }
        public Color BannerBackColor { get; set; } = Color.White;
        public Color BannerForeColor { get; set; } = HudColor.White.GetColor();
        public Color SubtitleBackColor { get; set; } = Color.Black;
        public Color SubtitleForeColor { get; set; } = Color.White;
        public TextureReference NavigationBar { get; set; }
        public Color NavigationBarColor { get; set; } = Color.White;
        public TextureReference UpDownArrows { get; set; }
        public Color UpDownArrowsBackColor { get; set; } = Color.FromArgb(204, 0, 0, 0);
        public Color UpDownArrowsForeColor { get; set; } = Color.FromArgb(255, 255, 255);
        public TextureReference Background { get; set; }
        public Color BackgroundColor { get; set; } = Color.FromArgb(255, 255, 255);
        public TextureReference DescriptionBackground { get; set; }
        public Color DescriptionBackColor { get; set; } = Color.FromArgb(186, 0, 0, 0);
        public Color DescriptionForeColor { get; set; } = Color.FromArgb(240, 240, 240);
        public Color DescriptionSeparatorBarColor { get; set; } = Color.FromArgb(0, 0, 0);
        // TODO: implement per item colors using MenuItem.Metadata
        public Color ItemEnabledColor { get; set; } = Color.FromArgb(240, 240, 240);
        public Color ItemEnabledAndSelectedColor { get; set; } = Color.FromArgb(204, 0, 0, 0);
        public Color ItemDisabledColor { get; set; } = Color.FromArgb(155, 155, 155, 155);
        public Color ItemDisabledAndSelectedColor { get; set; } = Color.FromArgb(155, 155, 155, 155);
        public TextureReference CheckboxUncheckedTexture { get; set; }
        public TextureReference CheckboxCheckedTexture { get; set; }
        public TextureReference CheckboxUncheckedAndSelectedTexture { get; set; }
        public TextureReference CheckboxCheckedAndSelectedTexture { get; set; }
        public TextureReference ArrowLeftTexture { get; set; }
        public TextureReference ArrowRightTexture { get; set; }

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
                    case "gradient_nav": NavigationBar = t; break;
                    case "shop_arrows_upanddown": UpDownArrows = t; break;
                    case "gradient_bgd": Background = t; DescriptionBackground = t; break;
                    case "shop_box_blank": CheckboxUncheckedTexture = t; break;
                    case "shop_box_tick": CheckboxCheckedTexture = t; break;
                    case "shop_box_blankb": CheckboxUncheckedAndSelectedTexture = t; break;
                    case "shop_box_tickb": CheckboxCheckedAndSelectedTexture = t; break;
                    case "arrowleft": ArrowLeftTexture = t; break;
                    case "arrowright": ArrowRightTexture = t; break;
                }
            }
        }

        public override MenuTheme Clone(Menu menu)
        {
            return new MenuDefaultTheme(menu)
            {
                Banner = Banner,
                BannerBackColor = BannerBackColor,
                BannerForeColor = BannerForeColor,
                SubtitleBackColor = SubtitleBackColor,
                SubtitleForeColor = SubtitleForeColor,
                NavigationBar = NavigationBar,
                NavigationBarColor = NavigationBarColor,
                UpDownArrowsBackColor = UpDownArrowsBackColor,
                UpDownArrowsForeColor = UpDownArrowsForeColor,
                Background = Background,
                BackgroundColor = BackgroundColor,
                DescriptionBackground = DescriptionBackground,
                DescriptionBackColor = DescriptionBackColor,
                DescriptionForeColor = DescriptionForeColor,
                DescriptionSeparatorBarColor = DescriptionSeparatorBarColor,
                ItemEnabledColor = ItemEnabledColor,
                ItemEnabledAndSelectedColor = ItemEnabledAndSelectedColor,
                ItemDisabledColor = ItemDisabledColor,
                ItemDisabledAndSelectedColor = ItemDisabledAndSelectedColor,
                CheckboxUncheckedTexture = CheckboxUncheckedTexture,
                CheckboxCheckedTexture = CheckboxCheckedTexture,
                CheckboxUncheckedAndSelectedTexture = CheckboxUncheckedAndSelectedTexture,
                CheckboxCheckedAndSelectedTexture = CheckboxCheckedAndSelectedTexture,
                ArrowLeftTexture = ArrowLeftTexture,
                ArrowRightTexture = ArrowRightTexture,
            };
        }

        public override void Draw(Graphics g)
        {
            // Note: any magic constants found here were taken from the game scripts

            aspectRatio = N.GetAspectRatio(false);
            menuWidth = 0.225f;
            if (aspectRatio < 1.77777f) // less than 16:9
            {
                menuWidth = 0.225f * (16f / 9f / aspectRatio);
            }

            N.SetScriptGfxAlign('L', 'T');
            N.SetScriptGfxAlignParams(-0.05f, -0.05f, 0.0f, 0.0f);

            float x = 0.05f;
            float y = 0.05f;

            // testoffset
            //x += 0.02222f;
            
            DrawBanner(x, ref y);
            
            DrawSubtitle(x, ref y);

            float headerBottom = y;
            
            DrawBackground(x, headerBottom);
            
            DrawNavigationBar(x, headerBottom);

            DrawItems(x, ref y);

            DrawUpDownArrows(x, ref y);

            DrawDescription(x, ref y);

            N.ResetScriptGfxAlign();
        }

        private void DrawBanner(float x, ref float y)
        {
            if (Banner != null)
            {
                if (!Banner.Dictionary.IsLoaded) Banner.Dictionary.Load();

                GetTextureDrawSize(Banner, true, out float bannerWidth, out float bannerHeight, false);

                DrawSprite(Banner, x + menuWidth * 0.5f, y + bannerHeight * 0.5f, bannerWidth, bannerHeight, BannerBackColor);

                DrawTitle(x, ref y, bannerHeight);

                y += bannerHeight;
            }
        }

        private void DrawTitle(float x, ref float y, float bannerHeight)
        {
            float titleX = x + menuWidth * 0.5f;
            float titleY = y + bannerHeight * 0.225f;

            N.SetTextFont((int)Text.TextFont.HouseScript);
            N.SetTextScale(0.0f, 1.025f);
            N.SetTextColour(BannerForeColor.R, BannerForeColor.G, BannerForeColor.B, BannerForeColor.A);
            N.SetTextWrap(x, x + menuWidth);
            N.SetTextCentre(true);

            N.BeginTextCommandDisplayText("STRING");
            N.AddTextComponentSubstringPlayerName(Menu.Title);
            N.EndTextCommandDisplayText(titleX, titleY);
        }

        private void DrawSubtitle(float x, ref float y)
        {
            float subtitleWidth = menuWidth;
            float subtitleHeight = itemHeight;

            DrawRect(x + menuWidth * 0.5f, y + subtitleHeight * 0.5f, subtitleWidth, subtitleHeight, SubtitleBackColor);

            DrawSubtitleText(x, y);
            
            DrawSubtitleCounter(x, y);

            y += subtitleHeight;
        }

        private void DrawSubtitleText(float x, float y)
        {
            float subTextX = x + 0.00390625f;
            float subTextY = y + 0.00416664f;

            N.SetTextFont(0);
            N.SetTextScale(0f, 0.35f);
            N.SetTextColour(SubtitleForeColor.R, SubtitleForeColor.G, SubtitleForeColor.B, SubtitleForeColor.A);
            N.SetTextWrap(x + 0.0046875f, x + menuWidth - 0.0046875f);
            N.SetTextCentre(false);
            N.SetTextDropshadow(0, 0, 0, 0, 0);
            N.SetTextEdge(0, 0, 0, 0, 0);

            N.BeginTextCommandDisplayText("STRING");
            N.AddTextComponentSubstringPlayerName(Menu.Subtitle);
            N.EndTextCommandDisplayText(subTextX, subTextY);
        }

        private void DrawSubtitleCounter(float x, float y)
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
                    N.SetTextWrap(x + 0.0046875f, x + menuWidth - 0.0046875f);
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

                float counterX = x + menuWidth - 0.00390625f - counterWidth;
                float counterY = y + 0.00416664f;

                SetCounterTextOptions();
                N.BeginTextCommandDisplayText(CounterFormat);
                PushCounterComponents();
                N.EndTextCommandDisplayText(counterX, counterY);

            }
        }

        private void DrawBackground(float x, float headerBottom)
        {
            if (Background != null)
            {
                if (!Background.Dictionary.IsLoaded) Background.Dictionary.Load();

                float bgBottom = headerBottom;
                Menu.ForEachItemOnScreen((item, index) => bgBottom += itemHeight);

                float bgWidth = menuWidth;
                float bgHeight = bgBottom - headerBottom;

                DrawSprite(Background,
                           x + bgWidth * 0.5f,
                           headerBottom + bgHeight * 0.5f - 0.00138888f,
                           bgWidth,
                           bgHeight,
                           BackgroundColor);
            }
        }

        private void DrawNavigationBar(float x, float headerBottom)
        {
            if (!Menu.IsAnyItemOnScreen)
            {
                return;
            }


            float navWidth = menuWidth;
            float navHeight = itemHeight;

            int offset = 0;
            for (int i = Menu.ItemsOnScreenStartIndex; i < Menu.SelectedIndex; i++)
            {
                if (Menu.Items[i].IsVisible)
                {
                    offset++;
                }
            }

            float navX = x + navWidth * 0.5f;
            float navY = headerBottom + navHeight * 0.5f + (navHeight * offset);

            if (NavigationBar != null)
            {
                if (!NavigationBar.Dictionary.IsLoaded) NavigationBar.Dictionary.Load();

                DrawSprite(NavigationBar,
                           navX, navY,
                           navWidth,
                           navHeight,
                           NavigationBarColor);
            }
            else
            {
                DrawRect(navX, navY,
                         navWidth,
                         navHeight,
                         NavigationBarColor);
            }
        }

        private void DrawUpDownArrows(float x, ref float y)
        {
            if (UpDownArrows != null && Menu.Items.Count > Menu.MaxItemsOnScreen)
            {
                if (!UpDownArrows.Dictionary.IsLoaded) UpDownArrows.Dictionary.Load();

                float upDownRectWidth = menuWidth;
                float upDownRectHeight = itemHeight;

                y += 0.0001f;
                DrawRect(x + upDownRectWidth * 0.5f, y + upDownRectHeight * 0.5f, upDownRectWidth, upDownRectHeight, UpDownArrowsBackColor);

                float fVar61 = 1.0f; // TODO: this may need to be calculated based on current resolution
                float upDownWidth = UpDownArrows.Width * (0.5f / fVar61);
                float upDownHeight = UpDownArrows.Height * (0.5f / fVar61);
                upDownWidth = upDownWidth / 1280.0f * fVar61;
                upDownHeight = upDownHeight / 720f * fVar61;
                DrawSprite(UpDownArrows, x + upDownRectWidth * 0.5f, y + upDownRectHeight * 0.5f, upDownWidth, upDownHeight, UpDownArrowsForeColor);

                y += itemHeight;
            }
        }

        private void DrawDescription(float x, ref float y)
        {
            if (!String.IsNullOrWhiteSpace(Menu.CurrentDescription))
            {
                y += 0.00277776f * 2f;

                float textX = x + 0.0046875f;  // x - menuWidth * 0.5f + 0.0046875f
                float textXEnd = x + menuWidth - 0.0046875f; // x - menuWidth * 0.5f + menuWidth - 0.0046875f

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

                DrawRect(x + menuWidth * 0.5f, y - 0.00277776f * 0.5f, menuWidth, 0.00277776f, DescriptionSeparatorBarColor);

                float descHeight = (N.GetTextScaleHeight(0.35f, 0) * lineCount) + (0.00138888f * 13f) + (0.00138888f * 5f * (lineCount - 1));
                DrawSprite(DescriptionBackground,
                           x + menuWidth * 0.5f,
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
        }

        private void DrawItems(float x, ref float y)
        {
            float posY = y;
            Menu.ForEachItemOnScreen((item, index) =>
            {
                DrawItemBase(item, x, posY);

                // TODO: implement MenuItem left/right badge drawing
                switch (item)
                {
                    case MenuItemScroller scroller: DrawItemScroller(scroller, x, posY); break;
                    case MenuItemCheckbox checkbox: DrawItemCheckbox(checkbox, x, posY); break;
                }

                posY += itemHeight;
            });

            y = posY;
        }

        private void SetItemTextOptions(MenuItem item)
        {
            Color c = GetItemColor(item.IsDisabled, item.IsSelected);
            N.SetTextColour(c.R, c.G, c.B, c.A);
            N.SetTextScale(0f, 0.35f);
            N.SetTextJustification(1);
            N.SetTextFont(0);
            N.SetTextWrap(0f, 1f);
            N.SetTextCentre(false);
            N.SetTextDropshadow(0, 0, 0, 0, 0);
            N.SetTextEdge(0, 0, 0, 0, 0);
        }

        private void DrawItemBase(MenuItem item, float x, float y)
        {
            SetItemTextOptions(item);
            N.BeginTextCommandDisplayText("STRING");
            N.AddTextComponentSubstringPlayerName(item.Text);
            N.EndTextCommandDisplayText(x + 0.0046875f, y + 0.00277776f);
        }

        private void DrawItemScroller(MenuItemScroller scroller, float x, float y)
        {
            const string ScrollerOptionTextFormat = "STRING";

            if (scroller.IsSelected && !scroller.IsDisabled)
            {
                string selectedOption = scroller.SelectedOptionText;

                SetItemTextOptions(scroller);
                N.BeginTextCommandGetWidth(ScrollerOptionTextFormat);
                N.AddTextComponentSubstringPlayerName(selectedOption);
                float optTextWidth = N.EndTextCommandGetWidth(true);

                float optTextX = x + menuWidth - 0.00390625f - optTextWidth - (0.0046875f * 1.5f);
                float optTextY = y + 0.00277776f;

                SetItemTextOptions(scroller);
                N.BeginTextCommandDisplayText(ScrollerOptionTextFormat);
                N.AddTextComponentSubstringPlayerName(selectedOption);
                N.EndTextCommandDisplayText(optTextX, optTextY);

                {
                    TextureReference tex = ArrowRightTexture;

                    GetTextureDrawSize(tex, true, out float w, out float h, false);
                    w *= 0.65f;
                    h *= 0.65f;

                    float spriteX = x + menuWidth - (0.00390625f * 1.0f) - (w * 0.5f);
                    float spriteY = y + (0.034722f * 0.5f);

                    DrawSprite(tex, spriteX, spriteY, w, h, HudColor.Black.GetColor());
                }
                {
                    TextureReference tex = ArrowLeftTexture;

                    GetTextureDrawSize(tex, true, out float w, out float h, false);
                    w *= 0.65f;
                    h *= 0.65f;

                    float spriteX = x + menuWidth - (0.00390625f * 1.0f) - (w * 0.5f) - optTextWidth - (0.0046875f * 1.5f);
                    float spriteY = y + (0.034722f * 0.5f);

                    DrawSprite(tex, spriteX, spriteY, w, h, HudColor.Black.GetColor());
                }
            }
            else
            {
                string selectedOption = scroller.SelectedOptionText;

                SetItemTextOptions(scroller);
                N.BeginTextCommandGetWidth(ScrollerOptionTextFormat);
                N.AddTextComponentSubstringPlayerName(selectedOption);
                float optTextWidth = N.EndTextCommandGetWidth(true);

                float optTextX = x + menuWidth - 0.00390625f - optTextWidth;
                float optTextY = y + 0.00277776f;// + 0.00416664f;

                SetItemTextOptions(scroller);
                N.BeginTextCommandDisplayText(ScrollerOptionTextFormat);
                N.AddTextComponentSubstringPlayerName(selectedOption);
                N.EndTextCommandDisplayText(optTextX, optTextY);
            }
        }

        private void DrawItemCheckbox(MenuItemCheckbox checkbox, float x, float y)
        {
            TextureReference tex = checkbox.IsChecked ?
                (checkbox.IsSelected ? CheckboxCheckedAndSelectedTexture : CheckboxCheckedTexture) :
                (checkbox.IsSelected ? CheckboxUncheckedAndSelectedTexture : CheckboxUncheckedTexture);

            GetTextureDrawSize(tex, true, out float w, out float h, false);

            float spriteX = x + menuWidth - (w * 0.5f);
            float spriteY = y + (h * 0.5f) - (0.00138888f * 4.0f);

            DrawSprite(tex, spriteX, spriteY, w, h, HudColor.White.GetColor());
        }

        // Converted from game scripts
        void GetTextureDrawSize(TextureReference texture, bool bParam2, out float width, out float height, bool bParam5)
        {
            float func_341()
            {
                if (texture == Banner)
                {
                    return 1.0f;
                }
                else
                {
                    return 0.5f;
                }
            }

            //string dictName;
            //string texName;
            int screenWidth;
            int screenHeight;
            float fVar4;
            Vector2 texSize;

            //dictName = texture.Dictionary;
            //texName = texture.Name;
            fVar4 = 1f;
            if (bParam5)
            {
                N.GetActiveScreenResolution(out screenWidth, out screenHeight);
                if (IsUltraWideScreen)
                {
                    screenWidth = (int)Math.Round(screenHeight * aspectRatio);
                }
                float screenRatio = (float)screenWidth / screenHeight;
                fVar4 = screenRatio / aspectRatio;
                if (IsUltraWideScreen)
                {
                    fVar4 = 1f;
                }
                //if (SCRIPT::_GET_NUMBER_OF_INSTANCES_OF_SCRIPT_WITH_NAME_HASH(joaat("director_mode")) > 0)
                //{
                //    N.GetScreenResolution(out iVar2, out iVar3);
                //}
                screenWidth = (int)Math.Round(screenWidth / fVar4);
                screenHeight = (int)Math.Round(screenHeight / fVar4);
            }
            else
            {
                N.GetScreenResolution(out screenWidth, out screenHeight);
            }
            texSize = new Vector2(texture.Width, texture.Height);
            texSize.X = (texSize.X * (func_341() / fVar4));
            texSize.Y = (texSize.Y * (func_341() / fVar4));
            if (!bParam2)
            {
                texSize.X = (texSize.X - 2f);
                texSize.Y = (texSize.Y - 2f);
            }
            //if (iParam0 == 30) // texture with id 30 is unused for menu drawing
            //{
            //    vVar7.x = 288f;
            //    vVar7.y = 106f;
            //}
            //if (iParam0 == Banner && MISC::GET_HASH_KEY(&(Global_17345.f_6719[29 /*16*/])) == -1487683087/*crew_logo*/) // no crew logos here
            //{
            //    vVar7.x = 106f;
            //    vVar7.y = 106f;
            //}
            width = texSize.X / screenWidth * (screenWidth / screenHeight);
            height = texSize.Y / screenHeight / (texSize.X / screenWidth) * width;
            if (!bParam5)
            {
                if (!IsWideScreen && true/*iParam0 != 30*/) // texture with id 30 is unused for menu drawing, so it's always not equal to 30
                {
                    width = width * 1.33f;
                }
            }
            if (texture == Banner)
            {
                if (width > menuWidth)
                {
                    height = (height * (menuWidth / width));
                    width = menuWidth;
                }
            }
        }

        private void DrawSprite(TextureReference tex, float x, float y, float w, float h, Color c)
            => N.DrawSprite(tex.Dictionary, tex.Name, x, y, w, h, 0.0f, c.R, c.G, c.B, c.A);

        private void DrawRect(float x, float y, float w, float h, Color c)
            => N.DrawRect(x, y, w, h, c.R, c.G, c.B, c.A);

        private bool IsWideScreen => aspectRatio > 1.5f; // equivalent to GET_IS_WIDESCREEN
        private bool IsUltraWideScreen => aspectRatio > 3.5f; // > 32:9

        private Color GetItemColor(bool disabled, bool selected)
        {
            if (!disabled)
            {
                if (selected)
                {
                    return ItemEnabledAndSelectedColor;
                }
                else
                {
                    return ItemEnabledColor;
                }
            }
            else if (selected)
            {
                return ItemDisabledAndSelectedColor;
            }
            else
            {
                return ItemDisabledColor;
            }
        }
    }
}

