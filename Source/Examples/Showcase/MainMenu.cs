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

            AddItem(new UIMenuItem("Dummy1"));
            AddItem(new UIMenuItem("Dummy2"));
            AddItem(new UIMenuItem("Dummy3"));
            AddItem(new UIMenuItem("Dummy4"));
            AddItem(new UIMenuItem("Dummy5"));
            AddItem(new UIMenuItem("Dummy6"));

            Offset = new System.Drawing.Point(510, 0);
            var p = new UIMenuStatsPanel();
            int i = 0;
            for (float perc = -0.6f; perc <= 4.0f; perc += 0.4f, i++)
            {
                var upgrade = new[] { -0.2f, 0.0f, 0.2f }[i % 3];
                p.Stats.Add(new UIMenuStatsPanel.Stat("Stat"+ perc, perc, upgrade));
            }
            Panel = p;
        }
    }
}
