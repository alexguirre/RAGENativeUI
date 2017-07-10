namespace RAGENativeUI.Menus
{
    using System.Drawing;

    using Rage;
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Rendering;
    using RAGENativeUI.Menus.Rendering;

    public enum MenuItemCheckboxState
    {
        Empty,
        Cross,
        Tick,
    }

    public class MenuItemCheckbox : MenuItem
    {
        public MenuItemCheckboxState State { get; set; }

        public MenuItemCheckbox(string text) : base(text)
        {
        }

        protected internal override bool OnPreviewAccept(Menu menuSender)
        {
            State = (State == MenuItemCheckboxState.Empty) ? MenuItemCheckboxState.Tick : MenuItemCheckboxState.Empty;
            return true;
        }

        public override void Draw(Graphics graphics, MenuSkin skin, bool selected, ref float x, ref float y)
        {
            if (selected)
            {
                skin.DrawSelectedGradient(graphics, x, y, Size.Width, Size.Height);
                skin.DrawText(graphics, Text, "Arial", 20.0f, new RectangleF(x + BorderSafezone, y, Size.Width, Size.Height), Color.FromArgb(225, 10, 10, 10));

                switch (State)
                {
                    case MenuItemCheckboxState.Empty:
                        skin.DrawCheckboxEmptyBlack(graphics, x + Size.Width - Size.Height - BorderSafezone * 0.5f, y + BorderSafezone * 0.5f, Size.Height - BorderSafezone, Size.Height - BorderSafezone);
                        break;
                    case MenuItemCheckboxState.Cross:
                        skin.DrawCheckboxCrossBlack(graphics, x + Size.Width - Size.Height - BorderSafezone * 0.5f, y + BorderSafezone * 0.5f, Size.Height - BorderSafezone, Size.Height - BorderSafezone);
                        break;
                    case MenuItemCheckboxState.Tick:
                        skin.DrawCheckboxTickBlack(graphics, x + Size.Width - Size.Height - BorderSafezone * 0.5f, y + BorderSafezone * 0.5f, Size.Height - BorderSafezone, Size.Height - BorderSafezone);
                        break;
                }
            }
            else
            {
                skin.DrawText(graphics, Text, "Arial", 20.0f, new RectangleF(x + BorderSafezone, y, Size.Width, Size.Height), Color.FromArgb(240, 240, 240, 240));

                switch (State)
                {
                    case MenuItemCheckboxState.Empty:
                        skin.DrawCheckboxEmptyWhite(graphics, x + Size.Width - Size.Height - BorderSafezone * 0.5f, y + BorderSafezone * 0.5f, Size.Height - BorderSafezone, Size.Height - BorderSafezone);
                        break;
                    case MenuItemCheckboxState.Cross:
                        skin.DrawCheckboxCrossWhite(graphics, x + Size.Width - Size.Height - BorderSafezone * 0.5f, y + BorderSafezone * 0.5f, Size.Height - BorderSafezone, Size.Height - BorderSafezone);
                        break;
                    case MenuItemCheckboxState.Tick:
                        skin.DrawCheckboxTickWhite(graphics, x + Size.Width - Size.Height - BorderSafezone * 0.5f, y + BorderSafezone * 0.5f, Size.Height - BorderSafezone, Size.Height - BorderSafezone);
                        break;
                }
            }

            y += Size.Height;
        }

        public override void DebugDraw(Graphics graphics, MenuSkin skin, bool selected, float x, float y)
        {
            base.DebugDraw(graphics, skin, selected, x, y);

            graphics.DrawRectangle(new RectangleF(x + Size.Width - Size.Height - BorderSafezone, y + BorderSafezone * 0.5f, Size.Height - BorderSafezone, Size.Height - BorderSafezone), Color.Green);
        }
    }
}

