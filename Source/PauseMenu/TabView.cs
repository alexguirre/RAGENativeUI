using System;
using System.Collections.Generic;
using System.Drawing;
using Rage;
using Rage.Native;
using RAGENativeUI.Elements;
using RAGENativeUI.Internals;

namespace RAGENativeUI.PauseMenu
{
    public class TabView
    {
        /// <summary>
        /// Keeps track of the number of visible pause menus from the executing plugin.
        /// Used to keep <see cref="Shared.NumberOfVisiblePauseMenus"/> consistent when unloading the plugin with some menu open.
        /// </summary>
        internal static uint NumberOfVisiblePauseMenus { get; set; }

        /// <summary>
        /// Gets whether any pause menu is currently visible. Includes pause menus from the executing plugin and from other plugins.
        /// </summary>
        public static bool IsAnyPauseMenuVisible => Shared.NumberOfVisiblePauseMenus > 0;

        public TabView(string title)
        {
            Title = title;
            Tabs = new List<TabItem>();

            Index = 0;
            Name = Game.LocalPlayer.Name;
            TemporarilyHidden = false;
            CanLeave = true;

            MoneySubtitle = "";

            InstructionalButtons = new InstructionalButtons();
            InstructionalButtons.Buttons.Add(new InstructionalButton(GameControl.FrontendAccept, "Select"));
            InstructionalButtons.Buttons.Add(new InstructionalButton(GameControl.CellphoneCancel, "Back"));
            InstructionalButtons.Buttons.Add(new InstructionalButtonGroup("Browse", GameControl.FrontendLb, GameControl.FrontendRb));
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
                    Shared.NumberOfVisiblePauseMenus++;
                    NumberOfVisiblePauseMenus++;
                    N.SetPlayerControl(Game.LocalPlayer, false, 0);
                    N.AnimPostFxPlay("MinigameTransitionIn", 0, true);
                }
                else
                {
                    CleanUp();
                    Shared.NumberOfVisiblePauseMenus--;
                    NumberOfVisiblePauseMenus--;
                }
            }
        }


        public int Index;
        private bool _visible;

        public InstructionalButtons InstructionalButtons { get; }

        public void AddTab(TabItem item)
        {
            Tabs.Add(item);
            item.Parent = this;
        }

        public void ShowInstructionalButtons()
        {
            InstructionalButtons.Draw();
        }

        public void ProcessControls()
        {
            if (!Visible || TemporarilyHidden || Game.Console.IsOpen)
            {
                return;
            }

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

                var photoPos = new Point((int)res.Width - safe.X - 64, safe.Y - 80);
                var photoSize = new Size(64, 64);
                if (Photo == null)
                {
                    Sprite.Draw("char_multiplayer", "char_multiplayer", photoPos, photoSize, 0f, Color.White);
                }
                else
                {
                    Photo.Position = photoPos;
                    Photo.Size = photoSize;
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
        }

        public void DrawTextures(Rage.Graphics g)
        {
            if (!Visible) return;

            Tabs[Index].DrawTextures(g);
        }

        private static readonly StaticFinalizer cleanUpFinalizer = new StaticFinalizer(() =>
        {
            if (NumberOfVisiblePauseMenus > 0)
            {
                CleanUp();
            }
        });

        private static void CleanUp()
        {
            N.SetPlayerControl(Game.LocalPlayer, true, 0);
            N.AnimPostFxStop("MinigameTransitionIn");
        }
    }
}

