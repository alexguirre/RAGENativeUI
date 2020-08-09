namespace RNUIExamples
{
    using System.Windows.Forms;

    using Rage;
    using Rage.Attributes;
    using Rage.Native;

    using RAGENativeUI;

    internal static class InstructionalKeysExample
    {
        private static Keys ModifierKey { get; } = Keys.LControlKey;
        private static Keys SpawnCarKey { get; } = Keys.Y;
        
        private static ControllerButtons ModifierButton { get; } = ControllerButtons.LeftShoulder;
        private static ControllerButtons SpawnCarButton { get; } = ControllerButtons.DPadLeft;

        private static string SpawnCarKeyFormat { get; set; } = FormatKeyBinding(ModifierKey, SpawnCarKey);
        private static string SpawnCarButtonFormat { get; set; } = FormatKeyBinding(ModifierButton, SpawnCarButton);

        private static bool IsUsingController => !NativeFunction.Natives.xA571D46727E2B718<bool>(2);

        private static void Main()
        {
            while (true)
            {
                GameFiber.Yield();

                bool spawn;
                string format;
                if (IsUsingController)
                {
                    spawn = (ModifierButton == ControllerButtons.None || Game.IsControllerButtonDownRightNow(ModifierButton)) && Game.IsControllerButtonDown(SpawnCarButton);
                    format = SpawnCarButtonFormat;
                }
                else
                {
                    spawn = (ModifierKey == Keys.None || Game.IsKeyDownRightNow(ModifierKey)) && Game.IsKeyDown(SpawnCarKey);
                    format = SpawnCarKeyFormat;
                }

                Game.DisplayHelp($"Press {format} to spawn a car.");
                if (spawn)
                {
                    GameFiber.StartNew(() => new Vehicle(m => m.IsCar, Game.LocalPlayer.Character.GetOffsetPositionFront(5.0f)).Dismiss());
                }
            }
        }

        private static string FormatKeyBinding(ControllerButtons modifierKey, ControllerButtons key)
            => modifierKey == ControllerButtons.None ? $"~{key.GetInstructionalId()}~" :
                                                       $"~{modifierKey.GetInstructionalId()}~ ~+~ ~{key.GetInstructionalId()}~";

        private static string FormatKeyBinding(Keys modifierKey, Keys key)
            => modifierKey == Keys.None ? $"~{key.GetInstructionalId()}~" :
                                          $"~{modifierKey.GetInstructionalId()}~ ~+~ ~{key.GetInstructionalId()}~";

        // a command that simulates loading the plugin
        [ConsoleCommand]
        private static void RunInstructionalKeysExample() => GameFiber.StartNew(Main);
    }
}
