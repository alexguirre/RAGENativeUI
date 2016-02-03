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

        public void ProcessControls()
        {
            if (JustOpened)
            {
                JustOpened = false;
                return;
            }

            if (!Focused) return;

            if (Common.IsDisabledControlJustPressed(0, GameControl.CellphoneSelect) && Focused)
            {
                NativeFunction.CallByName<uint>("PLAY_SOUND_FRONTEND", -1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", 1);
                Items[Index].OnActivated();
            }

            if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendUp) || Common.IsDisabledControlJustPressed(0, GameControl.MoveUpOnly))
            {
                Index = (1000 - (1000 % Items.Count) + Index - 1) % Items.Count;
                NativeFunction.CallByName<uint>("PLAY_SOUND_FRONTEND", -1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", 1);
            }

            else if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendDown) || Common.IsDisabledControlJustPressed(0, GameControl.MoveDownOnly))
            {
                Index = (1000 - (1000 % Items.Count) + Index + 1) % Items.Count;
                NativeFunction.CallByName<uint>("PLAY_SOUND_FRONTEND", -1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", 1);
            }
        }

        public override void Draw()
        {
            if (!Visible) return;
            base.Draw();

            ProcessControls();

            var res = UIMenu.GetScreenResolutionMantainRatio();

            var alpha = Focused ? 120 : 30;
            var blackAlpha = Focused ? 200 : 100;
            var fullAlpha = Focused ? 255 : 150;

            var activeWidth = res.Width - SafeSize.X * 2;
            var submenuWidth = (int)(activeWidth * 0.6818f);
            var itemSize = new Size((int)activeWidth - (submenuWidth + 3), 40);

            for (int i = 0; i < Items.Count; i++)
            {
                new ResRectangle(SafeSize.AddPoints(new Point(0, (itemSize.Height + 3) * i)), itemSize, (Index == i && Focused) ? Color.FromArgb(fullAlpha, Color.White) : Color.FromArgb(blackAlpha, Color.Black)).Draw();
                new ResText(Items[i].Title, SafeSize.AddPoints(new Point(6, 5 + (itemSize.Height + 3) * i)), 0.35f, Color.FromArgb(fullAlpha, (Index == i && Focused) ? Color.Black : Color.White)).Draw();
            }

            Items[Index].Visible = true;
            Items[Index].FadeInWhenFocused = true;
            Items[Index].CanBeFocused = true;
            Items[Index].Focused = Focused;
            Items[Index].UseDynamicPositionment = false;
            Items[Index].SafeSize = SafeSize.AddPoints(new Point((int)activeWidth - submenuWidth, 0));
            Items[Index].TopLeft = SafeSize.AddPoints(new Point((int)activeWidth - submenuWidth, 0));
            Items[Index].BottomRight = new Point((int)res.Width - SafeSize.X, (int)res.Height - SafeSize.Y);
            Items[Index].Draw();
        }
    }
}

