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
    public class MenuPool : BaseCollection<UIMenu>
    {
        public bool MouseEdgeEnabled { set { InternalList.ForEach(m => m.MouseEdgeEnabled = value); } }
        public bool ControlDisablingEnabled { set { InternalList.ForEach(m => m.ControlDisablingEnabled = value); } }
        public bool ResetCursorOnOpen { set { InternalList.ForEach(m => m.ResetCursorOnOpen = value); } }
        public bool FormatDescriptions { set { InternalList.ForEach(m => m.FormatDescriptions = value); } }
        public string AUDIO_LIBRARY { set { InternalList.ForEach(m => m.AUDIO_LIBRARY = value); } }
        public string AUDIO_UPDOWN { set { InternalList.ForEach(m => m.AUDIO_UPDOWN = value); } }
        public string AUDIO_SELECT { set { InternalList.ForEach(m => m.AUDIO_SELECT = value); } }
        public string AUDIO_BACK { set { InternalList.ForEach(m => m.AUDIO_BACK = value); } }
        public string AUDIO_ERROR { set { InternalList.ForEach(m => m.AUDIO_ERROR = value); } }
        public int WidthOffset { set { InternalList.ForEach(m => m.SetMenuWidthOffset(value)); } }
        public string CounterPretext { set { InternalList.ForEach(m => m.CounterPretext = value); } }
        public bool DisableInstructionalButtons { set { InternalList.ForEach(m => m.DisableInstructionalButtons(value)); } }

        // added for backwards compatibility, BaseCollection.Remove returns a bool and this one doesn't return anything
        public new void Remove(UIMenu menu)
        {
            base.Remove(menu);
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
            foreach (UIMenu menu in InternalList) menu.RefreshIndex();
        }

        /// <summary>
        /// Returns all of your menus.
        /// </summary>
        /// <returns></returns>
        public List<UIMenu> ToList()
        {
            return InternalList;
        }

        /// <summary>
        /// Processes all of your visible menus' controls.
        /// </summary>
        public void ProcessControl()
        {
            foreach (UIMenu menu in InternalList)
            {
                if (menu.Visible)
                {
                    menu.ProcessControl();
                }
            }
        }


        /// <summary>
        /// Processes all of your visible menus' keys.
        /// </summary>
        /// <param name="key"></param>
        public void ProcessKey(Keys key)
        {
            foreach (UIMenu menu in InternalList)
            {
                if (menu.Visible)
                {
                    menu.ProcessKey(key);
                }
            }
        }


        /// <summary>
        /// Processes all of your visible menus' mouses.
        /// </summary>
        public void ProcessMouse()
        {
            foreach (UIMenu menu in InternalList)
            {
                if (menu.Visible)
                {
                    menu.ProcessMouse();
                }
            }
        }
        

        /// <summary>
        /// Draws all visible menus.
        /// </summary>
        public void Draw()
        {
            foreach (UIMenu menu in InternalList)
            {
                if (menu.Visible)
                {
                    menu.Draw();
                }
            }
        }


        /// <summary>
        /// Checks if any menu is currently visible.
        /// </summary>
        /// <returns>true if at least one menu is visible, false if not.</returns>
        public bool IsAnyMenuOpen()
        {
            return InternalList.Any(menu => menu.Visible);
        }


        /// <summary>
        /// Process all of your menus' functions. Call this in a tick event.
        /// </summary>
        public void ProcessMenus()
        {
            ICollection<Keys> pressedKeys = Common.GetPressedKeys();
            bool checkPressedKeys = pressedKeys != null;

            foreach (UIMenu menu in InternalList)
            {
                if (menu.Visible)
                {
                    menu.ProcessControl();
                    if (checkPressedKeys)
                    {
                        foreach (Keys key in pressedKeys)
                        {
                            menu.ProcessKey(key);
                        }
                    }
                    menu.ProcessMouse();
                    menu.Draw();
                }
            }
        }

        /// <summary>
        /// Draw all of your menus' custom banners.
        /// </summary>
        /// <param name="canvas">Canvas to draw on.</param>
        [System.Obsolete("MenuPool.DrawBanners(GraphicsEventArgs) will be removed soon, use MenuPool.DrawBanners(Graphics) instead")]
        public void DrawBanners(GraphicsEventArgs canvas)
        {
            InternalList.ForEach(menu => menu.DrawBanner(canvas));
        }

        /// <summary>
        /// Draw all of your menus' custom banners.
        /// </summary>
        /// <param name="g">The <see cref="Rage.Graphics"/> to draw on.</param>
        public void DrawBanners(Graphics g)
        {
            InternalList.ForEach(menu => menu.DrawBanner(g));
        }

        /// <summary>
        /// Closes all of your menus.
        /// </summary>
        public void CloseAllMenus()
        {
            foreach (UIMenu menu in InternalList)
            {
                if (menu.Visible)
                {
                    menu.Visible = false;
                }
            }
        }

        /// <summary>
        /// Sets the index of all lists to 0 and unchecks all the checkboxes from your menus. 
        /// </summary>
        /// <param name="resetLists">If true the index of all lists will be set to 0.</param>
        /// <param name="resetCheckboxes">If true all the checkboxes will be unchecked.</param>
        public void ResetMenus(bool resetLists, bool resetCheckboxes)
        {
            InternalList.ForEach(m => m.Reset(resetLists, resetCheckboxes));
        }

        public void SetBannerType(Sprite bannerType)
        {
            InternalList.ForEach(m => m.SetBannerType(bannerType));
        }

        public void SetBannerType(ResRectangle bannerType)
        {
            InternalList.ForEach(m => m.SetBannerType(bannerType));
        }

        public void SetBannerType(Texture banner)
        {
            InternalList.ForEach(m => m.SetBannerType(banner));
        }

        public void SetKey(Common.MenuControls menuControl, GameControl control)
        {
            InternalList.ForEach(m => m.SetKey(menuControl, control));
        }

        public void SetKey(Common.MenuControls menuControl, GameControl control, int controllerIndex)
        {
            InternalList.ForEach(m => m.SetKey(menuControl, control, controllerIndex));
        }

        public void SetKey(Common.MenuControls menuControl, Keys control)
        {
            InternalList.ForEach(m => m.SetKey(menuControl, control));
        }

        public void ResetKey(Common.MenuControls menuControl)
        {
            InternalList.ForEach(m => m.ResetKey(menuControl));
        }
    }
}
