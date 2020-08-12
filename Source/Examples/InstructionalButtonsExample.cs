namespace RNUIExamples
{
    using System.Windows.Forms;

    using Rage;
    using Rage.Attributes;
    using Rage.Native;

    using RAGENativeUI;
    using RAGENativeUI.Elements;

    internal static class InstructionalButtonsExample
    {
        private static Keys ModifierKey { get; } = Keys.LControlKey;
        private static Keys SpawnCarKey { get; } = Keys.Y;

        private static ControllerButtons ModifierButton { get; } = ControllerButtons.LeftShoulder;
        private static ControllerButtons SpawnCarButton { get; } = ControllerButtons.DPadLeft;

        private static bool IsUsingController => !NativeFunction.Natives.xA571D46727E2B718<bool>(2);
        private static bool HasInputJustChanged => NativeFunction.Natives.x6CD79468A1E595C6<bool>(2);

        private static void Main()
        {
            var instructional = new InstructionalButtons();
            var spawnCarGroup = new InstructionalButtonGroup("Spawn Car", ModifierKey.GetInstructionalKey(), InstructionalKey.SymbolPlus, SpawnCarKey.GetInstructionalKey());

            instructional.Buttons.Add(spawnCarGroup);

            while (true)
            {
                GameFiber.Yield();

                if (HasInputJustChanged)
                {
                    var (modifier, key) = IsUsingController ? (ModifierButton.GetInstructionalKey(), SpawnCarButton.GetInstructionalKey()) :
                                                              (ModifierKey.GetInstructionalKey(), SpawnCarKey.GetInstructionalKey());
                    spawnCarGroup.Buttons[0] = modifier;
                    spawnCarGroup.Buttons[2] = key;
                    instructional.Update();
                }

                instructional.Draw();

                bool spawn = IsUsingController ? (ModifierButton == ControllerButtons.None || Game.IsControllerButtonDownRightNow(ModifierButton)) && Game.IsControllerButtonDown(SpawnCarButton) :
                                                 (ModifierKey == Keys.None || Game.IsKeyDownRightNow(ModifierKey)) && Game.IsKeyDown(SpawnCarKey);

                if (spawn)
                {
                    GameFiber.StartNew(() => new Vehicle(m => m.IsCar, Game.LocalPlayer.Character.GetOffsetPositionFront(5.0f)).Dismiss());
                }
            }
        }

        // a command that simulates loading the plugin
        [ConsoleCommand]
        private static void RunInstructionalButtonsExample() => GameFiber.StartNew(Main);
    }
}
