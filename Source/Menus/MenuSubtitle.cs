namespace RAGENativeUI.Menus
{
    using System.Drawing;

    using Rage;
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Rendering;
    using RAGENativeUI.Menus.Rendering;

    public class MenuSubtitle
    {
        public string Text { get; set; }
        public SizeF Size { get; set; } = new SizeF(432f, 37f);

        public float BorderSafezone { get; set; } = 8.5f;

        public virtual void Draw(Graphics graphics, MenuSkin skin, ref float x, ref float y)
        {
            graphics.DrawRectangle(new RectangleF(x, y, Size.Width, Size.Height), Color.Black);
            skin.DrawText(graphics, Text, "Arial", 20.0f, new RectangleF(x + BorderSafezone, y, Size.Width, Size.Height), Color.White, TextHorizontalAligment.Left, TextVerticalAligment.Center);

            y += Size.Height;
        }

        public virtual void DebugDraw(Graphics graphics, MenuSkin skin, float x, float y)
        {
            graphics.DrawLine(new Vector2(x, y), new Vector2(x + Size.Width, y), Color.FromArgb(220, Color.Purple));
            graphics.DrawLine(new Vector2(x, y), new Vector2(x, y + Size.Height), Color.FromArgb(220, Color.Purple));
            graphics.DrawLine(new Vector2(x + Size.Width, y), new Vector2(x + Size.Width, y + Size.Height), Color.FromArgb(220, Color.Purple));
            graphics.DrawLine(new Vector2(x, y + Size.Height), new Vector2(x + Size.Width, y + Size.Height), Color.FromArgb(220, Color.Purple));

            graphics.DrawRectangle(new RectangleF(new PointF(x, y), Size), Color.FromArgb(50, Color.MediumPurple));
        }
    }
}

