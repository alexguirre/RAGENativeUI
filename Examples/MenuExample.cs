namespace Examples
{
    using System;
    using System.Linq;
    using System.Drawing;
    using System.Collections.Generic;

    using Rage;
    using Rage.Attributes;
    using Graphics = Rage.Graphics;

    using RAGENativeUI;
    using RAGENativeUI.Menus;
    using RAGENativeUI.Menus.Themes;

    internal static class MenuExample
    {
        [ConsoleCommand(Name = "MenuExample", Description = "Example showing RAGENativeUI menu API")]
        private static void Command()
        {
            GameFiber.StartNew(() =>
            {
                Menu menu = new Menu("title", "SUBTITLE");
                menu.SetTheme<MenuDebugTheme>();

                menu.Items.Add(new MenuItem("item #0") { Description = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa." });
                menu.Items.Add(new MenuItemCheckbox("cb #0") { IsChecked = true, Description = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim." });
                menu.Items.Add(new MenuItemCheckbox("cb #1") { Description = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim." });
                menu.Items.Add(new MenuItemCheckbox("cb #2"));
                menu.Items.Add(new MenuItem("disabled item #0") { Description = "I'm disabled.", IsDisabled = true });
                menu.Items.Add(new MenuItemEnumScroller("enum scroller #0", typeof(GameControl)));
                menu.Items.Add(new MenuItemEnumScroller<GameControl>("enum scroller #1"));
                menu.Items.Add(new MenuItemNumericScroller("num scroller #0"));
                menu.Items.Add(new MenuItemNumericScroller("num scroller #1") { ThousandsSeparator = true, Minimum = -50000.0m, Maximum = 50000.0m, Value = 0.0m, Increment = 100.0m });
                menu.Items.Add(new MenuItemNumericScroller("num scroller #2") { Increment = 1.0m, Hexadecimal = true });
                menu.Items.Add(new MenuItemNumericScroller("num scroller #3") { Minimum = -1.0m, Maximum = 1.0m, Value = 0.0m, Increment = 0.00005m, DecimalPlaces = 5 });
                menu.Items.Add(new MenuItemListScroller("list scroller #0"));
                menu.Items.Add(new MenuItemListScroller("list scroller #1", new[] { "text #1", "other text #2", "and text #3" }));
                menu.Items.Add(new MenuItem("disabled item #1") { Description = "I'm disabled.", IsDisabled = true });
                menu.Items.Add(new MenuItemCheckbox("disabled item #1") { Description = "I'm disabled.", IsDisabled = true });
                menu.Items.Add(new MenuItemEnumScroller<PedFormationType>("disabled item #2") { Description = "I'm disabled.", IsDisabled = true });
                menu.Items.Add(new MenuItemCheckbox("skipped disabled item #0") { Description = "I'm disabled.", IsDisabled = true, IsSkippedIfDisabled = true, IsChecked = true });
                menu.Items.Add(new MenuItemEnumScroller<PedFormationType>("skipped disabled item #1") { Description = "I'm disabled.", IsDisabled = true, IsSkippedIfDisabled = true });

                Menu subMenu = new Menu("SubMenu", "SUB!");
                subMenu.CopyThemeFrom(menu);
                MenuDebugTheme subMenuTheme = subMenu.Theme as MenuDebugTheme;
                for (int i = 1; i <= 999; i++)
                {
                    Menu subSubMenu = new Menu("SubSubMenu", "SUB! SUB! #" + i);
                    subSubMenu.CopyThemeFrom(menu);
                    subSubMenu.Items.Add(new MenuItem("An item in the SubSubMenu #" + i));
                    MenuItem item = new MenuItem("item #" + i) { BindedMenu = subSubMenu };
                    subMenu.Items.Add(item);
                    subMenuTheme.SetItemTextColor(item, Color.FromArgb(255, MathHelper.GetRandomInteger(256), MathHelper.GetRandomInteger(256), MathHelper.GetRandomInteger(256)));
                }

                menu.Items.Add(new MenuItem("item with binded menu #0") { BindedMenu = subMenu, Description = "If you click this item it opens a menu with a LOT of items." });

                menu.SelectedItemChanged += (s, e) => { Game.DisplayHelp($"Selected index changed from #{e.OldIndex} to #{e.NewIndex}"); };
                menu.VisibleChanged += (s, e) => { Game.DisplayHelp($"Visible now: {e.IsVisible}"); };

                for (int i = 0; i < menu.Items.Count; i++)
                {
                    int idx = i;

                    if (menu.Items[idx] is MenuItemCheckbox)
                    {
                        (menu.Items[idx] as MenuItemCheckbox).CheckedChanged += (s, e) => { Game.DisplayHelp($"Checkbox at index #{idx} {(e.IsChecked ? "checked" : "unchecked")}"); };
                        continue;
                    }

                    if (menu.Items[idx] is MenuItemScroller)
                    {
                        (menu.Items[idx] as MenuItemScroller).SelectedIndexChanged += (s, e) => { Game.DisplayHelp($"Scroller at index #{idx} selected index changed from #{e.OldIndex} to #{e.NewIndex}"); };
                    }

                    menu.Items[idx].Activated += (s, e) => { Game.DisplayHelp($"Activated item at index #{idx}"); };
                    menu.Items[idx].SelectedChanged += (s, e) => { Game.DisplayHelp($"{(e.IsSelected ? "Selected" : "Unselected")} item at index #{idx}"); };
                }

                menu.Metadata["Test"] = new Vector3(50f, 75f, 100f);
                menu.Items[0].Metadata.Test = 100;
                menu.Items[0].Activated += (s, e) =>
                {
                    Game.DisplayNotification(menu.Items[0].Metadata.Test.ToString());
                    Game.DisplayNotification(menu.Metadata.Test.ToString());
                    menu.Items[0].Metadata["Test"]++;
                    menu.Metadata.Test += new Vector3(1f, 1f, 1f);
                };


                // create a page with random items
                ScrollableMenuPage CreatePageForScroller(string text)
                {
                    ScrollableMenuPage p = new ScrollableMenuPage(text);
                    int count = MathHelper.GetRandomInteger(5, 1000);
                    for (int i = 1; i <= count; i++)
                    {
                        p.Items.Add(new MenuItem($"{text} - #{i}"));
                    }

                    return p;
                }

                ScrollableMenu scrollableMenu = new ScrollableMenu("Scrollable Menu", "Subtitle", "Page");
                scrollableMenu.CopyThemeFrom(menu);

                scrollableMenu.Pages.Add(CreatePageForScroller("First Page"));
                scrollableMenu.Pages.Add(CreatePageForScroller("Second Page"));
                scrollableMenu.Pages.Add(CreatePageForScroller("Third Page"));
                scrollableMenu.Pages.Add(CreatePageForScroller("Fourth Page"));
                scrollableMenu.Pages.Add(CreatePageForScroller("Fifth Page"));
                scrollableMenu.Pages.Add(CreatePageForScroller("Sixth Page"));
                scrollableMenu.Pages.Add(CreatePageForScroller("Seventh Page"));
                scrollableMenu.Pages.Add(CreatePageForScroller("Eighth Page"));
                scrollableMenu.Pages.Add(CreatePageForScroller("Ninth Page"));
                scrollableMenu.Pages.Add(CreatePageForScroller("Tenth Page"));

                scrollableMenu.SelectedPageChanged += (s, e) => { Game.DisplayHelp($"Page changed from '{e.OldPage.Text}' to '{e.NewPage.Text}'."); };

                menu.Items.Add(new MenuItem("item with binded scrollable menu") { BindedMenu = scrollableMenu });

                menu.Items.Add(new MenuItem("skipped disabled item #2") { Description = "I'm disabled.", IsDisabled = true, IsSkippedIfDisabled = true });


                while (true)
                {
                    GameFiber.Yield();

                    if (Game.WasKeyJustPressed(System.Windows.Forms.Keys.T))
                    {
                        if (menu.IsAnyChildMenuVisible)
                        {
                            menu.Hide();
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


                    if (Game.IsKeyDown(System.Windows.Forms.Keys.U))
                    {
                        menu.Items[0].Description = GetRandomString();
                    }

                    if (Game.WasKeyJustPressed(System.Windows.Forms.Keys.I))
                    {
                        if (menu.DescriptionOverride == null)
                        {
                            menu.DescriptionOverride = "Description override set! Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa.";
                        }
                        else
                        {
                            menu.DescriptionOverride = null;
                        }
                    }

                    if (Game.WasKeyJustPressed(System.Windows.Forms.Keys.K))
                    {
                        foreach (MenuItem m in menu.Items)
                        {
                            m.IsVisible = true;
                        }
                    }

                    if (Game.WasKeyJustPressed(System.Windows.Forms.Keys.L))
                    {
                        foreach (MenuItem m in menu.Items)
                        {
                            m.IsVisible = MathHelper.Choose(true, false);
                        }
                    }

                    if (Game.WasKeyJustPressed(System.Windows.Forms.Keys.Y))
                    {
                        //if(menu.Style is MyCustomMenuStyle)
                        {
                            //menu.Style = new MenuStyle();
                            //subMenu.Style = new MenuStyle();
                            //scrollableMenu.Style = new MenuStyle();
                        }
                        //else
                        //{
                        //    menu.Style = new MyCustomMenuStyle();
                        //    subMenu.Style = new MyCustomMenuStyle();
                        //    scrollableMenu.Style = new MyCustomMenuStyle();
                        //}
                    }

                    if (Game.WasKeyJustPressed(System.Windows.Forms.Keys.O))
                    {
                        menu.Items.ClearAndAdd(menu.Items.OrderBy(i => i.Text));
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


        //private class MyCustomMenuStyle : IMenuStyle
        //{
        //    public PointF InitialMenuLocation { get; set; } = new PointF(30.0f, 23.0f);
        //    public float MenuWidth { get; set; } = 750.0f;
        //    public float BannerHeight { get; set; } = 80.0f;
        //    public float SubtitleHeight { get; set; } = 35.0f;
        //    public float ItemHeight { get; set; } = 30.0f;
        //    public float UpDownDisplayHeight { get; set; } = 30.0f;

        //    public GraphicsFont BigFont { get; set; } = new GraphicsFont("Consolas", 32.0f);
        //    public GraphicsFont SmallFont { get; set; } = new GraphicsFont("Consolas", 20.0f);

        //    public void DrawBackground(Graphics graphics, MenuBackground background, ref float x, ref float y)
        //    {
        //        // no background
        //    }

        //    public void DrawBanner(Graphics graphics, MenuBanner banner, ref float x, ref float y)
        //    {
        //        DrawText(graphics, banner.Title.ToUpper(), BigFont, new RectangleF(x, y, MenuWidth, BannerHeight), Color.Red, TextHorizontalAligment.Center);
        //        y += BannerHeight;
        //    }

        //    public void DrawDescription(Graphics graphics, MenuDescription description, ref float x, ref float y)
        //    {
        //        // no description
        //    }

        //    public void DrawItem(Graphics graphics, MenuItem item, ref float x, ref float y)
        //    {
        //        switch (item)
        //        {
        //            case MenuItemCheckbox checkbox: DrawItemCheckbox(graphics, checkbox, ref x, ref y); break;
        //            case MenuItemScroller scroller: DrawItemScroller(graphics, scroller, ref x, ref y); break;
        //            default: DrawItemBase(graphics, item, ref x, ref y); break;
        //        }
        //    }

        //    private void DrawItemBase(Graphics graphics, MenuItem item, ref float x, ref float y)
        //    {
        //        RectangleF r = new RectangleF(x, y, MenuWidth, ItemHeight);

        //        if (item.IsSelected)
        //        {
        //            graphics.DrawRectangle(r, Color.FromArgb(230, 165, 165, 165));
        //        }

        //        DrawText(graphics, item.Text, SmallFont, r, item.IsSelected ? Color.FromArgb(240, 5, 5, 5) : Color.FromArgb(240, 175, 175, 175), TextHorizontalAligment.Center, TextVerticalAligment.Center);

        //        y += ItemHeight;
        //    }

        //    private void DrawItemCheckbox(Graphics graphics, MenuItemCheckbox item, ref float x, ref float y)
        //    {
        //        RectangleF r = new RectangleF(x, y, MenuWidth, ItemHeight);

        //        if (item.IsSelected)
        //        {
        //            graphics.DrawRectangle(r, Color.FromArgb(230, 165, 165, 165));
        //        }

        //        DrawText(graphics, item.Text, SmallFont, r, item.IsSelected ? Color.FromArgb(240, 5, 5, 5) : Color.FromArgb(240, 175, 175, 175), TextHorizontalAligment.Left, TextVerticalAligment.Center);
        //        DrawText(graphics, $"[{(item.IsChecked ? "X" : " ")}] ", SmallFont, r, item.IsSelected ? Color.FromArgb(240, 5, 5, 5) : Color.FromArgb(240, 175, 175, 175), TextHorizontalAligment.Right, TextVerticalAligment.Center);

        //        y += ItemHeight;
        //    }

        //    private void DrawItemScroller(Graphics graphics, MenuItemScroller item, ref float x, ref float y)
        //    {
        //        RectangleF r = new RectangleF(x, y, MenuWidth, ItemHeight);

        //        if (item.IsSelected)
        //        {
        //            graphics.DrawRectangle(r, Color.FromArgb(230, 165, 165, 165));
        //        }

        //        DrawText(graphics, item.Text, SmallFont, r, item.IsSelected ? Color.FromArgb(240, 5, 5, 5) : Color.FromArgb(240, 175, 175, 175), TextHorizontalAligment.Left, TextVerticalAligment.Center);
        //        DrawText(graphics, item.IsSelected ? $"<{item.GetSelectedOptionText()}> " : item.GetSelectedOptionText(), SmallFont, r, item.IsSelected ? Color.FromArgb(240, 5, 5, 5) : Color.FromArgb(240, 175, 175, 175), TextHorizontalAligment.Right, TextVerticalAligment.Center);

        //        y += ItemHeight;
        //    }

        //    public void DrawSubtitle(Graphics graphics, MenuSubtitle subtitle, ref float x, ref float y)
        //    {
        //        DrawText(graphics, subtitle.Text, SmallFont, new RectangleF(x, y, MenuWidth, SubtitleHeight), Color.Red, TextHorizontalAligment.Right);
        //        // no counter

        //        y += SubtitleHeight;
        //    }

        //    public void DrawUpDownDisplay(Graphics graphics, MenuUpDownDisplay upDownDisplay, ref float x, ref float y)
        //    {
        //        // no up down display
        //    }

        //    public string FormatDescriptionText(MenuDescription description, string text, out SizeF textMeasurement)
        //    {
        //        textMeasurement = SmallFont.Measure(text);
        //        return text;
        //    }

        //    public virtual IEnumerable<IMenuComponent> EnumerateComponentsInDrawOrder(Menu menu)
        //    {
        //        yield return menu.Banner;
        //        yield return menu.Subtitle;
        //        yield return menu.Background;
        //        yield return menu.Items;
        //        yield return menu.UpDownDisplay;
        //        yield return menu.Description;
        //    }


        //    private void DrawText(Graphics graphics, string text, string fontName, float fontSize, RectangleF rectangle, Color color, TextHorizontalAligment horizontalAligment = TextHorizontalAligment.Left, TextVerticalAligment verticalAligment = TextVerticalAligment.Center)
        //    {
        //        DrawText(graphics, text, new GraphicsFont(fontName, fontSize), rectangle, color, horizontalAligment, verticalAligment);
        //    }

        //    private void DrawText(Graphics graphics, string text, GraphicsFont font, RectangleF rectangle, Color color, TextHorizontalAligment horizontalAligment = TextHorizontalAligment.Left, TextVerticalAligment verticalAligment = TextVerticalAligment.Center)
        //    {
        //        SizeF textSize = font.Measure(text);
        //        textSize.Height = font.Height;
        //        float x = 0.0f, y = 0.0f;

        //        switch (horizontalAligment)
        //        {
        //            case TextHorizontalAligment.Left:
        //                x = rectangle.X;
        //                break;
        //            case TextHorizontalAligment.Center:
        //                x = rectangle.X + rectangle.Width * 0.5f - textSize.Width * 0.5f;
        //                break;
        //            case TextHorizontalAligment.Right:
        //                x = rectangle.Right - textSize.Width - 2.0f;
        //                break;
        //        }

        //        switch (verticalAligment)
        //        {
        //            case TextVerticalAligment.Top:
        //                y = rectangle.Y;
        //                break;
        //            case TextVerticalAligment.Center:
        //                y = rectangle.Y + rectangle.Height * 0.5f - textSize.Height * 0.8f;
        //                break;
        //            case TextVerticalAligment.Down:
        //                y = rectangle.Y + rectangle.Height - textSize.Height * 1.6f;
        //                break;
        //        }

        //        graphics.DrawText(text, font.Name, font.Size, new PointF(x, y), color, rectangle);
        //    }
        //}
    }
}

