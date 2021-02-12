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
using RAGENativeUI.Internals;

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
        /// <summary>
        /// Represents the default value of <see cref="MaxItemsOnScreen"/>.
        /// </summary>
        public const int DefaultMaxItemsOnScreen = 10;

        /// <summary>
        /// Represents the default value of <see cref="Width"/>.
        /// </summary>
        public const float DefaultWidth = 0.225f;

        /// <summary>
        /// Represents the default value of <see cref="TitleStyle"/>.
        /// </summary>
        public static readonly TextStyle DefaultTitleStyle = TextStyle.Default.With(
                                                                    color: Color.White,
                                                                    font: TextFont.HouseScript,
                                                                    justification: TextJustification.Center,
                                                                    scale: 1.025f);
        /// <summary>
        /// Represents the default value of <see cref="SubtitleStyle"/>.
        /// </summary>
        public static readonly TextStyle DefaultSubtitleStyle = TextStyle.Default.With(
                                                                    color: Color.White,
                                                                    font: TextFont.ChaletLondon,
                                                                    justification: TextJustification.Left,
                                                                    scale: 0.35f);
        /// <summary>
        /// Represents the default value of <see cref="CounterStyle"/>.
        /// </summary>
        public static readonly TextStyle DefaultCounterStyle = TextStyle.Default.With(
                                                                    color: Color.White,
                                                                    font: TextFont.ChaletLondon,
                                                                    justification: TextJustification.Left,
                                                                    scale: 0.35f);
        /// <summary>
        /// Represents the default value of <see cref="DescriptionStyle"/>.
        /// </summary>
        public static readonly TextStyle DefaultDescriptionStyle = TextStyle.Default.With(
                                                                    color: Color.WhiteSmoke,
                                                                    font: TextFont.ChaletLondon,
                                                                    justification: TextJustification.Left,
                                                                    scale: 0.35f);

        /// <summary>
        /// Represets the default value of <see cref="DescriptionSeparatorColor"/>.
        /// </summary>
        public static readonly Color DefaultDescriptionSeparatorColor = Color.Black;

        /// <summary>
        /// Represets the default value of <see cref="SubtitleBackgroundColor"/>.
        /// </summary>
        public static readonly Color DefaultSubtitleBackgroundColor = Color.Black;

        /// <summary>
        /// Represets the default value of <see cref="UpDownArrowsBackgroundColor"/>.
        /// </summary>
        public static readonly Color DefaultUpDownArrowsBackgroundColor = Color.FromArgb(204, 0, 0, 0);

        /// <summary>
        /// Represets the default value of <see cref="UpDownArrowsHighlightColor"/>.
        /// </summary>
        public static readonly Color DefaultUpDownArrowsHighlightColor = Color.FromArgb(25, 255, 255, 255);

        /// <summary>
        /// Represets the default value of <see cref="UpDownArrowsForegroundColor"/>.
        /// </summary>
        public static readonly Color DefaultUpDownArrowsForegroundColor = Color.White;

        internal const string CommonTxd = "commonmenu";
        internal const string BackgroundTextureName = "gradient_bgd";
        internal const string UpAndDownTextureName = "shop_arrows_upanddown";
        internal const string NavBarTextureName = "gradient_nav";
        internal const string ArrowLeftTextureName = "arrowleft";
        internal const string ArrowRightTextureName = "arrowright";
        internal const string CheckboxTickTextureName = "shop_box_tick";
        internal const string CheckboxCrossTextureName = "shop_box_cross";
        internal const string CheckboxBlankTextureName = "shop_box_blank";
        internal const string CheckboxTickSelectedTextureName = "shop_box_tickb";
        internal const string CheckboxCrossSelectedTextureName = "shop_box_crossb";
        internal const string CheckboxBlankSelectedTextureName = "shop_box_blankb";
        internal const string DefaultBannerTextureName = "interaction_bgd";

        /// <summary>
        /// Keeps track of the number of visible menus from the executing plugin.
        /// Used to keep <see cref="Shared.NumberOfVisibleMenus"/> consistent when unloading the plugin with some menu open.
        /// </summary>
        internal static uint NumberOfVisibleMenus { get; set; }

        /// <summary>
        /// Gets whether any menu is currently visible. Includes menus from the executing plugin and from other plugins.
        /// </summary>
        public static bool IsAnyMenuVisible => Shared.NumberOfVisibleMenus > 0;

#if false
        // This can be used to detect if any native script menu is initialized such that the script can draw it.
        // Initially added to add support for native script menus to IsAnyMenuVisible but this method is not accurate enough,
        // some scripts may only set the menuId in the array while the menu is visible, others may keep it when not drawing it (i.e. gunshop, golf).
        // So for now disabled
        public static bool IsAnyGameMenuReady
        {
            get
            {
                foreach (uint menuId in ScriptGlobals.MenuIds)
                {
                    if (menuId != 0)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
#endif

        private Sprite _bannerSprite;
        private ResRectangle _bannerRectangle;

        private Texture _customBanner;

        private int currentItem;
        private RectangleF currentItemBounds = RectangleF.Empty;
        private int minItem;
        private int maxItem;
        private int hoveredItem = -1;
        private int maxItemsOnScreen = DefaultMaxItemsOnScreen;

        private bool visible;
        private bool justOpenedProcessInputFirstTick = true;
        private bool justOpenedProcessInput = true;
        private bool justOpenedProcessMouse = true;

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

        // Whether the value of ResetCursorOnOpen should be respected,
        // set to true while opening the parent menu to avoid resetting the cursor while navigating the menus
        private bool IgnoreResetCursorOnOpen { get; set; }

        //Events

        /// <summary>
        /// Called when user presses up or down, changing current selection.
        /// </summary>
        public event IndexChangedEvent OnIndexChange;

        /// <summary>
        /// Called when user presses left or right, changing a list position.
        /// </summary>
        public event ListChangedEvent OnListChange;

        /// <summary>
        /// Called when user scrolls through a <see cref="UIMenuScrollerItem"/>, changing its index.
        /// </summary>
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

        // internal, needed by UIMenuSwitchMenusItem
        internal readonly Controls controls = new Controls();

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

            InstructionalButtons = new InstructionalButtons { MouseButtonsEnabled = true };
            InstructionalButtons.Buttons.Add(new InstructionalButton(GameControl.FrontendAccept, "Select"),
                                             new InstructionalButton(GameControl.FrontendCancel, "Back"));

            _bannerSprite = new Sprite(spriteLibrary, spriteName, Point.Empty, Size.Empty);
            TitleText = title;
            SubtitleText = subtitle;

            if (subtitle != null && subtitle.StartsWith("~"))
            {
                CounterPretext = subtitle.Substring(0, subtitle.IndexOf('~', 1) + 1);
            }

            CurrentSelection = -1;

            SetKey(Common.MenuControls.Up, GameControl.FrontendUp, 2);
            SetKey(Common.MenuControls.Up, GameControl.CursorScrollUp, 2);

            SetKey(Common.MenuControls.Down, GameControl.FrontendDown, 2);
            SetKey(Common.MenuControls.Down, GameControl.CursorScrollDown, 2);

            SetKey(Common.MenuControls.Left, GameControl.FrontendLeft, 2);

            SetKey(Common.MenuControls.Right, GameControl.FrontendRight, 2);

            SetKey(Common.MenuControls.Select, GameControl.FrontendAccept, 2);

            SetKey(Common.MenuControls.Back, GameControl.FrontendCancel, 2);
            SetKey(Common.MenuControls.Back, GameControl.FrontendPause, 2);
            SetKey(Common.MenuControls.Back, GameControl.CursorCancel, 2);
        }

        /// <summary>
        /// Gets or sets the current width offset in pixels.
        /// </summary>
        public int WidthOffset { get; set; } = 0;

        /// <summary>
        /// Gets or sets the menu width in relative coordinates.
        /// </summary>
        public float Width { get; set; } = DefaultWidth;

        /// <summary>
        /// Gets the menu width in relative coordinates, adjusted for the current aspect ratio and <see cref="WidthOffset"/> value.
        /// </summary>
        public float AdjustedWidth
        {
            get
            {
                float adjusted = Width * (16f / 9f / AspectRatio);

                if (WidthOffset != 0)
                {
                    adjusted += WidthOffset / ActualResolution.Width;
                }

                return adjusted;
            }
        }

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
            }
            else
            {
                for (int i = 0; i < ControlsToDisable.Length; i++)
                {
                    N.DisableControlAction(0, ControlsToDisable[i]);
                }
            }
        }

        // controls taken from the game scripts
        private static readonly GameControl[] ControlsToDisable = new[]
        {
            GameControl.SelectWeapon,
            GameControl.SelectWeaponUnarmed,
            GameControl.SelectWeaponMelee,
            GameControl.SelectWeaponHandgun,
            GameControl.SelectWeaponShotgun,
            GameControl.SelectWeaponSmg,
            GameControl.SelectWeaponAutoRifle,
            GameControl.SelectWeaponSniper,
            GameControl.SelectWeaponHeavy,
            GameControl.SelectWeaponSpecial,
            GameControl.WeaponWheelNext,
            GameControl.WeaponWheelPrev,
            GameControl.WeaponSpecial,
            GameControl.WeaponSpecialTwo,
            GameControl.MeleeAttackLight,
            GameControl.MeleeAttackHeavy,
            GameControl.MeleeBlock,
            GameControl.Detonate,
            GameControl.Context,
            GameControl.Reload,
            GameControl.Dive,
            GameControl.Sprint,
            GameControl.VehicleDuck,
            GameControl.VehicleHeadlight,
            GameControl.VehicleRadioWheel,
            GameControl.NextCamera,
            GameControl.VehicleCinCam,
            GameControl.VehiclePushbikeSprint,
            GameControl.VehiclePushbikePedal,
            GameControl.SelectCharacterMichael,
            GameControl.SelectCharacterFranklin,
            GameControl.SelectCharacterTrevor,
            GameControl.SelectCharacterMultiplayer,
            GameControl.CharacterWheel,
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
        [Obsolete("Use UIMenu.SetBannerType(Color) instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public void SetBannerType(ResRectangle rectangle)
        {
            _bannerSprite = null;
            _customBanner = null;
            _bannerRectangle = rectangle;
        }

        /// <summary>
        /// Sets the banner to a single color rectangle.
        /// </summary>
        /// <param name="color">The new color of the banner.</param>
        public void SetBannerType(Color color)
        {
#pragma warning disable CS0612, CS0618 // Type or member is obsolete
            SetBannerType(new ResRectangle(Point.Empty, Size.Empty, color));
#pragma warning restore CS0612, CS0618 // Type or member is obsolete
        }

        /// <summary>
        /// Set the banner to your own custom texture.
        /// </summary>
        /// <param name="texture">Rage.Texture object</param>
        public void SetBannerType(Texture texture)
        {
            _bannerSprite = null;
            _bannerRectangle = null;
            _customBanner = texture;
        }

        /// <summary>
        /// Removes the banner.
        /// </summary>
        public void RemoveBanner() => SetBannerType((Sprite)null);

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
            if (index >= 0 && index < MenuItems.Count && MenuItems[index].Selected)
            {
                // if the item at the specified position is selected, unselect it and select the new item
                MenuItems[index].Selected = false;
                item.Selected = true;
            }

            item.Parent = this;
            MenuItems.Insert(index, item);

            RefreshCurrentSelection();
        }

        public void AddItems(IEnumerable<UIMenuItem> items)
        {
            foreach (UIMenuItem i in items)
            {
                i.Parent = this;
                MenuItems.Add(i);
            }
            RefreshCurrentSelection();
        }

        public void AddItems(params UIMenuItem[] items) => AddItems((IEnumerable<UIMenuItem>)items);

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
        public virtual void DrawBanner(Rage.Graphics g)
        {
            if (!Visible || _customBanner == null)
            {
                return;
            }

            g.DrawTexture(_customBanner, customBannerX, customBannerY, customBannerW, customBannerH);
        }

        // drawing variables
        private float menuWidth;
        private float itemHeight = 0.034722f;
        private float itemsX, itemsY; // start position of the items, for mouse input
        private float upDownX, upDownY; // start position of the up-down arrows, for mouse input
        private float customBannerX, customBannerY,
                      customBannerW, customBannerH;

        private static SizeF ActualResolution => Internals.Screen.ActualResolution;
        private static float AspectRatio => N.GetAspectRatio(false);
        private static bool IsWideScreen => AspectRatio > 1.5f; // equivalent to GET_IS_WIDESCREEN
        private static bool IsUltraWideScreen => AspectRatio > 3.5f; // > 32:9

        internal static void DrawSprite(string txd, string texName, float x, float y, float w, float h, Color c)
            => N.DrawSprite(txd, texName, x, y, w, h, 0.0f, c.R, c.G, c.B, c.A);

        internal static void DrawRect(float x, float y, float w, float h, Color c)
            => N.DrawRect(x, y, w, h, c.R, c.G, c.B, c.A);

        /// <summary>
        /// Called before drawing the menu or before getting screen coordinates for mouse input.
        /// Intended to align the menu with the safe zone using the <c>SET_SCRIPT_GFX_*</c> natives.
        /// </summary>
        protected virtual void BeginDraw()
        {
            if (ScaleWithSafezone)
            {
                N.SetScriptGfxAlign('L', 'T');
                N.SetScriptGfxAlignParams(-0.05f, -0.05f, 0.0f, 0.0f);
            }
        }

        /// <summary>
        /// Called after drawing the menu or after getting screen coordinates for mouse input to reset the state modified in <see cref="BeginDraw"/>.
        /// </summary>
        protected virtual void EndDraw()
        {
            if (ScaleWithSafezone)
            {
                N.ResetScriptGfxAlign();
            }
        }

        /// <summary>
        /// Draw the menu and all of it's components.
        /// </summary>
        public virtual void Draw()
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
            {
                InstructionalButtons.Draw();
            }

            menuWidth = AdjustedWidth; // save to a field to avoid calculating AdjustedWidth multiple times each tick

            BeginDraw();

            var res = ActualResolution;
            float x = 0.05f + Offset.X / res.Width;
            float y = 0.05f + Offset.Y / res.Height;

            DrawBanner(x, ref y);

            DrawSubtitle(x, ref y);

            DrawBackground(x, y);

            DrawItems(x, ref y);

            DrawUpDownArrows(x, ref y);

            DrawDescription(x, ref y);

            EndDraw();
        }

        /// <summary>
        /// Draws the banner and title at the specified position.
        /// </summary>
        /// <param name="x">The position along the X-axis in relative coordinates.</param>
        /// <param name="y">
        /// The position along the Y-axis in relative coordinates.
        /// When this method returns, contains the position right below the banner.
        /// </param>
        protected virtual void DrawBanner(float x, ref float y)
        {
            if (_bannerSprite == null && _bannerRectangle == null && _customBanner == null)
            {
                return;
            }

            GetBannerDrawSize(out float bannerWidth, out float bannerHeight);
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

                N.GetActiveScreenResolution(out int w, out int h);
                customBannerX *= w;
                customBannerY *= h;
                customBannerW *= w;
                customBannerH *= h;
            }

            y += bannerHeight;
        }

        private void DrawTitle(float x, float y, float bannerHeight)
        {
            if (string.IsNullOrEmpty(TitleText))
            {
                return;
            }

            float titleX = x + menuWidth * 0.5f;
            float titleY = y + bannerHeight * 0.225f;

            TextStyle s = TitleStyle;
            s.Wrap = (x, x + menuWidth);
            TextCommands.Display(TitleText, s, titleX, titleY);
        }

        /// <summary>
        /// Draws the subtitle at the specified position.
        /// </summary>
        /// <param name="x">The position along the X-axis in relative coordinates.</param>
        /// <param name="y">
        /// The position along the Y-axis in relative coordinates.
        /// When this method returns, contains the position right below the subtitle.
        /// </param>
        protected virtual void DrawSubtitle(float x, ref float y)
        {
            if (string.IsNullOrEmpty(SubtitleText))
            {
                return;
            }

            float subtitleWidth = menuWidth;
            float subtitleHeight = itemHeight;

            DrawRect(x + subtitleWidth * 0.5f, y + subtitleHeight * 0.5f, subtitleWidth, subtitleHeight, SubtitleBackgroundColor);

            DrawSubtitleText(x, y);

            DrawSubtitleCounter(x, y);

            y += subtitleHeight;
        }

        private void DrawSubtitleText(float x, float y)
        {
            float subTextX = x + 0.00390625f;
            float subTextY = y + 0.00416664f;

            TextStyle s = SubtitleStyle;
            s.Wrap = (x + 0.0046875f, x + menuWidth - 0.0046875f);
            TextCommands.Display(SubtitleText, s, subTextX, subTextY);
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

            TextStyle s = CounterStyle;
            s.Wrap = (x + 0.0046875f, x + menuWidth - 0.0046875f);

            float counterWidth = TextCommands.GetWidth(counterText, s);

            float counterX = x + menuWidth - 0.00390625f - counterWidth;
            float counterY = y + 0.00416664f;

            TextCommands.Display(counterText, s, counterX, counterY);
        }

        /// <summary>
        /// Draws the items background at the specified position.
        /// </summary>
        /// <param name="x">The position along the X-axis in relative coordinates.</param>
        /// <param name="y">The position along the Y-axis in relative coordinates.</param>
        protected virtual void DrawBackground(float x, float y)
        {
            float bgWidth = menuWidth;
            float bgHeight = itemHeight * Math.Min(MenuItems.Count, MaxItemsOnScreen);

            DrawSprite(CommonTxd, BackgroundTextureName,
                       x + bgWidth * 0.5f,
                       y + bgHeight * 0.5f - 0.00138888f,
                       bgWidth,
                       bgHeight,
                       Color.White);
        }

        /// <summary>
        /// Draws the items beginning at the specified position.
        /// </summary>
        /// <param name="x">The position along the X-axis in relative coordinates.</param>
        /// <param name="y">
        /// The position along the Y-axis in relative coordinates.
        /// When this method returns, contains the position right below the items.
        /// </param>
        protected virtual void DrawItems(float x, ref float y)
        {
            if (MenuItems.Count == 0)
            {
                return;
            }

            itemsX = x;
            itemsY = y;

            for (int index = minItem; index <= maxItem; index++)
            {
                if (CurrentSelection == index)
                {
                    float x1 = x, y1 = y;
                    float x2 = x + menuWidth, y2 = y + itemHeight;
                    N.GetScriptGfxPosition(x1, y1, out x1, out y1);
                    N.GetScriptGfxPosition(x2, y2, out x2, out y2);

                    currentItemBounds = new RectangleF(x1, y1, x2 - x1, y2 - y1);
                }

                var item = MenuItems[index];
                item.Draw(x, y, menuWidth, itemHeight);
                y += itemHeight;
            }
        }

        /// <summary>
        /// Draws the up-down arrows at the specified position.
        /// </summary>
        /// <param name="x">The position along the X-axis in relative coordinates.</param>
        /// <param name="y">
        /// The position along the Y-axis in relative coordinates.
        /// When this method returns, contains the position right below the up-down arrows.
        /// </param>
        protected virtual void DrawUpDownArrows(float x, ref float y)
        {
            if (MenuItems.Count > MaxItemsOnScreen)
            {
                float upDownRectWidth = menuWidth;
                float upDownRectHeight = itemHeight;

                upDownX = x;
                upDownY = y;

                DrawRect(x + upDownRectWidth * 0.5f, y + upDownRectHeight * 0.5f, upDownRectWidth, upDownRectHeight, UpDownArrowsBackgroundColor);

                if (hoveredUpDown != 0)
                {
                    float hoverRectH = upDownRectHeight * 0.5f;
                    DrawRect(x + upDownRectWidth * 0.5f, y + (hoverRectH * (hoveredUpDown - 0.5f)), upDownRectWidth, hoverRectH, UpDownArrowsHighlightColor);
                }

                Vector3 upDownSize = N.GetTextureResolution(CommonTxd, UpAndDownTextureName);
                float upDownWidth = (upDownSize.X * 0.5f) / 1280.0f;
                float upDownHeight = (upDownSize.Y * 0.5f) / 720.0f;
                DrawSprite(CommonTxd, UpAndDownTextureName, x + upDownRectWidth * 0.5f, y + upDownRectHeight * 0.5f, upDownWidth, upDownHeight, UpDownArrowsForegroundColor);

                y += itemHeight;
            }
        }

        /// <summary>
        /// Draws the description text and background at the specified position.
        /// </summary>
        /// <param name="x">The position along the X-axis in relative coordinates.</param>
        /// <param name="y">
        /// The position along the Y-axis in relative coordinates.
        /// When this method returns, contains the position right below the description.
        /// </param>
        protected virtual void DrawDescription(float x, ref float y)
        {
            if (DescriptionOverride == null && MenuItems.Count == 0)
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

                TextStyle s = DescriptionStyle;
                s.Wrap = (textX, textXEnd);

                int lineCount = TextCommands.GetLineCount(description, s, textX, y + 0.00277776f);

                float descHeight = (s.CharacterHeight * lineCount) + (0.00138888f * 13f) + (0.00138888f * 5f * (lineCount - 1));
                DrawSprite(CommonTxd, BackgroundTextureName,
                           x + menuWidth * 0.5f,
                           y + (descHeight * 0.5f) - 0.00138888f,
                           menuWidth,
                           descHeight,
                           backColor);

                DrawRect(x + menuWidth * 0.5f, y - 0.00277776f * 0.5f, menuWidth, 0.00277776f, DescriptionSeparatorColor);

                TextCommands.Display(description, s, textX, y + 0.00277776f);

                y += descHeight;
            }
        }

        // Converted from game scripts
        internal static void GetTextureDrawSize(string txd, string texName, out float width, out float height)
        {
            N.GetScreenResolution(out int screenWidth, out int screenHeight);
            Vector3 texSize = N.GetTextureResolution(txd, texName);
            texSize.X *= 0.5f;
            texSize.Y *= 0.5f;

            width = texSize.X / screenWidth * (screenWidth / screenHeight);
            height = texSize.Y / screenHeight / (texSize.X / screenWidth) * width;

            if (!IsWideScreen)
            {
                width *= 1.33f;
            }
        }

        private void GetBannerDrawSize(out float width, out float height)
        {
            const float TexWidth = 512.0f; // resolution of banner texture ('commonmenu', 'interaction_bgd')
            const float TexHeight = 128.0f;
            const float TexRatio = TexWidth / TexHeight;

            var res = ActualResolution;
            var ratio = res.Width / res.Height;

            width = menuWidth;
            height = DefaultWidth / TexRatio * ratio;
        }

        /// <summary>
        /// Returns the 1080pixels-based screen resolution while mantaining current aspect ratio.
        /// </summary>
        /// <returns></returns>
        public static SizeF GetScreenResolutionMantainRatio()
        {
            var res = Internals.Screen.ActualResolution;
            var screenw = res.Width;
            var screenh = res.Height;
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

        [Obsolete("Use UIMenu.GoUp() instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public void GoUpOverflow()
        {
            GoUp();
        }

        /// <summary>
        /// Sends a <see cref="Common.MenuControls.Up"/> input event to the selected item. If not consumed, the previous item is selected.
        /// </summary>
        public virtual void GoUp()
        {
            if (MenuItems.Count == 0)
            {
                return;
            }

            if (!MenuItems[CurrentSelection].OnInput(this, Common.MenuControls.Up))
            {
                CurrentSelection--;

                Common.PlaySound(AUDIO_UPDOWN, AUDIO_LIBRARY);
                IndexChange(CurrentSelection);
            }
        }

        [Obsolete("Use UIMenu.GoDown() instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public void GoDownOverflow()
        {
            GoDown();
        }

        /// <summary>
        /// Sends a <see cref="Common.MenuControls.Down"/> input event to the selected item. If not consumed, the next item is selected.
        /// </summary>
        public virtual void GoDown()
        {
            if (MenuItems.Count == 0)
            {
                return;
            }
            
            if (!MenuItems[CurrentSelection].OnInput(this, Common.MenuControls.Down))
            {
                CurrentSelection++;

                Common.PlaySound(AUDIO_UPDOWN, AUDIO_LIBRARY);
                IndexChange(CurrentSelection);
            }
        }

        /// <summary>
        /// Sends a <see cref="Common.MenuControls.Left"/> input event to the selected item.
        /// </summary>
        public virtual void GoLeft()
        {
            if (MenuItems.Count == 0)
            {
                return;
            }
            
            MenuItems[CurrentSelection].OnInput(this, Common.MenuControls.Left);
        }

        /// <summary>
        /// Sends a <see cref="Common.MenuControls.Right"/> input event to the selected item.
        /// </summary>
        public virtual void GoRight()
        {
            if (MenuItems.Count == 0)
            {
                return;
            }

            MenuItems[CurrentSelection].OnInput(this, Common.MenuControls.Right);
        }

        /// <summary>
        /// Sends a <see cref="Common.MenuControls.Select"/> input event to the selected item.
        /// </summary>
        public virtual void SelectItem()
        {
            if (MenuItems.Count == 0)
            {
                return;
            }

            MenuItems[CurrentSelection].OnInput(this, Common.MenuControls.Select);
        }

        /// <summary>
        /// Sends a <see cref="Common.MenuControls.Back"/> input event to the selected item. If not consumed, the menu is closed.
        /// </summary>
        public virtual void GoBack()
        {
            if (MenuItems.Count == 0 || !MenuItems[CurrentSelection].OnInput(this, Common.MenuControls.Back))
            {
                Common.PlaySound(AUDIO_BACK, AUDIO_LIBRARY);
                Close();
            }
        }

        /// <summary>
        /// Opens the menu binded to the specified item, if any.
        /// </summary>
        /// <param name="item">The item with the binded menu.</param>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is null.</exception>
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

        /// <summary>
        /// Closes this menu.
        /// </summary>
        /// <param name="openParentMenu">If <c>true</c> and <see cref="ParentMenu"/> is not <c>null</c>, the parent menu is opened after closing this menu.</param>
        public void Close(bool openParentMenu = true)
        {
            Visible = false;
            if (openParentMenu && ParentMenu != null)
            {
                ParentMenu.IgnoreResetCursorOnOpen = true;
                ParentMenu.Visible = true;
                ParentMenu.IgnoreResetCursorOnOpen = false;
                MenuChangeEv(ParentMenu, false);
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
        /// Process the mouse input. Checks if the mouse is hovering any <see cref="UIMenuItem"/> or the up-down arrows,
        /// and sends the appropriate input events to the menu items.
        /// </summary>
        public virtual void ProcessMouse()
        {
            if (!Visible || justOpenedProcessMouse || MenuItems.Count == 0 || IsUsingController || !MouseControlsEnabled)
            {
                if (hoveredItem != -1)
                {
                    MenuItems[hoveredItem].Hovered = false;
                    hoveredItem = -1;
                }
                hoveredUpDown = 0;
                justOpenedProcessMouse = false;
                return;
            }

            if (Game.Console.IsOpen)
            {
                justOpenedProcessMouse = true;
                return;
            }

            N.SetMouseCursorActiveThisFrame();
            N.SetMouseCursorSprite(1);

            N.x5B73C77D9EB66E24(true);

            // make mouse exclusive to CursorX/Y controls to avoid moving the camera when interacting with the menu
            N.SetInputExclusive(2, GameControl.CursorX);
            N.SetInputExclusive(2, GameControl.CursorY);

            float mouseX = N.GetControlNormal(2, GameControl.CursorX);
            float mouseY = N.GetControlNormal(2, GameControl.CursorY);

            // send mouse input event to selected item
            UIMenuItem selectedItem = MenuItems[CurrentSelection];

            UIMenuItem.MouseInput input = UIMenuItem.MouseInput.Released;
            if (controls.CursorAccept.IsJustPressed)
            {
                input = UIMenuItem.MouseInput.JustPressed;
            }
            else if (controls.CursorAccept.IsJustReleased)
            {
                input = UIMenuItem.MouseInput.JustReleased;
            }
            else if (controls.CursorAccept.IsJustPressedRepeated)
            {
                input = UIMenuItem.MouseInput.PressedRepeat;
            }
            else if (controls.CursorAccept.IsPressed)
            {
                input = UIMenuItem.MouseInput.Pressed;
            }

            bool mouseInputConsumed = selectedItem.OnMouseInput(this, currentItemBounds, new PointF(mouseX, mouseY), input);
            if (!mouseInputConsumed)
            {
                UpdateHoveredItem(mouseX, mouseY);

                if (hoveredItem != -1)
                {
                    hoveredUpDown = 0;
                    if (hoveredItem != CurrentSelection && input == UIMenuItem.MouseInput.JustReleased)
                    {
                        CurrentSelection = hoveredItem;
                        Common.PlaySound(AUDIO_UPDOWN, AUDIO_LIBRARY);
                        IndexChange(CurrentSelection);
                    }
                }
                else if (hoveredItem == -1)
                {
                    UpdateHoveredUpDown(mouseX, mouseY);

                    if (hoveredUpDown != 0 && controls.CursorAccept.IsJustPressedRepeated)
                    {
                        if (hoveredUpDown == 1)
                        {
                            GoUp();
                        }
                        else
                        {
                            GoDown();
                        }
                    }

                    if (MouseEdgeEnabled)
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
            }
        }

        private void UpdateHoveredItem(float mouseX, float mouseY)
        {
            BeginDraw();

            int visibleItemCount = Math.Min(MenuItems.Count, MaxItemsOnScreen);
            float x1 = itemsX, y1 = itemsY - 0.00138888f; // background top-left
            float x2 = itemsX + menuWidth, y2 = y1 + visibleItemCount * itemHeight; // background bottom-right
            N.GetScriptGfxPosition(x1, y1, out x1, out y1);
            N.GetScriptGfxPosition(x2, y2, out x2, out y2);

            EndDraw();

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
                BeginDraw();

                float x1 = upDownX, y1 = upDownY; // up&down rect top-left
                float x2 = x1 + menuWidth, y2 = y1 + itemHeight; // up&down rect bottom-right
                N.GetScriptGfxPosition(x1, y1, out x1, out y1);
                N.GetScriptGfxPosition(x2, y2, out x2, out y2);

                EndDraw();

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
        /// Set a <see cref="GameControl"/> to control a menu. Can be multiple controls. This applies it to all indexes.
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
        /// Set a <see cref="GameControl"/> to control a menu only on a specific index.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="gtaControl"></param>
        /// <param name="controlIndex"></param>
        public void SetKey(Common.MenuControls control, GameControl gtaControl, int controlIndex) => controls[control].NativeControls.Add((controlIndex, gtaControl));

        /// <summary>
        /// Sets how fast the input events of <paramref name="control"/> are triggered.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="acceleration">An array containing the control acceleration. If <c>null</c>, the default acceleration is used.</param>
        /// <exception cref="ArgumentException"><paramref name="acceleration"/> length is 0.</exception>
        public void SetKeyAcceleration(Common.MenuControls control, AccelerationStep[] acceleration)
        {
            AccelerationStep[] newAcceleration = DefaultRepeatAcceleration;
            if (acceleration != null)
            {
                if (acceleration.Length == 0)
                {
                    throw new ArgumentException("The array lenght must be greater than 0", nameof(acceleration));
                }

                newAcceleration = new AccelerationStep[acceleration.Length];
                Array.Copy(acceleration, newAcceleration, acceleration.Length);
                Array.Sort(newAcceleration, (a, b) => a.HeldTime.CompareTo(b.HeldTime));
            }

            controls[control].RepeatAcceleration = newAcceleration;
        }

        /// <summary>
        /// Remove all controls on a control and sets its default acceleration.
        /// </summary>
        /// <param name="control"></param>
        public void ResetKey(Common.MenuControls control) => controls[control].Reset();

        /// <summary>
        /// Process control-stroke. Call this in the Game.FrameRender event or in a loop.
        /// </summary>
        public virtual void ProcessControl()
        {
            if (!Visible || Game.Console.IsOpen)
            {
                return;
            }

            // Ignore the first tick because RPH Game.IsControllerButton* methods return true a tick earlier than the natives we use.
            // So, for example:
            //  - A menu is opened with DPadRight and a list item is currently selected,
            //  - In the first tick `controls.AllReleasedSinceOpening()` would return true (since the natives consider it released)
            //  - In the next tick `controls.Update()` would set `controls[Common.MenuControls.Right].IsJustPressedRepeated` to true
            //    (since now the natives consider it pressed), causing the list item to scroll to the next value
            if (justOpenedProcessInputFirstTick)
            {
                justOpenedProcessInputFirstTick = false;
                return;
            }

            controls.Update();

            if (justOpenedProcessInput)
            {
                // wait until the controls are released, in case any was used to open the menu
                if (controls.AllReleasedSinceOpening())
                {
                    justOpenedProcessInput = false;
                }
                return;
            }

            if (controls[Common.MenuControls.Back].IsJustReleased)
            {
                GoBack();
            }
            else if (controls[Common.MenuControls.Select].IsJustReleased)
            {
                SelectItem();
            }
            else if (controls[Common.MenuControls.Up].IsJustPressedRepeated)
            {
                GoUp();
            }
            else if (controls[Common.MenuControls.Down].IsJustPressedRepeated)
            {
                GoDown();
            }
            else if (controls[Common.MenuControls.Left].IsJustPressedRepeated)
            {
                GoLeft();
            }
            else if (controls[Common.MenuControls.Right].IsJustPressedRepeated)
            {
                GoRight();
            }
        }

        public void AddInstructionalButton(InstructionalButton button) => InstructionalButtons.Buttons.Add(button);
        public void AddInstructionalButton(IInstructionalButtonSlot button) => InstructionalButtons.Buttons.Add(button);
        public void RemoveInstructionalButton(InstructionalButton button) => InstructionalButtons.Buttons.Remove(button);
        public void RemoveInstructionalButton(IInstructionalButtonSlot button) => InstructionalButtons.Buttons.Remove(button);

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
                    if (item is UIMenuListItem l)
                    {
                        l.Index = 0;
                    }
                    else if (item is UIMenuScrollerItem s)
                    {
                        s.Index = s.IsEmpty ? UIMenuScrollerItem.EmptyIndex : 0;
                    }
                }

                if (resetCheckboxes && item is UIMenuCheckboxItem c)
                {
                    c.Checked = false;
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
            CurrentSelection = CurrentSelection == -1 ? 0 : CurrentSelection;
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
                    justOpenedProcessInputFirstTick = value;
                    justOpenedProcessInput = value;
                    justOpenedProcessMouse = value;
                    controls.SetJustOpened(value);

                    if (visible)
                    {
                        if (!IgnoreVisibility)
                        {
                            Shared.NumberOfVisibleMenus++;
                            NumberOfVisibleMenus++;
                        }
                        MenuOpenEv();
                    }
                    else
                    {
                        if (!IgnoreVisibility)
                        {
                            Shared.NumberOfVisibleMenus--;
                            NumberOfVisibleMenus--;
                        }
                    }

                    InstructionalButtons.Update();
                    if (ParentMenu != null || !visible)
                    {
                        return;
                    }

                    if (ResetCursorOnOpen && !IgnoreResetCursorOnOpen)
                    {
                        N.SetMouseCursorLocation(0.5f, 0.5f);
                        N.SetMouseCursorSprite(1);
                    }
                }
            }
        }

        /// <summary>
        /// If <c>true</c>, this menu is not included in <see cref="NumberOfVisibleMenus"/>.
        /// Needed by <see cref="PauseMenu.TabInteractiveListItem"/>, which uses a hidden menu for managing the menu items.
        /// </summary>
        internal bool IgnoreVisibility { get; set; }

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
        /// Gets the index of the first visible item.
        /// </summary>
        public int FirstItemOnScreen => minItem;

        /// <summary>
        /// Gets the index of the last visible item.
        /// </summary>
        public int LastItemOnScreen => maxItem;

        /// <summary>
        /// Returns false if last input was made with mouse and keyboard, true if it was made with a controller.
        /// </summary>
        public static bool IsUsingController => !N.IsInputDisabled(2);

        /// <summary>
        /// Gets or sets the title text.
        /// </summary>
        [Obsolete("Use UIMenu.TitleText instead. The only functional property of the returned ResText is ResText.Caption"), EditorBrowsable(EditorBrowsableState.Never)]
        public ResText Title { get; set; } = new ResText(null, Point.Empty, 0.0f);

        /// <summary>
        /// Gets or sets the title text.
        /// </summary>
        public string TitleText
        {
#pragma warning disable CS0612, CS0618 // Type or member is obsolete
            get => Title.Caption;
            set => Title.Caption = value;
#pragma warning restore CS0612, CS0618 // Type or member is obsolete
        }

        /// <summary>
        /// Gets or sets the title text style. Note, <see cref="TextStyle.Wrap"/> is ignored and instead calculated based on the current menu width.
        /// </summary>
        /// <seealso cref="DefaultTitleStyle"/>
        public TextStyle TitleStyle { get; set; } = DefaultTitleStyle;

        /// <summary>
        /// Gets or sets the subtitle text.
        /// </summary>
        [Obsolete("Use UIMenu.SubtitleText instead. The only functional property of the returned ResText is ResText.Caption"), EditorBrowsable(EditorBrowsableState.Never)]
        public ResText Subtitle { get; set; } = new ResText(null, Point.Empty, 0.0f);

        /// <summary>
        /// Gets or sets the subtitle text.
        /// </summary>
        public string SubtitleText
        {
#pragma warning disable CS0612, CS0618 // Type or member is obsolete
            get => Subtitle.Caption;
            set => Subtitle.Caption = value;
#pragma warning restore CS0612, CS0618 // Type or member is obsolete
        }

        /// <summary>
        /// Gets or sets the subtitle text style. Note, <see cref="TextStyle.Wrap"/> is ignored and instead calculated based on the current menu width.
        /// </summary>
        /// <seealso cref="DefaultSubtitleStyle"/>
        public TextStyle SubtitleStyle { get; set; } = DefaultSubtitleStyle;

        /// <summary>
        /// Gets or sets the color of the subtitle background rectangle.
        /// </summary>
        /// <seealso cref="DefaultSubtitleBackgroundColor"/>
        public Color SubtitleBackgroundColor { get; set; } = DefaultSubtitleBackgroundColor;

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
        /// Gets or sets the counter text style. Note, <see cref="TextStyle.Wrap"/> is ignored and instead calculated based on the current menu width.
        /// </summary>
        /// <seealso cref="DefaultCounterStyle"/>
        public TextStyle CounterStyle { get; set; } = DefaultCounterStyle;

        /// <summary>
        /// Gets or sets the description override.
        /// If not <c>null</c>, this <see cref="string"/> is shown instead of
        /// the <see cref="UIMenuItem.Description"/> of the currently selected item.
        /// </summary>
        public string DescriptionOverride { get; set; }

        /// <summary>
        /// Gets or sets the description text style. Note, <see cref="TextStyle.Wrap"/> is ignored and instead calculated based on the current menu width.
        /// </summary>
        /// <seealso cref="DefaultDescriptionStyle"/>
        public TextStyle DescriptionStyle { get; set; } = DefaultDescriptionStyle;

        /// <summary>
        /// Gets or sets the color of the description separator bar.
        /// </summary>
        /// <seealso cref="DefaultDescriptionSeparatorColor"/>
        public Color DescriptionSeparatorColor { get; set; } = DefaultDescriptionSeparatorColor;

        /// <summary>
        /// Gets or sets the color of the up-down arrows background rectangle.
        /// </summary>
        /// <seealso cref="DefaultUpDownArrowsBackgroundColor"/>
        public Color UpDownArrowsBackgroundColor { get; set; } = DefaultUpDownArrowsBackgroundColor;

        /// <summary>
        /// Gets or sets the color used to highlight the up-down arrow selected with the mouse.
        /// </summary>
        /// <seealso cref="DefaultUpDownArrowsHighlightColor"/>
        public Color UpDownArrowsHighlightColor { get; set; } = DefaultUpDownArrowsHighlightColor;

        /// <summary>
        /// Gets or sets the color of the up-down arrows.
        /// </summary>
        /// <seealso cref="DefaultUpDownArrowsForegroundColor"/>
        public Color UpDownArrowsForegroundColor { get; set; } = DefaultUpDownArrowsForegroundColor;

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

        internal sealed class Controls
        {
            private static readonly int NumControls = Enum.GetValues(typeof(Common.MenuControls)).Length;

            private readonly Control[] controls = new Control[NumControls];

            public Control this[Common.MenuControls control] => controls[(int)control];
            public Control CursorAccept { get; } = new Control();

            public Controls()
            {
                for (int i = 0; i < controls.Length; i++)
                {
                    controls[i] = new Control();
                }

                CursorAccept.NativeControls.Add((2, GameControl.CursorAccept));
            }

            public void ResetState()
            {
                for (int i = 0; i < controls.Length; i++)
                {
                    controls[i].ResetState();
                }
                CursorAccept.ResetState();
            }

            public void Update()
            {
                uint gameTime = Game.GameTime;
                for (int i = 0; i < controls.Length; i++)
                {
                    controls[i].Update(gameTime);
                }
                CursorAccept.Update(gameTime);
            }

            public void SetJustOpened(bool value)
            {
                for (int i = 0; i < controls.Length; i++)
                {
                    controls[i].JustOpened = value;
                }
                CursorAccept.JustOpened = value;
            }

            public bool AllReleasedSinceOpening()
            {
                for (int i = 0; i < controls.Length; i++)
                {
                    if (controls[i].JustOpened)
                    {
                        return false;
                    }
                }

                return !CursorAccept.JustOpened;
            }
        }

        internal sealed class Control
        {
            private uint pressedStartTime;
            private uint nextRepeatTime;
            private int repeatAccelerationIndex;

            public IList<(int Index, GameControl Control)> NativeControls { get; } = new List<(int, GameControl)>();
            public AccelerationStep[] RepeatAcceleration { get; set; } = DefaultRepeatAcceleration;
            public bool IsJustReleased { get; private set; }
            public bool IsJustPressed { get; private set; }
            public bool IsReleased { get; private set; }
            public bool IsPressed { get; private set; }
            public bool IsJustPressedRepeated { get; private set; }

            public bool JustOpened { get; set; }

            public void Reset()
            {
                NativeControls.Clear();
                RepeatAcceleration = DefaultRepeatAcceleration;
            }

            public void ResetState()
            {
                pressedStartTime = 0;
                nextRepeatTime = 0;
                repeatAccelerationIndex = 0;
                IsJustReleased = false;
                IsJustPressed = false;
                IsReleased = false;
                IsPressed = false;
                IsJustPressedRepeated = false;
            }

            public void Update(uint gameTime)
            {
                foreach (var c in NativeControls)
                {
                    N.SetInputExclusive(c.Index, c.Control);
                }

                if (JustOpened)
                {
                    ResetState();
                    if (AllReleased())
                    {
                        JustOpened = false;
                    }
                    return;
                }

                bool prevPressed = IsPressed;
                IsJustPressed = AnyJustPressed();
                IsPressed = IsJustPressed || AnyPressed();
                IsReleased = !IsPressed;
                IsJustPressedRepeated = IsJustPressed;

                if (IsPressed)
                {
                    if (IsJustPressed || (prevPressed != IsPressed))
                    {
                        pressedStartTime = gameTime;
                        repeatAccelerationIndex = 0;
                        nextRepeatTime = GetNextRepeatTime(gameTime);
                    }

                    if (gameTime >= nextRepeatTime)
                    {
                        IsJustPressedRepeated = true;
                        nextRepeatTime = GetNextRepeatTime(gameTime);
                    }
                }
                else
                {
                    IsJustReleased = AnyJustReleased();
                }
            }

            private uint GetHeldTime(uint gameTime)
            {
                return gameTime - pressedStartTime;
            }

            private uint GetNextRepeatTime(uint gameTime)
            {
                uint heldTime = GetHeldTime(gameTime);
                AccelerationStep[] acc = RepeatAcceleration;
                while (repeatAccelerationIndex < acc.Length - 1 &&
                       acc[repeatAccelerationIndex + 1].HeldTime < heldTime)
                {
                    repeatAccelerationIndex++;
                }

                return gameTime + acc[repeatAccelerationIndex].TimeBetweenRepeats;
            }

            private bool AnyJustReleased()
            {
                for (int i = 0; i < NativeControls.Count; i++)
                {
                    var c = NativeControls[i];
                    if (Game.IsControlJustReleased(c.Index, c.Control))
                    {
                        return true;
                    }
                }

                return false;
            }

            private bool AnyJustPressed()
            {
                for (int i = 0; i < NativeControls.Count; i++)
                {
                    var c = NativeControls[i];
                    if (Game.IsControlJustPressed(c.Index, c.Control))
                    {
                        return true;
                    }
                }

                return false;
            }

            private bool AnyPressed()
            {
                for (int i = 0; i < NativeControls.Count; i++)
                {
                    var c = NativeControls[i];
                    if (Game.IsControlPressed(c.Index, c.Control))
                    {
                        return true;
                    }
                }

                return false;
            }

            private bool AllReleased()
            {
                for (int i = 0; i < NativeControls.Count; i++)
                {
                    var c = NativeControls[i];
                    if (!N.IsControlReleased(c.Index, c.Control))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Defines the how fast the input events of a control are triggered.
        /// </summary>
        public readonly struct AccelerationStep
        {
            /// <summary>
            /// Gets the number of milliseconds the control must be pressed before using the <see cref="TimeBetweenRepeats"/> of this step.
            /// </summary>
            public uint HeldTime { get; }
            
            /// <summary>
            /// Gets the number of milliseconds between input events of the control.
            /// </summary>
            public uint TimeBetweenRepeats { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="AccelerationStep"/> structure.
            /// </summary>
            /// <param name="heldTime">Determines the number of milliseconds the control must be pressed before using the <see cref="TimeBetweenRepeats"/> of this step.</param>
            /// <param name="timeBetweenRepeats">Determines the number of milliseconds between input events of the control.</param>
            public AccelerationStep(uint heldTime, uint timeBetweenRepeats)
            {
                HeldTime = heldTime;
                TimeBetweenRepeats = timeBetweenRepeats;
            }
        }

        private static readonly AccelerationStep[] DefaultRepeatAcceleration = new AccelerationStep[]
        {
            new AccelerationStep(0, 300),
            new AccelerationStep(1000, 180),
            new AccelerationStep(2000, 110),
            new AccelerationStep(6000, 50),
        };

        #region Obsolete Stuff

        [Obsolete("Use UIMenu.SetKeyAcceleration(Common.MenuControls, AccelerationStep[]) instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public uint HoldTimeBeforeScroll = 200;

        [Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
        public bool HasControlJustBeenPressed(Common.MenuControls control, Keys key = Keys.None) => controls[control].IsJustPressed;
        [Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
        public bool HasControlJustBeenReleaseed(Common.MenuControls control, Keys key = Keys.None) => controls[control].IsJustReleased;
        [Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsControlBeingPressed(Common.MenuControls control, Keys key = Keys.None) => controls[control].IsJustPressedRepeated;

        #endregion
    }
}
