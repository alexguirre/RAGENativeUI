using System;
using System.Collections.Generic;
using System.Drawing;
using Rage;
using RAGENativeUI.Elements;

namespace RAGENativeUI.PauseMenu
{
    public class TabInteractiveListItem : TabItem
    {
        private int lastItemCount = 0;

        public List<UIMenuItem> Items { get => BackingMenu.MenuItems; set => BackingMenu.MenuItems = value; }
        public bool IsInList { get; set; }
        public int Index { get => BackingMenu.CurrentSelection; set => BackingMenu.CurrentSelection = value; }
        /// <summary>
        /// Gets the hidden menu that holds the items and handles the input logic.
        /// </summary>
        /// <remarks>
        /// This <see cref="UIMenu"/> does not require a <see cref="MenuPool"/>, it is fully managed by the <see cref="TabInteractiveListItem"/>.
        /// </remarks>
        public UIMenu BackingMenu { get; }

        public TabInteractiveListItem(string name, IEnumerable<UIMenuItem> items) : base(name)
        {
            DrawBg = false;
            CanBeFocused = true;
            BackingMenu = new UIMenu(nameof(TabInteractiveListItem), nameof(BackingMenu))
            {
                MaxItemsOnScreen = TabView.MaxTabRowItemsOnScreen,
                MenuItems = new List<UIMenuItem>(items),
                IgnoreVisibility = true,
            };
            RefreshIndex();
            IsInList = true;
        }

        public void MoveDown() => BackingMenu.GoDown();
        public void MoveUp() => BackingMenu.GoUp();

        public void RefreshIndex()
        {
            BackingMenu.RefreshIndex();
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
            {
                if (BackingMenu.Visible)
                {
                    BackingMenu.Close(openParentMenu: false);
                }
                return;
            }

            if (Items.Count == 0)
                return;

            if (lastItemCount != Items.Count)
            {
                // Previous versions didn't really require calling RefreshIndex.
                // Now that we're using UIMenu, it is required since we expose the items List and don't use UIMenu.Add/RemoveItem,
                // so refresh here if the item count is different to avoid breaking old code
                RefreshIndex();
            }

            if (BackingMenu.Visible)
            {
                BackingMenu.ProcessControl();
            }
            else
            {
                BackingMenu.Visible = true;
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

            int blackAlpha = Focused ? 200 : 100;
            int fullAlpha = Focused ? 255 : 150;

            int subMenuWidth = (BottomRight.X - TopLeft.X);
            int subMenuHeight = (BottomRight.Y - TopLeft.Y);
            Size itemSize = new Size(subMenuWidth, 40);

            // taken from ResRectangle.Draw
            var res = Internals.Screen.ActualResolution;
            const float height = 1080f;
            float ratio = res.Width / res.Height;
            var width = height * ratio;

            foreach ((int iterIndex, int itemIndex, UIMenuItem item, bool itemSelected) in BackingMenu.IterateVisibleItems())
            {
                //bool hovering = UIMenu.IsMouseInBounds(SafeSize.AddPoints(new Point(0, (itemSize.Height + 3) * i)), itemSize);

                Point pos = SafeSize.AddPoints(new Point(0, (itemSize.Height + 3) * iterIndex));
                
                // draw item background
                ResRectangle.Draw(pos, itemSize, (itemSelected && Focused) ? Color.FromArgb(fullAlpha, Color.White) : /*Focused && hovering ? Color.FromArgb(100, 50, 50, 50) :*/ Color.FromArgb(blackAlpha, Color.Black));

                // taken from ResRectangle.Draw
                float w = itemSize.Width / width;
                float h = itemSize.Height / height;
                float x = pos.X / width;
                float y = pos.Y / height;

                // only draw the navigation bar if this tab is focused
                bool canDrawNavBar = item.CanDrawNavBar;
                item.CanDrawNavBar = canDrawNavBar && Focused;

                item.Draw(x, y, w, h);

                item.CanDrawNavBar = canDrawNavBar;

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
            }
        }
    }
}

