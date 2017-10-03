namespace RAGENativeUI.Menus
{
    using System;

    public sealed class SelectedIndexChangedEventArgs : EventArgs
    {
        public int OldIndex { get; }
        public int NewIndex { get; }

        public SelectedIndexChangedEventArgs(int oldIndex, int newIndex)
        {
            OldIndex = oldIndex;
            NewIndex = newIndex;
        }
    }

    public sealed class VisibleChangedEventArgs : EventArgs
    {
        public bool IsVisible { get; }

        public VisibleChangedEventArgs(bool isVisible)
        {
            IsVisible = isVisible;
        }
    }

    public sealed class SelectedChangedEventArgs : EventArgs
    {
        public bool IsSelected { get; }

        public SelectedChangedEventArgs(bool isSelected)
        {
            IsSelected = isSelected;
        }
    }

    public sealed class CheckedChangedEventArgs : EventArgs
    {
        public bool IsChecked { get; }

        public CheckedChangedEventArgs(bool isChecked)
        {
            IsChecked = isChecked;
        }
    }

    public sealed class ActivatedEventArgs : EventArgs
    {
        public ActivatedEventArgs()
        {
        }
    }

    public sealed class SelectedPageChangedEventArgs : EventArgs
    {
        public ScrollableMenuPage OldPage { get; }
        public ScrollableMenuPage NewPage { get; }

        public SelectedPageChangedEventArgs(ScrollableMenuPage oldPage, ScrollableMenuPage newPage)
        {
            OldPage = oldPage;
            NewPage = newPage;
        }
    }
}

