[assembly: Rage.Attributes.Plugin("MenuExample", Author = "alexguirre", Description = "Example using RAGENativeUI")]

namespace MenuExample
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using Rage;
    using Rage.Native;
    using RAGENativeUI;
    using RAGENativeUI.Elements;

    public static class EntryPoint
    {
        private static UIMenu mainMenu;    
        private static UIMenu newMenu;
        
        private static MenuCheckboxItem ketchupCheckbox;     
        private static MenuListItem dishesListItem;
        private static NativeMenuItem cookItem;
        private static NativeMenuItem spawnCar;
        private static MenuListItem carsList;

        private static MenuPool _menuPool;

        public static void Main()
        {
            Game.FrameRender += Process;
            
            _menuPool = new MenuPool();

            mainMenu = new UIMenu("RAGENative UI", "~b~RAGENATIVEUI SHOWCASE");

            _menuPool.Add(mainMenu);

            mainMenu.AddItem(ketchupCheckbox = new MenuCheckboxItem("Add ketchup?", false, "Do you wish to add ketchup?"));
            
            var foods = new List<dynamic>
            {
                "Banana",
                "Apple",
                "Pizza",
                "Quartilicious",
                0xF00D, // Dynamic!
            };
            mainMenu.AddItem(dishesListItem = new MenuListItem("Food", foods, 0));
            mainMenu.AddItem(cookItem = new NativeMenuItem("Cook!", "Cook the dish with the appropiate ingredients and ketchup."));

            var menuItem = new NativeMenuItem("Go to another menu.");
            mainMenu.AddItem(menuItem);
            cookItem.SetLeftBadge(NativeMenuItem.BadgeStyle.Star);
            cookItem.SetRightBadge(NativeMenuItem.BadgeStyle.Tick);

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
            carsList = new MenuListItem("Cars Models", carsModel, 0);
            mainMenu.AddItem(carsList);

            spawnCar = new NativeMenuItem("Spawn Car");
            mainMenu.AddItem(spawnCar);
           
            mainMenu.RefreshIndex();

            mainMenu.OnItemSelect += OnItemSelect;
            mainMenu.OnListChange += OnListChange;
            mainMenu.OnCheckboxChange += OnCheckboxChange;
            mainMenu.OnIndexChange += OnItemChange;
            

            newMenu = new UIMenu("RAGENative UI", "~b~RAGENATIVEUI SHOWCASE");
            _menuPool.Add(newMenu);
            for (int i = 0; i < 35; i++)
            {
                newMenu.AddItem(new NativeMenuItem("PageFiller " + i.ToString(), "Sample description that takes more than one line. Moreso, it takes way more than two lines since it's so long. Wow, check out this length!"));
            }
            newMenu.RefreshIndex();
            mainMenu.BindMenuToItem(newMenu, menuItem);

            while (true)
                GameFiber.Yield();
        }


        public static void OnItemChange(UIMenu sender, int index)
        {
            sender.MenuItems[index].SetLeftBadge(NativeMenuItem.BadgeStyle.None);
        }

        public static void OnCheckboxChange(UIMenu sender, MenuCheckboxItem checkbox, bool Checked)
        {
            if (sender != mainMenu || checkbox != ketchupCheckbox) return; // We only want to detect changes from our menu.
            Game.DisplayNotification("~r~Ketchup status: ~b~" + Checked);
        }

        public static void OnListChange(UIMenu sender, MenuListItem list, int index)
        {
            if (sender != mainMenu || list != dishesListItem) return; // We only want to detect changes from our menu.
            string dish = list.IndexToItem(index).ToString();
            Game.DisplayNotification("Preparing ~b~" + dish + "~w~...");
        }

        public static void OnItemSelect(UIMenu sender, NativeMenuItem selectedItem, int index)
        {
            if (sender != mainMenu) return; // We only want to detect changes from our menu.
            // You can also detect the button by using index

            if (selectedItem == cookItem)   // We check wich item has been selected and do different things for each.
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
                    new Vehicle(((string)carsList.IndexToItem(carsList.Index)).ToLower(), Game.LocalPlayer.Character.Position + Game.LocalPlayer.Character.ForwardVector * 5.6f).Dismiss();
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

