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
        public void ProcessKeys()
        {
            foreach (var menu in _menuList.Where(menu => menu.Visible))
            {
                menu.ProcessKeys());
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
            ProcessControl();
            ProcessKeys();  
            ProcessMouse();
            Draw();
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
