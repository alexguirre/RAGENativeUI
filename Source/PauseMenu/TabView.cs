using System;
using System.Collections.Generic;
using System.Drawing;
using Rage;
using Rage.Native;
using RAGENativeUI.Elements;
using RAGENativeUI.Internals;

namespace RAGENativeUI.PauseMenu
{
    public class TabView : ScrollableListBase<TabItem>
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

            Name = Game.LocalPlayer.Name;
            TemporarilyHidden = false;
            CanLeave = true;
            PauseGame = false;
            PlayBackgroundEffect = true;

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
        public List<TabItem> Tabs { get; set; } = new List<TabItem>();

        protected override List<TabItem> Items 
        { 
            get => Tabs; 
            set => Tabs = value; 
        }

        public int FocusLevel { get; set; }
        public bool TemporarilyHidden { get; set; }
        public bool CanLeave { get; set; }
        public bool PauseGame { get; set; }
        public bool HideTabs { get; set; }
        public bool ScrollTabs { get; set; } = true;
        public Color SelectedTabHighlight { get; set; } = Color.DodgerBlue;
        /// <summary>
        /// Gets or sets whether the background effect plays when the menu is open.
        /// </summary>
        public bool PlayBackgroundEffect { get; set; }

        public event EventHandler OnMenuClose;

        public bool Visible
        {
            get { return _visible; }
            set
            {
                // if the value is not changed, then don't change any properties or call any methods to avoid false data
                if (_visible == value)
                    return;

                _visible = value;

                if (value)
                {
                    Shared.NumberOfVisiblePauseMenus++;
                    NumberOfVisiblePauseMenus++;
                    N.SetPlayerControl(Game.LocalPlayer, false, 0);
                    if (PlayBackgroundEffect)
                        N.AnimPostFxPlay("MinigameTransitionIn", 0, true);
                    if (PauseGame)
                        Game.IsPaused = true;
                }
                else
                {
                    CleanUp(this);
                    Shared.NumberOfVisiblePauseMenus--;
                    NumberOfVisiblePauseMenus--;
                    if (PauseGame)
                        Game.IsPaused = false;
                }
            }
        }

        [Obsolete("Use CurrentSelection instead", true)]
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

            // avoid recalculating unnecessarily
            var curTab = CurrentItem;

            if (curTab == null)
            {
                RefreshIndex();
                return;
            }

            if (Common.IsDisabledControlJustPressed(0, GameControl.CellphoneLeft) && FocusLevel == 0)
            {
                MoveToPreviousItem();
                Common.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
            }

            else if (Common.IsDisabledControlJustPressed(0, GameControl.CellphoneRight) && FocusLevel == 0)
            {
                MoveToNextItem();
                Common.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
            }

            else if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendAccept) && FocusLevel == 0)
            {
                if (curTab.CanBeFocused)
                {
                    curTab.Focused = true;
                    curTab.JustOpened = true;
                    FocusLevel = 1;
                }
                else
                {
                    curTab.JustOpened = true;
                    curTab.OnActivated();
                }

                Common.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
            }

            else if (Common.IsDisabledControlJustPressed(0, GameControl.CellphoneCancel) && FocusLevel == 1)
            {
                curTab.Focused = false;
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
                    FocusLevel = 0;
                    MoveToPreviousItem();
                    Common.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                }

                else if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendRb))
                {
                    FocusLevel = 0;
                    MoveToNextItem();
                    Common.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                }
            }

            if (Tabs.Count > 0) curTab.ProcessControls();
        }

        public override int CurrentSelection 
        { 
            get => base.CurrentSelection; 
            set 
            {
                var curTab = CurrentItem;
                if (curTab != null)
                {
                    CurrentItem.Focused = false;
                    CurrentItem.Selected = false;
                    CurrentItem.Visible = false;
                }

                base.CurrentSelection = value;

                var newTab = CurrentItem;
                if (newTab != null)
                {
                    newTab.Selected = true;
                    newTab.Focused = false;
                    newTab.Visible = true;
                }
            }
        }

        public override void RefreshIndex()
        {
            base.RefreshIndex();

            foreach (TabItem item in Tabs)
            {
                item.Focused = false;
                item.Selected = false;
                item.Visible = false;
            }

            CurrentSelection = Items.Count > 0 ? 0 : -1;
            FocusLevel = 0;
            
            var curTab = CurrentItem;
            if(curTab != null)
            {
                curTab.Selected = true;
                curTab.Focused = false;
                curTab.Visible = true;
            }
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

                float activeTabBarWidth = res.Width - 2 * safe.X;

                if (ScrollTabs)
                {
                    float maxTabWidth = 0;
                    foreach (var tab in Items)
                    {
                        maxTabWidth = Math.Max(maxTabWidth, ResText.MeasureStringWidth(tab.Title.ToUpper(), Common.EFont.ChaletLondon, 0.35f) + 5);
                    }

                    MaxItemsOnScreen = (int)Math.Max(3, Math.Floor(activeTabBarWidth / maxTabWidth));
                }
                else
                {
                    MaxItemsOnScreen = Tabs.Count;
                }

                int itemsOnScreen = Math.Min(MaxItemsOnScreen, Tabs.Count);

                // Draw arrows if there are more items than fit in the tab menu
                if (Tabs.Count > itemsOnScreen)
                {
                    int leftArrowAlpha = (minItem == CurrentSelection) ? 255 : 50;
                    int rightArrowAlpha = (maxItem == CurrentSelection) ? 255 : 50;

                    Sprite.Draw(UIMenu.CommonTxd, UIMenu.ArrowLeftTextureName, safe.AddPoints(new Point(-45, 0)), new Size(40, 40), 0, Color.FromArgb(leftArrowAlpha, Color.White), true);
                    Sprite.Draw(UIMenu.CommonTxd, UIMenu.ArrowRightTextureName, safe.AddPoints(new Point((int)activeTabBarWidth + 5, 0)), new Size(40, 40), 0, Color.FromArgb(rightArrowAlpha, Color.White), true);
                }

                int selectedTabWidth = (int)Math.Max(activeTabBarWidth / itemsOnScreen, ResText.MeasureStringWidth(CurrentItem.Title.ToUpper(), Common.EFont.ChaletLondon, 0.35f));
                int otherTabWidth = (int)((activeTabBarWidth - selectedTabWidth - ((itemsOnScreen - 1) * 5)) / Math.Max(1, itemsOnScreen - 1));
                int leftPadding = 0;

                foreach ((int i, int tabIndex, TabItem tab, bool selectedItem) in IterateVisibleItems())
                {
                    int tabWidth = selectedItem ? selectedTabWidth : otherTabWidth;
                    string tabLabel = tab.Title.ToUpper();
                    for (int textChars = tabLabel.Length; textChars > 5; textChars--)
                    {
                        tabLabel = tabLabel.Substring(0, textChars);
                        int textWidth = (int)ResText.MeasureStringWidth(tabLabel, Common.EFont.ChaletLondon, 0.35f);
                        if (textWidth <= tabWidth) break;
                    }

                    if (tabLabel.Length < tab.Title.Length) tabLabel += "...";


                    //Game.DisableControlAction(0, GameControl.CursorX, false);
                    //Game.DisableControlAction(0, GameControl.CursorY, false);

                    //bool hovering = UIMenu.IsMouseInBounds(safe.AddPoints(new Point((tabWidth + 5) * i, 0)), new Size(tabWidth, 40));

                    var tabBgColor = selectedItem ? Color.White : /*hovering ? Color.FromArgb(100, 50, 50, 50) :*/ (tab.Skipped ? Color.DarkSlateGray : Color.Black);
                    var tabTextColor = selectedItem ? Color.Black : (tab.Skipped ? Color.LightGray : Color.White);
                    ResRectangle.Draw(safe.AddPoints(new Point(leftPadding, 0)), new Size(tabWidth, 40), Color.FromArgb(selectedItem ? 255 : 200, tabBgColor));
                    ResText.Draw(tabLabel, safe.AddPoints(new Point((tabWidth / 2) + leftPadding, 5)), 0.35f, tabTextColor, Common.EFont.ChaletLondon, ResText.Alignment.Centered, false, false, Size.Empty);

                    if (selectedItem)
                    {
                        ResRectangle.Draw(safe.SubtractPoints(new Point(-leftPadding, 10)), new Size(tabWidth, 10), SelectedTabHighlight);
                    }

                    leftPadding += tabWidth + 5;

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

            CurrentItem?.Draw();
        }

        public void DrawTextures(Rage.Graphics g)
        {
            if (!Visible) return;

            CurrentItem?.DrawTextures(g);
        }

        private static readonly StaticFinalizer cleanUpFinalizer = new StaticFinalizer(() =>
        {
            if (NumberOfVisiblePauseMenus > 0)
            {
                CleanUp();
            }
        });

        private static void CleanUp(TabView view = null)
        {
            N.SetPlayerControl(Game.LocalPlayer, true, 0);
            if (view?.PlayBackgroundEffect ?? true)
                N.AnimPostFxStop("MinigameTransitionIn");
        }
    }
}
