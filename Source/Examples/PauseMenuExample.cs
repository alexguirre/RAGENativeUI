namespace RNUIExamples
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    using Rage;
    using Rage.Attributes;
    using RAGENativeUI;
    using RAGENativeUI.Elements;
    using RAGENativeUI.PauseMenu;

    internal static class PauseMenuExample
    {
        private static TabView tabView;

        private static TabItemSimpleList simpleListTab;
        private static TabMissionSelectItem missionSelectTab;
        private static TabTextItem textTab;
        private static TabSubmenuItem submenuTab;

        private static readonly Keys KeyBinding = Keys.F7;

        private static void Main()
        {
            tabView = new TabView("A RAGENativeUI Pause Menu");
            tabView.MoneySubtitle = "$10.000.000";
            tabView.Name = "Here goes the Name! :D";

            Dictionary<string, string> listDict = new Dictionary<string, string>()
            {
                { "First Item", "Text filler" },
                { "Second Item", "Hey, here there's some text" },
                { "Third Item", "Duh!" },
            };
            tabView.AddTab(simpleListTab = new TabItemSimpleList("A List", listDict));
            simpleListTab.Activated += (s, e) => Game.DisplaySubtitle("I'm in the simple list tab", 5000);

            tabView.AddTab(new TabTextItem("Skipped Tab Item", "This item is skipped", "You shouldn't be able to see this") { Skipped = true });

            List<MissionInformation> missionsInfo = new List<MissionInformation>()
            {
                new MissionInformation("Mission One", "I have description!", new Tuple<string, string>[] { Tuple.Create("Objective", "Mission One Objective") }),
                new MissionInformation("Mission Two", new Tuple<string, string>[] { new Tuple<string, string>("This the second info", "Random Info"), new Tuple<string, string>("This the second info", "Random Info #2") }) { Logo = new MissionLogo(Game.CreateTextureFromFile("DefaultSkin.png")) },
                new MissionInformation("Mission Three", "This has a description and a full texture", new Tuple<string, string>[] { Tuple.Create("Objective", "Mission Two Objective"), Tuple.Create("Some more details", "Mission Two Details") }) { Logo = new MissionLogo("candc_chopper", "banner_4") },
                new MissionInformation("Mission Four", new Tuple<string, string>[] { }) { Logo = new MissionLogo("candc_casinoheist", "stockade_b") },
                new MissionInformation("Unreasonably long mission name which really ought to be cut off because it's so stupidly long", "Long description with a~n~very very~n~very very~n~very very~n~very very~n~very very very very very very very very very very long string", new Tuple<string, string>[] { Tuple.Create("Objective", "Mission Two Objective"), Tuple.Create("Some more details", "Mission Two Details") }) { Logo = new MissionLogo("candc_default", "dump") },
                new MissionInformation("Mission Item", "Mission with a very tall texture", new Tuple<string, string>[] { Tuple.Create("Info 1", "#1"), Tuple.Create("Info 2", "#2"), Tuple.Create("Info 3", "#3") }) { Logo = new MissionLogo("helicopterhud", "hud_vert") },
            };
            tabView.AddTab(missionSelectTab = new TabMissionSelectItem("Static Mission Select Tab", missionsInfo));
            missionSelectTab.OnItemSelect += (info) =>
            {
                if (info.Name == "Mission One")
                {
                    Game.DisplaySubtitle("~g~Mission One Activated", 5000);
                }
                else if (info.Name == "Mission Two")
                {
                    Game.DisplaySubtitle("~b~Mission Two Activated", 5000);
                }
            };

            tabView.AddTab(new TabMissionSelectItem("Dynamic Mission Select Tab", missionsInfo) { DynamicMissionWidth = true });

            tabView.AddTab(new TabTextItem("Very very very very long and extremely detailed tab name", "This item has a stupid long title", "This should force dynamic tab resizing"));
            tabView.AddTab(textTab = new TabTextItem("TabTextItem", "Text Tab Item", "I'm a text tab item"));
            textTab.Activated += (s, e) => Game.DisplaySubtitle("I'm in the text tab", 5000);

            List<TabItem> items = new List<TabItem>();

            for (int i = 0; i < 30; i++)
            {
                TabItem tItem = i < 15 ? (TabItem)new TabTextItem("Item #" + i, "Title #" + i, "Some random text for #" + i) :
                                        (TabItem)new TabInteractiveListItem("Item #" + i, CreateMenuItems());

                tItem.Activated += (s, e) => Game.DisplaySubtitle("Activated Submenu Item #" + submenuTab.Index, 5000);
                items.Add(tItem);
            }

            items.Add(new TabMissionSelectItem("Submenu Mission Select Item", missionsInfo));
            items.Add(new TabMissionSelectItem("Submenu Dynamic Mission Select Item", missionsInfo) { DynamicMissionWidth = true });
            items.Add(new TabMissionSelectItem("Submenu Fixed Ratio Mission Select Item", missionsInfo) { DynamicLogoRatio = false });

            tabView.AddTab(submenuTab = new TabSubmenuItem("A submenu", items));

            UIMenuItem[] menuItems = CreateMenuItems();
            menuItems[0].Activated += (m, s) => Game.DisplaySubtitle("Activated first item!");
            TabInteractiveListItem interactiveListItem = new TabInteractiveListItem("An Interactive List", menuItems);
            // set fast scrolling on scroller item with a slider bar (see MenuExtensions.cs)
            interactiveListItem.BackingMenu.WithFastScrollingOn(menuItems.Where(i => i is UIMenuScrollerItem s && s.SliderBar != null));
            interactiveListItem.BackingMenu.OnIndexChange += (m, i) => Game.DisplaySubtitle("Selected #" + i);
            tabView.AddTab(interactiveListItem);

            tabView.RefreshIndex();

            // start the fiber which will handle drawing and processing the pause menu
            GameFiber.StartNew(ProcessMenus);

            // continue with the plugin...
            Game.Console.Print($"  Press {KeyBinding} to open the pause menu.");
            Game.DisplayHelp($"Press ~{KeyBinding.GetInstructionalId()}~ to open the pause menu.");
        }

        private static void ProcessMenus()
        {
            // draw the textures (only needed Rage.Texture are used)
            Game.RawFrameRender += (s, e) => tabView.DrawTextures(e.Graphics); ;

            while (true)
            {
                GameFiber.Yield();

                tabView.Update();

                if (Game.IsKeyDown(KeyBinding) && !UIMenu.IsAnyMenuVisible && !TabView.IsAnyPauseMenuVisible)
                {
                    tabView.Visible = !tabView.Visible;
                }
            }
        }

        private static UIMenuItem[] CreateMenuItems()
        {
            var values = new[] { "Hello", "World", "Foo", "Bar" };
            return new[]
            {
                new UIMenuItem("Simple item", ""),
                new UIMenuListScrollerItem<string>("List #1", "", values),
                new UIMenuListScrollerItem<string>("List #2", "", values) { Enabled = false },
                new UIMenuListScrollerItem<string>("List #3", "", values) { Enabled = false, ScrollingEnabledWhenDisabled = true },
                new UIMenuListScrollerItem<string>("List #4", "", values) { ScrollingEnabled = false },
                new UIMenuListScrollerItem<string>("List #5", "", values) { AllowWrapAround = false },
                new UIMenuListScrollerItem<string>("List #6", "", values) { RightBadge = UIMenuItem.BadgeStyle.Car },
                new UIMenuListScrollerItem<string>("List #7", "", values) { LeftBadge = UIMenuItem.BadgeStyle.Car },
                new UIMenuListScrollerItem<string>("List #8", "", values) { RightBadge = UIMenuItem.BadgeStyle.Car, LeftBadge = UIMenuItem.BadgeStyle.Car },
                new UIMenuNumericScrollerItem<int>("Numeric #1", "", -10, 10, 1),
                new UIMenuNumericScrollerItem<int>("Numeric #2", "", -10, 10, 1) { Enabled = false },
                new UIMenuNumericScrollerItem<int>("Numeric #3", "", -10, 10, 1) { Enabled = false, ScrollingEnabledWhenDisabled = true },
                new UIMenuNumericScrollerItem<int>("Numeric #4", "", -10, 10, 1) { ScrollingEnabled = false },
                new UIMenuNumericScrollerItem<int>("Numeric #5", "", -10, 10, 1) { AllowWrapAround = false },
                new UIMenuNumericScrollerItem<int>("Numeric #6", "", -10, 10, 1) { RightBadge = UIMenuItem.BadgeStyle.Car },
                new UIMenuNumericScrollerItem<int>("Numeric #7", "", -10, 10, 1) { LeftBadge = UIMenuItem.BadgeStyle.Car },
                new UIMenuNumericScrollerItem<int>("Numeric #8", "", -10, 10, 1) { RightBadge = UIMenuItem.BadgeStyle.Car, LeftBadge = UIMenuItem.BadgeStyle.Car },
                new UIMenuNumericScrollerItem<int>("Slider bar #1", "", -10, 10, 1) { SliderBar = new UIMenuScrollerSliderBar() },
                new UIMenuNumericScrollerItem<int>("Slider bar #2", "", -10, 10, 1) { Enabled = false, SliderBar = new UIMenuScrollerSliderBar() },
                new UIMenuNumericScrollerItem<int>("Slider bar #3", "", -10, 10, 1) { Enabled = false, ScrollingEnabledWhenDisabled = true, SliderBar = new UIMenuScrollerSliderBar() },
                new UIMenuNumericScrollerItem<int>("Slider bar #4", "", -10, 10, 1) { ScrollingEnabled = false, SliderBar = new UIMenuScrollerSliderBar() },
                new UIMenuNumericScrollerItem<int>("Slider bar #5", "", -10, 10, 1) { AllowWrapAround = false, SliderBar = new UIMenuScrollerSliderBar() },
                new UIMenuNumericScrollerItem<int>("Slider bar #6", "", -10, 10, 1) { RightBadge = UIMenuItem.BadgeStyle.Car, SliderBar = new UIMenuScrollerSliderBar() },
                new UIMenuNumericScrollerItem<int>("Slider bar #7", "", -10, 10, 1) { LeftBadge = UIMenuItem.BadgeStyle.Car, SliderBar = new UIMenuScrollerSliderBar() },
                new UIMenuNumericScrollerItem<int>("Slider bar #8", "", -10, 10, 1) { RightBadge = UIMenuItem.BadgeStyle.Car, LeftBadge = UIMenuItem.BadgeStyle.Car, SliderBar = new UIMenuScrollerSliderBar() },
                new UIMenuNumericScrollerItem<int>("Slider bar #9", "", -10, 10, 1)
                {
                    BackColor = System.Drawing.Color.FromArgb(190, HudColor.BlueDark.GetColor()),
                    HighlightedBackColor = System.Drawing.Color.FromArgb(220, HudColor.Blue.GetColor()),
                    SliderBar = new UIMenuScrollerSliderBar() { ForegroundColor = HudColor.Purple.GetColor(), BackgroundColor = System.Drawing.Color.FromArgb(120, HudColor.Purple.GetColor()) }
                },
                new UIMenuNumericScrollerItem<int>("Slider bar #10", "", -10, 10, 1)
                {
                    SliderBar = new UIMenuScrollerSliderBar() { Width = 1.0f, Height = 1.0f, ForegroundColor = HudColor.Red.GetColor(), BackgroundColor = System.Drawing.Color.FromArgb(120, HudColor.Red.GetColor()) }
                },
                new UIMenuNumericScrollerItem<int>("Slider bar #11", "", -10, 10, 1)
                {
                    SliderBar = new UIMenuScrollerSliderBar() { Width = 0.5f, Height = 0.5f, ForegroundColor = HudColor.Green.GetColor(), BackgroundColor = System.Drawing.Color.FromArgb(120, HudColor.Green.GetColor()) }
                },
                new UIMenuCheckboxItem("Checkbox #1", false),
                new UIMenuCheckboxItem("Checkbox #2", true), 
                new UIMenuCheckboxItem("Checkbox #3", false) { Enabled = false },
                new UIMenuCheckboxItem("Checkbox #4", true) { Enabled = false },
                new UIMenuCheckboxItem("Checkbox #5", false) { RightBadge = UIMenuItem.BadgeStyle.Armour },
                new UIMenuCheckboxItem("Checkbox #6", true) { RightBadge = UIMenuItem.BadgeStyle.Armour },
                new UIMenuCheckboxItem("Checkbox #7", false) { LeftBadge = UIMenuItem.BadgeStyle.Armour },
                new UIMenuCheckboxItem("Checkbox #8", true) { LeftBadge = UIMenuItem.BadgeStyle.Armour },
                new UIMenuCheckboxItem("Checkbox #9", false) { RightBadge = UIMenuItem.BadgeStyle.Armour, LeftBadge = UIMenuItem.BadgeStyle.Armour },
                new UIMenuCheckboxItem("Checkbox #10", true) { RightBadge = UIMenuItem.BadgeStyle.Armour, LeftBadge = UIMenuItem.BadgeStyle.Armour },
                new UIMenuCheckboxItem("Checkbox #11", false) { Enabled = false, RightBadge = UIMenuItem.BadgeStyle.Armour, LeftBadge = UIMenuItem.BadgeStyle.Armour },
                new UIMenuCheckboxItem("Checkbox #12", true) { Enabled = false, RightBadge = UIMenuItem.BadgeStyle.Armour, LeftBadge = UIMenuItem.BadgeStyle.Armour },
                new UIMenuCheckboxItem("Checkbox #13", false) { BackColor = System.Drawing.Color.FromArgb(140, HudColor.RedDark.GetColor()), HighlightedBackColor = System.Drawing.Color.FromArgb(230, HudColor.TechRed.GetColor()) },
                new UIMenuCheckboxItem("Checkbox #14", true) { BackColor = System.Drawing.Color.FromArgb(140, HudColor.RedDark.GetColor()), HighlightedBackColor = System.Drawing.Color.FromArgb(230, HudColor.TechRed.GetColor()) },
                new UIMenuCheckboxItem("Checkbox #14", false) { RightBadgeInfo = new UIMenuItem.BadgeInfo("commonmenu", "mp_alerttriangle", HudColor.Red.GetColor()) },
                new UIMenuCheckboxItem("Checkbox #15", true) { RightBadgeInfo = new UIMenuItem.BadgeInfo("commonmenu", "mp_alerttriangle", HudColor.Red.GetColor()) },
                new UIMenuCheckboxItem("Checkbox #16", true) { Style = UIMenuCheckboxStyle.Cross },
                Util.NewTriStateCheckbox("Checkbox #17", "")
            };
        }

        // a command that simulates loading the plugin
        [ConsoleCommand]
        private static void RunPauseMenuExample() => GameFiber.StartNew(Main);
    }
}
