[assembly: Rage.Attributes.Plugin("PauseMenuExample", Author = "alexguirre", Description = "Example using RAGENativeUI")]

namespace PauseMenuExampleProject
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Rage;
    using RAGENativeUI.PauseMenu;

    public static class EntryPoint
    {
        private static TabView tabView;

        private static TabItemSimpleList simpleListTab;
        private static TabMissionSelectItem missionSelectTab;
        private static TabTextItem textTab;
        private static TabSubmenuItem submenuTab;

        public static void Main()
        {
            Game.FrameRender += Process;

            tabView = new TabView("A RAGENativeUI Pause Menu");
            tabView.MoneySubtitle = "$10.000.000";

            Dictionary<string, string> listDict = new Dictionary<string, string>()
            {
                { "First Item", "Text filler" },
                { "Second Item", "Hey, here there's some text" },
                { "Third Item", "Duh!" },
            };
            tabView.Tabs.Add(simpleListTab = new TabItemSimpleList("A List", listDict));
            simpleListTab.Activated += SimpleListTab_Activated;

            List<MissionInformation> missionsInfo = new List<MissionInformation>()
            {
                new MissionInformation("Mission One", new Tuple<string, string>[] { new Tuple<string, string>("This the first info", "Random Info"), new Tuple<string, string>("This the second info", "Random Info #2") }),
                new MissionInformation("Mission Two", "I have description!", new Tuple<string, string>[] { new Tuple<string, string>("Objective", "Mission Two Objective") }),
            };
            tabView.Tabs.Add(missionSelectTab = new TabMissionSelectItem("I'm a Mission Select Tab", missionsInfo));
            missionSelectTab.OnItemSelect += MissionSelectTab_OnItemSelect;


            tabView.Tabs.Add(textTab = new TabTextItem("TabTextItem", "Text Tab Item", "I'm a text tab item"));
            textTab.Activated += TextTab_Activated;

            List<TabItem> items = new List<TabItem>();
            for (int i = 0; i < 10 ; i++)
            {
                TabItem tItem = new TabItem("Item #" + i);
                tItem.Activated += SubMenuItem_Activated;
                items.Add(tItem);
            }
            tabView.Tabs.Add(submenuTab = new TabSubmenuItem("A submenu", items));

            tabView.RefreshIndex();
           
            while (true)
                GameFiber.Yield();
        }

        private static void SubMenuItem_Activated(object sender, EventArgs e)
        {
            Game.DisplaySubtitle("Activated Submenu Item #" + submenuTab.Index, 5000);
        }

        private static void TextTab_Activated(object sender, EventArgs e)
        {
            Game.DisplaySubtitle("I'm in the text tab", 5000);
        }

        private static void MissionSelectTab_OnItemSelect(MissionInformation selectedItem)
        {
            if(selectedItem.Name == "Mission One")
            {
                Game.DisplaySubtitle("~g~Mission One Activated", 5000);
            }
            else if(selectedItem.Name == "Mission Two")
            {
                Game.DisplaySubtitle("~b~Mission Two Activated", 5000);
            }
        }

        private static void SimpleListTab_Activated(object sender, EventArgs e)
        {
            Game.DisplaySubtitle("I'm in the simple list tab", 5000);
        }

        public static void Process(object sender, GraphicsEventArgs e)
        {
            if (Game.IsKeyDown(Keys.F5)) // Our menu on/off switch.
                tabView.Visible = !tabView.Visible;

            tabView.Update();
        }
    }
}

