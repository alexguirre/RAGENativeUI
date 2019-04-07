namespace RAGENativeUI.Menus
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    // note: do not modifiy the Items collection directly(Add, Remove, Clear,...), changes will be overwritten once UpdateItems() is called
    public class ScrollableMenu : Menu
    {
        private ScrollableMenuPagesCollection pages;

        public event TypedEventHandler<ScrollableMenu, SelectedPageChangedEventArgs> SelectedPageChanged;

        public ScrollableMenuPagesCollection Pages => pages ?? (pages = new ScrollableMenuPagesCollection(this));

        public ScrollableMenuPage SelectedPage
        {
            get => (ScrollerItem.SelectedIndex >= 0 && ScrollerItem.SelectedIndex < Pages.Count) ? Pages[ScrollerItem.SelectedIndex] : null;
            set => ScrollerItem.SelectedIndex = Pages.IndexOf(value);
        }

        public MenuItemScroller ScrollerItem { get; }

        public ScrollableMenu(string title, string subtitle, string scrollerItemText) : base(title, subtitle)
        {
            Throw.IfNull(scrollerItemText, nameof(scrollerItemText));

            ScrollerItem = new MenuItemPagesScroller(scrollerItemText, Pages);
            ScrollerItem.SelectedIndexChanged += OnScrollerSelectedIndexChanged;
            Items.Add(ScrollerItem);
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
            int oldIndex = e.OldIndex;
            int newIndex = e.NewIndex;
            ScrollableMenuPage oldPage = (oldIndex >= 0 && oldIndex < Pages.Count) ? Pages[oldIndex] : null;
            ScrollableMenuPage newPage = (newIndex >= 0 && newIndex < Pages.Count) ? Pages[newIndex] : null;
            OnPropertyChanged(nameof(SelectedPage));
            OnSelectedPageChanged(new SelectedPageChangedEventArgs(oldIndex, newIndex, oldPage, newPage));
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
            if (!IsDisposed && disposing)
            {
                ScrollerItem.SelectedIndexChanged -= OnScrollerSelectedIndexChanged;
            }

            base.Dispose(disposing);
        }


        private sealed class MenuItemPagesScroller : MenuItemScroller
        {
            private ScrollableMenuPagesCollection pages;

            public ScrollableMenuPagesCollection Pages { get { return pages; } set { Throw.IfNull(value, nameof(value)); pages = value; } }

            public override int OptionCount => pages.Count;

            public override string SelectedOptionText
            {
                get
                {
                    int index = SelectedIndex;
                    if (index >= 0 && index < Pages.Count)
                        return Pages[index].Text;
                    return "-";
                }
            }

            public MenuItemPagesScroller(string text, ScrollableMenuPagesCollection pages) : base(text, String.Empty)
            {
                Throw.IfNull(pages, nameof(pages));

                Pages = pages;
            }
        }
    }


    public sealed class ScrollableMenuPage
    {
        private string text;
        private IList<MenuItem> items;

        public string Text
        {
            get => text;
            set
            {
                Throw.IfNull(value, nameof(value));
                text = value;
            }
        }

        public IList<MenuItem> Items
        {
            get => items;
            set
            {
                Throw.IfNull(value, nameof(value));
                items = value;
            }
        }

        public ScrollableMenuPage(string text)
        {
            Throw.IfNull(text, nameof(text));

            Text = text;
            Items = new List<MenuItem>();
        }
    }


    public sealed class ScrollableMenuPagesCollection : Collection<ScrollableMenuPage>
    {
        public ScrollableMenu Owner { get; }

        public ScrollableMenuPagesCollection(ScrollableMenu owner) : base()
        {
            Owner = owner;
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            Owner.UpdateItems();
        }

        protected override void InsertItem(int index, ScrollableMenuPage item)
        {
            base.InsertItem(index, item);
            Owner.UpdateItems();
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            Owner.UpdateItems();
        }

        protected override void SetItem(int index, ScrollableMenuPage item)
        {
            base.SetItem(index, item);
            Owner.UpdateItems();
        }
    }
}

