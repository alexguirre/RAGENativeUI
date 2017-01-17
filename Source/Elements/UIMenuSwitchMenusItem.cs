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
        [Obsolete("UIMenuSwitchMenusItem._menus will be removed soon, use UIMenuSwitchMenusItem.Collection instead.")]
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

                if (Collection == null ? !_menus.Contains(value) : !Collection.Contains(value))
                    throw new ArgumentException("The item doesn't contain the specified UIMenu", "value");

                Index = MenuToIndex(value);
            }
        }

        /// <summary>
        /// Returns the current selected index.
        /// </summary>
        public override int Index
        {
            get { return _index % (Collection == null ? _items.Count : Collection.Count); }
            set
            {
                _index = 100000 - (100000 % (Collection == null ? _items.Count : Collection.Count)) + value;
                if((Collection == null ? _menus != null : true))
                {
                    UIMenu newMenu = IndexToMenu(_index % (Collection == null ? _items.Count : Collection.Count));

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
        [Obsolete("This constructor overload will be removed soon, use one of the other overloads.")]
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
        [Obsolete("This constructor overload will be removed soon, use one of the other overloads.")]
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
        [Obsolete("This constructor overload will be removed soon, use one of the other overloads.")]
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
        [Obsolete("This constructor overload will be removed soon, use one of the other overloads.")]
        public UIMenuSwitchMenusItem(string text, List<UIMenu> menus, List<string> menusNames, int index, string description)
            : base(text, new List<dynamic>(menusNames), index, description)
        {
            if (menus.Count != menusNames.Count)
                throw new ArgumentException("The UIMenu list and the names strings list must have the same items count", "menus, menusNames");

            _menus = new List<UIMenu>(menus);
            _currentMenu = IndexToMenu(index);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIMenuSwitchMenusItem"/> class a list menu item, with left/right arrows that switches the current menu depending on the current <see cref="UIMenu"/> item,
        /// from a collection of <see cref="IDisplayItem"/>s.
        /// <para>
        /// All <see cref="IDisplayItem.Value"/>s need to be <see cref="UIMenu"/>s.
        /// </para>
        /// </summary>
        /// <param name="text">The item label.</param>
        /// <param name="description">The description for this item.</param>
        /// <param name="menusDisplayItems">The collection of <see cref="IDisplayItem"/>s with <see cref="UIMenu"/>s as <see cref="IDisplayItem.Value"/>s.</param>
        /// <exception cref="ArgumentException">Thrown if any <see cref="IDisplayItem.Value"/> isn't a <see cref="UIMenu"/></exception>
        public UIMenuSwitchMenusItem(string text, string description, IEnumerable<IDisplayItem> menusDisplayItems)
            : base(text, description, menusDisplayItems)
        {
            if (menusDisplayItems.Any(i => !(i.Value is UIMenu)))
                throw new ArgumentException($"All {nameof(IDisplayItem.Value)}s need to be {nameof(UIMenu)}s", nameof(menusDisplayItems));

            _currentMenu = IndexToMenu(0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIMenuSwitchMenusItem"/> class a list menu item, with left/right arrows that switches the current menu depending on the current <see cref="UIMenu"/> item,
        /// from a collection of <see cref="IDisplayItem"/>s.
        /// <para>
        /// All <see cref="IDisplayItem.Value"/>s need to be <see cref="UIMenu"/>s.
        /// </para>
        /// </summary>
        /// <param name="text">The item label.</param>
        /// <param name="description">The description for this item.</param>
        /// <param name="menusDisplayItems">The collection of <see cref="IDisplayItem"/>s with <see cref="UIMenu"/>s as <see cref="IDisplayItem.Value"/>s.</param>
        /// <exception cref="ArgumentException">Thrown if any <see cref="IDisplayItem.Value"/> isn't a <see cref="UIMenu"/></exception>
        public UIMenuSwitchMenusItem(string text, string description, params IDisplayItem[] menusDisplayItems)
            : this(text, description, (IEnumerable<IDisplayItem>)menusDisplayItems)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIMenuSwitchMenusItem"/> class a list menu item, with left/right arrows that switches the current menu depending on the current <see cref="UIMenu"/> item,
        /// from a collection of <see cref="UIMenu"/>s using the default implementation of <see cref="IDisplayItem"/>, <see cref="DisplayItem"/>, and the <see cref="UIMenu"/>'s title caption as display text.
        /// </summary>
        /// <param name="text">The item label.</param>
        /// <param name="description">The description for this item.</param>
        /// <param name="menus">The collection of <see cref="UIMenu"/>s.</param>
        public UIMenuSwitchMenusItem(string text, string description, IEnumerable<UIMenu> menus)
            : this(text, description, menus.Select(m => new DisplayItem(m, m.Title.Caption)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIMenuSwitchMenusItem"/> class a list menu item, with left/right arrows that switches the current menu depending on the current <see cref="UIMenu"/> item,
        /// from a collection of <see cref="UIMenu"/>s using the default implementation of <see cref="IDisplayItem"/>, <see cref="DisplayItem"/>, and the <see cref="UIMenu"/>'s title caption as display text.
        /// </summary>
        /// <param name="text">The item label.</param>
        /// <param name="description">The description for this item.</param>
        /// <param name="menus">The collection of <see cref="UIMenu"/>s.</param>
        public UIMenuSwitchMenusItem(string text, string description, params UIMenu[] menus)
            : this(text, description, (IEnumerable<UIMenu>)menus)
        {
        }

        /// <summary>
        /// Find a menu in the list and return it's index.
        /// </summary>
        /// <param name="menu">Menu to search for.</param>
        /// <returns>Item index.</returns>
        public virtual int MenuToIndex(UIMenu menu)
        {
            return Collection == null ? _menus.FindIndex(m => m == menu) : Collection.IndexOf(menu);
        }


        /// <summary>
        /// Find a menu by it's index and return the menu.
        /// </summary>
        /// <param name="index">Menu's index.</param>
        /// <returns>Menu</returns>
        public virtual UIMenu IndexToMenu(int index)
        {
            return Collection == null ? _menus[index] : Collection[index].Value as UIMenu;
        }
    }
}
