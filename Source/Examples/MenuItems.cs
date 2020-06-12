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

            // scroller
            {
                string[] values = new[] { "Hello", "World", "Foo", "Bar" };
                scrollerMenu.AddItem(new UIMenuListScrollerItem<string>("List #1", "List scroller.", values));
                scrollerMenu.AddItem(new UIMenuListScrollerItem<string>("List #2", "List scroller disabled.", values) { Enabled = false });
                scrollerMenu.AddItem(new UIMenuListScrollerItem<string>("List #3", "List scroller disabled with scrolling enabled.", values) { Enabled = false, ScrollingEnabledWhenDisabled = true });
                scrollerMenu.AddItem(new UIMenuListScrollerItem<string>("List #4", "List scroller with scrolling disabled.", values) { ScrollingEnabled = false });
                scrollerMenu.AddItem(new UIMenuListScrollerItem<string>("List #5", "List scroller without wrap around.", values) { AllowWrapAround = false });
                scrollerMenu.AddItem(new UIMenuListScrollerItem<string>("List #6", "List scroller with right badge.", values) { RightBadge = UIMenuItem.BadgeStyle.Car });
                scrollerMenu.AddItem(new UIMenuListScrollerItem<string>("List #7", "List scroller with left badge.", values) { LeftBadge = UIMenuItem.BadgeStyle.Car });
                scrollerMenu.AddItem(new UIMenuListScrollerItem<string>("List #8", "List scroller with both badges.", values) { RightBadge = UIMenuItem.BadgeStyle.Car, LeftBadge = UIMenuItem.BadgeStyle.Car });
                scrollerMenu.AddItem(new UIMenuNumericScrollerItem<int>("Numeric #1", "Numeric scroller.", -10, 10, 1));
                scrollerMenu.AddItem(new UIMenuNumericScrollerItem<int>("Numeric #2", "Numeric scroller disabled.", -10, 10, 1) { Enabled = false });
                scrollerMenu.AddItem(new UIMenuNumericScrollerItem<int>("Numeric #3", "Numeric scroller disabled with scrolling enabled.", -10, 10, 1) { Enabled = false, ScrollingEnabledWhenDisabled = true });
                scrollerMenu.AddItem(new UIMenuNumericScrollerItem<int>("Numeric #4", "Numeric scroller with scrolling disabled.", -10, 10, 1) { ScrollingEnabled = false });
                scrollerMenu.AddItem(new UIMenuNumericScrollerItem<int>("Numeric #5", "Numeric scroller without wrap around.", -10, 10, 1) { AllowWrapAround = false });
                scrollerMenu.AddItem(new UIMenuNumericScrollerItem<int>("Numeric #6", "Numeric scroller with right badge.", -10, 10, 1) { RightBadge = UIMenuItem.BadgeStyle.Car });
                scrollerMenu.AddItem(new UIMenuNumericScrollerItem<int>("Numeric #7", "Numeric scroller with left badge.", -10, 10, 1) { LeftBadge = UIMenuItem.BadgeStyle.Car });
                scrollerMenu.AddItem(new UIMenuNumericScrollerItem<int>("Numeric #8", "Numeric scroller with both badges.", -10, 10, 1) { RightBadge = UIMenuItem.BadgeStyle.Car, LeftBadge = UIMenuItem.BadgeStyle.Car });
                scrollerMenu.AddItem(new UIMenuNumericScrollerItem<int>("Slider bar #1", "Scroller with slider bar.", -10, 10, 1) { SliderBar = new UIMenuScrollerSliderBar() });
                scrollerMenu.AddItem(new UIMenuNumericScrollerItem<int>("Slider bar #2", "Scroller with slider bar disabled.", -10, 10, 1) { Enabled = false, SliderBar = new UIMenuScrollerSliderBar() });
                scrollerMenu.AddItem(new UIMenuNumericScrollerItem<int>("Slider bar #3", "Scroller with slider bar disabled with scrolling enabled.", -10, 10, 1) { Enabled = false, ScrollingEnabledWhenDisabled = true, SliderBar = new UIMenuScrollerSliderBar() });
                scrollerMenu.AddItem(new UIMenuNumericScrollerItem<int>("Slider bar #4", "Scroller with slider bar with scrolling disabled.", -10, 10, 1) { ScrollingEnabled = false, SliderBar = new UIMenuScrollerSliderBar() });
                scrollerMenu.AddItem(new UIMenuNumericScrollerItem<int>("Slider bar #5", "Scroller with slider bar without wrap around.", -10, 10, 1) { AllowWrapAround = false, SliderBar = new UIMenuScrollerSliderBar() });
                scrollerMenu.AddItem(new UIMenuNumericScrollerItem<int>("Slider bar #6", "Scroller with slider bar with right badge.", -10, 10, 1) { RightBadge = UIMenuItem.BadgeStyle.Car, SliderBar = new UIMenuScrollerSliderBar() });
                scrollerMenu.AddItem(new UIMenuNumericScrollerItem<int>("Slider bar #7", "Scroller with slider bar with left badge.", -10, 10, 1) { LeftBadge = UIMenuItem.BadgeStyle.Car, SliderBar = new UIMenuScrollerSliderBar() });
                scrollerMenu.AddItem(new UIMenuNumericScrollerItem<int>("Slider bar #8", "Scroller with slider bar with both badges.", -10, 10, 1) { RightBadge = UIMenuItem.BadgeStyle.Car, LeftBadge = UIMenuItem.BadgeStyle.Car, SliderBar = new UIMenuScrollerSliderBar() });
                scrollerMenu.AddItem(new UIMenuNumericScrollerItem<int>("Slider bar #9", "Scroller with slider bar with custom colors.", -10, 10, 1)
                    { BackColor = Color.FromArgb(190, HudColor.BlueDark.GetColor()), HighlightedBackColor = Color.FromArgb(220, HudColor.Blue.GetColor()), SliderBar = new UIMenuScrollerSliderBar() { ForegroundColor = HudColor.Purple.GetColor(), BackgroundColor = Color.FromArgb(120, HudColor.Purple.GetColor()) } });
                scrollerMenu.AddItem(new UIMenuNumericScrollerItem<int>("Slider bar #10", "Scroller with slider bar with full width and height and custom color.", -10, 10, 1)
                    { SliderBar = new UIMenuScrollerSliderBar() { Width = 1.0f, Height = 1.0f, ForegroundColor = HudColor.Red.GetColor(), BackgroundColor = Color.FromArgb(120, HudColor.Red.GetColor()) } });
                scrollerMenu.AddItem(new UIMenuNumericScrollerItem<int>("Slider bar #11", "Scroller with slider bar with half width and height and custom color.", -10, 10, 1)
                    { SliderBar = new UIMenuScrollerSliderBar() { Width = 0.5f, Height = 0.5f, ForegroundColor = HudColor.Green.GetColor(), BackgroundColor = Color.FromArgb(120, HudColor.Green.GetColor()) } });
            }
        }
    }
}
