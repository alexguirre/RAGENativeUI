using System.Collections.Generic;
using System.Drawing;
using Rage;
using RAGENativeUI.Elements;

namespace RAGENativeUI.PauseMenu
{
    public class TabInteractiveListItem : TabItem
    {
        protected const int MaxItemsPerView = 15;
        
        /// <summary>
        /// Hidden menu that holds the items and handles the input logic.
        /// </summary>
        private readonly UIMenu backingMenu;
        private int lastItemCount = 0;

        public List<UIMenuItem> Items { get => backingMenu.MenuItems; set => backingMenu.MenuItems = value; }
        public bool IsInList { get; set; }
        public int Index { get => backingMenu.CurrentSelection; set => backingMenu.CurrentSelection = value; }

        public TabInteractiveListItem(string name, IEnumerable<UIMenuItem> items) : base(name)
        {
            DrawBg = false;
            CanBeFocused = true;
            backingMenu = new UIMenu("", "")
            {
                MenuItems = new List<UIMenuItem>(items),
                MaxItemsOnScreen = MaxItemsPerView,
            };
            IsInList = true;
        }

        public void MoveDown() => backingMenu.GoDown();
        public void MoveUp() => backingMenu.GoUp();

        public void RefreshIndex()
        {
            backingMenu.RefreshIndex();
            lastItemCount = Items.Count;
        }

        public override void ProcessControls()
        {
            if (!Visible)
                return;
            if (JustOpened)
            {
                JustOpened = false;
                return;
            }

            if (!Focused)
                return;

            if (Items.Count == 0)
                return;

            if (lastItemCount != Items.Count)
            {
                // Previous versions didn't really require calling RefreshIndex.
                // Now that we're using UIMenu, it is required since we expose the items List and don't use UIMenu.Add/RemoveItem,
                // so refresh here if the item count is different to avoid breaking old code
                RefreshIndex();
            }

            if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendAccept) && Focused)
            {
                backingMenu.SelectItem();
            }


            if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendLeft) && Focused)
            {
                backingMenu.GoLeft();
            }


            if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendRight) && Focused)
            {
                backingMenu.GoRight();
            }


            if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendUp) || Common.IsDisabledControlJustPressed(0, GameControl.CursorScrollUp))
            {
                MoveUp();
            }
            else if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendDown) || Common.IsDisabledControlJustPressed(0, GameControl.CursorScrollDown))
            {
                MoveDown();
            }
        }

        public override void Draw()
        {
            if (!Visible)
                return;

            base.Draw();

            if (Items.Count == 0)
            {
                return;
            }

            UIMenu.UpdateScreenVars();

            int blackAlpha = Focused ? 200 : 100;
            int fullAlpha = Focused ? 255 : 150;

            int subMenuWidth = (BottomRight.X - TopLeft.X);
            Size itemSize = new Size(subMenuWidth, 40);

            // taken from ResRectangle.Draw
            int screenw = Game.Resolution.Width;
            int screenh = Game.Resolution.Height;
            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            var width = height * ratio;

            int i = 0;
            for (int c = backingMenu.FirstItemOnScreen; c <= backingMenu.LastItemOnScreen; c++)
            {
                //bool hovering = UIMenu.IsMouseInBounds(SafeSize.AddPoints(new Point(0, (itemSize.Height + 3) * i)), itemSize);

                Point pos = SafeSize.AddPoints(new Point(0, (itemSize.Height + 3) * i));
                
                // draw item background
                ResRectangle.Draw(pos, itemSize, (Index == c && Focused) ? Color.FromArgb(fullAlpha, Color.White) : /*Focused && hovering ? Color.FromArgb(100, 50, 50, 50) :*/ Color.FromArgb(blackAlpha, Color.Black));

                // taken from ResRectangle.Draw
                float w = itemSize.Width / width;
                float h = itemSize.Height / height;
                float x = pos.X / width;
                float y = pos.Y / height;

                var item = Items[c];

                // workaround to not draw the navigation bar when not focused
                // TODO: some other way to avoid drawing the nav bar as Selected may be overriden and could be an expensive operation
                bool selected = item.Selected;
                item.Selected = selected && Focused;

                item.Draw(x, y, w, h);

                item.Selected = selected;

                //if (Focused && hovering && (Common.IsDisabledControlJustPressed(0, GameControl.CursorAccept) || Game.IsControlJustPressed(0, GameControl.CursorAccept)))
                //{
                //    bool open = Index == c;
                //    Index = (1000 - (1000 % Items.Count) + c) % Items.Count;
                //    if (!open)
                //        Common.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                //    else
                //    {
                //        if (Items[Index] is UIMenuCheckboxItem)
                //        {
                //            Common.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                //            UIMenuCheckboxItem cb = (UIMenuCheckboxItem)Items[Index];
                //            cb.Checked = !cb.Checked;
                //            cb.CheckboxEventTrigger();
                //        }
                //        else
                //        {
                //            Common.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                //            Items[Index].ItemActivate(null);
                //        }
                //    }
                //}

                i++;
            }
        }
    }
}

