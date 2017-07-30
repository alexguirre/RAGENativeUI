namespace RAGENativeUI.Menus
{
    using System.Drawing;
    
    using Graphics = Rage.Graphics;

    public class MenuItem
    {
        public delegate void ActivatedEventHandler(MenuItem sender, Menu origin);
        public delegate bool PreviewControlEventHandler(MenuItem sender, Menu origin); // TODO: PreviewControlEventHandler replace returned bool with property in custom EventArgs?

        public event ActivatedEventHandler Activated;

        public event PreviewControlEventHandler PreviewMoveDown;
        public event PreviewControlEventHandler PreviewMoveUp;
        public event PreviewControlEventHandler PreviewMoveRight;
        public event PreviewControlEventHandler PreviewMoveLeft;
        public event PreviewControlEventHandler PreviewAccept;
        public event PreviewControlEventHandler PreviewBack;

        public string Text { get; set; }
        public string Description { get; set; }
        public SizeF Size { get; set; } = new SizeF(Menu.DefaultWidth, 37f);
        public bool IsVisible { get; set; } = true;
        public float BorderSafezone { get; set; } = 8.25f;
        /// <summary>
        /// Gets or sets the <see cref="Menu"/> that will be opened when this item is activated.
        /// </summary>
        /// <value>
        /// The binded <see cref="Menu"/> or <c>null</c> if no menu is binded.
        /// </value>
        public Menu BindedMenu { get; set; }

        public MenuItem(string text)
        {
            Text = text;
        }

        protected internal virtual bool OnPreviewMoveDown(Menu origin)
        {
            return PreviewMoveDown == null || PreviewMoveDown.Invoke(this, origin);
        }

        protected internal virtual bool OnPreviewMoveUp(Menu origin)
        {
            return PreviewMoveUp == null || PreviewMoveUp.Invoke(this, origin);
        }

        protected internal virtual bool OnPreviewMoveRight(Menu origin)
        {
            return PreviewMoveRight == null || PreviewMoveRight.Invoke(this, origin);
        }

        protected internal virtual bool OnPreviewMoveLeft(Menu origin)
        {
            return PreviewMoveLeft == null || PreviewMoveLeft.Invoke(this, origin);
        }

        protected internal virtual bool OnPreviewAccept(Menu origin)
        {
            if (PreviewAccept == null || PreviewAccept.Invoke(this, origin))
            {
                OnActivated(origin);
                BindedMenu?.Show(origin);
                return true;
            }

            return false;
        }

        protected internal virtual bool OnPreviewBack(Menu origin)
        {
            return PreviewBack == null || PreviewBack.Invoke(this, origin);
        }

        public virtual void Process(Menu sender, bool selected)
        {
        }

        public virtual void Draw(Graphics graphics, Menu sender, bool selected, ref float x, ref float y)
        {
            if (!IsVisible)
                return;

            if (selected)
            {
                sender.Skin.DrawSelectedGradient(graphics, x, y, Size.Width, Size.Height);
                sender.Skin.DrawText(graphics, Text, sender.Skin.ItemTextFont, new RectangleF(x + BorderSafezone, y, Size.Width, Size.Height), Color.FromArgb(225, 10, 10, 10));
            }
            else
            {
                sender.Skin.DrawText(graphics, Text, sender.Skin.ItemTextFont, new RectangleF(x + BorderSafezone, y, Size.Width, Size.Height), Color.FromArgb(240, 240, 240, 240));
            }

            y += Size.Height;
        }

        protected virtual void OnActivated(Menu origin)
        {
            Activated?.Invoke(this, origin);
        }
    }
}

