[assembly: Rage.Attributes.Plugin("MenuExample", Author = "Guad / alexguirre", Description = "Example using RAGENativeUI")]

namespace MenuExample
{
    using System;
    using System.Windows.Forms;
    using System.Drawing;

    using Rage;

    using RAGENativeUI;
    using RAGENativeUI.Elements;

    public static class EntryPoint
    {
        private static GameFiber MenusProcessFiber;

        private static UIMenu mainMenu;
        private static UIMenu newMenu;

        private static UIMenuCheckboxItem ketchupCheckbox;
        private static UIMenuListItem dishesListItem;
        private static UIMenuItem cookItem;
        private static UIMenuItem spawnCar;
        private static UIMenuListItem carsList;
        private static UIMenuItem coloredItem;

        private static MenuPool menuPool;

        public static void Main()
        {
            // Create a fiber to process our menus
            MenusProcessFiber = new GameFiber(ProcessLoop);

            // Create the MenuPool to easily process our menus
            menuPool = new MenuPool();

            // Create our main menu
            mainMenu = new UIMenu("RAGENative UI", "~b~RAGENATIVEUI SHOWCASE");

            // Add our main menu to the MenuPool
            menuPool.Add(mainMenu);

            // create our items and add them to our main menu
            mainMenu.AddItem(ketchupCheckbox = new UIMenuCheckboxItem("Add ketchup?", false, "Do you wish to add ketchup?"));

            mainMenu.AddItem(dishesListItem = new UIMenuListItem("Food", "",
                                                                    "Banana",
                                                                    "Apple",
                                                                    "Pizza",
                                                                    "Quartilicious",
                                                                    0xF00D));
            mainMenu.AddItem(cookItem = new UIMenuItem("Cook!", "Cook the dish with the appropiate ingredients and ketchup."));

            var menuItem = new UIMenuItem("Go to another menu.");
            mainMenu.AddItem(menuItem);
            cookItem.SetLeftBadge(UIMenuItem.BadgeStyle.Star);
            cookItem.SetRightBadge(UIMenuItem.BadgeStyle.Tick);

            carsList = new UIMenuListItem("Cars Models", "",
                                            "Adder",
                                            "Bullet",
                                            "Police",
                                            "Police2",
                                            "Asea",
                                            "FBI",
                                            "FBI2",
                                            "Firetruk",
                                            "Ambulance",
                                            "Rhino");
            mainMenu.AddItem(carsList);

            spawnCar = new UIMenuItem("Spawn Car");
            mainMenu.AddItem(spawnCar);

            coloredItem = new UIMenuItem("Color!");
            mainMenu.AddItem(coloredItem);

            mainMenu.RefreshIndex();

            mainMenu.OnItemSelect += OnItemSelect;
            mainMenu.OnListChange += OnListChange;
            mainMenu.OnCheckboxChange += OnCheckboxChange;
            mainMenu.OnIndexChange += OnItemChange;

            // Create another menu
            newMenu = new UIMenu("RAGENative UI", "~b~RAGENATIVEUI SHOWCASE");
            newMenu.CounterOverride = "Counter Override";
            menuPool.Add(newMenu); // add it to the menu pool
            for (int i = 0; i < 35; i++) // add items
            {
                newMenu.AddItem(new UIMenuItem("PageFiller " + i.ToString(), "Sample description that takes more than one line. More so, it takes way more than two lines since it's so long. Wow, check out this length!"));
            }
            newMenu.RefreshIndex();
            mainMenu.BindMenuToItem(newMenu, menuItem); // and bind it to an item in our main menu

            // Start our process fiber
            MenusProcessFiber.Start();

            // Continue with our plugin... in this example, hibernate to prevent it from being unloaded
            GameFiber.Hibernate();
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
            string dish = list.Collection[index].Value.ToString();
            Game.DisplayNotification("Preparing ~b~" + dish + "~w~...");
        }

        public static void OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (sender != mainMenu) return; // We only want to detect changes from our menu.
            // You can also detect the button by using index

            if (selectedItem == cookItem)   // We check which item has been selected and do different things for each.
            {
                string dish = dishesListItem.Collection[dishesListItem.Index].Value.ToString();
                bool ketchup = ketchupCheckbox.Checked;

                string output = ketchup
                    ? "You have ordered ~b~{0}~w~ ~r~with~w~ ketchup."
                    : "You have ordered ~b~{0}~w~ ~r~without~w~ ketchup.";
                Game.DisplaySubtitle(String.Format(output, dish), 5000);
            }
            else if (selectedItem == spawnCar)
            {
                GameFiber.StartNew(delegate // Start a new fiber if the code sleeps or waits and we don't want to block the MenusProcessFiber
                {
                    new Vehicle(((string)carsList.Collection[carsList.Index].Value).ToLower(), Game.LocalPlayer.Character.GetOffsetPositionFront(6f)).Dismiss();
                });
            }
            else if (selectedItem == coloredItem)
            {
                GameFiber.StartNew(delegate
                {
                    for (int i = 0; i < 100; i++)
                    {
                        coloredItem.HighlightedBackColor = Color.FromArgb(MathHelper.GetRandomInteger(256), MathHelper.GetRandomInteger(256), MathHelper.GetRandomInteger(256));
                        coloredItem.HighlightedForeColor = Color.FromArgb(MathHelper.GetRandomInteger(256), MathHelper.GetRandomInteger(256), MathHelper.GetRandomInteger(256));
                        coloredItem.BackColor = Color.FromArgb(MathHelper.GetRandomInteger(256), MathHelper.GetRandomInteger(256), MathHelper.GetRandomInteger(256));
                        coloredItem.ForeColor = Color.FromArgb(MathHelper.GetRandomInteger(256), MathHelper.GetRandomInteger(256), MathHelper.GetRandomInteger(256));
                        GameFiber.Sleep(10);
                    }
                });
            }
        }
        
        // The method that contains a loop to handle our menus
        public static void ProcessLoop()
        {
            // if we are using banners with a Rage.Texture (UIMenu.SetBannerType(...)), we need to draw them in the RawFrameRender, in this example we don't have a Rage.Texture banner so this isn't needed
            //Game.RawFrameRender += (s, e) => 
            //{
            //    _menuPool.DrawBanners(e.Graphics);
            //};

            while (true)
            {
                GameFiber.Yield();

                if (Game.IsKeyDown(Keys.F5) && !menuPool.IsAnyMenuOpen()) // Our menu on/off switch.
                {
                    mainMenu.Visible = !mainMenu.Visible;
                }

                menuPool.ProcessMenus();       // Process all our menus: draw the menu and process the key strokes and the mouse. 
            }
        }
    }
}


