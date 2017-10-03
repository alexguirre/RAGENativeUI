namespace RAGENativeUI.Menus
{
    using Graphics = Rage.Graphics;

    public class MenuItem
    {
        private Menu parent;
        private bool selected;

        public event TypedEventHandler<MenuItem, ActivatedEventArgs> Activated;
        public event TypedEventHandler<MenuItem, SelectedChangedEventArgs> SelectedChanged;
        
        public Menu Parent
        {
            get => parent;
            set
            {
                if(value != parent)
                {
                    if(value != null)
                    {
                        value.Items.Add(this);
                    }
                    else
                    {
                        parent.Items.Remove(this);
                    }
                }
            }
        }
        public string Text { get; set; }
        public string Description { get; set; }
        public bool IsVisible { get; set; } = true;
        /// <summary>
        /// Gets or sets the <see cref="Menu"/> that will be opened when this item is activated.
        /// </summary>
        /// <value>
        /// The binded <see cref="Menu"/> or <c>null</c> if no menu is binded.
        /// </value>
        public Menu BindedMenu { get; set; }
        public dynamic Metadata { get; } = new Metadata();
        public bool IsSelected
        {
            get => selected;
            internal set
            {
                if (value == selected)
                    return;
                selected = value;
                OnSelectedChanged(new SelectedChangedEventArgs(selected));
            }
        }

        public MenuItem(string text)
        {
            Text = text;
        }

        internal void SetParentInternal(Menu menu)
        {
            if (parent != null && menu != null)
            {
                parent.Items.Remove(this);
            }

            parent = menu;
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

        protected internal virtual void OnDraw(Graphics graphics, ref float x, ref float y)
        {
            if (!IsVisible)
                return;
            
            Parent.Style.DrawItem(graphics, this, ref x, ref y);
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

