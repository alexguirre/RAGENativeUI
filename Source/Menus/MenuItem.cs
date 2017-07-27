namespace RAGENativeUI.Menus
{
    using System.Drawing;
    
    using Graphics = Rage.Graphics;

    public class MenuItem
    {
        public delegate void ActivatedEventHandler(MenuItem sender, Menu origin);


        public event ActivatedEventHandler Activated;

        public string Text { get; set; }
        public string Description { get; set; }
        public SizeF Size { get; set; } = new SizeF(Menu.DefaultWidth, 37f);
        public bool IsVisible { get; set; } = true;

        public float BorderSafezone { get; set; } = 8.25f;

        public MenuItem(string text)
        {
            Text = text;
        }

        protected internal virtual bool OnPreviewMoveDown(Menu menuSender)
        {
            return true;
        }

        protected internal virtual bool OnPreviewMoveUp(Menu menuSender)
        {
            return true;
        }

        protected internal virtual bool OnPreviewMoveRight(Menu menuSender)
        {
            return true;
        }

        protected internal virtual bool OnPreviewMoveLeft(Menu menuSender)
        {
            return true;
        }

        protected internal virtual bool OnPreviewAccept(Menu menuSender)
        {
            OnActivated(menuSender);
            return true;
        }

        protected internal virtual bool OnPreviewCancel(Menu menuSender)
        {
            return true;
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

