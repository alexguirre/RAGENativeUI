namespace RAGENativeUI.Menus
{
    using System.Drawing;
    
    using Graphics = Rage.Graphics;

    public class MenuItem
    {
        public delegate void ActivatedEventHandler(MenuItem sender, Menu origin);
        public delegate bool PreviewControlEventHandler(MenuItem sender, Menu origin); // TODO: PreviewControlEventHandler replace returned bool with property in custom EventArgs?

        public event ActivatedEventHandler Activated;
        
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

        public MenuItem(string text)
        {
            Text = text;
        }

        // return true for the menu to process the control input
        // for example, if OnMoveDown returns false the menu won't move down to the next item
        protected internal virtual bool OnMoveDown(Menu origin) => true;
        protected internal virtual bool OnMoveUp(Menu origin) => true;
        protected internal virtual bool OnMoveRight(Menu origin) => true;
        protected internal virtual bool OnMoveLeft(Menu origin) => true;
        protected internal virtual bool OnAccept(Menu origin)
        {
            OnActivated(origin);
            BindedMenu?.Show(origin);
            return true;
        }
        protected internal virtual bool OnBack(Menu origin) => true;

        protected internal virtual void OnProcess(Menu sender, bool selected)
        {
        }

        protected internal virtual void OnDraw(Graphics graphics, Menu sender, bool selected, ref float x, ref float y)
        {
            if (!IsVisible)
                return;
            
            sender.Style.DrawItem(graphics, this, ref x, ref y, selected);
        }

        protected virtual void OnActivated(Menu origin)
        {
            Activated?.Invoke(this, origin);
        }
    }
}

