namespace RNUIExamples.Showcase
{
    using RAGENativeUI;
    using RAGENativeUI.Elements;

    internal sealed class MainMenu : UIMenu
    {
        public MainMenu() : base(Plugin.MenuTitle, "SHOWCASE")
        {
            Plugin.Pool.Add(this);

            {
                UIMenuItem item = new UIMenuItem("Menus", $"Showcases the ~b~{nameof(UIMenu)}~s~ class");

                AddItem(item);
                BindMenuToItem(new Menus(), item);
            }

            {
                UIMenuItem item = new UIMenuItem("Timer Bars", "Showcases the available timer bars");

                AddItem(item);
                BindMenuToItem(new TimerBars(), item);
            }

            {
                UIMenuItem item = new UIMenuItem("Big Messages", $"Showcases the ~b~{nameof(BigMessageThread)}~s~ and ~b~{nameof(BigMessageHandler)}~s~ classes.");

                AddItem(item);
                BindMenuToItem(new BigMessages(), item);
            }

            {
                UIMenuItem item = new UIMenuItem("Instructional Buttons", $"Showcases the ~b~{nameof(RAGENativeUI.Elements.InstructionalButtons)}~s~ class.");

                AddItem(item);
                BindMenuToItem(new InstructionalButtons(), item);
            }

            {
                UIMenuItem item = new UIMenuItem("Elements", $"Showcases the ~b~{nameof(ResRectangle)}~s~ and ~b~{nameof(ResText)}~s~ classes.");

                AddItem(item);
                BindMenuToItem(new Elements(), item);
            }

            {
                UIMenuItem item = new UIMenuItem("Localization", $"Showcases the ~b~{nameof(Localization)}~s~ class.");

                AddItem(item);
                BindMenuToItem(new LocalizationMenu(), item);
            }

            var dummy1 = new UIMenuItem("Dummy1");
            var dummy2 = new UIMenuItem("Dummy2");
            var dummy3 = new UIMenuItem("Dummy3");
            AddItems(dummy1, dummy2, dummy3);
            //AddItem(new UIMenuItem("Dummy2"));
            //AddItem(new UIMenuItem("Dummy3"));
            //AddItem(new UIMenuItem("Dummy4"));
            //AddItem(new UIMenuItem("Dummy5"));
            //AddItem(new UIMenuItem("Dummy6"));

            Offset = new System.Drawing.Point(510, 0);
            var p1 = new UIMenuSliderPanel() { Value = 0.5f, BackgroundColor = System.Drawing.Color.FromArgb(90, HudColor.Black.GetColor()) };
            p1.Markers.Add(new UIMenuSliderPanel.Marker(0.25f, HudColor.Bronze.GetColor()));
            p1.Markers.Add(new UIMenuSliderPanel.Marker(0.5f, HudColor.Silver.GetColor()));
            p1.Markers.Add(new UIMenuSliderPanel.Marker(0.75f, HudColor.Gold.GetColor()));
            var p2 = new UIMenuGridPanel();
            var p3 = new UIMenuStatsPanel();
            p3.Stats.Add(new UIMenuStatsPanel.Stat("Slider Value", p1.Value, 0.0f));
            p3.Stats.Add(new UIMenuStatsPanel.Stat("Grid Value X", p2.Value.X, 0.0f));
            p3.Stats.Add(new UIMenuStatsPanel.Stat("Grid Value Y", p2.Value.Y, 0.0f));

            p1.ValueChanged += (s, newValue, oldValue) =>
            {
                p3.Stats[0].Percentage = newValue;
                p3.Stats[0].Upgrade = oldValue - newValue;
            };
            p2.ValueChanged += (s, newValue, oldValue) =>
            {
                p3.Stats[1].Percentage = newValue.X;
                p3.Stats[1].Upgrade = oldValue.X - newValue.X;
                p3.Stats[2].Percentage = newValue.Y;
                p3.Stats[2].Upgrade = oldValue.Y - newValue.Y;
            };

            dummy1.Panels.Add(p1);
            dummy1.Panels.Add(p2);
            dummy3.Panels.Add(p3);
        }
    }
}
