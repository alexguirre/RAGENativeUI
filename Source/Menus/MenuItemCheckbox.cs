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

        public override void Draw(Graphics graphics, Menu sender, bool selected, ref float x, ref float y)
        {
            if (!IsVisible)
                return;

            if (selected)
            {
                sender.Skin.DrawSelectedGradient(graphics, x, y, Size.Width, Size.Height);
                sender.Skin.DrawText(graphics, Text, sender.Skin.ItemTextFont, new RectangleF(x + BorderSafezone, y, Size.Width, Size.Height), Color.FromArgb(225, 10, 10, 10));

                switch (State)
                {
                    case MenuItemCheckboxState.Empty:
                        sender.Skin.DrawCheckboxEmptyBlack(graphics, x + Size.Width - Size.Height - BorderSafezone * 0.5f, y + BorderSafezone * 0.5f, Size.Height - BorderSafezone, Size.Height - BorderSafezone);
                        break;
                    case MenuItemCheckboxState.Cross:
                        sender.Skin.DrawCheckboxCrossBlack(graphics, x + Size.Width - Size.Height - BorderSafezone * 0.5f, y + BorderSafezone * 0.5f, Size.Height - BorderSafezone, Size.Height - BorderSafezone);
                        break;
                    case MenuItemCheckboxState.Tick:
                        sender.Skin.DrawCheckboxTickBlack(graphics, x + Size.Width - Size.Height - BorderSafezone * 0.5f, y + BorderSafezone * 0.5f, Size.Height - BorderSafezone, Size.Height - BorderSafezone);
                        break;
                }
            }
            else
            {
                sender.Skin.DrawText(graphics, Text, sender.Skin.ItemTextFont, new RectangleF(x + BorderSafezone, y, Size.Width, Size.Height), Color.FromArgb(240, 240, 240, 240));

                switch (State)
                {
                    case MenuItemCheckboxState.Empty:
                        sender.Skin.DrawCheckboxEmptyWhite(graphics, x + Size.Width - Size.Height - BorderSafezone * 0.5f, y + BorderSafezone * 0.5f, Size.Height - BorderSafezone, Size.Height - BorderSafezone);
                        break;
                    case MenuItemCheckboxState.Cross:
                        sender.Skin.DrawCheckboxCrossWhite(graphics, x + Size.Width - Size.Height - BorderSafezone * 0.5f, y + BorderSafezone * 0.5f, Size.Height - BorderSafezone, Size.Height - BorderSafezone);
                        break;
                    case MenuItemCheckboxState.Tick:
                        sender.Skin.DrawCheckboxTickWhite(graphics, x + Size.Width - Size.Height - BorderSafezone * 0.5f, y + BorderSafezone * 0.5f, Size.Height - BorderSafezone, Size.Height - BorderSafezone);
                        break;
                }
            }

            y += Size.Height;
        }

        protected virtual void OnStateChanged(MenuItemCheckboxState oldState, MenuItemCheckboxState newState)
        {
            StateChanged?.Invoke(this, oldState, newState);
        }
    }
}

