namespace RNUIExamples
{
    using System.Drawing;
    using RAGENativeUI;
    using RAGENativeUI.Elements;

    internal class MenuItems : UIMenu
    {
        public MenuItems() : base(Plugin.MenuTitle, "MENU ITEMS")
        {
            Plugin.Pool.Add(this);

            CreateMenuItems();
        }

        private void CreateMenuItems()
        {
            UIMenuItem checkboxBindItem = new UIMenuItem("Checkbox", $"Demonstrates the ~b~{nameof(UIMenuCheckboxItem)}~s~ class.");
            UIMenuItem scrollerBindItem = new UIMenuItem("Scroller", $"Demonstrates the ~b~{nameof(UIMenuListScrollerItem<int>)}~s~ and ~b~{nameof(UIMenuNumericScrollerItem<int>)}~s~ classes.");
            
            UIMenu checkboxMenu = new UIMenu(Title, Subtitle + ": CHECKBOX");
            UIMenu scrollerMenu = new UIMenu(Title, Subtitle + ": SCROLLER");

            Plugin.Pool.Add(checkboxMenu);
            Plugin.Pool.Add(scrollerMenu);

            AddItem(checkboxBindItem);
            AddItem(scrollerBindItem);

            BindMenuToItem(checkboxMenu, checkboxBindItem);
            BindMenuToItem(scrollerMenu, scrollerBindItem);

            // checkbox
            {
                checkboxMenu.AddItem(new UIMenuCheckboxItem("#1", false, "Unchecked."));
                checkboxMenu.AddItem(new UIMenuCheckboxItem("#2", true, "Checked."));
                checkboxMenu.AddItem(new UIMenuCheckboxItem("#3", false, "Unchecked and disabled.") { Enabled = false });
                checkboxMenu.AddItem(new UIMenuCheckboxItem("#4", true, "Checked and disabled.") { Enabled = false });
                checkboxMenu.AddItem(new UIMenuCheckboxItem("#5", false, "With right badge.") { RightBadge = UIMenuItem.BadgeStyle.Armour });
                checkboxMenu.AddItem(new UIMenuCheckboxItem("#6", true, "With right badge.") { RightBadge = UIMenuItem.BadgeStyle.Armour });
                checkboxMenu.AddItem(new UIMenuCheckboxItem("#7", false, "With left badge.") { LeftBadge = UIMenuItem.BadgeStyle.Armour });
                checkboxMenu.AddItem(new UIMenuCheckboxItem("#8", true, "With left badge.") { LeftBadge = UIMenuItem.BadgeStyle.Armour });
                checkboxMenu.AddItem(new UIMenuCheckboxItem("#9", false, "With both badges.") { RightBadge = UIMenuItem.BadgeStyle.Armour, LeftBadge = UIMenuItem.BadgeStyle.Armour });
                checkboxMenu.AddItem(new UIMenuCheckboxItem("#10", true, "With both badges.") { RightBadge = UIMenuItem.BadgeStyle.Armour, LeftBadge = UIMenuItem.BadgeStyle.Armour });
                checkboxMenu.AddItem(new UIMenuCheckboxItem("#11", false, "With both badges and disabled.") { Enabled = false, RightBadge = UIMenuItem.BadgeStyle.Armour, LeftBadge = UIMenuItem.BadgeStyle.Armour });
                checkboxMenu.AddItem(new UIMenuCheckboxItem("#12", true, "With both badges and disabled.") { Enabled = false, RightBadge = UIMenuItem.BadgeStyle.Armour, LeftBadge = UIMenuItem.BadgeStyle.Armour });
                checkboxMenu.AddItem(new UIMenuCheckboxItem("#13", false, "With custom background color.") { BackColor = Color.FromArgb(140, HudColor.RedDark.GetColor()), HighlightedBackColor = Color.FromArgb(230, HudColor.TechRed.GetColor()) });
                checkboxMenu.AddItem(new UIMenuCheckboxItem("#14", true, "With custom background color.") { BackColor = Color.FromArgb(140, HudColor.RedDark.GetColor()), HighlightedBackColor = Color.FromArgb(230, HudColor.TechRed.GetColor()) });
                checkboxMenu.AddItem(new UIMenuCheckboxItem("#14", false, "With custom badge.") { RightBadgeInfo = new UIMenuItem.BadgeInfo("commonmenu", "mp_alerttriangle", HudColor.Red.GetColor()) });
                checkboxMenu.AddItem(new UIMenuCheckboxItem("#15", true, "With custom badge.") { RightBadgeInfo = new UIMenuItem.BadgeInfo("commonmenu", "mp_alerttriangle", HudColor.Red.GetColor()) });
            }
        }
    }
}
