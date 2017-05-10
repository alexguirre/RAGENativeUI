using System.Collections.Generic;
using System.Drawing;
using Rage;
using Rage.Native;
using RAGENativeUI.Elements;

namespace RAGENativeUI.PauseMenu
{
    public class TabSubmenuItem : TabItem
    {
        public TabSubmenuItem(string name, IEnumerable<TabItem> items) : base(name)
        {
            DrawBg = false;
            CanBeFocused = true;
            Items = new List<TabItem>(items);
            IsInList = true;
        }

        public List<TabItem> Items { get; set; }
        public int Index { get; set; }
        public bool IsInList { get; set; }

        public void RefreshIndex()
        {
            foreach (TabItem item in Items)
            {
                item.Focused = false;
                item.Active = false;
                item.Visible = false;
            }
            Index = (1000 - (1000 % Items.Count)) % Items.Count;
        }

        public override void ProcessControls()
        {
            if (JustOpened)
            {
                JustOpened = false;
                return;
            }

            if (!Focused) return;

            if (Common.IsDisabledControlJustPressed(0, GameControl.CellphoneSelect) && Focused && Parent.FocusLevel == 1)
            {
                Common.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");

                if (Items[Index].CanBeFocused && !Items[Index].Focused)
                {
                    Parent.FocusLevel++;
                    Items[Index].JustOpened = true;
                    Items[Index].Focused = true;
                }
                else
                {
                    Items[Index].OnActivated();
                }

                
            }

            if (Common.IsDisabledControlJustPressed(0, GameControl.CellphoneCancel) && Focused && Parent.FocusLevel > 1)
            {
                Common.PlaySound("CANCEL", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                if (Items[Index].CanBeFocused && Items[Index].Focused)
                {
                    Parent.FocusLevel--;
                    Items[Index].Focused = false;
                }
            }

            if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendUp) || Common.IsDisabledControlJustPressed(0, GameControl.MoveUpOnly) || Common.IsDisabledControlJustPressed(0, GameControl.CursorScrollUp) && Parent.FocusLevel == 1)
            {
                Index = (1000 - (1000 % Items.Count) + Index - 1) % Items.Count;
                Common.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
            }

            else if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendDown) || Common.IsDisabledControlJustPressed(0, GameControl.MoveDownOnly) || Common.IsDisabledControlJustPressed(0, GameControl.CursorScrollDown) && Parent.FocusLevel == 1)
            {
                Index = (1000 - (1000 % Items.Count) + Index + 1) % Items.Count;
                Common.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
            }

            if (Items.Count > 0) Items[Index].ProcessControls();
        }

        /// <summary>
        /// Draws this instance.
        /// </summary>
        public override void Draw()
        {
            if (!Visible) return;
            base.Draw();
            
            var res = UIMenu.GetScreenResolutionMantainRatio();
            
            var blackAlpha = Focused ? 200 : 100;
            var fullAlpha = Focused ? 255 : 150;

            var activeWidth = res.Width - SafeSize.X * 2;
            var submenuWidth = (int)(activeWidth * 0.6818f);
            var itemSize = new Size((int)activeWidth - (submenuWidth + 3), 40);

            for (int i = 0; i < Items.Count; i++)
            {
                //bool hovering = UIMenu.IsMouseInBounds(SafeSize.AddPoints(new Point(0, (itemSize.Height + 3) * i)), itemSize);

                ResRectangle.Draw(SafeSize.AddPoints(new Point(0, (itemSize.Height + 3) * i)), itemSize, (Index == i && Focused) ? Color.FromArgb(fullAlpha, Color.White) : Color.FromArgb(blackAlpha, Color.Black));
                ResText.Draw(Items[i].Title, SafeSize.AddPoints(new Point(6, 5 + (itemSize.Height + 3) * i)), 0.35f, Color.FromArgb(fullAlpha, (Index == i && Focused) ? Color.Black : Color.White), Common.EFont.ChaletLondon, false);

                //if (Focused && hovering && Common.IsDisabledControlJustPressed(0, GameControl.CursorAccept))
                //{
                //    Items[Index].Focused = false;
                //    Common.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                //    bool open = Index == i;
                //    Index = (1000 - (1000 % Items.Count) + i) % Items.Count;
                //    if (open)
                //    {
                //        if (Items[Index].CanBeFocused && !Items[Index].Focused)
                //        {
                //            Parent.FocusLevel = 2;
                //            Items[Index].JustOpened = true;
                //            Items[Index].Focused = true;
                //        }
                //        else
                //        {
                //            Items[Index].OnActivated();
                //        }
                //    }
                //    else
                //    {
                //        Parent.FocusLevel = 1;
                //    }
                //}
            }

            Items[Index].Visible = true;
            Items[Index].FadeInWhenFocused = true;
            if (Items[Index].CanBeFocused)
                Items[Index].Focused = true;
            Items[Index].UseDynamicPositionment = false;
            Items[Index].SafeSize = SafeSize.AddPoints(new Point((int)activeWidth - submenuWidth, 0));
            Items[Index].TopLeft = SafeSize.AddPoints(new Point((int)activeWidth - submenuWidth, 0));
            Items[Index].BottomRight = new Point((int)res.Width - SafeSize.X, (int)res.Height - SafeSize.Y);
            Items[Index].Draw();
        }
    }
}
