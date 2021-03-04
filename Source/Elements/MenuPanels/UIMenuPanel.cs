namespace RAGENativeUI.Elements
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public abstract class UIMenuPanel
    {
        public Color BackgroundColor { get; set; } = HudColor.InGameBackground.GetColor();
        public virtual IEnumerable<IInstructionalButtonSlot> InstructionalButtons => Enumerable.Empty<IInstructionalButtonSlot>();

        public abstract void Draw(float x, ref float y, float menuWidth);

        /// <returns>Whether any input was consumed.</returns>
        public virtual bool ProcessControl() => false;

        /// <returns>Whether any input was consumed.</returns>
        public virtual bool ProcessMouse(float mouseX, float mouseY) => false;

        protected virtual void DrawBackground(float x, float y, float height, float width)
        {
            var color = BackgroundColor;
            N.DrawRect(x + width * 0.5f,
                       y + height * 0.5f - 0.00138888f,
                       width,
                       height,
                       color.R, color.G, color.B, color.A);
        }
    }
}

