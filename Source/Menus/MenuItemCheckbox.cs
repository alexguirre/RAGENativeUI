namespace RAGENativeUI.Menus
{
    using System.Drawing;
    
    using Graphics = Rage.Graphics;

    public enum MenuItemCheckboxState
    {
        Empty,
        Cross,
        Tick,
    }

    public class MenuItemCheckbox : MenuItem
    {
        public delegate void StateChangedEventHandler(MenuItem sender, MenuItemCheckboxState oldState, MenuItemCheckboxState newState);

        public event StateChangedEventHandler StateChanged;
        
        private MenuItemCheckboxState state;
        public MenuItemCheckboxState State
        {
            get { return state; }
            set
            {
                if (value == state)
                    return;

                MenuItemCheckboxState oldState = state;
                state = value;
                OnStateChanged(oldState, state);
            }
        }

        public MenuItemCheckbox(string text) : base(text)
        {
        }

        protected internal override bool OnPreviewAccept(Menu origin)
        {
            if (base.OnPreviewAccept(origin))
            {
                State = (State == MenuItemCheckboxState.Empty) ? MenuItemCheckboxState.Tick : MenuItemCheckboxState.Empty;
                return true;
            }

            return false;
        }

        protected internal override void OnDraw(Graphics graphics, Menu sender, bool selected, ref float x, ref float y)
        {
            if (!IsVisible)
                return;

            sender.Skin.DrawItemCheckbox(graphics, this, x, y, selected);
            y += Size.Height;
        }

        protected virtual void OnStateChanged(MenuItemCheckboxState oldState, MenuItemCheckboxState newState)
        {
            StateChanged?.Invoke(this, oldState, newState);
        }
    }
}

