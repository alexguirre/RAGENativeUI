namespace RAGENativeUI.Menus
{
    using System;
    using System.ComponentModel;

    public class MenuItem : INotifyPropertyChanged
    {
        private Menu parent;
        private string text;
        private string description;
        private bool isVisible = true;
        private Menu bindedMenu;
        private bool isSelected;
        private bool isDisabled;
        private bool isSkippedIfDisabled;

        public event PropertyChangedEventHandler PropertyChanged;
        public event TypedEventHandler<MenuItem, ActivatedEventArgs> Activated;
        public event TypedEventHandler<MenuItem, SelectedChangedEventArgs> SelectedChanged;
        
        public Menu Parent
        {
            get => parent;
            internal set
            {
                if(value != parent)
                {
                    parent = value;
                    OnPropertyChanged(nameof(Parent));
                }
            }
        }

        public string Text
        {
            get => text;
            set
            {
                Throw.IfNull(value, nameof(value));
                if(value != text)
                {
                    text = value;
                    OnPropertyChanged(nameof(Text));
                }
            }
        }

        public string Description
        {
            get => description;
            set
            {
                Throw.IfNull(value, nameof(value));
                if (value != description)
                {
                    description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public bool IsVisible
        {
            get => isVisible;
            set
            {
                if (value != isVisible)
                {
                    isVisible = value;
                    OnPropertyChanged(nameof(IsVisible));
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Menu"/> that will be opened when this item is activated.
        /// </summary>
        /// <value>
        /// The binded <see cref="Menu"/> or <c>null</c> if no menu is binded.
        /// </value>
        public Menu BindedMenu
        {
            get => bindedMenu;
            set
            {
                if (value != bindedMenu)
                {
                    bindedMenu = value;
                    OnPropertyChanged(nameof(BindedMenu));
                }
            }
        }

        public bool IsSelected
        {
            get => isSelected;
            internal set
            {
                if (value != isSelected)
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                    OnSelectedChanged(new SelectedChangedEventArgs(isSelected));
                }
            }
        }

        public bool IsDisabled
        {
            get => isDisabled;
            set
            {
                if (value != isDisabled)
                {
                    isDisabled = value;
                    OnPropertyChanged(nameof(IsDisabled));
                }
            }
        }

        public bool IsSkippedIfDisabled
        {
            get => isSkippedIfDisabled;
            set
            {
                if (value != isSkippedIfDisabled)
                {
                    isSkippedIfDisabled = value;
                    OnPropertyChanged(nameof(IsSkippedIfDisabled));
                }
            }
        }

        public dynamic Metadata { get; } = new Metadata();

        public MenuItem(string text, string description)
        {
            Throw.IfNull(text, nameof(text));
            Throw.IfNull(description, nameof(description));

            Text = text;
            Description = description;
        }

        public MenuItem(string text) : this(text, String.Empty)
        {
        }

        // return true for the menu to process the control input
        // for example, if OnMoveDown returns false the menu won't move down to the next item
        protected internal virtual bool OnMoveDown() => true;
        protected internal virtual bool OnMoveUp() => true;
        protected internal virtual bool OnMoveRight() => true;
        protected internal virtual bool OnMoveLeft() => true;
        protected internal virtual bool OnAccept()
        {
            OnActivated(new ActivatedEventArgs());
            BindedMenu?.Show(Parent);
            return true;
        }
        protected internal virtual bool OnBack() => true;

        protected internal virtual void OnProcess()
        {
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnActivated(ActivatedEventArgs e)
        {
            Activated?.Invoke(this, e);
        }

        protected virtual void OnSelectedChanged(SelectedChangedEventArgs e)
        {
            SelectedChanged?.Invoke(this, e);
        }
    }
}

