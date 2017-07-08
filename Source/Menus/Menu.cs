namespace RAGENativeUI.Menus
{
    using System;
    using System.Linq;
    using System.Drawing;

    using Rage;
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Menus.Rendering;

    public class Menu
    {
        public PointF Location { get; set; } = new PointF(480, 17);

        private MenuItemsCollection items;
        public MenuItemsCollection Items { get { return items; } set { items = value ?? throw new InvalidOperationException($"The menu {nameof(Items)} can't be null."); } }

        private MenuSkin skin;
        public MenuSkin Skin { get { return skin; } set { skin = value ?? throw new InvalidOperationException($"The menu {nameof(Skin)} can't be null."); } }

        private MenuBanner banner;
        public MenuBanner Banner { get { return banner; } set { banner = value ?? throw new InvalidOperationException($"The menu {nameof(Banner)} can't be null."); } }

        private MenuSubtitle subtitle;
        public MenuSubtitle Subtitle { get { return subtitle; } set { subtitle = value ?? throw new InvalidOperationException($"The menu {nameof(Subtitle)} can't be null."); } }

        private int selectedIndex;
        public int SelectedIndex { get { return selectedIndex; } set { selectedIndex = MathHelper.Clamp(value, 0, Items.Count); } }
        public MenuItem SelectedItem { get { return Items[SelectedIndex]; } set { SelectedIndex = Items.IndexOf(value); } }

        public Menu(string title, string subtitle)
        {
            Items = new MenuItemsCollection();
            Skin = MenuSkin.DefaultSkin;
            Banner = new MenuBanner();
            Subtitle = new MenuSubtitle();

            Banner.Title = title;
            Subtitle.Text = subtitle;
        }

        public virtual void ProcessInput()
        {
            if (Game.IsKeyDown(System.Windows.Forms.Keys.Up))
            {
                MoveUp();
            }
            else if (Game.IsKeyDown(System.Windows.Forms.Keys.Down))
            {
                MoveDown();
            }
        }

        public void MoveUp()
        {
            int newIndex = SelectedIndex - 1;

            if (newIndex < 0)
                newIndex = Items.Count - 1;

            SelectedIndex = newIndex;
        }

        public void MoveDown()
        {
            int newIndex = SelectedIndex + 1;

            if (newIndex > (Items.Count - 1))
                newIndex = 0;

            SelectedIndex = newIndex;
        }

        public virtual void Draw(Graphics graphics)
        {
            float x = Location.X, y = Location.Y;

#if DEBUG
            bool debugDrawing = Game.IsKeyDownRightNow(System.Windows.Forms.Keys.D0);
            if (debugDrawing) Banner.DebugDraw(graphics, skin, x, y);
#endif
            Banner.Draw(graphics, skin, ref x, ref y);

#if DEBUG
            if (debugDrawing) Subtitle.DebugDraw(graphics, skin, x, y);
#endif
            Subtitle.Draw(graphics, skin, ref x, ref y);

            skin.DrawBackground(graphics, x, y - 1, Items.Max(m => m.Size.Width), Items.Sum(m => m.Size.Height));

            for (int i = 0; i < Items.Count; i++)
            {
                MenuItem item = Items[i];

#if DEBUG
                if (debugDrawing) item.DebugDraw(graphics, skin, i == SelectedIndex, x, y);
#endif
                item.Draw(graphics, skin, i == SelectedIndex, ref x, ref y);
            }
        }
    }


    public class MenuItemsCollection : Utility.BaseCollection<MenuItem>
    {
    }
}

