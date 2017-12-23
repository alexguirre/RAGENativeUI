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

    public delegate void ItemSelectEvent(UIMenu sender, UIMenuItem selectedItem, int index);

    public delegate void MenuCloseEvent(UIMenu sender);

    public delegate void MenuChangeEvent(UIMenu oldMenu, UIMenu newMenu, bool forward);

    public delegate void ItemActivatedEvent(UIMenu sender, UIMenuItem selectedItem);

    public delegate void ItemCheckboxEvent(UIMenuCheckboxItem sender, bool Checked);

    public delegate void ItemListEvent(UIMenuItem sender, int newIndex);

    /// <summary>
    /// Base class for RAGENativeUI. Calls the next events: OnIndexChange, OnListChanged, OnCheckboxChange, OnItemSelect, OnMenuClose, OnMenuchange.
    /// </summary>
    public class UIMenu
    {
        private readonly Container _mainMenu;
        private Sprite _logo;
        private readonly Sprite _background;

        private readonly ResRectangle _descriptionBar;
        private readonly Sprite _descriptionRectangle;
        private readonly ResText _descriptionText;
        private readonly ResText _counterText;

        private Texture _customBanner;

        private int _activeItem = 1000;

        private bool _visible;

        private bool _justOpened = true;

        //Pagination
        private const int MaxItemsOnScreen = 9;
        private int _minItem;
        private int _maxItem = MaxItemsOnScreen;

        
        private readonly Sprite _upAndDownSprite;
        private readonly ResRectangle _extraRectangleUp;
        private readonly ResRectangle _extraRectangleDown;

        private Point _offset;
        private readonly int _extraYOffset;

        private readonly InstructionalButtons instructionalButtons;

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

        /// <summary>
        /// Called when user presses enter on a checkbox item.
        /// </summary>
        public event CheckboxChangeEvent OnCheckboxChange;

        /// <summary>
        /// Called when user selects a simple item.
        /// </summary>
        public event ItemSelectEvent OnItemSelect;

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
        /// Basic Menu constructor.
        /// </summary>
        /// <param name="title">Title that appears on the big banner.</param>
        /// <param name="subtitle">Subtitle that appears in capital letters in a small black bar.</param>
        public UIMenu(string title, string subtitle) : this(title, subtitle, new Point(0, 0), "commonmenu", "interaction_bgd")
        {
        }

        /// <summary>
        /// Basic Menu constructor with an offset.
        /// </summary>
        /// <param name="title">Title that appears on the big banner.</param>
        /// <param name="subtitle">Subtitle that appears in capital letters in a small black bar. Set to "" if you dont want a subtitle.</param>
        /// <param name="offset">Point object with X and Y data for offsets. Applied to all menu elements.</param>
        public UIMenu(string title, string subtitle, Point offset) : this(title, subtitle, offset, "commonmenu", "interaction_bgd")
        {
        }

        /// <summary>
        /// Initialise a menu with a custom texture banner.
        /// </summary>
        /// <param name="title">Title that appears on the big banner. Set to "" if you don't want a title.</param>
        /// <param name="subtitle">Subtitle that appears in capital letters in a small black bar. Set to "" if you dont want a subtitle.</param>
        /// <param name="offset">Point object with X and Y data for offsets. Applied to all menu elements.</param>
        /// <param name="customBanner">Your custom Rage.Texture.</param>
        public UIMenu(string title, string subtitle, Point offset, Texture customBanner) : this(title, subtitle, offset, "commonmenu", "interaction_bgd")
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
            _offset = offset;
            Children = new Dictionary<UIMenuItem, UIMenu>();
            WidthOffset = 0;
            
            instructionalButtons = new InstructionalButtons();
            instructionalButtons.Buttons.Add(new InstructionalButton(GameControl.CellphoneSelect, "Select"));
            instructionalButtons.Buttons.Add(new InstructionalButton(GameControl.CellphoneCancel, "Back"));

            _mainMenu = new Container(new Point(0, 0), new Size(700, 500), Color.FromArgb(0, 0, 0, 0));
            _logo = new Sprite(spriteLibrary, spriteName, new Point(0 + _offset.X, 0 + _offset.Y), new Size(431, 107));
            _mainMenu.Items.Add(Title = new ResText(title, new Point(215 + _offset.X, 20 + _offset.Y), 1.15f, Color.White, Common.EFont.HouseScript, ResText.Alignment.Centered));
            if (!String.IsNullOrWhiteSpace(subtitle))
            {
                _mainMenu.Items.Add(new ResRectangle(new Point(0 + offset.X, 107 + _offset.Y), new Size(431, 37), Color.Black));
                _mainMenu.Items.Add(Subtitle = new ResText(subtitle, new Point(8 + _offset.X, 110 + _offset.Y), 0.35f, Color.WhiteSmoke, 0, ResText.Alignment.Left));

                if (subtitle.StartsWith("~"))
                {
                    CounterPretext = subtitle.Substring(0, 3);
                }
                _counterText = new ResText("", new Point(425 + _offset.X, 110 + _offset.Y), 0.35f, Color.WhiteSmoke, 0, ResText.Alignment.Right);
                _extraYOffset = 37;
            }

            _upAndDownSprite = new Sprite("commonmenu", "shop_arrows_upanddown", new Point(190 + _offset.X, 147 + 37 * (MaxItemsOnScreen + 1) + _offset.Y - 37 + _extraYOffset), new Size(50, 50));
            _extraRectangleUp = new ResRectangle(new Point(0 + _offset.X, 144 + 38 * (MaxItemsOnScreen + 1) + _offset.Y - 37 + _extraYOffset), new Size(431, 18), Color.FromArgb(200, 0, 0, 0));
            _extraRectangleDown = new ResRectangle(new Point(0 + _offset.X, 144 + 18 + 38 * (MaxItemsOnScreen + 1) + _offset.Y - 37 + _extraYOffset), new Size(431, 18), Color.FromArgb(200, 0, 0, 0));

            _descriptionBar = new ResRectangle(new Point(_offset.X, 123), new Size(431, 4), Color.Black);
            _descriptionRectangle = new Sprite("commonmenu", "gradient_bgd", new Point(_offset.X, 127), new Size(431, 30));
            _descriptionText = new ResText("Description", new Point(_offset.X + 5, 125), 0.35f, Color.FromArgb(255, 255, 255, 255), Common.EFont.ChaletLondon, ResText.Alignment.Left);

            _background = new Sprite("commonmenu", "gradient_bgd", new Point(_offset.X, 144 + _offset.Y - 37 + _extraYOffset), new Size(290, 25));

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

        private void RecalculateDescriptionPosition()
        {
            //_descriptionText.WordWrap = new Size(425 + WidthOffset, 0);

            _descriptionBar.Position = new Point(_offset.X, 149 - 37 + _extraYOffset + _offset.Y);
            _descriptionRectangle.Position = new Point(_offset.X, 149 - 37 + _extraYOffset + _offset.Y);
            _descriptionText.Position = new Point(_offset.X + 8, 155 - 37 + _extraYOffset + _offset.Y);

            _descriptionBar.Size = new Size(431 + WidthOffset, 4);
            _descriptionRectangle.Size = new Size(431 + WidthOffset, 30);

            int count = MenuItems.Count;
            if (count > MaxItemsOnScreen + 1)
                count = MaxItemsOnScreen + 2;

            _descriptionBar.Position = new Point(_offset.X, 38*count + _descriptionBar.Position.Y);
            _descriptionRectangle.Position = new Point(_offset.X, 38*count + _descriptionRectangle.Position.Y);
            _descriptionText.Position = new Point(_offset.X + 8, 38*count + _descriptionText.Position.Y);
        }

        /// <summary>
        /// Returns the current width offset.
        /// </summary>
        public int WidthOffset { get; private set; }

        /// <summary>
        /// Change the menu's width. The width is calculated as DefaultWidth + WidthOffset, so a width offset of 10 would enlarge the menu by 10 pixels.
        /// </summary>
        /// <param name="widthOffset">New width offset.</param>
        public void SetMenuWidthOffset(int widthOffset)
        {
            WidthOffset = widthOffset;
            if (_logo != null)
            {
                _logo.Size = new Size(431 + WidthOffset, 107);
            }
            _mainMenu.Items[0].Position = new Point((WidthOffset + _offset.X + 431) / 2, 20 + _offset.Y); // Title
            if (_counterText != null)
            {
                _counterText.Position = new Point(425 + _offset.X + widthOffset, 110 + _offset.Y);
            }
            if (_mainMenu.Items.Count >= 2)
            {
                var tmp = (ResRectangle)_mainMenu.Items[1];
                tmp.Size = new Size(431 + WidthOffset, 37);
            }
            if (_tmpRectangle != null)
            {
                _tmpRectangle.Size = new Size(431 + WidthOffset, 107);
            }
        }
        
        /// <summary>
        /// Enable or disable all controls but the necessary to operate a menu.
        /// </summary>
        /// <param name="enable"></param>
        public static void DisEnableControls(bool enable)
        {
            if (enable)
            {
                NativeFunction.Natives.EnableAllControlActions(0);
                return;
            }
            else
            {
                NativeFunction.Natives.DisableAllControlActions(0);
            }

            //Controls we want
            // -Frontend
            // -Mouse
            // -Walk/Move
            // -

            for (int i = 0; i < menuNavigationNeededControls.Length; i++)
            {
                Common.EnableControl(0, menuNavigationNeededControls[i]);
            }
            
            if (IsUsingController)
            {
                for (int i = 0; i < menuNavigationControllerNeededControls.Length; i++)
                {
                    Common.EnableControl(0, menuNavigationControllerNeededControls[i]);
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

        private bool _buttonsEnabled = true;
        /// <summary>
        /// Enable or disable the instructional buttons.
        /// </summary>
        /// <param name="disable"></param>
        public void DisableInstructionalButtons(bool disable)
        {
            _buttonsEnabled = !disable;
        }
            
        /// <summary>
        /// Set the banner to your own Sprite object.
        /// </summary>
        /// <param name="spriteBanner">Sprite object. The position and size does not matter.</param>
        public void SetBannerType(Sprite spriteBanner)
        {
            _logo = spriteBanner;
            _logo.Size = new Size(431 + WidthOffset, 107);
            _logo.Position = new Point(_offset.X, _offset.Y);
        }
                   
        private ResRectangle _tmpRectangle;
        /// <summary>
        ///  Set the banner to your own Rectangle.
        /// </summary>
        /// <param name="rectangle">UIResRectangle object. Position and size does not matter.</param>
        public void SetBannerType(ResRectangle rectangle)
        {
            _logo = null;
            _tmpRectangle = rectangle;
            _tmpRectangle.Position = new Point(_offset.X, _offset.Y);
            _tmpRectangle.Size = new Size(431 + WidthOffset, 107);
        }

        /// <summary>
        /// Set the banner to your own custom texture. Set it to null if you want to restore the banner.
        /// </summary>
        /// <param name="texture">Rage.Texture object</param>
        public void SetBannerType(Texture texture)
        {
            _customBanner = texture;
        }

        /// <summary>
        /// Add an item to the menu.
        /// </summary>
        /// <param name="item">Item object to be added. Can be normal item, checkbox or list item.</param>
        public void AddItem(UIMenuItem item)
        {
            item.Offset = _offset;
            item.Parent = this;
            item.SetVerticalPosition((MenuItems.Count * 25) - 37 + _extraYOffset);
            MenuItems.Add(item);

            RecalculateDescriptionPosition();
        }

        /// <summary>
        /// Add an item to the menu at the specified index.
        /// </summary>
        /// <param name="item">Item object to be added. Can be normal item, checkbox or list item.</param>
        /// <param name="index"></param>
        public void AddItem(UIMenuItem item, int index)
        {
            item.Offset = _offset;
            item.Parent = this;
            MenuItems.Insert(index, item);

            for (int i = index; i < MenuItems.Count; i++) // recalculate items positions
            {
                item.SetVerticalPosition((i * 25) - 37 + _extraYOffset);
            }

            RecalculateDescriptionPosition();
        }

        /// <summary>
        /// Remove an item at index n.
        /// </summary>
        /// <param name="index">Index to remove the item at.</param>
        public void RemoveItemAt(int index)
        {
            if (MenuItems.Count > MaxItemsOnScreen && _maxItem == MenuItems.Count - 1)
            {
                _maxItem--;
                _minItem--;
            }
            MenuItems.RemoveAt(index);
            RecalculateDescriptionPosition();
        }

        /// <summary>
        /// Reset the current selected item to 0. Use this after you add or remove items dynamically.
        /// </summary>
        public void RefreshIndex()
        {
            if (MenuItems.Count == 0)
            {
                _activeItem = 1000;
                _maxItem = MaxItemsOnScreen;
                _minItem = 0;
                return;
            }

            for (int i = 0; i < MenuItems.Count; i++)
                MenuItems[i].Selected = false;
            
            _activeItem = 1000 - (1000 % MenuItems.Count);
            _maxItem = MaxItemsOnScreen;
            _minItem = 0;
        }

        /// <summary>
        /// Remove all items from the menu.
        /// </summary>
        public void Clear()
        {
            MenuItems.Clear();
            RecalculateDescriptionPosition();
        }
        
        /// <summary>
        /// Draw your custom banner.
        /// </summary>
        /// <param name="e">Rage.GraphicsEventArgs to draw on.</param>
        [Obsolete("UIMenu.DrawBanner(GraphicsEventArgs) will be removed soon, use UIMenu.DrawBanner(Graphics) instead.")]
        public void DrawBanner(GraphicsEventArgs e)
        {
            if (!Visible || _customBanner == null) return;
            var origRes = Game.Resolution;
            float aspectRaidou = origRes.Width / (float)origRes.Height;

            Point bannerPos = new Point(_offset.X + safezoneOffset.X, _offset.Y + safezoneOffset.Y);
            Size bannerSize = new Size(431 + WidthOffset, 107);

            PointF pos = new PointF(bannerPos.X / (1080 * aspectRaidou), bannerPos.Y / 1080f);
            SizeF siz = new SizeF(bannerSize.Width / (1080 * aspectRaidou), bannerSize.Height / 1080f);

            //Bug: funky positionment on windowed games + max resolution.
            e.Graphics.DrawTexture(_customBanner, pos.X * Game.Resolution.Width, pos.Y * Game.Resolution.Height, siz.Width * Game.Resolution.Width, siz.Height * Game.Resolution.Height);
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
            if (!Visible || _customBanner == null) return;
            var origRes = Game.Resolution;
            float aspectRaidou = origRes.Width / (float)origRes.Height;

            Point bannerPos = new Point(_offset.X + safezoneOffset.X, _offset.Y + safezoneOffset.Y);
            Size bannerSize = new Size(431 + WidthOffset, 107);

            PointF pos = new PointF(bannerPos.X / (1080 * aspectRaidou), bannerPos.Y / 1080f);
            SizeF siz = new SizeF(bannerSize.Width / (1080 * aspectRaidou), bannerSize.Height / 1080f);

            //Bug: funky positionment on windowed games + max resolution.
            g.DrawTexture(_customBanner, pos.X * Game.Resolution.Width, pos.Y * Game.Resolution.Height, siz.Width * Game.Resolution.Width, siz.Height * Game.Resolution.Height);
        }

        /// <summary>
        /// Draw the menu and all of it's components.
        /// </summary>
        public void Draw()
        {
            if (!Visible) return;


            if (_justOpened)
            {
                if (_logo != null && !_logo.IsTextureDictionaryLoaded)
                    _logo.LoadTextureDictionary();
                if (!_background.IsTextureDictionaryLoaded)
                    _background.LoadTextureDictionary();
                if (!_descriptionRectangle.IsTextureDictionaryLoaded)
                    _descriptionRectangle.LoadTextureDictionary();
                if (!_upAndDownSprite.IsTextureDictionaryLoaded)
                    _upAndDownSprite.LoadTextureDictionary();
            }

            if (ControlDisablingEnabled)
                DisEnableControls(false);

            if (AllowCameraMovement && ControlDisablingEnabled)     
                EnableCameraMovement();

            if(_buttonsEnabled)
                instructionalButtons.Draw();

            safezoneOffset = GetSafezoneBounds();
            if (ScaleWithSafezone)
            {
                NativeFunction.CallByHash<uint>(0xb8a850f20a067eb6, 76, 84);           // Safezone
                NativeFunction.CallByHash<uint>(0xf5a2c681787e579d, 0f, 0f, 0f, 0f);   // stuff
            }
            else
            {
                safezoneOffset = new Point(0, 0);
            }

            if (_customBanner == null)
            {
                if (_logo != null)
                    _logo.Draw();
                else
                {
                    _tmpRectangle.Draw();
                }
            }

            _mainMenu.Draw();
            if (MenuItems.Count == 0)
            {
                if (ScaleWithSafezone)
                    NativeFunction.CallByHash<uint>(0xe3a3db414a373dab); // Safezone end
                return;
            }

            _background.Size = MenuItems.Count > MaxItemsOnScreen + 1 ? new Size(431 + WidthOffset, 38 * (MaxItemsOnScreen + 1)) : new Size(431 + WidthOffset, 38 * MenuItems.Count);
            _background.Draw();
            MenuItems[_activeItem % (MenuItems.Count)].Selected = true;
            if (!String.IsNullOrWhiteSpace(MenuItems[_activeItem%(MenuItems.Count)].Description))
            {
                RecalculateDescriptionPosition();
                string descCaption = MenuItems[_activeItem % (MenuItems.Count)].Description;
                if (FormatDescriptions)
                    _descriptionText.Caption = FormatDescription(descCaption);
                else
                    _descriptionText.Caption = descCaption;
                int numLines = _descriptionText.Caption.Split('\n').Length;
                _descriptionRectangle.Size = new Size(431 + WidthOffset, (numLines * 25) + 15);

                _descriptionBar.Draw();
                _descriptionRectangle.Draw();
                _descriptionText.Draw();
            }

            if (MenuItems.Count <= MaxItemsOnScreen + 1)
            {
                int count = 0;
                foreach (var item in MenuItems)
                {
                    item.SetVerticalPosition(count * 38 - 37 + _extraYOffset);
                    item.Draw();
                    count++;
                }
                if (_counterText != null && CounterOverride != null)
                {
                    _counterText.Caption = CounterPretext + CounterOverride;
                    _counterText.Draw();
                }
            }
            else
            {
                int count = 0;
                for (int index = _minItem; index <= _maxItem; index++)
                {
                    var item = MenuItems[index];
                    item.SetVerticalPosition(count * 38 - 37 + _extraYOffset);
                    item.Draw();
                    count++;
                }
                _extraRectangleUp.Size = new Size(431 + WidthOffset, 18);
                _extraRectangleDown.Size = new Size(431 + WidthOffset, 18);
                _upAndDownSprite.Position = new Point(190 + _offset.X + (WidthOffset/2),
                    147 + 37*(MaxItemsOnScreen + 1) + _offset.Y - 37 + _extraYOffset);

                _extraRectangleUp.Draw();
                _extraRectangleDown.Draw();
                _upAndDownSprite.Draw();
                if (_counterText != null)
                {
                    if (CounterOverride == null)
                    {
                        string cap = (CurrentSelection + 1) + " / " + MenuItems.Count;
                        _counterText.Caption = CounterPretext + cap;
                    }
                    else
                    {
                        _counterText.Caption = CounterPretext + CounterOverride;
                    }
                    _counterText.Draw();
                }
            }

            if (ScaleWithSafezone)
                NativeFunction.CallByHash<uint>(0xe3a3db414a373dab); // Safezone end
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
        public static bool IsMouseInBounds(Point topLeft, Size boxSize)
        {
            var res = GetScreenResolutionMantainRatio();

            int mouseX = Convert.ToInt32(Math.Round(Common.GetControlNormal(0, GameControl.CursorX) * res.Width));
            int mouseY = Convert.ToInt32(Math.Round(Common.GetControlNormal(0, GameControl.CursorY) * res.Height));

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
        public int IsMouseInListItemArrows(UIMenuListItem item, Point topLeft, Point safezone) // TODO: Ability to scroll left and right
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
            float wmp = ratio*hmp;
            
            return new Point(Convert.ToInt32(Math.Round(g*wmp)), Convert.ToInt32(Math.Round(g*hmp)));
        }

        /// <summary>
        /// Go up the menu if the number of items is more than maximum items on screen.
        /// </summary>
        public void GoUpOverflow()
        {
            if (MenuItems.Count <= MaxItemsOnScreen + 1) return;
            if (_activeItem % MenuItems.Count <= _minItem)
            {
                if (_activeItem % MenuItems.Count == 0)
                {
                    _minItem = MenuItems.Count - MaxItemsOnScreen - 1;
                    _maxItem = MenuItems.Count - 1;
                    MenuItems[_activeItem % (MenuItems.Count)].Selected = false;
                    _activeItem = 1000 - (1000 % MenuItems.Count);
                    _activeItem += MenuItems.Count - 1;
                    MenuItems[_activeItem % (MenuItems.Count)].Selected = true;
                }
                else
                {
                    _minItem--;
                    _maxItem--;
                    MenuItems[_activeItem % (MenuItems.Count)].Selected = false;
                    _activeItem--;
                    MenuItems[_activeItem % (MenuItems.Count)].Selected = true;
                }
            }
            else
            {
                MenuItems[_activeItem % (MenuItems.Count)].Selected = false;
                _activeItem--;
                MenuItems[_activeItem % (MenuItems.Count)].Selected = true;
            }
            Common.PlaySound(AUDIO_UPDOWN, AUDIO_LIBRARY);
            IndexChange(CurrentSelection);
        }

        /// <summary>
        /// Go up the menu if the number of items is less than or equal to the maximum items on screen.
        /// </summary>
        public void GoUp()
        {
            if (MenuItems.Count > MaxItemsOnScreen + 1) return;
            MenuItems[_activeItem % (MenuItems.Count)].Selected = false;
            _activeItem--;
            MenuItems[_activeItem % (MenuItems.Count)].Selected = true;
            Common.PlaySound(AUDIO_UPDOWN, AUDIO_LIBRARY);
            IndexChange(CurrentSelection);
        }

        /// <summary>
        /// Go down the menu if the number of items is more than maximum items on screen.
        /// </summary>
        public void GoDownOverflow()
        {
            if (MenuItems.Count <= MaxItemsOnScreen + 1) return;
            if (_activeItem % MenuItems.Count >= _maxItem)
            {
                if (_activeItem % MenuItems.Count == MenuItems.Count - 1)
                {
                    _minItem = 0;
                    _maxItem = MaxItemsOnScreen;
                    MenuItems[_activeItem % (MenuItems.Count)].Selected = false;
                    _activeItem = 1000 - (1000 % MenuItems.Count);
                    MenuItems[_activeItem % (MenuItems.Count)].Selected = true;
                }
                else
                {
                    _minItem++;
                    _maxItem++;
                    MenuItems[_activeItem % (MenuItems.Count)].Selected = false;
                    _activeItem++;
                    MenuItems[_activeItem % (MenuItems.Count)].Selected = true;
                }
            }
            else
            {
                MenuItems[_activeItem % (MenuItems.Count)].Selected = false;
                _activeItem++;
                MenuItems[_activeItem % (MenuItems.Count)].Selected = true;
            }
            Common.PlaySound(AUDIO_UPDOWN, AUDIO_LIBRARY);
            IndexChange(CurrentSelection);
        }

        /// <summary>
        /// Go up the menu if the number of items is less than or equal to the maximum items on screen.
        /// </summary>
        public void GoDown()
        {
            if (MenuItems.Count > MaxItemsOnScreen + 1) return;
            MenuItems[_activeItem % (MenuItems.Count)].Selected = false;
            _activeItem++;
            MenuItems[_activeItem % (MenuItems.Count)].Selected = true;
            Common.PlaySound(AUDIO_UPDOWN, AUDIO_LIBRARY);
            IndexChange(CurrentSelection);
        }

        /// <summary>
        /// Go left on a MenuListItem.
        /// </summary>
        public void GoLeft()
        {
            if (!(MenuItems[CurrentSelection] is UIMenuListItem)) return;
            var it = (UIMenuListItem)MenuItems[CurrentSelection];
            if ((it.Collection == null ? it.Items.Count : it.Collection.Count) == 0) return;
            it.Index--;
            Common.PlaySound(AUDIO_LEFTRIGHT, AUDIO_LIBRARY);
            ListChange(it, it.Index);
            it.ListChangedTrigger(it.Index);
        }

        /// <summary>
        /// Go right on a MenuListItem.
        /// </summary>
        public void GoRight()
        {
            if (!(MenuItems[CurrentSelection] is UIMenuListItem)) return;
            var it = (UIMenuListItem)MenuItems[CurrentSelection];
            if ((it.Collection == null ? it.Items.Count : it.Collection.Count) == 0) return;
            it.Index++;
            Common.PlaySound(AUDIO_LEFTRIGHT, AUDIO_LIBRARY);
            ListChange(it, it.Index);
            it.ListChangedTrigger(it.Index);
        }

        /// <summary>
        /// Activate the current selected item.
        /// </summary>
        public void SelectItem()
        {
            if (!MenuItems[CurrentSelection].Enabled)
            {
                Common.PlaySound(AUDIO_ERROR, AUDIO_LIBRARY);
                return;
            }
            if (MenuItems[CurrentSelection] is UIMenuCheckboxItem)
            {
                var it = (UIMenuCheckboxItem)MenuItems[CurrentSelection];
                it.Checked = !it.Checked;
                Common.PlaySound(AUDIO_SELECT, AUDIO_LIBRARY);
                CheckboxChange(it, it.Checked);
                it.CheckboxEventTrigger();
            }
            else
            {
                Common.PlaySound(AUDIO_SELECT, AUDIO_LIBRARY);
                ItemSelect(MenuItems[CurrentSelection], CurrentSelection);
                MenuItems[CurrentSelection].ItemActivate(this);
                if (!Children.ContainsKey(MenuItems[CurrentSelection])) return;
                Visible = false;
                Children[MenuItems[CurrentSelection]].Visible = true;
                MenuChangeEv(Children[MenuItems[CurrentSelection]], true);
            }
        }

        /// <summary>
        /// Close or go back in a menu chain.
        /// </summary>
        public void GoBack()
        {
            Common.PlaySound(AUDIO_BACK, AUDIO_LIBRARY);
            Visible = false;
            if (ParentMenu != null)
            {
                var tmp = Cursor.Position;
                ParentMenu.Visible = true;
                MenuChangeEv(ParentMenu, false);
                if(ResetCursorOnOpen)
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

        private Point safezoneOffset;
        /// <summary>
        /// Process the mouse's position and check if it's hovering over any Element. Call this in Game.FrameRender or in a loop.
        /// </summary>
        public void ProcessMouse()
        {
            if (!Visible || _justOpened || MenuItems.Count == 0 || IsUsingController || !MouseControlsEnabled)
            {
                Common.EnableControl(0, GameControl.LookUpDown);
                Common.EnableControl(0, GameControl.LookLeftRight);
                Common.EnableControl(0, GameControl.Aim);
                Common.EnableControl(0, GameControl.Attack);
                MenuItems.Where(i => i.Hovered).ToList().ForEach(i => i.Hovered = false);
                return;
            }

            NativeFunction.CallByHash<uint>(0xaae7ce1d63167423);  // _SHOW_CURSOR_THIS_FRAME
            int limit = MenuItems.Count - 1;
            int counter = 0;
            if (MenuItems.Count > MaxItemsOnScreen + 1)
                limit = _maxItem;

            if (IsMouseInBounds(new Point(0, 0), new Size(30, 1080)) && MouseEdgeEnabled)
            {
                NativeFunction.CallByName<uint>("SET_GAMEPLAY_CAM_RELATIVE_HEADING", NativeFunction.CallByName<float>("GET_GAMEPLAY_CAM_RELATIVE_HEADING") + 5.0f);
                NativeFunction.CallByHash<uint>(0x8db8cffd58b62552, 6);
            }
            else if (IsMouseInBounds(new Point(Convert.ToInt32(GetScreenResolutionMantainRatio().Width - 30f), 0), new Size(30, 1080)) &&  MouseEdgeEnabled)
            {
                NativeFunction.CallByName<uint>("SET_GAMEPLAY_CAM_RELATIVE_HEADING", NativeFunction.CallByName<float>("GET_GAMEPLAY_CAM_RELATIVE_HEADING") - 5.0f);
                NativeFunction.CallByHash<uint>(0x8db8cffd58b62552, 7);
            }
            else if(MouseEdgeEnabled)
            {
                NativeFunction.CallByHash<uint>(0x8db8cffd58b62552, 1);
            }

            for (int i = _minItem; i <= limit; i++)
            {
                int xpos = _offset.X + safezoneOffset.X;
                int ypos = _offset.Y + 144 - 37 + _extraYOffset + (counter*38) + safezoneOffset.Y;
                int xsize = 431 + WidthOffset;
                const int ysize = 38;
                UIMenuItem uiMenuItem = MenuItems[i];
                if (IsMouseInBounds(new Point(xpos, ypos), new Size(xsize, ysize)))
                {
                    uiMenuItem.Hovered = true;
                    if (Game.IsControlJustPressed(0, GameControl.Attack) || Common.IsDisabledControlJustPressed(0, GameControl.Attack))
                        if (uiMenuItem.Selected && uiMenuItem.Enabled)
                        {
                            if (MenuItems[i] is UIMenuListItem &&
                                IsMouseInListItemArrows((UIMenuListItem) MenuItems[i], new Point(xpos, ypos),
                                    safezoneOffset) > 0)
                            {
                                int res = IsMouseInListItemArrows((UIMenuListItem) MenuItems[i], new Point(xpos, ypos),
                                    safezoneOffset);
                                switch (res)
                                {
                                    case 1:
                                        Common.PlaySound(AUDIO_SELECT, AUDIO_LIBRARY);
                                        MenuItems[i].ItemActivate(this);
                                        ItemSelect(MenuItems[i], i);
                                        break;
                                    case 2:
                                        var it = (UIMenuListItem) MenuItems[i];
                                        if ((it.Collection == null ? it.Items.Count : it.Collection.Count) > 0)
                                        {
                                            it.Index++;
                                            Common.PlaySound(AUDIO_LEFTRIGHT, AUDIO_LIBRARY);
                                            ListChange(it, it.Index);
                                            it.ListChangedTrigger(it.Index);
                                        }
                                        break;
                                }
                            }
                            else
                                SelectItem();
                        }
                        else if(!uiMenuItem.Selected)
                        {
                            CurrentSelection = i;
                            Common.PlaySound(AUDIO_UPDOWN, AUDIO_LIBRARY);
                            IndexChange(CurrentSelection);
                            instructionalButtons.Update();
                        }
                        else if (!uiMenuItem.Enabled && uiMenuItem.Selected)
                        {
                            Common.PlaySound(AUDIO_ERROR, AUDIO_LIBRARY);
                        }
                }
                else
                    uiMenuItem.Hovered = false;
                counter++;
            }
            int extraY = 144 + 38*(MaxItemsOnScreen + 1) + _offset.Y - 37 + _extraYOffset + safezoneOffset.Y;
            int extraX = safezoneOffset.X + _offset.X;
            if (MenuItems.Count <= MaxItemsOnScreen + 1) return;
            if (IsMouseInBounds(new Point(extraX, extraY), new Size(431 + WidthOffset, 18)))
            {
                _extraRectangleUp.Color = Color.FromArgb(255, 30, 30, 30);
                if (Game.IsControlJustPressed(0, GameControl.Attack) || Common.IsDisabledControlJustPressed(0, GameControl.Attack))
                {
                    if (MenuItems.Count > MaxItemsOnScreen + 1)
                        GoUpOverflow();
                    else
                        GoUp();
                }
            }
            else
                _extraRectangleUp.Color = Color.FromArgb(200, 0, 0, 0);
            
            if (IsMouseInBounds(new Point(extraX, extraY+18), new Size(431 + WidthOffset, 18)))
            {
                _extraRectangleDown.Color = Color.FromArgb(255, 30, 30, 30);
                if (Game.IsControlJustPressed(0, GameControl.Attack) || Common.IsDisabledControlJustPressed(0, GameControl.Attack)) // Fixed move down arrow not working v1.1
                {
                    if (MenuItems.Count > MaxItemsOnScreen + 1)
                        GoDownOverflow();
                    else
                        GoDown();
                }
            }
            else
                _extraRectangleDown.Color = Color.FromArgb(200, 0, 0, 0);
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
                if(Game.GameTime <= _holdTime)
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
            else if ((MenuItems[CurrentSelection] is UIMenuListItem) && MenuItems[CurrentSelection].Enabled)
            {
                
                UIMenuListItem it = (UIMenuListItem)MenuItems[CurrentSelection];
                if(it.ScrollingEnabled)
                {                   
                    if (HasControlJustBeenReleaseed(control, key)) { it._holdTime = 0; }
                    if(Game.GameTime <= it._holdTime)
                    {
                        return false;
                    }
                    List<Keys> tmpKeys = new List<Keys>(_keyDictionary[control].Item1);
                    List<Tuple<GameControl, int>> tmpControls = new List<Tuple<GameControl, int>>(_keyDictionary[control].Item2);
                    if (key != Keys.None)
                    {
                        if (tmpKeys.Any(Game.IsKeyDownRightNow))
                        {
                            it._holdTime = Game.GameTime + it.HoldTimeBeforeScroll;
                            return true;
                        }
                    }
                    if (tmpControls.Any(tuple => Game.IsControlPressed(tuple.Item2, tuple.Item1) || Common.IsDisabledControlPressed(tuple.Item2, tuple.Item1)))
                    {
                        it._holdTime = Game.GameTime + it.HoldTimeBeforeScroll;
                        return true;
                    }
                }
                else if(HasControlJustBeenPressed(control, key))
                {
                    return true;
                }
                
            }
            return false;
        }

        /// <summary>
        /// Process control-stroke. Call this in the Game.FrameRender event or in a loop.
        /// </summary>
        public void ProcessControl(Keys key = Keys.None)
        {
            if(!Visible) return;
            if (_justOpened)
            {
                _justOpened = false;
                return;
            }

            if (HasControlJustBeenReleaseed(Common.MenuControls.Back, key))
            {
                GoBack();
            }
            if (MenuItems.Count == 0) return;
            if (IsControlBeingPressed(Common.MenuControls.Up, key))
            {
                if (MenuItems.Count > MaxItemsOnScreen + 1)
                    GoUpOverflow();
                else
                    GoUp();
                instructionalButtons.Update();
            }

            else if (IsControlBeingPressed(Common.MenuControls.Down, key))
            {
                if (MenuItems.Count > MaxItemsOnScreen + 1)
                    GoDownOverflow();
                else
                    GoDown();
                instructionalButtons.Update();
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

        /// <summary>
        /// Formats the input string so it doesn't go out of bounds of the description box.
        /// </summary>
        /// <param name="input">String to format.</param>
        /// <returns></returns>
        private string FormatDescription(string input)
        {
            int maxPixelsPerLine = 425 + WidthOffset;
            int aggregatePixels = 0;
            string output = "";
            string[] words = input.Split(' ');
            foreach (string word in words)
            {
                int offset = StringMeasurer.MeasureString(word);
                aggregatePixels += offset;
                if (aggregatePixels > maxPixelsPerLine)
                {
                    output += "\n" + word + " ";
                    aggregatePixels = offset + StringMeasurer.MeasureString(" ");
                }
                else
                {
                    output += word + " ";
                    aggregatePixels += StringMeasurer.MeasureString(" ");
                }
            }
            return output;
        }
        
        public void AddInstructionalButton(InstructionalButton button)
        {
            instructionalButtons.Buttons.Add(button);
        }

        public void RemoveInstructionalButton(InstructionalButton button)
        {
            instructionalButtons.Buttons.Remove(button);
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
                    UIMenuListItem itemAsList = item as UIMenuListItem;
                    if(itemAsList != null)
                    {
                        itemAsList.Index = 0;
                        continue;
                    }
                }

                if (resetCheckboxes)
                {
                    UIMenuCheckboxItem itemAsCheckbox = item as UIMenuCheckboxItem;
                    if(itemAsCheckbox != null)
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
            Common.EnableControl(0, GameControl.LookLeftRight);
            Common.EnableControl(0, GameControl.LookUpDown);
        }

        /// <summary>
        /// Change whether this menu is visible to the user.
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set
            {   
                _visible = value;
                _justOpened = value;
                instructionalButtons.Update();
                if (ParentMenu != null || !value) return;
                if (!ResetCursorOnOpen) return;
                Cursor.Position = new Point(Screen.PrimaryScreen.Bounds.Width/2, Screen.PrimaryScreen.Bounds.Height/2);
                NativeFunction.CallByHash<uint>(0x8db8cffd58b62552, 1);
            }
        }

        /// <summary>
        /// Returns the current selected item's index.
        /// Change the current selected item to index. Use this after you add or remove items dynamically.
        /// </summary>
        public int CurrentSelection
        {
            get { return _activeItem % MenuItems.Count; }
            set
            {
                MenuItems[_activeItem%(MenuItems.Count)].Selected = false;
                _activeItem = 1000 - (1000 % MenuItems.Count) + value;
                if (CurrentSelection > _maxItem)
                {
                    _maxItem = CurrentSelection;
                    _minItem = CurrentSelection - MaxItemsOnScreen;
                }
                else if (CurrentSelection < _minItem)
                {
                    _maxItem = MaxItemsOnScreen + CurrentSelection;
                    _minItem = CurrentSelection;
                }
            }
        }

        /// <summary>
        /// Returns false if last input was made with mouse and keyboard, true if it was made with a controller.
        /// </summary>
        public static bool IsUsingController => !NativeFunction.CallByHash<bool>(0xa571d46727e2b718, 2);

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

        protected virtual void ListChange(UIMenuListItem sender, int newindex)
        {
            OnListChange?.Invoke(this, sender, newindex);
        }

        protected virtual void ItemSelect(UIMenuItem selecteditem, int index)
        {
            OnItemSelect?.Invoke(this, selecteditem, index);
        }

        protected virtual void CheckboxChange(UIMenuCheckboxItem sender, bool Checked)
        {
            OnCheckboxChange?.Invoke(this, sender, Checked);
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
