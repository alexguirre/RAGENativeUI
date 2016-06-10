using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using RAGENativeUI.Elements;
using Rage;

namespace RAGENativeUI
{
    /// <summary>
    /// Helper class that handles all of your Menus. After instatiating it, you will have to add your menu by using the Add method.
    /// </summary>
    public class MenuPool
    {
        public bool MouseEdgeEnabled { set { _menuList.ForEach(m => m.MouseEdgeEnabled = value); } }

        public bool ControlDisablingEnabled { set { _menuList.ForEach(m => m.ControlDisablingEnabled = value); } }

        public bool ResetCursorOnOpen { set { _menuList.ForEach(m => m.ResetCursorOnOpen = value); } }

        public bool FormatDescriptions { set { _menuList.ForEach(m => m.FormatDescriptions = value); } }

        public string AUDIO_LIBRARY { set { _menuList.ForEach(m => m.AUDIO_LIBRARY = value); } }

        public string AUDIO_UPDOWN { set { _menuList.ForEach(m => m.AUDIO_UPDOWN = value); } }

        public string AUDIO_SELECT { set { _menuList.ForEach(m => m.AUDIO_SELECT = value); } }

        public string AUDIO_BACK { set { _menuList.ForEach(m => m.AUDIO_BACK = value); } }

        public string AUDIO_ERROR { set { _menuList.ForEach(m => m.AUDIO_ERROR = value); } }

        public int WidthOffset { set { _menuList.ForEach(m => m.SetMenuWidthOffset(value)); } }

        public string CounterPretext { set { _menuList.ForEach(m => m.CounterPretext = value); } }
        
        public bool DisableInstructionalButtons { set { _menuList.ForEach(m => m.DisableInstructionalButtons(value)); } }

        private readonly List<UIMenu> _menuList = new List<UIMenu>();


        /// <summary>
        /// Add your menu to the menu pool.
        /// </summary>
        /// <param name="menu"></param>
        public void Add(UIMenu menu)
        {
            _menuList.Add(menu);
            Logger.Log(System.String.Format("Added UIMenu(T::{0}, S::{1}) to MenuPool", menu.Title, menu.Subtitle));
        }

        /// <summary>
        /// Removes the specified menu from the pool
        /// </summary>
        /// <param name="menu"></param>
        public void Remove(UIMenu menu)
        {
            _menuList.Remove(menu);
            Logger.Log(System.String.Format("Removed UIMenu(T::{0}, S::{1}) from MenuPool", menu.Title, menu.Subtitle));
        }

        /// <summary>
        /// Create and add a submenu to the menu pool.
        /// Adds an item with the given text to the menu, creates a corresponding submenu, and binds the submenu to the item.
        /// The submenu inherits its title from the menu, and its subtitle from the item text.
        /// </summary>
        /// <param name="menu">The parent menu to which the submenu must be added.</param>
        /// <param name="text">The name of the submenu.</param>
        /// <returns>The newly created submenu.</returns>
        public UIMenu AddSubMenu(UIMenu menu, string text)
        {
            var item = new UIMenuItem(text);
            menu.AddItem(item);
            var submenu = new UIMenu(menu.Title.Caption, text);
            this.Add(submenu);
            menu.BindMenuToItem(submenu, item);
            return submenu;
        }


        /// <summary>
        /// Refresh index of every menu in the pool.
        /// Use this after you have finished constructing the entire menu pool.
        /// </summary>
        public void RefreshIndex()
        {
            foreach (UIMenu menu in _menuList) menu.RefreshIndex();
        }

        /// <summary>
        /// Returns all of your menus.
        /// </summary>
        /// <returns></returns>
        public List<UIMenu> ToList()
        {
            return _menuList;
        }

        /// <summary>
        /// Processes all of your visible menus' controls.
        /// </summary>
        public void ProcessControl()
        {
            foreach (var menu in _menuList.Where(menu => menu.Visible))
            {
                menu.ProcessControl();
            }
        }


        /// <summary>
        /// Processes all of your visible menus' keys.
        /// </summary>
        /// <param name="key"></param>
        public void ProcessKey(Keys key)
        {
            foreach (var menu in _menuList.Where(menu => menu.Visible))
            {
                menu.ProcessKey(key);
            }
        }


        /// <summary>
        /// Processes all of your visible menus' mouses.
        /// </summary>
        public void ProcessMouse()
        {
            foreach (var menu in _menuList.Where(menu => menu.Visible))
            {
                menu.ProcessMouse();
            }
        }
        

        /// <summary>
        /// Draws all visible menus.
        /// </summary>
        public void Draw()
        {
            foreach (var menu in _menuList.Where(menu => menu.Visible))
            {
                menu.Draw();
            }
        }


        /// <summary>
        /// Checks if any menu is currently visible.
        /// </summary>
        /// <returns>true if at least one menu is visible, false if not.</returns>
        public bool IsAnyMenuOpen()
        {
            return _menuList.Any(menu => menu.Visible);
        }


        /// <summary>
        /// Process all of your menus' functions. Call this in a tick event.
        /// </summary>
        public void ProcessMenus()
        {
            IList<Keys> pressedKeys = Common.GetPressedKeys();
            foreach (var menu in _menuList.Where(menu => menu.Visible))
            {
                Logger.Log(System.String.Format("Processing UIMenu(T::{0}, S::{1}) from MenuPool", menu.Title, menu.Subtitle));
                menu.ProcessControl();
                foreach (Keys key in pressedKeys)
                {
                    menu.ProcessKey(key);
                }
                menu.ProcessMouse();
                menu.Draw();
            }
        }

        /// <summary>
        /// Draw all of your menus' custom banners.
        /// </summary>
        /// <param name="canvas">Canvas to draw on.</param>
        public void DrawBanners(GraphicsEventArgs canvas)
        {
            _menuList.ForEach(menu => menu.DrawBanner(canvas));
        }

        /// <summary>
        /// Closes all of your menus.
        /// </summary>
        public void CloseAllMenus()
        {
            foreach (var menu in _menuList.Where(menu => menu.Visible))
            {
                menu.Visible = false;
            }
        }

        /// <summary>
        /// Sets the index of all lists to 0 and unchecks all the checkboxes from your menus. 
        /// </summary>
        /// <param name="resetLists">If true the index of all lists will be set to 0.</param>
        /// <param name="resetCheckboxes">If true all the checkboxes will be unchecked.</param>
        public void ResetMenus(bool resetLists, bool resetCheckboxes)
        {
            _menuList.ForEach(m => m.Reset(resetLists, resetCheckboxes));
        }

        public void SetBannerType(Sprite bannerType)
        {
            _menuList.ForEach(m => m.SetBannerType(bannerType));
        }

        public void SetBannerType(ResRectangle bannerType)
        {
            _menuList.ForEach(m => m.SetBannerType(bannerType));
        }

        public void SetBannerType(Texture banner)
        {
            _menuList.ForEach(m => m.SetBannerType(banner));
        }

        public void SetKey(Common.MenuControls menuControl, GameControl control)
        {
            _menuList.ForEach(m => m.SetKey(menuControl, control));
        }

        public void SetKey(Common.MenuControls menuControl, GameControl control, int controllerIndex)
        {
            _menuList.ForEach(m => m.SetKey(menuControl, control, controllerIndex));
        }

        public void SetKey(Common.MenuControls menuControl, Keys control)
        {
            _menuList.ForEach(m => m.SetKey(menuControl, control));
        }

        public void ResetKey(Common.MenuControls menuControl)
        {
            _menuList.ForEach(m => m.ResetKey(menuControl));
        }
    }
}

