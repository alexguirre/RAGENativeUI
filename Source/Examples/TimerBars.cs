namespace RNUIExamples
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Linq;
    using Rage;
    using RAGENativeUI;
    using RAGENativeUI.Elements;

    internal class TimerBars : UIMenu
    {
        private TimerBarPool tbPool;
        private TextTimerBar text;
        private BarTimerBar progressBar;
        private CheckpointsTimerBar checkpoints;
        private IconsTimerBar icons;

        public TimerBars() : base(Plugin.MenuTitle, "TIMER BARS")
        {
            Plugin.Pool.Add(this);

            CreateTimerBars();

            CreateMenuItems();

            GameFiber.StartNew(() => { while (true) { GameFiber.Yield(); tbPool.Draw(); } });
        }

        private void CreateTimerBars()
        {
            tbPool = new TimerBarPool();
            text = new TextTimerBar("LABEL", "TEXT");
            progressBar = new BarTimerBar("PROGRESS");
            checkpoints = new CheckpointsTimerBar("CHECKPOINTS", 4);
            icons = new IconsTimerBar("ICONS");
        }

        private void CreateMenuItems()
        {
            {
                var visible = new UIMenuCheckboxItem("Visible", false);
                visible.CheckboxEvent += (s, v) =>
                {
                    if (v)
                    {
                        tbPool.Add(text);
                        tbPool.Add(progressBar);
                        tbPool.Add(checkpoints);
                        tbPool.Add(icons);
                    }
                    else
                    {
                        tbPool.Clear();
                    }
                };

                AddItem(visible);
            }

            UIMenuItem textBindItem = new UIMenuItem("Text", $"Demonstrates the ~b~{nameof(TextTimerBar)}~s~ class.");
            UIMenuItem progressBarBindItem = new UIMenuItem("Progress Bar", $"Demonstrates the ~b~{nameof(BarTimerBar)}~s~ class.");
            UIMenuItem checkpointsBindItem = new UIMenuItem("Checkpoints", $"Demonstrates the ~b~{nameof(CheckpointsTimerBar)}~s~ class.");
            UIMenuItem iconsBindItem = new UIMenuItem("Icons", $"Demonstrates the ~b~{nameof(IconsTimerBar)}~s~ class.");

            UIMenu textMenu = new UIMenu(Title, Subtitle + ": TEXT");
            UIMenu progressBarMenu = new UIMenu(Title, Subtitle + ": PROGRESS BAR");
            UIMenu checkpointsMenu = new UIMenu(Title, Subtitle + ": CHECKPOINTS");
            UIMenu iconsMenu = new UIMenu(Title, Subtitle + ": ICONS");

            Plugin.Pool.Add(textMenu);
            Plugin.Pool.Add(progressBarMenu);
            Plugin.Pool.Add(checkpointsMenu);
            Plugin.Pool.Add(iconsMenu);

            AddItem(textBindItem);
            AddItem(progressBarBindItem);
            AddItem(checkpointsBindItem);
            AddItem(iconsBindItem);

            BindMenuToItem(textMenu, textBindItem);
            BindMenuToItem(progressBarMenu, progressBarBindItem);
            BindMenuToItem(checkpointsMenu, checkpointsBindItem);
            BindMenuToItem(iconsMenu, iconsBindItem);

            // text
            {
                AddCommonMenuItems(textMenu, text);
            }

            // progressBar
            {
                var percentage = new UIMenuNumericScrollerItem<float>(nameof(BarTimerBar.Percentage), "", 0.0f, 1.0f, 0.05f);
                percentage.Value = progressBar.Percentage;
                percentage.IndexChanged += (s, o, n) =>
                {
                    progressBar.Percentage = percentage.Value;
                };

                var foreground = NewColorsItem(nameof(BarTimerBar.ForegroundColor), "");
                progressBar.ForegroundColor = (foreground.SelectedItem = HudColor.Red).GetColor();
                foreground.IndexChanged += (s, o, n) =>
                {
                    progressBar.ForegroundColor = foreground.SelectedItem.GetColor();
                };

                var background = NewColorsItem(nameof(BarTimerBar.BackgroundColor), "");
                progressBar.BackgroundColor = (background.SelectedItem = HudColor.RedDark).GetColor();
                background.IndexChanged += (s, o, n) =>
                {
                    progressBar.BackgroundColor = background.SelectedItem.GetColor();
                };

                progressBarMenu.AddItem(percentage);
                progressBarMenu.AddItem(foreground);
                progressBarMenu.AddItem(background);
                AddCommonMenuItems(progressBarMenu, progressBar);
            }

            // checkpoints
            {
                var num = new UIMenuNumericScrollerItem<int>("Number of Checkpoints", "", 0, 16, 1);
                num.Value = checkpoints.Checkpoints.Count;
                num.IndexChanged += (s, o, n) =>
                {
                    checkpoints.Checkpoints.Clear();
                    for (int i = 0; i < num.Value; i++)
                    {
                        checkpoints.Checkpoints.Add(new TimerBarCheckpoint());
                    }
                };

                checkpointsMenu.AddItem(num);
                AddCommonMenuItems(checkpointsMenu, checkpoints);
            }

            // icons
            {
                var num = new UIMenuNumericScrollerItem<int>("Number of Icons", "", 0, 8, 1);
                var type = new UIMenuListScrollerItem<(string Name, Func<TimerBarIcon> Creator)>("Type", "", new (string, Func<TimerBarIcon>)[]
                {
                    ("Rocket", () => TimerBarIcon.Rocket),
                    ("Spike", () => TimerBarIcon.Spike),
                    ("Boost", () => TimerBarIcon.Boost),
                    ("Beast", () => TimerBarIcon.Beast),
                    ("Random", () => TimerBarIcon.Random),
                    ("Slow Time", () => TimerBarIcon.SlowTime),
                    ("Swap", () => TimerBarIcon.Swap),
                    ("Testosterone", () => TimerBarIcon.Testosterone),
                    ("Thermal", () => TimerBarIcon.Thermal),
                    ("Weed", () => TimerBarIcon.Weed),
                    ("Hidden", () => TimerBarIcon.Hidden),
                    ("Time", () => TimerBarIcon.Time)
                })
                {
                    Formatter = v => v.Name
                };
                var color = NewColorsItem("Icons Color", "");
                color.IndexChanged += (s, o, n) =>
                {
                    Color c = color.SelectedItem.GetColor();
                    foreach (TimerBarIcon i in icons.Icons)
                    {
                        i.Color = c;
                    }
                };


                void updateIcons()
                {
                    icons.Icons.Clear();
                    Color c = color.SelectedItem.GetColor();
                    for (int i = 0; i < num.Value; i++)
                    {
                        TimerBarIcon ic = type.SelectedItem.Creator();
                        ic.Color = c;
                        icons.Icons.Add(ic);
                    }
                }

                num.IndexChanged += (s, o, n) => updateIcons();
                type.IndexChanged += (s, o, n) => updateIcons();
                num.Value = 1;

                iconsMenu.AddItem(num);
                iconsMenu.AddItem(type);
                iconsMenu.AddItem(color);
                AddCommonMenuItems(iconsMenu, icons);
            }
        }

        private void AddCommonMenuItems(UIMenu menu, TimerBarBase tb)
        {
            var accent = NewColorsItem(nameof(BarTimerBar.Accent), "Select to toggle the timer bar accent.");
            accent.ScrollingEnabled = false;
            accent.Activated += (m, s) =>
            {
                accent.ScrollingEnabled = !accent.ScrollingEnabled;
                tb.Accent = accent.ScrollingEnabled ? (Color?)accent.SelectedItem.GetColor() : null;
            };
            accent.IndexChanged += (s, o, n) =>
            {
                tb.Accent = accent.SelectedItem.GetColor();
            };

            var highlight = NewColorsItem(nameof(BarTimerBar.Highlight), "Select to toggle the timer bar highlight.");
            highlight.ScrollingEnabled = false;
            highlight.Activated += (m, s) =>
            {
                highlight.ScrollingEnabled = !highlight.ScrollingEnabled;
                tb.Highlight = highlight.ScrollingEnabled ? (Color?)highlight.SelectedItem.GetColor() : null;
            };
            highlight.IndexChanged += (s, o, n) =>
            {
                tb.Highlight = highlight.SelectedItem.GetColor();
            };

            menu.AddItem(accent);
            menu.AddItem(highlight);
        }

        private UIMenuListScrollerItem<HudColor> NewColorsItem(string text, string description)
            => new UIMenuListScrollerItem<HudColor>(text, description, (HudColor[])Enum.GetValues(typeof(HudColor)))
            {
                // custom formatter that adds whitespace between words (i.e. "RedDark" -> "Red Dark")
                Formatter = v => v.ToString().Aggregate("", (acc, c) => acc + (acc.Length > 0 && char.IsUpper(c) ? " " : "") + c)
            };
    }
}
