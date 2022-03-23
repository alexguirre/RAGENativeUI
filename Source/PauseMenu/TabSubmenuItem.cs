using System;
using System.Collections.Generic;
using System.Drawing;
using Rage;
using Rage.Native;
using RAGENativeUI.Elements;

namespace RAGENativeUI.PauseMenu
{
    public delegate void OnSubmenuSelect(TabItem selectedSubemnu);
    public delegate void OnSubmenuIndexChanged(TabSubmenuItem sender, int newIndex, TabItem newItem);


    public class TabSubmenuItem : TabItem
    {
        private class ScrollableTabList : ScrollableListBase<TabItem>
        {
            public override int MaxItemsOnScreen { get => TabView.MaxTabRowItemsOnScreen; set { }  }

            protected override List<TabItem> Items { get; set; } = new List<TabItem>();

            public List<TabItem> TabItems
            {
                get => Items;
                set => Items = value;
            }
        }

        public TabSubmenuItem(string name, IEnumerable<TabItem> items) : base(name)
        {
            DrawBg = false;
            CanBeFocused = true;
            Items = new List<TabItem>(items);
            IsInList = true;
            RefreshIndex();
        }

        public event OnSubmenuSelect OnItemSelect;
        public event OnSubmenuIndexChanged OnIndexChanged;

        private ScrollableTabList tabList = new ScrollableTabList();

        public List<TabItem> Items
        {
            get => tabList.TabItems;
            set => tabList.TabItems = value;
        }

        /// <summary>
        /// Gets or sets the current selected item's index. Set to -1 if no selection exists, for example, when <see cref="Items"/> is empty.
        /// </summary>
        public int Index
        {
            get => tabList.CurrentSelection;
            set => tabList.CurrentSelection = value;
        }

        public TabItem CurrentSubmenuItem => tabList.CurrentItem;

        public bool IsInList { get; set; }

        public void RefreshIndex() => tabList.RefreshIndex();

        public override void ProcessControls()
        {
            if (JustOpened)
            {
                JustOpened = false;
                return;
            }

            // reset focus if the child menu is focused but this menu is not
            if (!Focused && Index >= 0 && Index < Items.Count && Items[Index].Focused)
                Items[Index].Focused = false;

            // reset index if it's an invalid value
            if (Index < 0 || Index >= Items.Count)
                RefreshIndex();

            if (!Focused || Items.Count == 0 || Index < 0 || Index >= Items.Count)
                return;

            if (Items[Index].Focused)
            {
                if (Common.IsDisabledControlJustPressed(0, GameControl.CellphoneCancel) && Focused && Parent.FocusLevel > 1)
                {
                    TabView.PlayCancelSound();
                    if (Items[Index].CanBeFocused && Items[Index].Focused)
                    {
                        Parent.FocusLevel--;
                        Items[Index].Focused = false;
                    }
                }

                Items[Index].ProcessControls();
            }
            else
            {
                if (Common.IsDisabledControlJustPressed(0, GameControl.CellphoneSelect) && Focused && Parent.FocusLevel == 1)
                {
                    TabView.PlaySelectSound();
                    OnItemSelect?.Invoke(Items[Index]);

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

                if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendUp) || Common.IsDisabledControlJustPressed(0, GameControl.MoveUpOnly) || Common.IsDisabledControlJustPressed(0, GameControl.CursorScrollUp) && Parent.FocusLevel == 1)
                {
                    tabList.MoveToPreviousItem();
                    TabView.PlayNavUpDownSound();
                    OnIndexChanged?.Invoke(this, tabList.CurrentSelection, tabList.CurrentItem);
                }

                else if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendDown) || Common.IsDisabledControlJustPressed(0, GameControl.MoveDownOnly) || Common.IsDisabledControlJustPressed(0, GameControl.CursorScrollDown) && Parent.FocusLevel == 1)
                {
                    tabList.MoveToNextItem();
                    TabView.PlayNavUpDownSound();
                    OnIndexChanged?.Invoke(this, tabList.CurrentSelection, tabList.CurrentItem);
                }
            }
        }

        /// <summary>
        /// Draws this instance.
        /// </summary>
        public override void Draw()
        {
            if (!Visible) return;
            base.Draw();

            if (Items.Count == 0)
                return;

            var res = UIMenu.GetScreenResolutionMantainRatio();
            
            var blackAlpha = Focused ? 200 : 100;
            var fullAlpha = Focused ? 255 : 150;

            var activeWidth = res.Width - SafeSize.X * 2;
            var activeHeight = res.Height - SafeSize.Y * 2;
            var submenuWidth = (int)(activeWidth * 0.6818f);
            var itemSize = new Size((int)activeWidth - (submenuWidth + 3), 40);

            foreach ((int iterIndex, int tabIndex, TabItem tab, bool tabSelected) in tabList.IterateVisibleItems())
            {

                //bool hovering = UIMenu.IsMouseInBounds(SafeSize.AddPoints(new Point(0, (itemSize.Height + 3) * i)), itemSize);

                ResRectangle.Draw(SafeSize.AddPoints(new Point(0, (itemSize.Height + 3) * iterIndex)), itemSize, (tabSelected && Focused) ? Color.FromArgb(fullAlpha, Color.White) : Color.FromArgb(blackAlpha, Color.Black));
                ResText.Draw(tab.Title, SafeSize.AddPoints(new Point(6, 5 + (itemSize.Height + 3) * iterIndex)), 0.35f, Color.FromArgb(fullAlpha, (tabSelected && Focused) ? Color.Black : Color.White), Common.EFont.ChaletLondon, false);

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

            if (Index >= 0 && Index < Items.Count)
            {
                Items[Index].Visible = true;
                Items[Index].FadeInWhenFocused = true;
                //if (Items[Index].CanBeFocused)
                //    Items[Index].Focused = true;
                Items[Index].UseDynamicPositionment = false;
                Items[Index].SafeSize = SafeSize.AddPoints(new Point((int)activeWidth - submenuWidth, 0));
                Items[Index].TopLeft = SafeSize.AddPoints(new Point((int)activeWidth - submenuWidth, 0));
                Items[Index].BottomRight = new Point((int)res.Width - SafeSize.X, (int)res.Height - SafeSize.Y);
                Items[Index].Draw();
            }
        }

        public override void DrawTextures(Rage.Graphics g)
        {
            if (Index >= 0 && Index < Items.Count)
            {
                Items[Index].DrawTextures(g);
            }
        }
    }
}

