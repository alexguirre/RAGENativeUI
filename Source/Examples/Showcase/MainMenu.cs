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
        }
    }
}
