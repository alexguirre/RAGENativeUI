using System;
using System.Collections.Generic;
using System.Linq;

namespace RAGENativeUI.Elements
{
    /// <summary>
    /// List item that holds an <see cref="UIMenu"/> list and allows to easily switch between those menus.
    /// </summary>
    /// <remarks>
    /// All the <see cref="UIMenu"/>s must have the same instance of this <see cref="UIMenuSwitchMenusItem"/> and
    /// is recommended to use the property <see cref="UIMenuSwitchMenusItem.CurrentMenu"/> to make the menu visible without issues.
    /// </remarks>
    public class UIMenuSwitchMenusItem : UIMenuListItem
    {
        private List<UIMenu> _menus;

        private UIMenu _currentMenu;
        /// <summary>
        /// The currently selected menu. Use this to make the proper menu visible when using this item.
        /// </summary>
        public UIMenu CurrentMenu
        {
            get
            {
                return _currentMenu;
            }
            set
            {
                if (value == _currentMenu)
                    return;

                if (!_menus.Contains(value))
                    throw new ArgumentException("The item doesn't contain the specified UIMenu", "value");

                Index = MenuToIndex(value);
            }
        }

        /// <summary>
        /// Returns the current selected index.
        /// </summary>
        public override int Index
        {
            get { return _index % _items.Count; }
            set
            {
                _index = 100000 - (100000 % _items.Count) + value;
                if(_menus != null)
                {
                    UIMenu newMenu = IndexToMenu(_index % _items.Count);

                    if (newMenu == _currentMenu)
                        return;

                    if (!newMenu.MenuItems.Contains(this))
                        throw new ArgumentException("The specified UIMenu doesn't contain this item", "value");

                    _currentMenu.Visible = false;
                    _currentMenu = newMenu;
                    _currentMenu.Visible = true;

                    _currentMenu.CurrentSelection = _currentMenu.MenuItems.IndexOf(this);
                }
            }
        }

        /// <summary>
        /// List item, with left/right arrows that switches the current menu depending on the current <see cref="UIMenu"/> item.
        /// Uses the menus titles as the names in the list.
        /// </summary>
        /// <param name="text">Item label.</param>
        /// <param name="menus">List that contains your <see cref="UIMenu"/>s.</param>
        /// <param name="index">Index in the list. If unsure user 0.</param>
        public UIMenuSwitchMenusItem(string text, List<UIMenu> menus, int index)
            : this(text, menus, new List<string>(menus.Select(m => m.Title.Caption)), index, "")
        {
        }

        /// <summary>
        /// List item, with left/right arrows that switches the current menu depending on the current <see cref="UIMenu"/> item.
        /// Uses the menus titles as the names in the list.
        /// </summary>
        /// <param name="text">Item label.</param>
        /// <param name="menus">List that contains your <see cref="UIMenu"/>s.</param>
        /// <param name="index">Index in the list. If unsure user 0.</param>
        /// <param name="description">Description for this item.</param>
        public UIMenuSwitchMenusItem(string text, List<UIMenu> menus, int index, string description) 
            : this(text, menus, new List<string>(menus.Select(m => m.Title.Caption)), index, description)
        {
        }

        /// <summary>
        /// List item, with left/right arrows that switches the current menu depending on the current <see cref="UIMenu"/> item.
        /// The menus list and the menus names list must have the same items count.
        /// </summary>
        /// <param name="text">Item label.</param>
        /// <param name="menus">List that contains your <see cref="UIMenu"/>s.</param>
        /// <param name="menusNames">List that contains a name for each <see cref="UIMenu"/> in the same order</param>
        /// <param name="index">Index in the list. If unsure user 0.</param>
        public UIMenuSwitchMenusItem(string text, List<UIMenu> menus, List<string> menusNames, int index)
            : this(text, menus, menusNames, index, "")
        {
        }

        /// <summary>
        /// List item, with left/right arrows that switches the current menu depending on the current <see cref="UIMenu"/> item.
        /// The menus list and the menus names list must have the same items count.
        /// </summary>
        /// <param name="text">Item label.</param>
        /// <param name="menus">List that contains your <see cref="UIMenu"/>s.</param>
        /// <param name="menusNames">List that contains a name for each <see cref="UIMenu"/> in the same order</param>
        /// <param name="index">Index in the list. If unsure user 0.</param>
        /// <param name="description">Description for this item.</param>
        public UIMenuSwitchMenusItem(string text, List<UIMenu> menus, List<string> menusNames, int index, string description)
            : base(text, new List<dynamic>(menusNames), index, description)
        {
            if (menus.Count != menusNames.Count)
                throw new ArgumentException("The UIMenu list and the names strings list must have the same items count", "menus, menusNames");

            _menus = new List<UIMenu>(menus);
            _currentMenu = IndexToMenu(index);
        }


        /// <summary>
        /// Find a menu in the list and return it's index.
        /// </summary>
        /// <param name="menu">Menu to search for.</param>
        /// <returns>Item index.</returns>
        public virtual int MenuToIndex(UIMenu menu)
        {
            return _menus.FindIndex(m => m == menu);
        }


        /// <summary>
        /// Find a menu by it's index and return the menu.
        /// </summary>
        /// <param name="index">Menu's index.</param>
        /// <returns>Menu</returns>
        public virtual UIMenu IndexToMenu(int index)
        {
            return _menus[index];
        }
    }
}
