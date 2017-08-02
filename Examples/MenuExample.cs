namespace Examples
{
    using System;
    using System.Drawing;

    using Rage;
    using Rage.Attributes;

    using RAGENativeUI.Menus;
    using RAGENativeUI.Menus.Rendering;

    internal static class MenuExample
    {
        [ConsoleCommand(Name = "MenuExample", Description = "Example showing RAGENativeUI menu API")]
        private static void Command()
        {
            MenusManager menusMgr = new MenusManager();

            Menu menu = new Menu("title", "SUBTITLE");
            menu.Location = new PointF(480, 17);

            menu.Items.Add(new MenuItem("item #0") { Description = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa." });
            menu.Items.Add(new MenuItemCheckbox("cb #0") { State = MenuItemCheckboxState.Empty, Description = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim." });
            menu.Items.Add(new MenuItemCheckbox("cb #1") { State = MenuItemCheckboxState.Cross, Description = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim." });
            menu.Items.Add(new MenuItemCheckbox("cb #2") { State = MenuItemCheckboxState.Tick });
            menu.Items.Add(new MenuItemEnumScroller("enum scroller #0", typeof(GameControl)));
            menu.Items.Add(new MenuItemEnumScroller<GameControl>("enum scroller #1"));
            menu.Items.Add(new MenuItemNumericScroller("num scroller #0"));
            menu.Items.Add(new MenuItemNumericScroller("num scroller #1") { ThousandsSeparator = true, Minimum = -50000.0m, Maximum = 50000.0m, Value = 0.0m, Increment = 100.0m });
            menu.Items.Add(new MenuItemNumericScroller("num scroller #2") { Increment = 1.0m, Hexadecimal = true });
            menu.Items.Add(new MenuItemNumericScroller("num scroller #3") { Minimum = -1.0m, Maximum = 1.0m, Value = 0.0m, Increment = 0.00005m, DecimalPlaces = 5 });
            menu.Items.Add(new MenuItemListScroller("list scroller #0"));
            menu.Items.Add(new MenuItemListScroller("list scroller #1", new[] { "text #1", "other text #2", "and text #3" }));

            Menu subMenu = new Menu("SubMenu", "SUB!");
            subMenu.Location = menu.Location;
            for (int i = 1; i <= 999; i++)
            {
                subMenu.Items.Add(new MenuItem("item #" + i));
            }

            menu.Items.Add(new MenuItem("item with binded menu #0") { BindedMenu = subMenu, Description = "If you click this item it opens a menu with a LOT of items." });

            menu.SelectedIndexChanged += (s, oldIndex, newIndex) => { Game.DisplayHelp($"Selected index changed from #{oldIndex} to #{newIndex}"); };
            menu.VisibleChanged += (s, visible) => { Game.DisplayHelp($"Visible now: {visible}"); };

            for (int i = 0; i < menu.Items.Count; i++)
            {
                int idx = i;

                if (menu.Items[idx] is MenuItemCheckbox)
                {
                    (menu.Items[idx] as MenuItemCheckbox).StateChanged += (s, oldState, newState) => { Game.DisplayHelp($"Checkbox at index #{idx} state changed from {oldState} to {newState}"); };
                    continue;
                }

                if (menu.Items[idx] is MenuItemScroller)
                {
                    (menu.Items[idx] as MenuItemScroller).SelectedIndexChanged += (s, oldIndex, newIndex) => { Game.DisplayHelp($"Scroller at index #{idx} selected index changed from #{oldIndex} to #{newIndex}"); };
                }

                menu.Items[idx].Activated += (s, origin) => { Game.DisplayHelp($"Activated item at index #{idx}"); };
            }

            menu.Metadata["Test"] = new Vector3(50f, 75f, 100f);
            menu.Items[0].Metadata.Test = 100;
            menu.Items[0].Activated += (sender, origin) => 
            {
                Game.DisplayNotification(menu.Items[0].Metadata.Test.ToString());
                Game.DisplayNotification(menu.Metadata.Test.ToString());
                menu.Items[0].Metadata["Test"]++;
                menu.Metadata.Test += new Vector3(1f, 1f, 1f);
            };

            menusMgr.Menus.Add(menu);
            menusMgr.Menus.Add(subMenu);
            
            GameFiber.StartNew(() =>
            {
                while (true)
                {
                    GameFiber.Yield();

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.T))
                    {
                        if (menusMgr.IsAnyMenuVisible)
                        {
                            menusMgr.HideAllMenus();
                        }
                        else
                        {
                            menu.Show();
                        }
                    }


                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Add))
                    {
                        menu.MaxItemsOnScreen++;
                    }
                    else if (Game.IsKeyDown(System.Windows.Forms.Keys.Subtract) && menu.MaxItemsOnScreen > 0)
                    {
                        menu.MaxItemsOnScreen--;
                    }


                    if (Game.IsKeyDownRightNow(System.Windows.Forms.Keys.U))
                    {
                        menu.Items[0].Description = GetRandomString();
                    }

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.I))
                    {
                        if(menu.Description.TextOverride == null)
                        {
                            menu.Description.TextOverride = "Description override set! Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa.";
                        }
                        else
                        {
                            menu.Description.TextOverride = null;
                        }
                    }

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.K))
                    {
                        foreach (MenuItem m in menu.Items)
                        {
                            m.IsVisible = true;
                        }
                    }

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.L))
                    {
                        foreach (MenuItem m in menu.Items)
                        {
                            m.IsVisible = MathHelper.Choose(true, false);
                        }
                    }
                }
            });



            // gets a random alpha numeric string with whitespaces
            string GetRandomString(int minLength = 10, int maxLenght = 600)
            {
                int length = MathHelper.GetRandomInteger(minLength, maxLenght);
                char[] chars = new char[length];
                int wordLenght = 0;
                for (int i = 0; i < length; i++)
                {
                    chars[i] = MathHelper.GetRandomAlphaNumericCharacter();
                    wordLenght++;
                    if (wordLenght == MathHelper.GetRandomInteger(3, 7) || wordLenght >= 7)
                    {
                        i++;
                        if (i < length)
                        {
                            chars[i] = ' ';
                        }
                        wordLenght = 0;
                    }
                }
                return new String(chars);
            }
        }
    }
}

