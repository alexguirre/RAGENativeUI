namespace RAGENativeUI.Menus
{
    using System.Drawing;

    using Rage;
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Rendering;
    using RAGENativeUI.Menus.Rendering;

    public class MenuBanner
    {
        public string Title { get; set; }
        public SizeF Size { get; set; } = new SizeF(432f, 109f);

        public virtual void Process(Menu sender)
        {
        }

        public virtual void Draw(Graphics graphics, Menu sender, MenuSkin skin, ref float x, ref float y)
        {
            skin.DrawBanner(graphics, x, y, Size.Width, Size.Height);
            skin.DrawText(graphics, Title, "Arial", 35.0f, new RectangleF(x, y, Size.Width, Size.Height), Color.White, TextHorizontalAligment.Center, TextVerticalAligment.Center);

            y += Size.Height;
        }

        public virtual void DebugDraw(Graphics graphics, Menu sender, MenuSkin skin, float x, float y)
        {
            graphics.DrawLine(new Vector2(x, y), new Vector2(x + Size.Width, y), Color.FromArgb(220, Color.Blue));
            graphics.DrawLine(new Vector2(x, y), new Vector2(x, y + Size.Height), Color.FromArgb(220, Color.Blue));
            graphics.DrawLine(new Vector2(x + Size.Width, y), new Vector2(x + Size.Width, y + Size.Height), Color.FromArgb(220, Color.Blue));
            graphics.DrawLine(new Vector2(x, y + Size.Height), new Vector2(x + Size.Width, y + Size.Height), Color.FromArgb(220, Color.Blue));

            graphics.DrawRectangle(new RectangleF(new PointF(x, y), Size), Color.FromArgb(50, Color.LightBlue));
        }
    }
}

