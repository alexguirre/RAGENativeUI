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

            Name = (string)NativeFunction.CallByName("GET_PLAYER_NAME", typeof(string), Game.LocalPlayer);
            IsControlInTabs = true;

            MoneySubtitle = "";
        }

        public string Title { get; set; }
        public Sprite Photo { get; set; }
        public string Name { get; set; }
        public string Money { get; set; }
        public string MoneySubtitle { get; set; }
        public List<TabItem> Tabs { get; set; }
        public bool IsControlInTabs { get; set; }

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

        public void ProcessMouse()
        {
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

        public void ProcessControls()
        {
            NativeFunction.CallByName<uint>("DISABLE_ALL_CONTROL_ACTIONS", 0);

            if (Common.IsDisabledControlJustPressed(0, GameControl.CellphoneLeft) && IsControlInTabs)
            {
                Tabs[Index].Active = false;
                Tabs[Index].Focused = false;
                Tabs[Index].Visible = false;
                Index = (1000 - (1000 % Tabs.Count) + Index - 1) % Tabs.Count;
                Tabs[Index].Active = true;
                Tabs[Index].Focused = false;
                Tabs[Index].Visible = true;

                NativeFunction.CallByName<uint>("PLAY_SOUND_FRONTEND", -1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", 1);
            }

            else if (Common.IsDisabledControlJustPressed(0, GameControl.CellphoneRight) && IsControlInTabs)
            {
                Tabs[Index].Active = false;
                Tabs[Index].Focused = false;
                Tabs[Index].Visible = false;
                Index = (1000 - (1000 % Tabs.Count) + Index + 1) % Tabs.Count;
                Tabs[Index].Active = true;
                Tabs[Index].Focused = false;
                Tabs[Index].Visible = true;

                NativeFunction.CallByName<uint>("PLAY_SOUND_FRONTEND", -1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", 1);
            }

            else if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendAccept) && IsControlInTabs)
            {
                if (Tabs[Index].CanBeFocused)
                {
                    Tabs[Index].Focused = true;
                    Tabs[Index].JustOpened = true;
                    IsControlInTabs = false;
                }
                else
                {
                    Tabs[Index].JustOpened = true;
                    Tabs[Index].OnActivated();
                }

                NativeFunction.CallByName<uint>("PLAY_SOUND_FRONTEND", -1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", 1);

            }

            else if (Common.IsDisabledControlJustPressed(0, GameControl.CellphoneCancel) && !IsControlInTabs)
            {
                Tabs[Index].Focused = false;
                IsControlInTabs = true;

                NativeFunction.CallByName<uint>("PLAY_SOUND_FRONTEND", -1, "BACK", "HUD_FRONTEND_DEFAULT_SOUNDSET", 1);
            }

            else if (Common.IsDisabledControlJustPressed(0, GameControl.CellphoneCancel) && IsControlInTabs)
            {
                Visible = false;
                NativeFunction.CallByName<uint>("PLAY_SOUND_FRONTEND", -1, "BACK", "HUD_FRONTEND_DEFAULT_SOUNDSET", 1);

                OnMenuClose?.Invoke(this, EventArgs.Empty);
            }

            else if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendLb))
            {
                Tabs[Index].Active = false;
                Tabs[Index].Focused = false;
                Tabs[Index].Visible = false;
                Index = (1000 - (1000 % Tabs.Count) + Index - 1) % Tabs.Count;
                Tabs[Index].Active = true;
                Tabs[Index].Focused = false;
                Tabs[Index].Visible = true;

                IsControlInTabs = true;

                NativeFunction.CallByName<uint>("PLAY_SOUND_FRONTEND", -1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", 1);
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

                IsControlInTabs = true;

                NativeFunction.CallByName<uint>("PLAY_SOUND_FRONTEND", -1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", 1);
            }

        }

        public void RefreshIndex()
        {
            Index = (1000 - (1000 % Tabs.Count)) % Tabs.Count;
            Tabs[Index].Active = true;
            Tabs[Index].Focused = false;
            Tabs[Index].Visible = true;
        }

        public void Update()
        {
            if (!Visible) return;

            ShowInstructionalButtons();
            NativeFunction.CallByName<uint>("HIDE_HUD_AND_RADAR_THIS_FRAME");

            ProcessControls();
            ProcessMouse();

            var res = UIMenu.GetScreenResolutionMantainRatio();
            var safe = new Point(300, 180);

            new ResText(Title, new Point(safe.X, safe.Y - 80), 1f, Color.White, Common.EFont.ChaletComprimeCologne, ResText.Alignment.Left)
            {
                DropShadow = true,
            }.Draw();

            if (Photo == null)
            {
                new Sprite("char_multiplayer", "char_multiplayer", new Point((int)res.Width - safe.X - 64, safe.Y - 80), new Size(64, 64)).Draw();
            }
            else
            {
                Photo.Position = new Point((int)res.Width - safe.X - 100, safe.Y - 80);
                Photo.Size = new Size(64, 64);
                Photo.Draw();
            }

            new ResText(Name, new Point((int)res.Width - safe.X - 70, safe.Y - 95), 0.7f, Color.White,
                Common.EFont.ChaletComprimeCologne, ResText.Alignment.Right)
            {
                DropShadow = true,
            }.Draw();

            string subt = Money;
            if (string.IsNullOrEmpty(Money))
            {
                subt = DateTime.Now.ToString();
            }


            new ResText(subt, new Point((int)res.Width - safe.X - 70, safe.Y - 60), 0.4f, Color.White,
                Common.EFont.ChaletComprimeCologne, ResText.Alignment.Right)
            {
                DropShadow = true,
            }.Draw();

            new ResText(MoneySubtitle, new Point((int)res.Width - safe.X - 70, safe.Y - 40), 0.4f, Color.White,
                Common.EFont.ChaletComprimeCologne, ResText.Alignment.Right)
            {
                DropShadow = true,
            }.Draw();

            for (int i = 0; i < Tabs.Count; i++)
            {

                var activeSize = res.Width - 2 * safe.X;
                activeSize -= 4 * 5;
                int tabWidth = (int)activeSize / 5;

                var tabColor = Tabs[i].Active ? Color.White : Color.Black;
                new ResRectangle(safe.AddPoints(new Point((tabWidth + 5) * i, 0)), new Size(tabWidth, 40), Color.FromArgb(Tabs[i].Active ? 255 : 200, tabColor)).Draw();

                new ResText(Tabs[i].Title.ToUpper(), safe.AddPoints(new Point((tabWidth / 2) + (tabWidth + 5) * i, 5)), 0.35f,
                    Tabs[i].Active ? Color.Black : Color.White, Common.EFont.ChaletLondon, ResText.Alignment.Centered).Draw();

                if (Tabs[i].Active)
                {
                    new ResRectangle(safe.SubtractPoints(new Point(-((tabWidth + 5) * i), 10)), new Size(tabWidth, 10), Color.DodgerBlue).Draw();
                }
            }

            Tabs[Index].Draw();

            _sc.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", -1);

            _sc.Render2D();
        }
    }
}

