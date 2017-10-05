namespace RAGENativeUI.Menus
{
    using System;

    public class SelectedIndexChangedEventArgs : EventArgs
    {
        public int OldIndex { get; }
        public int NewIndex { get; }

        public SelectedIndexChangedEventArgs(int oldIndex, int newIndex)
        {
            OldIndex = oldIndex;
            NewIndex = newIndex;
        }
    }

    public class VisibleChangedEventArgs : EventArgs
    {
        public bool IsVisible { get; }

        public VisibleChangedEventArgs(bool isVisible)
        {
            IsVisible = isVisible;
        }
    }

    public class SelectedChangedEventArgs : EventArgs
    {
        public bool IsSelected { get; }

        public SelectedChangedEventArgs(bool isSelected)
        {
            IsSelected = isSelected;
        }
    }

    public class CheckedChangedEventArgs : EventArgs
    {
        public bool IsChecked { get; }

        public CheckedChangedEventArgs(bool isChecked)
        {
            IsChecked = isChecked;
        }
    }

    public class ActivatedEventArgs : EventArgs
    {
        public ActivatedEventArgs()
        {
        }
    }

    public class SelectedPageChangedEventArgs : SelectedIndexChangedEventArgs
    {
        public ScrollableMenuPage OldPage { get; }
        public ScrollableMenuPage NewPage { get; }

        public SelectedPageChangedEventArgs(int oldIndex, int newIndex, ScrollableMenuPage oldPage, ScrollableMenuPage newPage) : base(oldIndex, newIndex)
        {
            OldPage = oldPage;
            NewPage = newPage;
        }
    }

    public class SelectedItemChangedEventArgs : SelectedIndexChangedEventArgs
    {
        public MenuItem OldItem { get; }
        public MenuItem NewItem { get; }

        public SelectedItemChangedEventArgs(int oldIndex, int newIndex, MenuItem oldItem, MenuItem newItem) : base(oldIndex, newIndex)
        {
            OldItem = oldItem;
            NewItem = newItem;
        }
    }
}

