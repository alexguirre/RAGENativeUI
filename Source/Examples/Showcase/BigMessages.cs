namespace RNUIExamples.Showcase
{
    using RAGENativeUI;
    using RAGENativeUI.Elements;
    using Rage;

    internal sealed class BigMessages : UIMenu
    {
        private readonly BigMessageThread bigMessageThread;
        private readonly BigMessageHandler bigMessage;

        public BigMessages() : base(Plugin.MenuTitle, "BIG MESSAGES")
        {
            Plugin.Pool.Add(this);

            bigMessageThread = new BigMessageThread();
            bigMessage = bigMessageThread.MessageInstance;

            CreateMenuItems();
        }

        private void CreateMenuItems()
        {
            UIMenuItem simple = new UIMenuItem("Simple", $"Calls the ~b~{nameof(BigMessageHandler)}.{nameof(BigMessageHandler.ShowSimpleShard)}~s~ method.");
            simple.Activated += (m, s) => bigMessage.ShowSimpleShard("Title", "Subtitle");

            UIMenuItem old = new UIMenuItem("Old", $"Calls the ~b~{nameof(BigMessageHandler)}.{nameof(BigMessageHandler.ShowOldMessage)}~s~ method.");
            old.Activated += (m, s) => bigMessage.ShowOldMessage("Message");

            UIMenuItem rankup = new UIMenuItem("Rankup", $"Calls the ~b~{nameof(BigMessageHandler)}.{nameof(BigMessageHandler.ShowRankupMessage)}~s~ method.");
            rankup.Activated += (m, s) => bigMessage.ShowRankupMessage("Message", "Subtitle", 42);

            UIMenuItem missionPassed = new UIMenuItem("Mission Passed", $"Calls the ~b~{nameof(BigMessageHandler)}.{nameof(BigMessageHandler.ShowMissionPassedMessage)}~s~ method.");
            missionPassed.Activated += (m, s) => bigMessage.ShowMissionPassedMessage("Message");

            UIMenuItem colored = new UIMenuItem("Colored", $"Calls the ~b~{nameof(BigMessageHandler)}.{nameof(BigMessageHandler.ShowColoredShard)}~s~ method.");
            colored.Activated += (m, s) => bigMessage.ShowColoredShard("Message", "Description", HudColor.Red, HudColor.Green);

            UIMenuItem weaponPurchased = new UIMenuItem("Weapon Purchased", $"Calls the ~b~{nameof(BigMessageHandler)}.{nameof(BigMessageHandler.ShowWeaponPurchasedMessage)}~s~ method.");
            weaponPurchased.Activated += (m, s) => bigMessage.ShowWeaponPurchasedMessage("Message", "Name", WeaponHash.Pistol);

            UIMenuItem messageLarge = new UIMenuItem("Message Large", $"Calls the ~b~{nameof(BigMessageHandler)}.{nameof(BigMessageHandler.ShowMpMessageLarge)}~s~ method.");
            messageLarge.Activated += (m, s) => bigMessage.ShowMpMessageLarge("Message");

            AddItems(simple, old, rankup, missionPassed, colored, weaponPurchased, messageLarge);
        }
    }
}
