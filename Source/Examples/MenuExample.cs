namespace RNUIExamples
{
    using System;
    using System.Linq;
    using Rage;
    using Rage.Attributes;
    using Rage.Native;
    using RAGENativeUI;
    using RAGENativeUI.Elements;

    internal static class MenuExample
    {
        private static MenuPool pool;
        private static UIMenu mainMenu;

        private static void Main()
        {
            // create the pool that handles drawing and processing the menus
            pool = new MenuPool();

            // create the main menu
            mainMenu = new UIMenu("RAGENativeUI", "EXAMPLE");

            // create the menu items
            {
                var cb = new UIMenuCheckboxItem("Checkbox", false, "A checkbox menu item.");
                // show a message when toggling the checkbox
                cb.CheckboxEvent += (item, isChecked) => Game.DisplaySubtitle($"The checkbox is {(isChecked ? "" : "~r~not~s~ ")}checked");

                var spawnCar = new UIMenuItem("Spawn car", "Spawns a random car after 5 seconds.");
                spawnCar.Activated += (menu, item) =>
                {
                    // disable the item so the user cannot activate it again until the car has spawned
                    spawnCar.Enabled = false;

                    GameFiber.StartNew(() =>
                    {
                        // 5 second countdown
                        for (int sec = 5; sec > 0; sec--)
                        {
                            // show the countdown in the menu description
                            mainMenu.DescriptionOverride = $"The car will spawn in ~b~{sec}~s~ second(s).";

                            GameFiber.Sleep(1000); // sleep for 1 second
                        }

                        // remove the countdown from the description
                        mainMenu.DescriptionOverride = null;

                        // spawn the random car
                        new Vehicle(m => m.IsCar, Game.LocalPlayer.Character.GetOffsetPositionFront(5.0f)).Dismiss();

                        // wait a bit and re-enable the item
                        GameFiber.Sleep(500);
                        spawnCar.Enabled = true;
                    });
                };

                // a numeric scroller from -50 to 50 in intervals of 5
                var numbers = new UIMenuNumericScrollerItem<int>("Numbers", "A numeric scroller menu item.", -50, 50, 5);
                numbers.IndexChanged += (item, oldIndex, newIndex) => Game.DisplaySubtitle($"{oldIndex} -> {newIndex}| Selected number = {numbers.Value}");

                // a list scroller with strings
                var strings = new UIMenuListScrollerItem<string>("Strings", "A list scroller menu item with strings.", new[] { "Hello", "World", "Foo", "Bar" });
                strings.IndexChanged += (item, oldIndex, newIndex) => Game.DisplaySubtitle($"{oldIndex} -> {newIndex}| Selected string = {strings.SelectedItem}");

                // item in which the user can choose the cash amount to give or remove when activated
                var cash = new UIMenuNumericScrollerItem<int>("Cash", "Give or remove cash.", -10_000, 10_000, 50);
                // custom formatter that adds the dollar sign, the positive/negative sign and
                // colors it green when positive and red when negative (and keeps it the default color when selected, for readability)
                cash.Formatter = n => cash.Selected ? $"{n:+$#;-$#;$0}" : $"{n:~g~+$#;~r~-$#;$0}";
                // reformat the item when the menu selection changes since the formatter depends on the Selected property, and it could have changed
                mainMenu.OnIndexChange += (menu, itemIndex) => cash.Reformat();
                cash.Activated += (menu, item) =>
                {
                    if (cash.Value == 0)
                    {
                        return;
                    }

                    // give cash to the player
                    uint stat = Game.LocalPlayer.Model.Hash switch
                    {
                        0x0D7114C9 /*player_zero*/ => Game.GetHashKey("SP0_TOTAL_CASH"),
                        0x9B22DBAF /*player_one*/ => Game.GetHashKey("SP1_TOTAL_CASH"),
                        0x9B810FA2 /*player_two*/ => Game.GetHashKey("SP2_TOTAL_CASH"),
                        _ => 0
                    };

                    if (stat != 0)
                    {
                        NativeFunction.Natives.STAT_GET_INT(stat, out int totalCash, -1);
                        NativeFunction.Natives.STAT_SET_INT(stat, totalCash + cash.Value, true);
                    }
                };

                // add the items to the menu
                mainMenu.AddItems(cb, spawnCar, numbers, strings, cash);
            }

            // create a child menu
            UIMenu childMenu = new UIMenu("RAGENativeUI", "CHILD MENU");

            // create a new item in the main menu and bind the child menu to it
            {
                UIMenuItem bindItem = new UIMenuItem("Go to child menu");

                mainMenu.AddItem(bindItem);
                mainMenu.BindMenuToItem(childMenu, bindItem);


                bindItem.RightBadge = UIMenuItem.BadgeStyle.Star;
                mainMenu.OnIndexChange += (menu, index) => mainMenu.MenuItems[index].RightBadge = UIMenuItem.BadgeStyle.None;
            }

            // create the child menu items
            childMenu.AddItems(Enumerable.Range(1, 50).Select(i => new UIMenuItem($"Item #{i}")));

            // add all the menus to the pool
            pool.Add(mainMenu, childMenu);

            // start the fiber which will handle drawing and processing the menus
            GameFiber.StartNew(ProcessMenus);

            // continue with the plugin...
            Game.Console.Print("Press F6 to open the menu.");
        }

        private static void ProcessMenus()
        {
            // draw the menu banners (only needed if UIMenu.SetBannerType(Rage.Texture) is used)
            // Game.RawFrameRender += (s, e) => pool.DrawBanners(e.Graphics);

            while (true)
            {
                GameFiber.Yield();

                pool.ProcessMenus();

                if (Game.IsKeyDown(System.Windows.Forms.Keys.F6) && !UIMenu.IsAnyMenuVisible)
                {
                    mainMenu.Visible = true;
                }
            }
        }


        // a command that simulates loading the plugin
        [ConsoleCommand]
        private static void RunMenuExample() => GameFiber.StartNew(Main);
    }
}
