namespace RAGENativeUI.Menus
{
    using System.Drawing;

    using Rage;
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Rendering;
    using RAGENativeUI.Menus.Rendering;

    public class MenuItem
    {
        public string Text { get; set; }
        public SizeF Size { get; set; } = new SizeF(432f, 37f);

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
            return true;
        }

        protected internal virtual bool OnPreviewCancel(Menu menuSender)
        {
            return true;
        }

        public virtual void Process(Menu sender)
        {
        }

        public virtual void Draw(Graphics graphics, Menu sender, MenuSkin skin, bool selected, ref float x, ref float y)
        {
            if (selected)
            {
                skin.DrawSelectedGradient(graphics, x, y, Size.Width, Size.Height);
                skin.DrawText(graphics, Text, "Arial", 20.0f, new RectangleF(x + BorderSafezone, y, Size.Width, Size.Height), Color.FromArgb(225, 10, 10, 10));
            }
            else
            {
                skin.DrawText(graphics, Text, "Arial", 20.0f, new RectangleF(x + BorderSafezone, y, Size.Width, Size.Height), Color.FromArgb(240, 240, 240, 240));
            }

            y += Size.Height;
        }

        public virtual void DebugDraw(Graphics graphics, Menu sender, MenuSkin skin, bool selected, float x, float y)
        {
            graphics.DrawLine(new Vector2(x, y), new Vector2(x + Size.Width, y), Color.FromArgb(220, Color.Red));
            graphics.DrawLine(new Vector2(x, y), new Vector2(x, y + Size.Height), Color.FromArgb(220, Color.Red));
            graphics.DrawLine(new Vector2(x + Size.Width, y), new Vector2(x + Size.Width, y + Size.Height), Color.FromArgb(220, Color.Red));
            graphics.DrawLine(new Vector2(x, y + Size.Height), new Vector2(x + Size.Width, y + Size.Height), Color.FromArgb(220, Color.Red));

            graphics.DrawRectangle(new RectangleF(new PointF(x, y), Size), Color.FromArgb(50, selected ? Color.Orange : Color.Red));
        }
    }
}

