namespace RAGENativeUI.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    public abstract class UIMenuPanel
    {
        private IList<IInstructionalButtonSlot> instructionalButtons = new List<IInstructionalButtonSlot>();
        private RectangleF backgroundBounds;

        public Color BackgroundColor { get; set; } = HudColor.InGameBackground.GetColor();
        /// <summary>
        /// Gets the instructional buttons to show when this panel is visible.
        /// </summary>
        public IList<IInstructionalButtonSlot> InstructionalButtons
        {
            get => instructionalButtons;
            set
            {
                instructionalButtons = value ?? throw new ArgumentNullException(nameof(value));
                InstructionalButtonsChanged = true;
            }
        }

        public bool InstructionalButtonsChanged { get; set; }

        public abstract void Draw(float x, ref float y, float menuWidth);

        /// <returns>Whether any input was consumed.</returns>
        public virtual bool ProcessControl() => false;

        /// <returns>Whether any input was consumed.</returns>
        public virtual bool ProcessMouse(float mouseX, float mouseY) => backgroundBounds.Contains(mouseX, mouseY);

        protected virtual void DrawBackground(float x, float y, float width, float height)
        {
            var color = BackgroundColor;
            N.DrawRect(x + width * 0.5f,
                       y + height * 0.5f,
                       width,
                       height,
                       color.R, color.G, color.B, color.A);

            backgroundBounds = Common.GetScriptGfxRect(new RectangleF(x, y, width, height));
        }
    }
}
