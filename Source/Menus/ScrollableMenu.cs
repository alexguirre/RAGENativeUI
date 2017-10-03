namespace RAGENativeUI.Menus
{
    using System;
    using System.Collections.Generic;

    using RAGENativeUI.Menus.Styles;

    // note: do not modifiy the Items collection directly(Add, Remove, Clear,...), changes will be overwritten once UpdateItems() is called
    public class ScrollableMenu : Menu
    {
        private ScrollableMenuPagesCollection pages;

        public event TypedEventHandler<ScrollableMenu, SelectedPageChangedEventArgs> SelectedPageChanged;

        public ScrollableMenuPagesCollection Pages
        {
            get { return pages; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException($"The menu {nameof(Pages)} can't be null.");
                if (value == pages)
                    return;
                pages = value;
                pages.SetMenu(this);
                if (ScrollerItem is MenuItemPagesScroller scroller)
                {
                    scroller.Pages = pages;
                }
            }
        }
        public ScrollableMenuPage SelectedPage { get { return (ScrollerItem.SelectedIndex >= 0 && ScrollerItem.SelectedIndex < Pages.Count) ? Pages[ScrollerItem.SelectedIndex] : null; } set { ScrollerItem.SelectedIndex = Pages.IndexOf(value); } }
        public MenuItemScroller ScrollerItem { get; }

        public ScrollableMenu(string title, string subtitle, string scrollerItemText, MenuStyle style) : base(title, subtitle, style)
        {
            Pages = new ScrollableMenuPagesCollection();
            ScrollerItem = new MenuItemPagesScroller(scrollerItemText, pages);
            ScrollerItem.SelectedIndexChanged += OnScrollerSelectedIndexChanged;
            Items.Add(ScrollerItem);
        }

        public ScrollableMenu(string title, string subtitle, string scrollerItemText) : this(title, subtitle, scrollerItemText, MenuStyle.Default)
        {
        }

        protected override void OnVisibleChanged(VisibleChangedEventArgs e)
        {
            if (e.IsVisible)
            {
                UpdateItems();
            }

            base.OnVisibleChanged(e);
        }

        protected virtual void OnSelectedPageChanged(SelectedPageChangedEventArgs e)
        {
            SelectedPageChanged?.Invoke(this, e);
        }

        private void OnScrollerSelectedIndexChanged(MenuItemScroller sender, SelectedIndexChangedEventArgs e)
        {
            ScrollableMenuPage oldPage = (e.OldIndex >= 0 && e.OldIndex < Pages.Count) ? Pages[e.OldIndex] : null;
            ScrollableMenuPage newPage = (e.NewIndex >= 0 && e.NewIndex < Pages.Count) ? Pages[e.NewIndex] : null;
            OnSelectedPageChanged(new SelectedPageChangedEventArgs(oldPage, newPage));
            UpdateItems();
        }

        // clears the items collection and adds the scroller item and the items from the current page
        internal void UpdateItems()
        {
            int index = ScrollerItem.SelectedIndex;
            Items.Clear();
            Items.Add(ScrollerItem);
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
                ScrollerItem.SelectedIndexChanged -= OnScrollerSelectedIndexChanged;
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
        public ScrollableMenu Menu { get; private set; }

        public ScrollableMenuPagesCollection()
        {
        }

        internal void SetMenu(ScrollableMenu menu)
        {
            if (Menu != null && Menu != menu)
                throw new InvalidOperationException($"{nameof(ScrollableMenuPagesCollection)} already set to a {nameof(ScrollableMenu)}.");
            Menu = menu ?? throw new ArgumentNullException($"The {nameof(ScrollableMenuPagesCollection)} {nameof(Menu)} can't be null.");
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

