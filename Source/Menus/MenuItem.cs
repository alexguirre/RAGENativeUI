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

        public float TextSafezone { get; set; } = 11.5f;

        public MenuItem(string text)
        {
            Text = text;
        }

        public virtual bool OnPreviewMoveDown(Menu menuSender)
        {
            return true;
        }

        public virtual bool OnPreviewMoveUp(Menu menuSender)
        {
            return true;
        }

        public virtual bool OnPreviewMoveRight(Menu menuSender)
        {
            return true;
        }

        public virtual bool OnPreviewMoveLeft(Menu menuSender)
        {
            return true;
        }

        public virtual bool OnPreviewAccept(Menu menuSender)
        {
            return true;
        }

        public virtual bool OnPreviewCancel(Menu menuSender)
        {
            return true;
        }

        public virtual void Draw(Graphics graphics, MenuSkin skin, bool selected, ref float x, ref float y)
        {
            if (selected)
            {
                skin.DrawSelectedGradient(graphics, x, y, Size.Width, Size.Height);
                skin.DrawText(graphics, Text, "Arial", 20.0f, new RectangleF(x + TextSafezone, y, Size.Width, Size.Height), Color.FromArgb(225, 10, 10, 10));
            }
            else
            {
                skin.DrawText(graphics, Text, "Arial", 20.0f, new RectangleF(x + TextSafezone, y, Size.Width, Size.Height), Color.FromArgb(240, 240, 240, 240));
            }

            y += Size.Height;
        }

        public virtual void DebugDraw(Graphics graphics, MenuSkin skin, bool selected, float x, float y)
        {
            graphics.DrawLine(new Vector2(x, y), new Vector2(x + Size.Width, y), Color.FromArgb(220, Color.Red));
            graphics.DrawLine(new Vector2(x, y), new Vector2(x, y + Size.Height), Color.FromArgb(220, Color.Red));
            graphics.DrawLine(new Vector2(x + Size.Width, y), new Vector2(x + Size.Width, y + Size.Height), Color.FromArgb(220, Color.Red));
            graphics.DrawLine(new Vector2(x, y + Size.Height), new Vector2(x + Size.Width, y + Size.Height), Color.FromArgb(220, Color.Red));

            graphics.DrawRectangle(new RectangleF(new PointF(x, y), Size), Color.FromArgb(50, selected ? Color.Orange : Color.Red));
        }
    }
}

