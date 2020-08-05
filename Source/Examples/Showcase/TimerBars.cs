namespace RNUIExamples.Showcase
{
    using System;
    using System.Drawing;
    using System.Linq;
    using Rage;
    using RAGENativeUI;
    using RAGENativeUI.Elements;

    using static Util;

    internal sealed class TimerBars : UIMenu
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
                        tbPool.Add(text, progressBar, checkpoints, icons);
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

            UIMenu textMenu = new UIMenu(TitleText, SubtitleText + ": TEXT");
            UIMenu progressBarMenu = new UIMenu(TitleText, SubtitleText + ": PROGRESS BAR");
            UIMenu checkpointsMenu = new UIMenu(TitleText, SubtitleText + ": CHECKPOINTS");
            UIMenu iconsMenu = new UIMenu(TitleText, SubtitleText + ": ICONS");

            Plugin.Pool.Add(textMenu, progressBarMenu, checkpointsMenu, iconsMenu);

            AddItems(textBindItem, progressBarBindItem, checkpointsBindItem, iconsBindItem);

            BindMenuToItem(textMenu, textBindItem);
            BindMenuToItem(progressBarMenu, progressBarBindItem);
            BindMenuToItem(checkpointsMenu, checkpointsBindItem);
            BindMenuToItem(iconsMenu, iconsBindItem);

            // text
            {
                var textItem = new UIMenuItem(nameof(TextTimerBar.Text), "Select to edit the timer bar text.")
                                    .WithTextEditing(() => text.Text,
                                                     s => text.Text = s);

                textMenu.AddItem(textItem);
                AddCommonMenuItems(textMenu, text);
            }

            // progressBar
            {
                var percentage = new UIMenuNumericScrollerItem<float>(nameof(BarTimerBar.Percentage), "", 0.0f, 1.0f, 0.01f)
                                        .WithTextEditing();
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

                var markers = new UIMenuCheckboxItem("Markers", false);
                markers.CheckboxEvent += (m, v) =>
                {
                    if (v)
                    {
                        progressBar.Markers.Add(new TimerBarMarker(0.25f));
                        progressBar.Markers.Add(new TimerBarMarker(0.50f));
                        progressBar.Markers.Add(new TimerBarMarker(0.75f));
                    }
                    else
                    {
                        progressBar.Markers.Clear();
                    }
                };

                progressBarMenu.AddItems(percentage, foreground, background, markers);
                progressBarMenu.WithFastScrollingOn(percentage);
                AddCommonMenuItems(progressBarMenu, progressBar);
            }

            // checkpoints
            {
                var state = new UIMenuListScrollerItem<TimerBarCheckpointState>("State", "", (TimerBarCheckpointState[])Enum.GetValues(typeof(TimerBarCheckpointState)));
                state.IndexChanged += (s, o, n) =>
                {
                    foreach (var cp in checkpoints.Checkpoints)
                    {
                        cp.State = state.SelectedItem;
                    }
                };

                var crossedOut = new UIMenuCheckboxItem("Crossed Out", false);
                crossedOut.CheckboxEvent += (s, v) =>
                {
                    foreach (var cp in checkpoints.Checkpoints)
                    {
                        cp.IsCrossedOut = v;
                    }
                };

                var color = NewColorsItem("Color", "");
                color.IndexChanged += (s, o, n) =>
                {
                    Color c = color.SelectedItem.GetColor();
                    foreach (var cp in checkpoints.Checkpoints)
                    {
                        cp.Color = c;
                    }
                };

                var num = new UIMenuNumericScrollerItem<int>("Number of Checkpoints", "", 0, 16, 1)
                            .WithTextEditing();
                num.Value = checkpoints.Checkpoints.Count;
                num.IndexChanged += (s, o, n) =>
                {
                    if (checkpoints.Checkpoints.Count > num.Value)
                    {
                        checkpoints.Checkpoints = checkpoints.Checkpoints.Take(num.Value).ToList();
                    }
                    else
                    {
                        Color c = color.SelectedItem.GetColor();
                        for (int i = checkpoints.Checkpoints.Count; i < num.Value; i++)
                        {
                            var cp = new TimerBarCheckpoint();
                            cp.Color = c;
                            cp.IsCrossedOut = crossedOut.Checked;
                            cp.State = state.SelectedItem;
                            checkpoints.Checkpoints.Add(cp);
                        }
                    }
                };

                checkpointsMenu.AddItems(num, state, crossedOut, color);
                AddCommonMenuItems(checkpointsMenu, checkpoints);
            }

            // icons
            {
                var num = new UIMenuNumericScrollerItem<int>("Number of Icons", "", 0, 8, 1).WithTextEditing();
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

                iconsMenu.AddItems(num, type, color);
                AddCommonMenuItems(iconsMenu, icons);
            }
        }

        private void AddCommonMenuItems(UIMenu menu, TimerBarBase tb)
        {
            var label = new UIMenuItem(nameof(TimerBarBase.Label), "Select to edit the timer bar label.")
                            .WithTextEditing(() => tb.Label,
                                             s => tb.Label = s);

            var accent = NewColorsItem(nameof(TimerBarBase.Accent), "Select to toggle the timer bar accent.");
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

            var highlight = NewColorsItem(nameof(TimerBarBase.Highlight), "Select to toggle the timer bar highlight.");
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

            menu.AddItems(label, accent, highlight);
        }
    }
}
