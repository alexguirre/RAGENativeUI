namespace Examples
{
    using System;
    using System.Drawing;

    using Rage;
    using Rage.Attributes;
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Rendering;
    using RAGENativeUI.Menus;
    using RAGENativeUI.Menus.Rendering;
    using Font = RAGENativeUI.Rendering.Font;

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

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                    {
                        if(menu.Skin is MyCustomMenuSkin)
                        {
                            menu.Skin = MenuSkin.DefaultSkin;
                        }
                        else
                        {
                            menu.Skin = new MyCustomMenuSkin();
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


        private class MyCustomMenuSkin : IMenuSkin
        {
            public Font BigFont { get; } = new Font("Consolas", 32.5f);
            public Font SmallFont { get; } = new Font("Consolas", 22.5f);

            public void DrawBackground(Graphics graphics, MenuBackground background, float x, float y)
            {
                // no background
            }

            public void DrawBanner(Graphics graphics, MenuBanner banner, float x, float y)
            {
                DrawText(graphics, banner.Title.ToUpper(), BigFont, new RectangleF(x, y, banner.Size.Width, banner.Size.Height), Color.Red, TextHorizontalAligment.Center);
            }

            public void DrawDescription(Graphics graphics, MenuDescription description, float x, float y)
            {
                // no description
            }

            public void DrawSubtitle(Graphics graphics, MenuSubtitle subtitle, float x, float y)
            {
                DrawText(graphics, subtitle.Text, SmallFont, new RectangleF(x, y, subtitle.Size.Width, subtitle.Size.Height), Color.Red, TextHorizontalAligment.Right);
                // no counter
            }

            public void DrawUpDownDisplay(Graphics graphics, MenuUpDownDisplay upDownDisplay, float x, float y)
            {
                // no up down display
            }

            public void DrawItem(Graphics graphics, MenuItem item, float x, float y, bool selected)
            {
                RectangleF r = new RectangleF(x, y, item.Size.Width, item.Size.Height);

                if (selected)
                {
                    graphics.DrawRectangle(r, Color.FromArgb(230, 165, 165, 165));
                }

                DrawText(graphics, item.Text, SmallFont, r, selected ? Color.FromArgb(240, 5, 5, 5) : Color.FromArgb(240, 175, 175, 175), TextHorizontalAligment.Center, TextVerticalAligment.Center);
            }

            public void DrawItemCheckbox(Graphics graphics, MenuItemCheckbox item, float x, float y, bool selected)
            {
                DrawItem(graphics, item, x, y, selected);
                // no checkbox implementation
            }

            public void DrawItemScroller(Graphics graphics, MenuItemScroller item, float x, float y, bool selected)
            {
                DrawItem(graphics, item, x, y, selected);
                // no scroller implementation
            }

            public string FormatDescriptionText(MenuDescription description, string text, out SizeF textMeasurement)
            {
                textMeasurement = SmallFont.Measure(text);
                return text;
            }



            private void DrawText(Graphics graphics, string text, string fontName, float fontSize, RectangleF rectangle, Color color, TextHorizontalAligment horizontalAligment = TextHorizontalAligment.Left, TextVerticalAligment verticalAligment = TextVerticalAligment.Center)
            {
                DrawText(graphics, text, new Font(fontName, fontSize), rectangle, color, horizontalAligment, verticalAligment);
            }

            private void DrawText(Graphics graphics, string text, Font font, RectangleF rectangle, Color color, TextHorizontalAligment horizontalAligment = TextHorizontalAligment.Left, TextVerticalAligment verticalAligment = TextVerticalAligment.Center)
            {
                SizeF textSize = font.Measure(text);
                textSize.Height = font.Height;
                float x = 0.0f, y = 0.0f;

                switch (horizontalAligment)
                {
                    case TextHorizontalAligment.Left:
                        x = rectangle.X;
                        break;
                    case TextHorizontalAligment.Center:
                        x = rectangle.X + rectangle.Width * 0.5f - textSize.Width * 0.5f;
                        break;
                    case TextHorizontalAligment.Right:
                        x = rectangle.Right - textSize.Width - 2.0f;
                        break;
                }

                switch (verticalAligment)
                {
                    case TextVerticalAligment.Top:
                        y = rectangle.Y;
                        break;
                    case TextVerticalAligment.Center:
                        y = rectangle.Y + rectangle.Height * 0.5f - textSize.Height * 0.8f;
                        break;
                    case TextVerticalAligment.Down:
                        y = rectangle.Y + rectangle.Height - textSize.Height * 1.6f;
                        break;
                }

                graphics.DrawText(text, font.Name, font.Size, new PointF(x, y), color, rectangle);
            }
        }
    }
}

