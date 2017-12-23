using System;
using System.Collections.Generic;
using System.Drawing;
using Rage;
using Rage.Native;
using RAGENativeUI.Elements;

namespace RAGENativeUI.PauseMenu
{
    public class TabView
    {
        public TabView(string title)
        {
            Title = title;
            Tabs = new List<TabItem>();

            Index = 0;
            Name = Game.LocalPlayer.Name;
            TemporarilyHidden = false;
            CanLeave = true;

            MoneySubtitle = "";
        }

        public string Title { get; set; }
        public Sprite Photo { get; set; }
        public string Name { get; set; }
        public string Money { get; set; }
        public string MoneySubtitle { get; set; }
        public List<TabItem> Tabs { get; set; }
        public int FocusLevel { get; set; }
        public bool TemporarilyHidden { get; set; }
        public bool CanLeave { get; set; }
        public bool HideTabs { get; set; }

        public event EventHandler OnMenuClose;

        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;

                if (value)
                {
                    NativeFunction.CallByHash<uint>(0x2206bf9a37b7f724, "MinigameTransitionIn", 0, true); //_START_SCREEN_EFFECT

                }
                else
                {
                    NativeFunction.CallByHash<uint>(0x068e835a1d0dc0e3, "MinigameTransitionIn"); //_STOP_SCREEN_EFFECT
                }
            }
        }


        public int Index;
        private bool _visible;

        public void AddTab(TabItem item)
        {
            Tabs.Add(item);
            item.Parent = this;
        }

        private Scaleform _sc;
        public void ShowInstructionalButtons()
        {
            if (_sc == null)
            {
                _sc = new Scaleform(0);
                _sc.Load("instructional_buttons");
            }

            _sc.CallFunction("CLEAR_ALL");
            _sc.CallFunction("TOGGLE_MOUSE_BUTTONS", 0);
            _sc.CallFunction("CREATE_CONTAINER");


            _sc.CallFunction("SET_DATA_SLOT", 0, (string)NativeFunction.CallByHash(0x0499d7b09fc9b407, typeof(string), 2, (int)GameControl.CellphoneSelect, 0), "Select");
            _sc.CallFunction("SET_DATA_SLOT", 1, (string)NativeFunction.CallByHash(0x0499d7b09fc9b407, typeof(string), 2, (int)GameControl.CellphoneCancel, 0), "Back");

            _sc.CallFunction("SET_DATA_SLOT", 2, (string)NativeFunction.CallByHash(0x0499d7b09fc9b407, typeof(string), 2, (int)GameControl.FrontendRb, 0), "");
            _sc.CallFunction("SET_DATA_SLOT", 3, (string)NativeFunction.CallByHash(0x0499d7b09fc9b407, typeof(string), 2, (int)GameControl.FrontendLb, 0), "Browse");

        }

        public void DrawInstructionalButton(int slot, GameControl control, string text)
        {
            _sc.CallFunction("SET_DATA_SLOT", slot, (string)NativeFunction.CallByHash(0x0499d7b09fc9b407, typeof(string), 2, (int)control, 0), text);
        }

        public void ProcessControls()
        {
            if (!Visible || TemporarilyHidden) return;
            NativeFunction.Natives.DisableAllControlActions(0);

            if (Common.IsDisabledControlJustPressed(0, GameControl.CellphoneLeft) && FocusLevel == 0)
            {
                Tabs[Index].Active = false;
                Tabs[Index].Focused = false;
                Tabs[Index].Visible = false;
                Index = (1000 - (1000 % Tabs.Count) + Index - 1) % Tabs.Count;
                Tabs[Index].Active = true;
                Tabs[Index].Focused = false;
                Tabs[Index].Visible = true;
                
                Common.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
            }

            else if (Common.IsDisabledControlJustPressed(0, GameControl.CellphoneRight) && FocusLevel == 0)
            {
                Tabs[Index].Active = false;
                Tabs[Index].Focused = false;
                Tabs[Index].Visible = false;
                Index = (1000 - (1000 % Tabs.Count) + Index + 1) % Tabs.Count;
                Tabs[Index].Active = true;
                Tabs[Index].Focused = false;
                Tabs[Index].Visible = true;

                Common.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
            }

            else if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendAccept) && FocusLevel == 0)
            {
                if (Tabs[Index].CanBeFocused)
                {
                    Tabs[Index].Focused = true;
                    Tabs[Index].JustOpened = true;
                    FocusLevel = 1;
                }
                else
                {
                    Tabs[Index].JustOpened = true;
                    Tabs[Index].OnActivated();
                }

                Common.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");

            }

            else if (Common.IsDisabledControlJustPressed(0, GameControl.CellphoneCancel) && FocusLevel == 1)
            {
                Tabs[Index].Focused = false;
                FocusLevel = 0;

                Common.PlaySound("BACK", "HUD_FRONTEND_DEFAULT_SOUNDSET");
            }

            else if (Common.IsDisabledControlJustPressed(0, GameControl.CellphoneCancel) && FocusLevel == 0 && CanLeave)
            {
                Visible = false;
                Common.PlaySound("BACK", "HUD_FRONTEND_DEFAULT_SOUNDSET");

                OnMenuClose?.Invoke(this, EventArgs.Empty);
            }

            if (!HideTabs)
            {
                if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendLb))
                {
                    Tabs[Index].Active = false;
                    Tabs[Index].Focused = false;
                    Tabs[Index].Visible = false;
                    Index = (1000 - (1000 % Tabs.Count) + Index - 1) % Tabs.Count;
                    Tabs[Index].Active = true;
                    Tabs[Index].Focused = false;
                    Tabs[Index].Visible = true;

                    FocusLevel = 0;

                    Common.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                }

                else if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendRb))
                {
                    Tabs[Index].Active = false;
                    Tabs[Index].Focused = false;
                    Tabs[Index].Visible = false;
                    Index = (1000 - (1000 % Tabs.Count) + Index + 1) % Tabs.Count;
                    Tabs[Index].Active = true;
                    Tabs[Index].Focused = false;
                    Tabs[Index].Visible = true;

                    FocusLevel = 0;

                    Common.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                }
            }

            if (Tabs.Count > 0) Tabs[Index].ProcessControls();
        }

        public void RefreshIndex()
        {
            foreach (TabItem item in Tabs)
            {
                item.Focused = false;
                item.Active = false;
                item.Visible = false;
            }

            Index = (1000 - (1000 % Tabs.Count)) % Tabs.Count;
            Tabs[Index].Active = true;
            Tabs[Index].Focused = false;
            Tabs[Index].Visible = true;
            FocusLevel = 0;
        }

        public void Update()
        {
            if (!Visible || TemporarilyHidden) return;

            ShowInstructionalButtons();
            NativeFunction.Natives.HideHudAndRadarThisFrame();
            //NativeFunction.CallByHash<uint>(0xaae7ce1d63167423); // _SHOW_CURSOR_THIS_FRAME
            
            ProcessControls();
            
            var res = UIMenu.GetScreenResolutionMantainRatio();
            var safe = new Point(300, 180);

            if (!HideTabs)
            {
                ResText.Draw(Title, new Point(safe.X, safe.Y - 80), 1f, Color.White, Common.EFont.ChaletComprimeCologne, ResText.Alignment.Left, true, false, Size.Empty);

                if (Photo == null)
                {
                    Sprite.Draw("char_multiplayer", "char_multiplayer", new Point((int)res.Width - safe.X - 64, safe.Y - 80), new Size(64, 64), 0f, Color.White);
                }
                else
                {
                    Photo.Position = new Point((int)res.Width - safe.X - 100, safe.Y - 80);
                    Photo.Size = new Size(64, 64);
                    Photo.Draw();
                }

                ResText.Draw(Name, new Point((int)res.Width - safe.X - 70, safe.Y - 95), 0.7f, Color.White, Common.EFont.ChaletComprimeCologne, ResText.Alignment.Right, true, false, Size.Empty);

                string subt = Money;
                if (string.IsNullOrEmpty(Money))
                {
                    subt = DateTime.Now.ToString();
                }


                ResText.Draw(subt, new Point((int)res.Width - safe.X - 70, safe.Y - 60), 0.4f, Color.White, Common.EFont.ChaletComprimeCologne, ResText.Alignment.Right, true, false, Size.Empty);

                ResText.Draw(MoneySubtitle, new Point((int)res.Width - safe.X - 70, safe.Y - 40), 0.4f, Color.White, Common.EFont.ChaletComprimeCologne, ResText.Alignment.Right, true, false, Size.Empty);

                for (int i = 0; i < Tabs.Count; i++)
                {
                    var activeSize = res.Width - 2 * safe.X;
                    activeSize -= 4 * 5;
                    int tabWidth = (int)activeSize / Tabs.Count;

                    //Game.DisableControlAction(0, GameControl.CursorX, false);
                    //Game.DisableControlAction(0, GameControl.CursorY, false);

                    //bool hovering = UIMenu.IsMouseInBounds(safe.AddPoints(new Point((tabWidth + 5) * i, 0)), new Size(tabWidth, 40));

                    var tabColor = Tabs[i].Active ? Color.White : /*hovering ? Color.FromArgb(100, 50, 50, 50) :*/ Color.Black;
                    ResRectangle.Draw(safe.AddPoints(new Point((tabWidth + 5) * i, 0)), new Size(tabWidth, 40), Color.FromArgb(Tabs[i].Active ? 255 : 200, tabColor));

                    ResText.Draw(Tabs[i].Title.ToUpper(), safe.AddPoints(new Point((tabWidth / 2) + (tabWidth + 5) * i, 5)), 0.35f, Tabs[i].Active ? Color.Black : Color.White, Common.EFont.ChaletLondon, ResText.Alignment.Centered, false, false, Size.Empty);

                    if (Tabs[i].Active)
                    {
                        ResRectangle.Draw(safe.SubtractPoints(new Point(-((tabWidth + 5) * i), 10)), new Size(tabWidth, 10), Color.DodgerBlue);
                    }

                    //if (hovering && Common.IsDisabledControlJustPressed(0, GameControl.CursorAccept) && !Tabs[i].Active)
                    //{
                    //    Tabs[Index].Active = false;
                    //    Tabs[Index].Focused = false;
                    //    Tabs[Index].Visible = false;
                    //    Index = (1000 - (1000 % Tabs.Count) + i) % Tabs.Count;
                    //    Tabs[Index].Active = true;
                    //    Tabs[Index].Focused = true;
                    //    Tabs[Index].Visible = true;
                    //    Tabs[Index].JustOpened = true;

                    //    FocusLevel = Tabs[Index].CanBeFocused ? 1 : 0;

                    //    Common.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                    //}
                }
            }

            Tabs[Index].Draw();

            _sc.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", -1);

            _sc.Render2D();
        }

        public void DrawTextures(Rage.Graphics g)
        {
            if (!Visible) return;

            Tabs[Index].DrawTextures(g);
        }
    }
}
