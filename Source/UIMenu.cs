/*
*
*
* Created by: Guad, CamxxCore, jedijosh920
*
* 
* Ported by: alexguirre, Stealth22, LtFlash 
*
*/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Rage;
using Rage.Native;
using RAGENativeUI.Elements;

namespace RAGENativeUI
{
    public delegate void IndexChangedEvent(UIMenu sender, int newIndex);

    public delegate void ListChangedEvent(UIMenu sender, UIMenuListItem listItem, int newIndex);

    public delegate void CheckboxChangeEvent(UIMenu sender, UIMenuCheckboxItem checkboxItem, bool Checked);

    public delegate void ScrollerChangedEvent(UIMenu sender, UIMenuScrollerItem item, int oldIndex, int newIndex);
    
    public delegate void ItemSelectEvent(UIMenu sender, UIMenuItem selectedItem, int index);

    public delegate void MenuOpenEvent(UIMenu sender);

    public delegate void MenuCloseEvent(UIMenu sender);

    public delegate void MenuChangeEvent(UIMenu oldMenu, UIMenu newMenu, bool forward);

    public delegate void ItemActivatedEvent(UIMenu sender, UIMenuItem selectedItem);

    public delegate void ItemCheckboxEvent(UIMenuCheckboxItem sender, bool Checked);

    public delegate void ItemListEvent(UIMenuItem sender, int newIndex);

    public delegate void ItemScrollerEvent(UIMenuScrollerItem sender, int oldIndex, int newIndex);

    /// <summary>
    /// Base class for RAGENativeUI. Calls the next events: OnIndexChange, OnListChanged, OnCheckboxChange, OnItemSelect, OnMenuClose, OnMenuchange.
    /// </summary>
    public class UIMenu
    {
        public const int DefaultMaxItemsOnScreen = 10;

        internal const string CommonTxd = "commonmenu";
        internal const string BackgroundTextureName = "gradient_bgd";
        internal const string UpAndDownTextureName = "shop_arrows_upanddown";
        internal const string NavBarTextureName = "gradient_nav";
        internal const string ArrowLeftTextureName = "arrowleft";
        internal const string ArrowRightTextureName = "arrowright";
        internal const string CheckboxTickTextureName = "shop_box_tick";
        internal const string CheckboxBlankTextureName = "shop_box_blank";
        internal const string CheckboxTickSelectedTextureName = "shop_box_tickb";
        internal const string CheckboxBlankSelectedTextureName = "shop_box_blankb";
        internal const string DefaultBannerTextureName = "interaction_bgd";

        private Sprite _bannerSprite;
        private ResRectangle _bannerRectangle;

        private Texture _customBanner;

        private int currentItem;
        private int minItem;
        private int maxItem;
        private int hoveredItem = -1;
        private int maxItemsOnScreen = DefaultMaxItemsOnScreen;

        private bool visible;
        private bool justOpened = true;

        private int hoveredUpDown = 0; // 0 = none, 1 = up, 2 = down

        public string AUDIO_LIBRARY = "HUD_FRONTEND_DEFAULT_SOUNDSET";

        public string AUDIO_UPDOWN = "NAV_UP_DOWN";
        public string AUDIO_LEFTRIGHT = "NAV_LEFT_RIGHT";
        public string AUDIO_SELECT = "SELECT";
        public string AUDIO_BACK = "BACK";
        public string AUDIO_ERROR = "ERROR";

        public List<UIMenuItem> MenuItems = new List<UIMenuItem>();

        public bool MouseEdgeEnabled = true;
        public bool ControlDisablingEnabled = true;
        public bool ResetCursorOnOpen = true;
        [Obsolete("Now description are always wrapped to fit the description box."), EditorBrowsable(EditorBrowsableState.Never)]
        public bool FormatDescriptions = true;
        public bool MouseControlsEnabled = true;
        public bool AllowCameraMovement = false;
        public bool ScaleWithSafezone = true;

        //Events

        /// <summary>
        /// Called when user presses up or down, changing current selection.
        /// </summary>
        public event IndexChangedEvent OnIndexChange;

        /// <summary>
        /// Called when user presses left or right, changing a list position.
        /// </summary>
        public event ListChangedEvent OnListChange;

        public event ScrollerChangedEvent OnScrollerChange;

        /// <summary>
        /// Called when user presses enter on a checkbox item.
        /// </summary>
        public event CheckboxChangeEvent OnCheckboxChange;

        /// <summary>
        /// Called when user selects a simple item.
        /// </summary>
        public event ItemSelectEvent OnItemSelect;

        /// <summary>
        /// Called when the menu becomes visible.
        /// </summary>
        public event MenuOpenEvent OnMenuOpen;

        /// <summary>
        /// Called when user closes the menu or goes back in a menu chain.
        /// </summary>
        public event MenuCloseEvent OnMenuClose;

        /// <summary>
        /// Called when user either clicks on a binded button or goes back to a parent menu.
        /// </summary>
        public event MenuChangeEvent OnMenuChange;

        //Keys
        private readonly Dictionary<Common.MenuControls, Tuple<List<Keys>, List<Tuple<GameControl, int>>>> _keyDictionary = new Dictionary<Common.MenuControls, Tuple<List<Keys>, List<Tuple<GameControl, int>>>>();

        //Tree structure
        public Dictionary<UIMenuItem, UIMenu> Children { get; private set; }

        /// <summary>
        /// Gets the <see cref="Elements.InstructionalButtons"/> instance used by this menu.
        /// </summary>
        public InstructionalButtons InstructionalButtons { get; }

        /// <summary>
        /// Gets or sets whether the instructional buttons are currently visible.
        /// </summary>
        public bool InstructionalButtonsEnabled { get; set; } = true;

        /// <summary>
        /// Basic Menu constructor.
        /// </summary>
        /// <param name="title">Title that appears on the big banner.</param>
        /// <param name="subtitle">Subtitle that appears in capital letters in a small black bar.</param>
        public UIMenu(string title, string subtitle) : this(title, subtitle, new Point(0, 0), CommonTxd, DefaultBannerTextureName)
        {
        }

        /// <summary>
        /// Basic Menu constructor with an offset.
        /// </summary>
        /// <param name="title">Title that appears on the big banner.</param>
        /// <param name="subtitle">Subtitle that appears in capital letters in a small black bar. Set to "" if you dont want a subtitle.</param>
        /// <param name="offset">Point object with X and Y data for offsets. Applied to all menu elements.</param>
        public UIMenu(string title, string subtitle, Point offset) : this(title, subtitle, offset, CommonTxd, DefaultBannerTextureName)
        {
        }

        /// <summary>
        /// Initialise a menu with a custom texture banner.
        /// </summary>
        /// <param name="title">Title that appears on the big banner. Set to "" if you don't want a title.</param>
        /// <param name="subtitle">Subtitle that appears in capital letters in a small black bar. Set to "" if you dont want a subtitle.</param>
        /// <param name="offset">Point object with X and Y data for offsets. Applied to all menu elements.</param>
        /// <param name="customBanner">Your custom Rage.Texture.</param>
        public UIMenu(string title, string subtitle, Point offset, Texture customBanner) : this(title, subtitle, offset, CommonTxd, DefaultBannerTextureName)
        {
            _customBanner = customBanner;
        }

        /// <summary>
        /// Advanced Menu constructor that allows custom title banner.
        /// </summary>
        /// <param name="title">Title that appears on the big banner. Set to "" if you are using a custom banner.</param>
        /// <param name="subtitle">Subtitle that appears in capital letters in a small black bar.</param>
        /// <param name="offset">Point object with X and Y data for offsets. Applied to all menu elements.</param>
        /// <param name="spriteLibrary">Sprite library name for the banner.</param>
        /// <param name="spriteName">Sprite name for the banner.</param>
        public UIMenu(string title, string subtitle, Point offset, string spriteLibrary, string spriteName)
        {
            Offset = offset;
            Children = new Dictionary<UIMenuItem, UIMenu>();
            WidthOffset = 0;

            InstructionalButtons = new InstructionalButtons();
            InstructionalButtons.Buttons.Add(new InstructionalButton(GameControl.CellphoneSelect, "Select"));
            InstructionalButtons.Buttons.Add(new InstructionalButton(GameControl.CellphoneCancel, "Back"));

            _bannerSprite = new Sprite(spriteLibrary, spriteName, Point.Empty, Size.Empty);
            Title = new ResText(title, new Point(0, 0), 0.0f, Color.White, Common.EFont.HouseScript, ResText.Alignment.Centered);
            Subtitle = new ResText(subtitle, new Point(0, 0), 0.0f, Color.White, Common.EFont.ChaletLondon, ResText.Alignment.Left);
            if (subtitle != null && subtitle.StartsWith("~"))
            {
                CounterPretext = subtitle.Substring(0, subtitle.IndexOf('~', 1) + 1);
            }

            CurrentSelection = -1;

            SetKey(Common.MenuControls.Up, GameControl.CellphoneUp);
            SetKey(Common.MenuControls.Up, GameControl.CursorScrollUp);

            SetKey(Common.MenuControls.Down, GameControl.CellphoneDown);
            SetKey(Common.MenuControls.Down, GameControl.CursorScrollDown);

            SetKey(Common.MenuControls.Left, GameControl.CellphoneLeft);
            SetKey(Common.MenuControls.Right, GameControl.CellphoneRight);
            SetKey(Common.MenuControls.Select, GameControl.FrontendAccept);

            SetKey(Common.MenuControls.Back, GameControl.CellphoneCancel);
            SetKey(Common.MenuControls.Back, GameControl.FrontendPause);
        }

        /// <summary>
        /// Gets or sets the current width offset in pixels.
        /// </summary>
        public int WidthOffset { get; set; }

        /// <summary>
        /// Change the menu's width. The width is calculated as DefaultWidth + WidthOffset, so a width offset of 10 would enlarge the menu by 10 pixels.
        /// </summary>
        /// <param name="widthOffset">New width offset.</param>
        [Obsolete("Use UIMenu.WidthOffset setter instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public void SetMenuWidthOffset(int widthOffset)
        {
            WidthOffset = widthOffset;
        }

        /// <summary>
        /// Gets or sets the current position offset in pixels.
        /// </summary>
        public Point Offset { get; set; }

        /// <summary>
        /// Enable or disable all controls but the necessary to operate a menu.
        /// </summary>
        /// <param name="enable"></param>
        public static void DisEnableControls(bool enable)
        {
            if (enable)
            {
                N.EnableAllControlActions(0);
                return;
            }
            else
            {
                N.DisableAllControlActions(0);
            }

            //Controls we want
            // -Frontend
            // -Mouse
            // -Walk/Move
            // -

            for (int i = 0; i < menuNavigationNeededControls.Length; i++)
            {
                N.EnableControlAction(0, menuNavigationNeededControls[i]);
            }

            if (IsUsingController)
            {
                for (int i = 0; i < menuNavigationControllerNeededControls.Length; i++)
                {
                    N.EnableControlAction(0, menuNavigationControllerNeededControls[i]);
                }
            }
        }

        private static readonly GameControl[] menuNavigationNeededControls =
        {
            GameControl.FrontendAccept,
            GameControl.FrontendAxisX,
            GameControl.FrontendAxisY,
            GameControl.FrontendDown,
            GameControl.FrontendUp,
            GameControl.FrontendLeft,
            GameControl.FrontendRight,
            GameControl.FrontendCancel,
            GameControl.FrontendSelect,
            GameControl.CursorScrollDown,
            GameControl.CursorScrollUp,
            GameControl.CursorX,
            GameControl.CursorY,
            GameControl.CursorAccept,
            GameControl.CursorCancel,
            GameControl.MoveUpDown,
            GameControl.MoveLeftRight,
            GameControl.Sprint,
            GameControl.Jump,
            GameControl.Enter,
            GameControl.VehicleExit,
            GameControl.VehicleAccelerate,
            GameControl.VehicleBrake,
            GameControl.VehicleMoveLeftRight,
            GameControl.VehicleFlyYawLeft,
            GameControl.ScriptedFlyLeftRight,
            GameControl.ScriptedFlyUpDown,
            GameControl.VehicleFlyYawRight,
            GameControl.VehicleHandbrake,
        };

        private static readonly GameControl[] menuNavigationControllerNeededControls =
        {
            GameControl.LookUpDown,
            GameControl.LookLeftRight,
            GameControl.Aim,
            GameControl.Attack,
        };

        /// <summary>
        /// Enable or disable the instructional buttons.
        /// </summary>
        /// <param name="disable"></param>
        public void DisableInstructionalButtons(bool disable)
        {
            InstructionalButtonsEnabled = !disable;
        }

        /// <summary>
        /// Set the banner to your own Sprite object.
        /// </summary>
        /// <param name="spriteBanner">Sprite object. The position and size does not matter.</param>
        public void SetBannerType(Sprite spriteBanner)
        {
            _bannerRectangle = null;
            _customBanner = null;
            _bannerSprite = spriteBanner;
        }

        /// <summary>
        ///  Set the banner to your own Rectangle.
        /// </summary>
        /// <param name="rectangle">UIResRectangle object. Position and size does not matter.</param>
        public void SetBannerType(ResRectangle rectangle)
        {
            _bannerSprite = null;
            _customBanner = null;
            _bannerRectangle = rectangle;
        }

        /// <summary>
        /// Set the banner to your own custom texture. Set it to null if you want to restore the banner.
        /// </summary>
        /// <param name="texture">Rage.Texture object</param>
        public void SetBannerType(Texture texture)
        {
            _bannerSprite = null;
            _bannerRectangle = null;
            _customBanner = texture;
        }

        /// <summary>
        /// Add an item to the menu.
        /// </summary>
        /// <param name="item">Item object to be added. Can be normal item, checkbox or list item.</param>
        public void AddItem(UIMenuItem item)
        {
            item.Parent = this;
            MenuItems.Add(item);

            RefreshCurrentSelection();
        }

        /// <summary>
        /// Add an item to the menu at the specified index.
        /// </summary>
        /// <param name="item">Item object to be added. Can be normal item, checkbox or list item.</param>
        /// <param name="index"></param>
        public void AddItem(UIMenuItem item, int index)
        {
            item.Parent = this;
            MenuItems.Insert(index, item);

            RefreshCurrentSelection();
        }

        /// <summary>
        /// Remove an item at index n.
        /// </summary>
        /// <param name="index">Index to remove the item at.</param>
        public void RemoveItemAt(int index)
        {
            if (maxItem == MenuItems.Count - 1) // if max visible item matches last item, move the visible item range up by one item
            {
                minItem = Math.Max(minItem - 1, 0);
                maxItem = Math.Max(maxItem - 1, 0);
            }
            MenuItems.RemoveAt(index);
            RefreshCurrentSelection(); // refresh the current selection in case we removed the selected item
        }

        /// <summary>
        /// Reset the current selected item to 0. Use this after you add or remove items from <see cref="MenuItems"/> directly
        /// instead of through <see cref="AddItem(UIMenuItem)"/>, <see cref="AddItem(UIMenuItem, int)"/> or <see cref="RemoveItemAt(int)"/>.
        /// </summary>
        public void RefreshIndex()
        {
            if (MenuItems.Count == 0)
            {
                CurrentSelection = -1;
                return;
            }

            for (int i = 0; i < MenuItems.Count; i++)
                MenuItems[i].Selected = false;

            CurrentSelection = 0;
        }

        /// <summary>
        /// Remove all items from the menu.
        /// </summary>
        public void Clear()
        {
            MenuItems.Clear();
            CurrentSelection = -1;
        }

        /// <summary>
        /// Draw your custom banner.
        /// </summary>
        /// <param name="e">Rage.GraphicsEventArgs to draw on.</param>
        [Obsolete("UIMenu.DrawBanner(GraphicsEventArgs) will be removed soon, use UIMenu.DrawBanner(Graphics) instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public void DrawBanner(GraphicsEventArgs e)
        {
        }

        /// <summary>
        /// Draw your custom banner.
        /// <para>
        /// To prevent flickering call it inside a <see cref="Game.RawFrameRender"/> event handler.
        /// </para>
        /// </summary>
        /// <param name="g">The <see cref="Rage.Graphics"/> to draw on.</param>
        public void DrawBanner(Rage.Graphics g)
        {
            if (!Visible || _customBanner == null)
            {
                return;
            }

            Size res = Game.Resolution;
            SizeF primaryRes = ActualScreenResolution;
            PointF middle = new PointF(res.Width * 0.5f, res.Height * 0.5f);

            g.DrawTexture(_customBanner,
                          (customBannerX - 0.5f) * primaryRes.Width + middle.X,
                          (customBannerY - 0.5f) * primaryRes.Height + middle.Y,
                          customBannerW * primaryRes.Width,
                          customBannerH * primaryRes.Height);
        }

        // drawing variables
        private float menuWidth;
        private float itemHeight = 0.034722f;
        private float itemsX, itemsY;
        private float upDownX, upDownY;
        private float customBannerX, customBannerY,
                      customBannerW, customBannerH;

        private static SizeF ActualScreenResolution { get; set; }
        private static float AspectRatio { get; set; } // only updated when a UIMenu is visible
        private static bool IsWideScreen => AspectRatio > 1.5f; // equivalent to GET_IS_WIDESCREEN
        private static bool IsUltraWideScreen => AspectRatio > 3.5f; // > 32:9

        internal static void DrawSprite(string txd, string texName, float x, float y, float w, float h, Color c)
            => N.DrawSprite(txd, texName, x, y, w, h, 0.0f, c.R, c.G, c.B, c.A);

        internal static void DrawRect(float x, float y, float w, float h, Color c)
            => N.DrawRect(x, y, w, h, c.R, c.G, c.B, c.A);

        internal void BeginScriptGfx()
        {
            if (ScaleWithSafezone)
            {
                N.SetScriptGfxAlign('L', 'T');
                N.SetScriptGfxAlignParams(-0.05f, -0.05f, 0.0f, 0.0f);
            }
        }

        internal void EndScriptGfx()
        {
            if (ScaleWithSafezone)
            {
                N.ResetScriptGfxAlign();
            }
        }

        /// <summary>
        /// Draw the menu and all of it's components.
        /// </summary>
        public void Draw()
        {
            if (!Visible)
            {
                return;
            }

            if (_bannerSprite != null)
            {
                _bannerSprite.LoadTextureDictionary();
                if (!_bannerSprite.IsTextureDictionaryLoaded)
                {
                    return;
                }
            }

            N.RequestStreamedTextureDict(CommonTxd);
            if (!N.HasStreamedTextureDictLoaded(CommonTxd))
            {
                return;
            }

            if (ControlDisablingEnabled)
                DisEnableControls(false);

            if (AllowCameraMovement && ControlDisablingEnabled)
                EnableCameraMovement();

            if (InstructionalButtonsEnabled)
                InstructionalButtons.Draw();

            ActualScreenResolution = Internals.Screen.ActualResolution;
            AspectRatio = N.GetAspectRatio(false);

            menuWidth = 0.225f;
            if (AspectRatio < 1.77777f) // less than 16:9
            {
                menuWidth = 0.225f * (16f / 9f / AspectRatio);
            }

            menuWidth += WidthOffset / ActualScreenResolution.Width;

            BeginScriptGfx();

            float x = 0.05f + Offset.X / ActualScreenResolution.Width;
            float y = 0.05f + Offset.Y / ActualScreenResolution.Height;

            DrawBanner(x, ref y);

            DrawSubtitle(x, ref y);

            float headerBottom = y;

            DrawBackground(x, headerBottom);

            DrawItems(x, ref y);

            DrawUpDownArrows(x, ref y);

            DrawDescription(x, ref y);

            EndScriptGfx();
        }

        private void DrawBanner(float x, ref float y)
        {
            if (_bannerSprite == null && _bannerRectangle == null && _customBanner == null)
            {
                return;
            }

            GetBannerDrawSize(CommonTxd, DefaultBannerTextureName, out float bannerWidth, out float bannerHeight);
            if (_bannerSprite != null)
            {
                DrawSprite(_bannerSprite.TextureDictionary, _bannerSprite.TextureName, x + menuWidth * 0.5f, y + bannerHeight * 0.5f, bannerWidth, bannerHeight, Color.White);
            }
            else if (_bannerRectangle != null)
            {
                DrawRect(x + menuWidth * 0.5f, y + bannerHeight * 0.5f, bannerWidth, bannerHeight, _bannerRectangle.Color);
            }

            if (_bannerSprite != null || _bannerRectangle != null)
            {
                DrawTitle(x, y, bannerHeight);
            }
            else if (_customBanner != null)
            {
                N.GetScriptGfxPosition(x, y, out customBannerX, out customBannerY);
                N.GetScriptGfxPosition(x + menuWidth, y + bannerHeight, out customBannerW, out customBannerH);
                customBannerW -= customBannerX;
                customBannerH -= customBannerY;
            }

            y += bannerHeight;
        }

        private void DrawTitle(float x, float y, float bannerHeight)
        {
            float titleX = x + menuWidth * 0.5f;
            float titleY = y + bannerHeight * 0.225f;

            N.SetTextFont((int)Title.FontEnum);
            N.SetTextScale(0.0f, 1.025f);
            Color c = Title.Color;
            N.SetTextColour(c.R, c.G, c.B, c.A);
            N.SetTextWrap(x, x + menuWidth);
            N.SetTextCentre(true);

            TextCommands.Display(Title.Caption, titleX, titleY);
        }

        private void DrawSubtitle(float x, ref float y)
        {
            if (Subtitle.Caption != null)
            {
                float subtitleWidth = menuWidth;
                float subtitleHeight = itemHeight;

                DrawRect(x + menuWidth * 0.5f, y + subtitleHeight * 0.5f, subtitleWidth, subtitleHeight, Color.Black);

                DrawSubtitleText(x, y);

                DrawSubtitleCounter(x, y);

                y += subtitleHeight;
            }
        }

        private void DrawSubtitleText(float x, float y)
        {
            float subTextX = x + 0.00390625f;
            float subTextY = y + 0.00416664f;

            N.SetTextFont((int)Subtitle.FontEnum);
            N.SetTextScale(0f, 0.35f);
            Color c = Subtitle.Color;
            N.SetTextColour(c.R, c.G, c.B, c.A);
            N.SetTextWrap(x + 0.0046875f, x + menuWidth - 0.0046875f);
            N.SetTextCentre(false);
            N.SetTextDropshadow(0, 0, 0, 0, 0);
            N.SetTextEdge(0, 0, 0, 0, 0);

            TextCommands.Display(Subtitle.Caption, subTextX, subTextY);
        }

        private void DrawSubtitleCounter(float x, float y)
        {
            string counterText = null;
            if (CounterOverride != null)
            {
                counterText = CounterPretext + CounterOverride;
            }
            else if (MenuItems.Count > MaxItemsOnScreen)
            {
                counterText = CounterPretext + (CurrentSelection + 1) + " / " + MenuItems.Count;
            }

            if (counterText == null)
            {
                return;
            }

            void SetCounterTextOptions()
            {
                N.SetTextFont((int)Subtitle.FontEnum);
                N.SetTextScale(0f, 0.35f);
                Color c = Subtitle.Color;
                N.SetTextColour(c.R, c.G, c.B, c.A);
                N.SetTextWrap(x + 0.0046875f, x + menuWidth - 0.0046875f);
                N.SetTextCentre(false);
                N.SetTextDropshadow(0, 0, 0, 0, 0);
                N.SetTextEdge(0, 0, 0, 0, 0);
            }

            SetCounterTextOptions();
            float counterWidth = TextCommands.GetWidth(counterText);

            float counterX = x + menuWidth - 0.00390625f - counterWidth;
            float counterY = y + 0.00416664f;

            SetCounterTextOptions();
            TextCommands.Display(counterText, counterX, counterY);
        }

        private void DrawBackground(float x, float headerBottom)
        {
            float bgWidth = menuWidth;
            float bgHeight = itemHeight * Math.Min(MenuItems.Count, MaxItemsOnScreen);

            DrawSprite(CommonTxd, BackgroundTextureName,
                       x + bgWidth * 0.5f,
                       headerBottom + bgHeight * 0.5f - 0.00138888f,
                       bgWidth,
                       bgHeight,
                       Color.White);
        }

        private void DrawItems(float x, ref float y)
        {
            itemsX = x;
            itemsY = y;

            for (int index = minItem; index <= maxItem; index++)
            {
                var item = MenuItems[index];
                item.Draw(x, y, menuWidth, itemHeight);
                y += itemHeight;
            }
        }

        private void DrawUpDownArrows(float x, ref float y)
        {
            if (MenuItems.Count > MaxItemsOnScreen)
            {
                float upDownRectWidth = menuWidth;
                float upDownRectHeight = itemHeight;

                upDownX = x;
                upDownY = y;

                Color backColor = Color.FromArgb(204, 0, 0, 0);
                DrawRect(x + upDownRectWidth * 0.5f, y + upDownRectHeight * 0.5f, upDownRectWidth, upDownRectHeight, backColor);

                if (hoveredUpDown != 0)
                {
                    Color hoveredColor = Color.FromArgb(25, 255, 255, 255);
                    float hoverRectH = upDownRectHeight * 0.5f;
                    DrawRect(x + upDownRectWidth * 0.5f, y + (hoverRectH * (hoveredUpDown - 0.5f)), upDownRectWidth, hoverRectH, hoveredColor);
                }

                float fVar61 = 1.0f; // TODO: this may need to be calculated based on current resolution
                Vector3 upDownSize = N.GetTextureResolution(CommonTxd, UpAndDownTextureName);
                float upDownWidth = upDownSize.X * (0.5f / fVar61);
                float upDownHeight = upDownSize.Y * (0.5f / fVar61);
                upDownWidth = upDownWidth / 1280.0f * fVar61;
                upDownHeight = upDownHeight / 720f * fVar61;
                Color foreColor = Color.White;
                DrawSprite(CommonTxd, UpAndDownTextureName, x + upDownRectWidth * 0.5f, y + upDownRectHeight * 0.5f, upDownWidth, upDownHeight, foreColor);

                y += itemHeight;
            }
        }

        private void DrawDescription(float x, ref float y)
        {
            if (MenuItems.Count == 0)
            {
                return;
            }

            string description = DescriptionOverride ?? MenuItems[CurrentSelection].Description;
            if (!String.IsNullOrWhiteSpace(description))
            {
                y += 0.00277776f * 2f;

                float textX = x + 0.0046875f;  // x - menuWidth * 0.5f + 0.0046875f
                float textXEnd = x + menuWidth - 0.0046875f; // x - menuWidth * 0.5f + menuWidth - 0.0046875f

                Color backColor = Color.FromArgb(186, 0, 0, 0);
                Color foreColor = Color.FromArgb(240, 240, 240);

                void SetDescriptionTextOptions()
                {
                    N.SetTextFont(0);
                    N.SetTextScale(0f, 0.35f);
                    N.SetTextLeading(2);
                    N.SetTextColour(foreColor.R, foreColor.G, foreColor.B, foreColor.A);
                    N.SetTextWrap(textX, textXEnd);
                    N.SetTextCentre(false);
                    N.SetTextDropshadow(0, 0, 0, 0, 0);
                    N.SetTextEdge(0, 0, 0, 0, 0);
                }

                SetDescriptionTextOptions();
                int lineCount = TextCommands.GetLineCount(description, textX, y + 0.00277776f);

                Color separatorBarColor = Color.Black;
                DrawRect(x + menuWidth * 0.5f, y - 0.00277776f * 0.5f, menuWidth, 0.00277776f, separatorBarColor);

                float descHeight = (N.GetTextScaleHeight(0.35f, 0) * lineCount) + (0.00138888f * 13f) + (0.00138888f * 5f * (lineCount - 1));
                DrawSprite(CommonTxd, BackgroundTextureName,
                           x + menuWidth * 0.5f,
                           y + (descHeight * 0.5f) - 0.00138888f,
                           menuWidth,
                           descHeight,
                           backColor);

                SetDescriptionTextOptions();
                TextCommands.Display(description, textX, y + 0.00277776f);

                y += descHeight;
            }
        }

        // Converted from game scripts
        internal static void GetTextureDrawSize(string txd, string texName, out float width, out float height, bool isBanner = false)
        {
            float sizeMultiplier = isBanner ? 1.0f : 0.5f;

            N.GetScreenResolution(out int screenWidth, out int screenHeight);
            Vector3 texSize = N.GetTextureResolution(txd, texName);
            texSize.X = texSize.X * sizeMultiplier;
            texSize.Y = texSize.Y * sizeMultiplier;

            width = texSize.X / screenWidth * (screenWidth / screenHeight);
            height = texSize.Y / screenHeight / (texSize.X / screenWidth) * width;

            if (!IsWideScreen)
            {
                width = width * 1.33f;
            }
        }

        private void GetBannerDrawSize(string txd, string texName, out float width, out float height)
        {
            GetTextureDrawSize(txd, texName, out width, out height, true);

            // TODO: maybe add option to scale banner height with WidthOffset
            float menuWidthNoOffset = menuWidth - (WidthOffset / ActualScreenResolution.Width);
            if (width > menuWidthNoOffset)
            {
                height = height * (menuWidthNoOffset / width);
                width = menuWidth;
            }
        }

        /// <summary>
        /// Returns the 1080pixels-based screen resolution while mantaining current aspect ratio.
        /// </summary>
        /// <returns></returns>
        public static SizeF GetScreenResolutionMantainRatio()
        {
            int screenw = Game.Resolution.Width;
            int screenh = Game.Resolution.Height;
            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            var width = height * ratio;

            return new SizeF(width, height);
        }

        /// <summary>
        /// Chech whether the mouse is inside the specified rectangle.
        /// </summary>
        /// <param name="topLeft">top left point of your rectangle.</param>
        /// <param name="boxSize">size of your rectangle.</param>
        /// <returns></returns>
        [Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
        public static bool IsMouseInBounds(Point topLeft, Size boxSize)
        {
            var res = GetScreenResolutionMantainRatio();

            int mouseX = Convert.ToInt32(Math.Round(N.GetControlNormal(0, GameControl.CursorX) * res.Width));
            int mouseY = Convert.ToInt32(Math.Round(N.GetControlNormal(0, GameControl.CursorY) * res.Height));

            return (mouseX >= topLeft.X && mouseX <= topLeft.X + boxSize.Width)
                   && (mouseY > topLeft.Y && mouseY < topLeft.Y + boxSize.Height);
        }

        /// <summary>
        /// Function to get whether the cursor is in an arrow space, or in label of an MenuListItem.
        /// </summary>
        /// <param name="item">What item to check</param>
        /// <param name="topLeft">top left point of the item.</param>
        /// <param name="safezone">safezone size.</param>
        /// <returns>0 - Not in item at all, 1 - In label, 2 - In arrow space.</returns>
        [Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
        public int IsMouseInListItemArrows(UIMenuListItem item, Point topLeft, Point safezone)
        {
            NativeFunction.CallByHash<uint>(0x54ce8ac98e120cab, "jamyfafi");
            ResText.AddLongString(item.Text);
            var res = GetScreenResolutionMantainRatio();
            var screenw = res.Width;
            var screenh = res.Height;
            const float height = 1080f;
            float ratio = screenw / screenh;
            var width = height * ratio;
            int labelSize = Convert.ToInt32(NativeFunction.CallByHash<float>(0x85f061da64ed2f67, 0) * width * 0.35f);

            int labelSizeX = 5 + labelSize + 10;
            int arrowSizeX = 431 - labelSizeX;
            return IsMouseInBounds(topLeft, new Size(labelSizeX, 38))
                ? 1
                : IsMouseInBounds(new Point(topLeft.X + labelSizeX, topLeft.Y), new Size(arrowSizeX, 38)) ? 2 : 0;

        }

        /// <summary>
        /// Returns the safezone bounds in pixel, relative to the 1080pixel based system.
        /// </summary>
        /// <returns></returns>
        public static Point GetSafezoneBounds()
        {
            float t = NativeFunction.CallByHash<float>(0xbaf107b6bb2c97f0); // Safezone size.
            double g = Math.Round(Convert.ToDouble(t), 2);
            g = (g * 100) - 90;
            g = 10 - g;

            const float hmp = 5.4f;
            int screenw = Game.Resolution.Width;
            int screenh = Game.Resolution.Height;
            float ratio = (float)screenw / screenh;
            float wmp = ratio * hmp;

            return new Point(Convert.ToInt32(Math.Round(g * wmp)), Convert.ToInt32(Math.Round(g * hmp)));
        }

        /// <summary>
        /// Go up the menu if the number of items is more than maximum items on screen.
        /// </summary>
        [Obsolete("Use UIMenu.GoUp() instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public void GoUpOverflow()
        {
            GoUp();
        }

        /// <summary>
        /// Go up the menu if the number of items is less than or equal to the maximum items on screen.
        /// </summary>
        public void GoUp()
        {
            if (!MenuItems[CurrentSelection].OnInput(this, Common.MenuControls.Up))
            {
                CurrentSelection--;

                Common.PlaySound(AUDIO_UPDOWN, AUDIO_LIBRARY);
                IndexChange(CurrentSelection);
            }
        }

        /// <summary>
        /// Go down the menu if the number of items is more than maximum items on screen.
        /// </summary>
        [Obsolete("Use UIMenu.GoDown() instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public void GoDownOverflow()
        {
            GoDown();
        }

        /// <summary>
        /// Go up the menu if the number of items is less than or equal to the maximum items on screen.
        /// </summary>
        public void GoDown()
        {
            if (!MenuItems[CurrentSelection].OnInput(this, Common.MenuControls.Down))
            {
                CurrentSelection++;

                Common.PlaySound(AUDIO_UPDOWN, AUDIO_LIBRARY);
                IndexChange(CurrentSelection);
            }
        }

        /// <summary>
        /// Go left on a MenuListItem.
        /// </summary>
        public void GoLeft() => MenuItems[CurrentSelection].OnInput(this, Common.MenuControls.Left);

        /// <summary>
        /// Go right on a MenuListItem.
        /// </summary>
        public void GoRight() => MenuItems[CurrentSelection].OnInput(this, Common.MenuControls.Right);

        /// <summary>
        /// Activate the current selected item.
        /// </summary>
        public void SelectItem() => MenuItems[CurrentSelection].OnInput(this, Common.MenuControls.Select);

        /// <summary>
        /// Close or go back in a menu chain.
        /// </summary>
        public void GoBack() => MenuItems[CurrentSelection].OnInput(this, Common.MenuControls.Back);

        // opens the menu binded to the specified item
        public void OpenChildMenu(UIMenuItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (Children.TryGetValue(item, out UIMenu childMenu))
            {
                Visible = false;
                childMenu.Visible = true;
                MenuChangeEv(childMenu, true);
            }
        }

        public void Close(bool openParentMenu = true)
        {
            Visible = false;
            if (openParentMenu && ParentMenu != null)
            {
                var tmp = Cursor.Position;
                ParentMenu.Visible = true;
                MenuChangeEv(ParentMenu, false);
                if (ResetCursorOnOpen)
                    Cursor.Position = tmp;
            }
            MenuCloseEv();
        }

        /// <summary>
        /// Bind a menu to a button. When the button is clicked that menu will open.
        /// </summary>
        public void BindMenuToItem(UIMenu menuToBind, UIMenuItem itemToBindTo)
        {
            menuToBind.ParentMenu = this;
            menuToBind.ParentItem = itemToBindTo;
            if (Children.ContainsKey(itemToBindTo))
                Children[itemToBindTo] = menuToBind;
            else
                Children.Add(itemToBindTo, menuToBind);
        }

        /// <summary>
        /// Remove menu binding from button.
        /// </summary>
        /// <param name="releaseFrom">Button to release from.</param>
        /// <returns>Returns true if the operation was successful.</returns>
        public bool ReleaseMenuFromItem(UIMenuItem releaseFrom)
        {
            if (!Children.ContainsKey(releaseFrom)) return false;
            Children[releaseFrom].ParentItem = null;
            Children[releaseFrom].ParentMenu = null;
            Children.Remove(releaseFrom);
            return true;
        }

        /// <summary>
        /// Process the mouse's position and check if it's hovering over any Element. Call this in Game.FrameRender or in a loop.
        /// </summary>
        public void ProcessMouse()
        {
            if (!Visible || justOpened || MenuItems.Count == 0 || IsUsingController || !MouseControlsEnabled)
            {
                N.EnableControlAction(0, GameControl.LookUpDown);
                N.EnableControlAction(0, GameControl.LookLeftRight);
                N.EnableControlAction(0, GameControl.Aim);
                N.EnableControlAction(0, GameControl.Attack);
                if (hoveredItem != -1)
                {
                    MenuItems[hoveredItem].Hovered = false;
                    hoveredItem = -1;
                }
                hoveredUpDown = 0;
                return;
            }

            N.SetMouseCursorActiveThisFrame();
            N.SetMouseCursorSprite(1);

            N.x5B73C77D9EB66E24(true);

            float mouseX = N.GetControlNormal(2, GameControl.CursorX);
            float mouseY = N.GetControlNormal(2, GameControl.CursorY);

            UpdateHoveredItem(mouseX, mouseY);

            if (hoveredItem != -1 && Game.IsControlJustReleased(2, GameControl.CursorAccept))
            {
                UIMenuItem i = MenuItems[hoveredItem];

                if (i.Selected)
                {
                    if (i.ScrollerProxy == null) // list item select is handled below, along with arrow controls
                    {
                        SelectItem();
                    }
                }
                else
                {
                    CurrentSelection = hoveredItem;
                    Common.PlaySound(AUDIO_UPDOWN, AUDIO_LIBRARY);
                    IndexChange(CurrentSelection);
                    InstructionalButtons.Update();
                }
            }

            UpdateHoveredUpDown(mouseX, mouseY);

            if (hoveredUpDown != 0 && IsCursorAcceptBeingPressed())
            {
                if (hoveredUpDown == 1)
                {
                    GoUp();
                }
                else
                {
                    GoDown();
                }
                InstructionalButtons.Update();
            }

            // mouse controls for lists
            if (hoveredItem != -1)
            {
                UIMenuItem hovered = MenuItems[hoveredItem];
                if (hovered.ScrollerProxy != null && hovered.Selected && hovered.Enabled)
                {
                    BeginScriptGfx();
                    float selectBoundsX = itemsX + (menuWidth * 0.33333f);
                    N.GetScriptGfxPosition(selectBoundsX, 0.0f, out selectBoundsX, out _);
                    EndScriptGfx();

                    if (mouseX <= selectBoundsX)
                    {
                        // approximately hovering the label, first 1/3 of the item width
                        // TODO: game shows cursor sprite 5 when hovering this part, but only if the item does something when selected.
                        //       Here, we don't really know if the user does something when selected, maybe add some bool property in UIMenuListItem?
                        if (Game.IsControlJustReleased(2, GameControl.CursorAccept))
                        {
                            SelectItem();
                        }
                    }
                    else if (hovered.ScrollerProxy.GetScrollingEnabled() && IsCursorAcceptBeingPressed(hovered.ScrollerProxy))
                    {
                        GetTextureDrawSize(CommonTxd, ArrowRightTextureName, out float rightW, out _);
                        float rightX = itemsX + menuWidth - (0.00390625f * 1.0f) - (rightW * 0.5f) - (0.0046875f * 0.75f);

                        BeginScriptGfx();

                        N.GetScriptGfxPosition(rightX, 0.0f, out rightX, out _);

                        EndScriptGfx();

                        // It does not check if the mouse in exactly on top of the arrow sprites intentionally:
                        //  - If to the right of the right arrow's left border, go right
                        //  - Anywhere else in the item, go left.
                        // This is how the vanilla menus behave
                        if (mouseX >= rightX)
                        {
                            GoRight();
                        }
                        else
                        {
                            GoLeft();
                        }
                    }
                }
            }

            if (MouseEdgeEnabled && hoveredItem == -1)
            {
                const int CursorLeftArrow = 6, CursorRightArrow = 7;

                if (mouseX > (1f - (0.05f * 0.75f))) // right edge
                {
                    N.SetMouseCursorSprite(CursorRightArrow);
                    float mult = 0.05f - (1f - mouseX);
                    if (mult > 0.05f)
                    {
                        mult = 0.05f;
                    }

                    N.SetGameplayCamRelativeHeading(N.GetGameplayCamRelativeHeading() - (70f * mult));
                }
                else if (mouseX < (0.05f * 0.75f)) // left edge
                {
                    N.SetMouseCursorSprite(CursorLeftArrow);
                    float mult = 0.05f - mouseX;
                    if (mult > 0.05f)
                    {
                        mult = 0.05f;
                    }

                    N.SetGameplayCamRelativeHeading(N.GetGameplayCamRelativeHeading() + (70f * mult));
                }
            }
        }

        private void UpdateHoveredItem(float mouseX, float mouseY)
        {
            BeginScriptGfx();

            int visibleItemCount = Math.Min(MenuItems.Count, MaxItemsOnScreen);
            float x1 = itemsX, y1 = itemsY - 0.00138888f; // background top-left
            float x2 = itemsX + menuWidth, y2 = y1 + visibleItemCount * itemHeight; // background bottom-right
            N.GetScriptGfxPosition(x1, y1, out x1, out y1);
            N.GetScriptGfxPosition(x2, y2, out x2, out y2);

            EndScriptGfx();

            if (mouseX >= x1 && mouseX <= x2 && mouseY >= y1 && mouseY <= y2) // hovering items background
            {
                int hoveredIdx = minItem + (int)((mouseY - y1) / itemHeight);

                if (hoveredItem != hoveredIdx)
                {
                    if (hoveredItem != -1) // unhover previous item
                    {
                        MenuItems[hoveredItem].Hovered = false;
                    }

                    // hover new item
                    MenuItems[hoveredIdx].Hovered = true;
                    hoveredItem = hoveredIdx;
                }
            }
            else if (hoveredItem != -1)
            {
                MenuItems[hoveredItem].Hovered = false;
                hoveredItem = -1;
            }
        }

        private void UpdateHoveredUpDown(float mouseX, float mouseY)
        {
            if (MenuItems.Count > MaxItemsOnScreen)
            {
                BeginScriptGfx();

                float x1 = upDownX, y1 = upDownY; // up&down rect top-left
                float x2 = x1 + menuWidth, y2 = y1 + itemHeight; // up&down rect bottom-right
                N.GetScriptGfxPosition(x1, y1, out x1, out y1);
                N.GetScriptGfxPosition(x2, y2, out x2, out y2);

                EndScriptGfx();

                if (mouseX >= x1 && mouseX <= x2 && mouseY >= y1 && mouseY <= y2) // hovering up&down rect
                {
                    float h = y2 - y1;
                    bool up = mouseY <= (y1 + h * 0.5f);
                    hoveredUpDown = up ? 1 : 2;
                    return;
                }
            }

            hoveredUpDown = 0;
        }

        /// <summary>
        /// Set a key to control a menu. Can be multiple keys for each control.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="keyToSet"></param>
        public void SetKey(Common.MenuControls control, Keys keyToSet)
        {
            if (_keyDictionary.ContainsKey(control))
                _keyDictionary[control].Item1.Add(keyToSet);
            else
            {
                _keyDictionary.Add(control,
                    new Tuple<List<Keys>, List<Tuple<GameControl, int>>>(new List<Keys>(), new List<Tuple<GameControl, int>>()));
                _keyDictionary[control].Item1.Add(keyToSet);
            }
        }

        /// <summary>
        /// Set a Rage.GameControl to control a menu. Can be multiple controls. This applies it to all indexes.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="gtaControl"></param>
        public void SetKey(Common.MenuControls control, GameControl gtaControl)
        {
            SetKey(control, gtaControl, 0);
            SetKey(control, gtaControl, 1);
            SetKey(control, gtaControl, 2);
        }

        /// <summary>
        /// Set a Rage.GameControl to control a menu only on a specific index.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="gtaControl"></param>
        /// <param name="controlIndex"></param>
        public void SetKey(Common.MenuControls control, GameControl gtaControl, int controlIndex)
        {
            if (_keyDictionary.ContainsKey(control))
                _keyDictionary[control].Item2.Add(new Tuple<GameControl, int>(gtaControl, controlIndex));
            else
            {
                _keyDictionary.Add(control,
                    new Tuple<List<Keys>, List<Tuple<GameControl, int>>>(new List<Keys>(), new List<Tuple<GameControl, int>>()));
                _keyDictionary[control].Item2.Add(new Tuple<GameControl, int>(gtaControl, controlIndex));
            }

        }

        /// <summary>
        /// Remove all controls on a control.
        /// </summary>
        /// <param name="control"></param>
        public void ResetKey(Common.MenuControls control)
        {
            _keyDictionary[control].Item1.Clear();
            _keyDictionary[control].Item2.Clear();
        }

        /// <summary>
        /// Check whether a menucontrol has been pressed.
        /// </summary>
        /// <param name="control">Control to check for.</param>
        /// <param name="key">Key if you're using keys.</param>
        /// <returns></returns>
        public bool HasControlJustBeenPressed(Common.MenuControls control, Keys key = Keys.None)
        {
            List<Keys> tmpKeys = new List<Keys>(_keyDictionary[control].Item1);
            List<Tuple<GameControl, int>> tmpControls = new List<Tuple<GameControl, int>>(_keyDictionary[control].Item2);

            if (key != Keys.None)
            {
                if (tmpKeys.Any(Game.IsKeyDown))
                {
                    return true;
                }
            }
            if (tmpControls.Any(tuple => Game.IsControlJustPressed(tuple.Item2, tuple.Item1) || Common.IsDisabledControlJustPressed(tuple.Item2, tuple.Item1)))
                return true;
            return false;
        }

        /// <summary>
        /// Check whether a menucontrol has been released.
        /// </summary>
        /// <param name="control">Control to check for.</param>
        /// <param name="key">Key if you're using keys.</param>
        /// <returns></returns>
        public bool HasControlJustBeenReleaseed(Common.MenuControls control, Keys key = Keys.None)
        {
            List<Keys> tmpKeys = new List<Keys>(_keyDictionary[control].Item1);
            List<Tuple<GameControl, int>> tmpControls = new List<Tuple<GameControl, int>>(_keyDictionary[control].Item2);

            if (key != Keys.None)
            {
                if (tmpKeys.Any(Game.IsKeyDown))
                {
                    return true;
                }
            }
            if (tmpControls.Any(tuple => Game.IsControlJustReleased(tuple.Item2, tuple.Item1) || Common.IsDisabledControlJustReleased(tuple.Item2, tuple.Item1)))
            {
                return true;
            }

            return false;
        }

        public uint HoldTimeBeforeScroll = 200;

        private uint _holdTime;
        /// <summary>
        /// Checks whether a menucontrol is being pressed and if selected item is UIListItem, uses UIListItem variables
        /// </summary>
        /// <param name="control"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsControlBeingPressed(Common.MenuControls control, Keys key = Keys.None)
        {
            if (control != Common.MenuControls.Left && control != Common.MenuControls.Right)
            {
                if (HasControlJustBeenReleaseed(control, key)) _holdTime = 0;
                if (Game.GameTime <= _holdTime)
                {
                    return false;
                }
                List<Keys> tmpKeys = new List<Keys>(_keyDictionary[control].Item1);
                List<Tuple<GameControl, int>> tmpControls = new List<Tuple<GameControl, int>>(_keyDictionary[control].Item2);
                if (key != Keys.None)
                {
                    if (tmpKeys.Any(Game.IsKeyDownRightNow))
                    {
                        _holdTime = Game.GameTime + HoldTimeBeforeScroll;
                        return true;
                    }
                }
                if (tmpControls.Any(tuple => Game.IsControlPressed(tuple.Item2, tuple.Item1) || Common.IsDisabledControlPressed(tuple.Item2, tuple.Item1)))
                {
                    _holdTime = Game.GameTime + HoldTimeBeforeScroll;
                    return true;
                }
                return false;
            }
            else if (MenuItems[CurrentSelection].ScrollerProxy != null && (MenuItems[CurrentSelection].Enabled || MenuItems[CurrentSelection].ScrollerProxy.GetScrollingEnabledWhenDisabled()))
            {
                UIMenuItem it = MenuItems[CurrentSelection];
                if (it.ScrollerProxy.GetScrollingEnabled())
                {
                    ref uint itHoldTime = ref it.ScrollerProxy.GetHoldTime();
                    if (HasControlJustBeenReleaseed(control, key)) { itHoldTime = 0; }
                    if (Game.GameTime <= itHoldTime)
                    {
                        return false;
                    }
                    List<Keys> tmpKeys = new List<Keys>(_keyDictionary[control].Item1);
                    List<Tuple<GameControl, int>> tmpControls = new List<Tuple<GameControl, int>>(_keyDictionary[control].Item2);
                    if (key != Keys.None)
                    {
                        if (tmpKeys.Any(Game.IsKeyDownRightNow))
                        {
                            itHoldTime = Game.GameTime + it.ScrollerProxy.GetHoldTimeBeforeScroll();
                            return true;
                        }
                    }
                    if (tmpControls.Any(tuple => Game.IsControlPressed(tuple.Item2, tuple.Item1) || Common.IsDisabledControlPressed(tuple.Item2, tuple.Item1)))
                    {
                        itHoldTime = Game.GameTime + it.ScrollerProxy.GetHoldTimeBeforeScroll();
                        return true;
                    }
                }

            }
            return false;
        }

        private bool IsCursorAcceptBeingPressed() => IsCursorAcceptBeingPressed(ref _holdTime, HoldTimeBeforeScroll);
        private bool IsCursorAcceptBeingPressed(UIMenuScrollerProxy s) => IsCursorAcceptBeingPressed(ref s.GetHoldTime(), s.GetHoldTimeBeforeScroll());

        private bool IsCursorAcceptBeingPressed(ref uint holdTime, uint holdTimeBeforeScroll)
        {
            if (Game.IsControlJustReleased(2, GameControl.CursorAccept))
            {
                holdTime = 0;
            }

            if (Game.GameTime <= holdTime)
            {
                return false;
            }

            if (Game.IsControlPressed(2, GameControl.CursorAccept))
            {
                holdTime = Game.GameTime + holdTimeBeforeScroll;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Process control-stroke. Call this in the Game.FrameRender event or in a loop.
        /// </summary>
        public void ProcessControl(Keys key = Keys.None)
        {
            if (!Visible) return;
            if (justOpened)
            {
                justOpened = false;
                return;
            }

            if (MenuItems.Count == 0)
            {
                return;
            }

            if (HasControlJustBeenReleaseed(Common.MenuControls.Back, key))
            {
                GoBack();
            }

            if (IsControlBeingPressed(Common.MenuControls.Up, key))
            {
                GoUp();
                InstructionalButtons.Update();
            }
            else if (IsControlBeingPressed(Common.MenuControls.Down, key))
            {
                GoDown();
                InstructionalButtons.Update();
            }
            else if (IsControlBeingPressed(Common.MenuControls.Left, key))
            {
                GoLeft();
            }
            else if (IsControlBeingPressed(Common.MenuControls.Right, key))
            {
                GoRight();
            }
            else if (HasControlJustBeenPressed(Common.MenuControls.Select, key))
            {
                SelectItem();
            }

        }

        /// <summary>
        /// Process keystroke. Call this in the Game.FrameRender event or in a loop.
        /// </summary>
        public void ProcessKey(Keys key)
        {
            if ((from object menuControl in Enum.GetValues(typeof(Common.MenuControls)) select new List<Keys>(_keyDictionary[(Common.MenuControls)menuControl].Item1)).Any(tmpKeys => tmpKeys.Any(k => k == key)))
            {
                ProcessControl(key);
            }
        }

        public void AddInstructionalButton(InstructionalButton button)
        {
            InstructionalButtons.Buttons.Add(button);
        }

        public void RemoveInstructionalButton(InstructionalButton button)
        {
            InstructionalButtons.Buttons.Remove(button);
        }

        /// <summary>
        /// Sets the index of all lists to 0 and unchecks all the checkboxes. 
        /// </summary>
        /// <param name="resetLists">If true the index of all lists will be set to 0.</param>
        /// <param name="resetCheckboxes">If true all the checkboxes will be unchecked.</param>
        public void Reset(bool resetLists, bool resetCheckboxes)
        {
            foreach (UIMenuItem item in MenuItems)
            {
                if (resetLists)
                {
                    if (item.ScrollerProxy != null)
                    {
                        item.ScrollerProxy.SetIndex(0);
                        continue;
                    }
                }

                if (resetCheckboxes)
                {
                    UIMenuCheckboxItem itemAsCheckbox = item as UIMenuCheckboxItem;
                    if (itemAsCheckbox != null)
                    {
                        itemAsCheckbox.Checked = false;
                        continue;
                    }
                }
            }
            CurrentSelection = 0;
        }


        private void EnableCameraMovement()
        {
            N.EnableControlAction(0, GameControl.LookLeftRight);
            N.EnableControlAction(0, GameControl.LookUpDown);
        }

        private void UpdateVisibleItemsIndices()
        {
            if (MaxItemsOnScreen >= MenuItems.Count)
            {
                minItem = 0;
                maxItem = MenuItems.Count - 1;
                return;
            }

            if (minItem == -1 || maxItem == -1) // if no previous selection
            {
                minItem = 0;
                maxItem = Math.Min(MenuItems.Count, MaxItemsOnScreen) - 1;
            }
            else if (currentItem < minItem) // moved selection up, out of current visible item
            {
                minItem = currentItem;
                maxItem = currentItem + Math.Min(MaxItemsOnScreen, MenuItems.Count) - 1;
            }
            else if (currentItem > maxItem) // moved selection down, out of current visible item
            {
                minItem = currentItem - Math.Min(MaxItemsOnScreen, MenuItems.Count) + 1;
                maxItem = currentItem;
            }
            else if (maxItem - minItem + 1 != MaxItemsOnScreen) // MaxItemsOnScreen changed
            {
                if (maxItem == currentItem)
                {
                    minItem = maxItem - Math.Min(maxItemsOnScreen, MenuItems.Count) + 1;
                }
                else
                {
                    maxItem = minItem + Math.Min(maxItemsOnScreen, MenuItems.Count) - 1;
                    if (maxItem >= MenuItems.Count)
                    {
                        int diff = maxItem - MenuItems.Count + 1;
                        maxItem -= diff;
                        minItem -= diff;
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes the current item index and min/max visible items, in case they are out of bounds.
        /// </summary>
        private void RefreshCurrentSelection()
        {
            CurrentSelection = CurrentSelection;
        }

        /// <summary>
        /// Change whether this menu is visible to the user.
        /// </summary>
        public bool Visible
        {
            get => visible;
            set
            {
                if (visible != value)
                {
                    visible = value;
                    justOpened = value;
                    if (visible)
                    {
                        MenuOpenEv();
                    }

                    InstructionalButtons.Update();
                    if (ParentMenu != null || !value)
                    {
                        return;
                    }

                    if (!ResetCursorOnOpen)
                    {
                        return;
                    }

                    Cursor.Position = new Point(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2);
                    N.SetMouseCursorSprite(1);
                }
            }
        }

        /// <summary>
        /// Gets or sets the current selected item's index. When setting it, the specified value will be wrap around between 0 and the number of items.
        /// Returns -1 if no selection exists, for example, when no items have been added to the menu.
        /// </summary>
        public int CurrentSelection
        {
            get => currentItem;
            set
            {
                if (MenuItems.Count == 0)
                {
                    currentItem = -1;
                    minItem = -1;
                    maxItem = -1;
                }
                else
                {
                    int newIndex = Common.Wrap(value, 0, MenuItems.Count);

                    if (currentItem != newIndex || !MenuItems[newIndex].Selected)
                    {
                        if (currentItem >= 0 && currentItem < MenuItems.Count)
                        {
                            MenuItems[currentItem].Selected = false;
                        }
                        currentItem = newIndex;
                        MenuItems[currentItem].Selected = true;
                    }

                    UpdateVisibleItemsIndices();
                }
            }
        }

        /// <summary>
        /// Gets or set the maximum number of visible items.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">If the specified value is less than 1.</exception>
        public int MaxItemsOnScreen
        {
            get => maxItemsOnScreen;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(MaxItemsOnScreen)} must be at least 1");
                }

                if (maxItemsOnScreen != value)
                {
                    maxItemsOnScreen = value;
                    UpdateVisibleItemsIndices();
                }
            }
        }

        /// <summary>
        /// Returns false if last input was made with mouse and keyboard, true if it was made with a controller.
        /// </summary>
        public static bool IsUsingController => !N.IsInputDisabled(2);

        /// <summary>
        /// Returns the title object.
        /// </summary>
        public ResText Title { get; private set; }

        /// <summary>
        /// Returns the subtitle object.
        /// </summary>
        public ResText Subtitle { get; private set; }

        /// <summary>
        /// String to pre-attach to the counter string. Useful for color codes.
        /// </summary>
        public string CounterPretext { get; set; }

        /// <summary>
        /// Gets or sets the text that overrides the counter string.
        /// <para>
        /// If <c>null</c> the counter string isn't overrided.
        /// </para>
        /// </summary>
        /// <value>
        /// The text that overrides the counter string.
        /// </value>
        public string CounterOverride { get; set; }

        /// <summary>
        /// Gets or sets the description override.
        /// If not <c>null</c>, this <see cref="string"/> is shown instead of
        /// the <see cref="UIMenuItem.Description"/> of the currently selected item.
        /// </summary>
        public string DescriptionOverride { get; set; }

        /// <summary>
        /// If this is a nested menu, returns the parent menu. You can also set it to a menu so when pressing Back it goes to that menu.
        /// </summary>
        public UIMenu ParentMenu { get; set; }

        /// <summary>
        /// If this is a nested menu, returns the item it was binded to.
        /// </summary>
        public UIMenuItem ParentItem { get; set; }

        protected virtual void IndexChange(int newindex)
        {
            OnIndexChange?.Invoke(this, newindex);
        }

        protected internal virtual void ListChange(UIMenuListItem sender, int newindex)
        {
            OnListChange?.Invoke(this, sender, newindex);
        }

        protected internal virtual void ScrollerChange(UIMenuScrollerItem sender, int oldIndex, int newindex)
        {
            OnScrollerChange?.Invoke(this, sender, oldIndex, newindex);
        }

        protected internal virtual void ItemSelect(UIMenuItem selecteditem, int index)
        {
            OnItemSelect?.Invoke(this, selecteditem, index);
        }

        protected internal virtual void CheckboxChange(UIMenuCheckboxItem sender, bool Checked)
        {
            OnCheckboxChange?.Invoke(this, sender, Checked);
        }

        protected virtual void MenuOpenEv()
        {
            OnMenuOpen?.Invoke(this);
        }

        protected virtual void MenuCloseEv()
        {
            OnMenuClose?.Invoke(this);
        }

        protected virtual void MenuChangeEv(UIMenu newmenu, bool forward)
        {
            OnMenuChange?.Invoke(this, newmenu, forward);
        }
    }
}
