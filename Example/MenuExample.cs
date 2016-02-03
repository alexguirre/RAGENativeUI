[assembly: Rage.Attributes.Plugin("MenuExample", Author = "Guad / alexguirre", Description = "Example using RAGENativeUI")]

namespace MenuExample
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using System.Drawing;
    using Rage;
    using RAGENativeUI;
    using RAGENativeUI.Elements;

    public static class EntryPoint
    {
        private static UIMenu mainMenu;
        private static UIMenu newMenu;

        private static UIMenuCheckboxItem ketchupCheckbox;
        private static UIMenuListItem dishesListItem;
        private static UIMenuItem cookItem;
        private static UIMenuItem spawnCar;
        private static UIMenuListItem carsList;
        private static UIMenuColoredItem coloredItem;

        private static MenuPool _menuPool;

        public static void Main()
        {
            Game.FrameRender += Process;

            _menuPool = new MenuPool();

            mainMenu = new UIMenu("RAGENative UI", "~b~RAGENATIVEUI SHOWCASE");
            mainMenu.SetKey(Common.MenuControls.Up, Keys.W);
            mainMenu.SetKey(Common.MenuControls.Down, Keys.S);
            mainMenu.SetKey(Common.MenuControls.Left, Keys.A);
            mainMenu.SetKey(Common.MenuControls.Right, Keys.D);


            _menuPool.Add(mainMenu);

            mainMenu.AddItem(ketchupCheckbox = new UIMenuCheckboxItem("Add ketchup?", false, "Do you wish to add ketchup?"));

            var foods = new List<dynamic>
            {
                "Banana",
                "Apple",
                "Pizza",
                "Quartilicious",
                0xF00D, // Dynamic!
            };
            mainMenu.AddItem(dishesListItem = new UIMenuListItem("Food", foods, 0));
            mainMenu.AddItem(cookItem = new UIMenuItem("Cook!", "Cook the dish with the appropiate ingredients and ketchup."));

            var menuItem = new UIMenuItem("Go to another menu.");
            mainMenu.AddItem(menuItem);
            cookItem.SetLeftBadge(UIMenuItem.BadgeStyle.Star);
            cookItem.SetRightBadge(UIMenuItem.BadgeStyle.Tick);

            var carsModel = new List<dynamic>
            {
                "Adder",
                "Bullet",
                "Police",
                "Police2",
                "Asea",
                "FBI",
                "FBI2",
                "Firetruk",
                "Ambulance",
                "Rhino",
            };
            carsList = new UIMenuListItem("Cars Models", carsModel, 0);
            mainMenu.AddItem(carsList);

            spawnCar = new UIMenuItem("Spawn Car");
            mainMenu.AddItem(spawnCar);

            coloredItem = new UIMenuColoredItem("Color!", System.Drawing.Color.Red, System.Drawing.Color.Blue);
            mainMenu.AddItem(coloredItem);


            mainMenu.RefreshIndex();

            mainMenu.OnItemSelect += OnItemSelect;
            mainMenu.OnListChange += OnListChange;
            mainMenu.OnCheckboxChange += OnCheckboxChange;
            mainMenu.OnIndexChange += OnItemChange;


            newMenu = new UIMenu("RAGENative UI", "~b~RAGENATIVEUI SHOWCASE");
            _menuPool.Add(newMenu);
            for (int i = 0; i < 35; i++)
            {
                newMenu.AddItem(new UIMenuItem("PageFiller " + i.ToString(), "Sample description that takes more than one line. Moreso, it takes way more than two lines since it's so long. Wow, check out this length!"));
            }
            newMenu.RefreshIndex();
            mainMenu.BindMenuToItem(newMenu, menuItem);

            while (true)
                GameFiber.Yield();
        }


        public static void OnItemChange(UIMenu sender, int index)
        {
            sender.MenuItems[index].SetLeftBadge(UIMenuItem.BadgeStyle.None);
        }

        public static void OnCheckboxChange(UIMenu sender, UIMenuCheckboxItem checkbox, bool Checked)
        {
            if (sender != mainMenu || checkbox != ketchupCheckbox) return; // We only want to detect changes from our menu.
            Game.DisplayNotification("~r~Ketchup status: ~b~" + Checked);
        }

        public static void OnListChange(UIMenu sender, UIMenuListItem list, int index)
        {
            if (sender != mainMenu || list != dishesListItem) return; // We only want to detect changes from our menu.
            string dish = list.IndexToItem(index).ToString();
            Game.DisplayNotification("Preparing ~b~" + dish + "~w~...");
        }

        public static void OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (sender != mainMenu) return; // We only want to detect changes from our menu.
            // You can also detect the button by using index

            if (selectedItem == cookItem)   // We check which item has been selected and do different things for each.
            {
                string dish = dishesListItem.IndexToItem(dishesListItem.Index).ToString();
                bool ketchup = ketchupCheckbox.Checked;

                string output = ketchup
                    ? "You have ordered ~b~{0}~w~ ~r~with~w~ ketchup."
                    : "You have ordered ~b~{0}~w~ ~r~without~w~ ketchup.";
                Game.DisplaySubtitle(String.Format(output, dish), 5000);
            }
            else if (selectedItem == spawnCar)
            {
                GameFiber.StartNew(delegate
                {
                    new Vehicle(((string)carsList.IndexToItem(carsList.Index)).ToLower(), Game.LocalPlayer.Character.GetOffsetPositionFront(6f)).Dismiss();
                });
            }
            else if (selectedItem == coloredItem)
            {
                GameFiber.StartNew(delegate
                {
                    Game.DisplaySubtitle("~h~~r~COLOR", 500);
                    GameFiber.Sleep(500);
                    Game.DisplaySubtitle("~h~~b~COLOR", 500);
                    GameFiber.Sleep(500);
                    Game.DisplaySubtitle("~h~~g~COLOR", 500);
                    GameFiber.Sleep(500);
                    Game.DisplaySubtitle("~h~~o~COLOR", 500);
                    GameFiber.Sleep(500);
                    Game.DisplaySubtitle("~h~~d~COLOR", 500);
                });
                GameFiber.StartNew(delegate
                {
                    for (int i = 0; i < 100; i++)
                    {
                        coloredItem.HighlightColor = Color.FromArgb(MathHelper.GetRandomInteger(256), MathHelper.GetRandomInteger(256), MathHelper.GetRandomInteger(256));
                        coloredItem.HighlightedTextColor = Color.FromArgb(MathHelper.GetRandomInteger(256), MathHelper.GetRandomInteger(256), MathHelper.GetRandomInteger(256));
                        coloredItem.MainColor = Color.FromArgb(MathHelper.GetRandomInteger(256), MathHelper.GetRandomInteger(256), MathHelper.GetRandomInteger(256));
                        coloredItem.TextColor = Color.FromArgb(MathHelper.GetRandomInteger(256), MathHelper.GetRandomInteger(256), MathHelper.GetRandomInteger(256));
                        GameFiber.Sleep(10);
                    }
                });
            }
        }

        public static void Process(object sender, GraphicsEventArgs e)
        {
            if (Game.IsKeyDown(Keys.F5) && !_menuPool.IsAnyMenuOpen()) // Our menu on/off switch.
                mainMenu.Visible = !mainMenu.Visible;

            _menuPool.ProcessMenus();       // Procces all our menus: draw the menu and procces the key strokes and the mouse. 
        }
    }
}


