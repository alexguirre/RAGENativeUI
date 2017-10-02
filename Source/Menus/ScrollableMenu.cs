namespace RAGENativeUI.Menus
{
    using System;
    using System.Collections.Generic;

    using RAGENativeUI.Menus.Styles;

    // note: do not modifiy the Items collection directly(Add, Remove, Clear,...), changes will be overwritten once UpdateItems() is called
    public class ScrollableMenu : Menu
    {
        public delegate void SelectedPageChangedEventHandler(ScrollableMenu sender, ScrollableMenuPage oldPage, ScrollableMenuPage newPage);

        private ScrollableMenuPagesCollection pages;
        private MenuItemPagesScroller scrollerItem;

        public event SelectedPageChangedEventHandler SelectedPageChanged;

        public ScrollableMenuPagesCollection Pages { get { return pages; } set { pages = value ?? throw new ArgumentNullException($"The menu {nameof(Pages)} can't be null."); } }
        public ScrollableMenuPage SelectedPage { get { return (scrollerItem.SelectedIndex >= 0 && scrollerItem.SelectedIndex < Pages.Count) ? Pages[scrollerItem.SelectedIndex] : null; } set { scrollerItem.SelectedIndex = Pages.IndexOf(value); } }
        public MenuItemScroller ScrollerItem { get { return scrollerItem; } }

        public ScrollableMenu(string title, string subtitle, string scrollerItemText, MenuStyle style) : base(title, subtitle, style)
        {
            pages = new ScrollableMenuPagesCollection(this);
            scrollerItem = new MenuItemPagesScroller(scrollerItemText, pages);
            scrollerItem.SelectedIndexChanged += OnScrollerSelectedIndexChanged;
            Items.Add(scrollerItem);
        }

        public ScrollableMenu(string title, string subtitle, string scrollerItemText) : this(title, subtitle, scrollerItemText, MenuStyle.Default)
        {
        }

        protected override void OnVisibleChanged(bool visible)
        {
            if (visible)
            {
                UpdateItems();
            }

            base.OnVisibleChanged(visible);
        }

        protected virtual void OnSelectedPageChanged(ScrollableMenuPage oldPage, ScrollableMenuPage newPage)
        {
            SelectedPageChanged?.Invoke(this, oldPage, newPage);
        }

        private void OnScrollerSelectedIndexChanged(MenuItemScroller sender, int oldIndex, int newIndex)
        {
            ScrollableMenuPage oldPage = (oldIndex >= 0 && oldIndex < Pages.Count) ? Pages[oldIndex] : null;
            ScrollableMenuPage newPage = (newIndex >= 0 && newIndex < Pages.Count) ? Pages[newIndex] : null;
            OnSelectedPageChanged(oldPage, newPage);
            UpdateItems();
        }

        // clears the items collection and adds the scroller item and the items from the current page
        internal void UpdateItems()
        {
            int index = scrollerItem.SelectedIndex;
            Items.Clear();
            Items.Add(scrollerItem);
            if (index >= 0 && index < Pages.Count)
            {
                foreach (MenuItem item in Pages[index].Items)
                {
                    Items.Add(item);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if(!IsDisposed && disposing)
            {
                scrollerItem.SelectedIndexChanged -= OnScrollerSelectedIndexChanged;
            }

            base.Dispose(disposing);
        }


        private sealed class MenuItemPagesScroller : MenuItemScroller
        {
            private ScrollableMenuPagesCollection pages;

            public ScrollableMenuPagesCollection Pages { get { return pages; } set { pages = value ?? throw new ArgumentNullException(); } }

            public MenuItemPagesScroller(string text, ScrollableMenuPagesCollection pages) : base(text)
            {
                Pages = pages;
            }


            public override int GetOptionsCount()
            {
                return pages.Count;
            }

            public override string GetSelectedOptionText()
            {
                int index = SelectedIndex;
                if (index >= 0 && index < Pages.Count)
                    return Pages[index].Text;
                return "-";
            }
        }
    }


    public sealed class ScrollableMenuPage
    {
        private string text;
        private List<MenuItem> items;

        public string Text { get { return text; } set { text = value ?? throw new ArgumentNullException($"The page {nameof(Text)} can't be null."); } }
        public List<MenuItem> Items { get { return items; } set { items = value ?? throw new ArgumentNullException($"The page {nameof(Items)} can't be null."); } }

        public ScrollableMenuPage(string text)
        {
            Text = text;
            Items = new List<MenuItem>();
        }
    }


    public sealed class ScrollableMenuPagesCollection : BaseCollection<ScrollableMenuPage>
    {
        public ScrollableMenu Menu { get; }

        internal ScrollableMenuPagesCollection(ScrollableMenu menu)
        {
            Menu = menu ?? throw new ArgumentNullException($"The pages collection {nameof(Menu)} can't be null.");
        }

        public override void Add(ScrollableMenuPage item)
        {
            base.Add(item);
            Menu.UpdateItems();
        }

        public override void Insert(int index, ScrollableMenuPage item)
        {
            base.Insert(index, item);
            Menu.UpdateItems();
        }

        public override bool Remove(ScrollableMenuPage item)
        {
            bool b = base.Remove(item);
            if (b)
                Menu.UpdateItems();
            return b;
        }

        public override void RemoveAt(int index)
        {
            base.RemoveAt(index);
            Menu.UpdateItems();
        }

        public override void Clear()
        {
            base.Clear();
            Menu.UpdateItems();
        }
    }
}

