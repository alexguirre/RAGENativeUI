using System.Reflection;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using Rage;
using RAGENativeUI;
using RAGENativeUI.PauseMenu;
using Rage.Attributes;

internal static class Plugin
{
    public const string MenuTitle = "RAGENativeUI";

    public static MenuPool Pool { get; } = new MenuPool();
    private static UIMenu ShowcaseMenu { get; set; }

    public static void Main()
    {
        Game.Console.Print("- Press F5 to open the showcase menu");
        Game.Console.Print("- The following commands are available:");
        foreach (var name in Assembly.GetExecutingAssembly()
                                     .GetTypes()
                                     .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                                     .Where(m => m.GetCustomAttribute<Rage.Attributes.ConsoleCommandAttribute>() != null)
                                     .Select(m => m.Name))
        {
            Game.Console.Print($"      {name}");
        }

        ShowcaseMenu = new RNUIExamples.Showcase.MainMenu();

        // draw custom texture banners
        Game.RawFrameRender += (s, e) => Pool.DrawBanners(e.Graphics);

        while (true)
        {
            GameFiber.Yield();

            if (Game.IsKeyDown(Keys.F5) && !UIMenu.IsAnyMenuVisible && !TabView.IsAnyPauseMenuVisible)
            {
                ShowcaseMenu.Visible = true;
            }

            // process input and draw the visible menus
            Pool.ProcessMenus();
        }
    }
    [ConsoleCommand]
    private static void InstructionalKeysSimpleTest()
    {
        Game.DisplayHelp($"Press ~{Keys.Space.GetInstructionalId()}~ ~+~ ~{Keys.MButton.GetInstructionalId()}~ ~+~ ~{ControllerButtons.A.GetInstructionalId()}~ ~+~ ~{Keys.A.GetInstructionalId()}~ ~+~ ~{Keys.OemCloseBrackets.GetInstructionalId()}~");
    }

    [ConsoleCommand]
    private static void InstructionalKeysTest()
    {
        GameFiber.StartNew(() =>
        {
            foreach (var keys in GetKeys())
            {
                string str = string.Join("~n~", keys.Select(k => $"~{k.GetId()}~"));
                Game.DisplayHelp(str);
                GameFiber.Sleep(5000);
            }
        });

        static IEnumerable<InstructionalKey[]> GetKeys()
        {
            yield return new InstructionalKey[]
            {
                ControllerButtons.DPadUp.GetInstructionalKey(),
                ControllerButtons.DPadDown.GetInstructionalKey(),
                ControllerButtons.DPadLeft.GetInstructionalKey(),
                ControllerButtons.DPadRight.GetInstructionalKey(),
                ControllerButtons.Start.GetInstructionalKey(),
                ControllerButtons.Back.GetInstructionalKey(),
                ControllerButtons.LeftThumb.GetInstructionalKey(),
                ControllerButtons.RightThumb.GetInstructionalKey(),
                ControllerButtons.LeftShoulder.GetInstructionalKey(),
                ControllerButtons.RightShoulder.GetInstructionalKey(),
                InstructionalKey.ControllerLTrigger,
                InstructionalKey.ControllerRTrigger,
                ControllerButtons.A.GetInstructionalKey(),
                ControllerButtons.B.GetInstructionalKey(),
                ControllerButtons.X.GetInstructionalKey(),
                ControllerButtons.Y.GetInstructionalKey(),
            };

            yield return new InstructionalKey[]
            {
                InstructionalKey.MouseLeft,
                InstructionalKey.MouseRight,
                InstructionalKey.MouseMiddle,
                InstructionalKey.MouseExtra1,
                InstructionalKey.MouseExtra2,
                InstructionalKey.MouseExtra3,
                InstructionalKey.MouseExtra4,
                InstructionalKey.MouseExtra5,
            };

            yield return new InstructionalKey[]
            {
                InstructionalKey.MouseWheelUp,
                InstructionalKey.MouseWheelDown,
                InstructionalKey.MouseWheel,
            };

            yield return new InstructionalKey[]
            {
                InstructionalKey.MouseAxisX,
                InstructionalKey.MouseAxisXLeft,
                InstructionalKey.MouseAxisXRight,
                InstructionalKey.MouseAxisY,
                InstructionalKey.MouseAxisYUp,
                InstructionalKey.MouseAxisYDown,
            };

            yield return new InstructionalKey[]
            {
                InstructionalKey.ControllerAxisLX,
                InstructionalKey.ControllerAxisLY,
                InstructionalKey.ControllerAxisLYUp,
                InstructionalKey.ControllerAxisLYDown,
                InstructionalKey.ControllerAxisLXLeft,
                InstructionalKey.ControllerAxisLXRight,
                InstructionalKey.ControllerAxisRX,
                InstructionalKey.ControllerAxisRY,
                InstructionalKey.ControllerAxisRYUp,
                InstructionalKey.ControllerAxisRYDown,
                InstructionalKey.ControllerAxisRXLeft,
                InstructionalKey.ControllerAxisRXRight,
            };

            yield return new InstructionalKey[]
            {
                InstructionalKey.ControllerDPadAll,
                InstructionalKey.ControllerDPadUpDown,
                InstructionalKey.ControllerDPadLeftRight,
                InstructionalKey.ControllerLStickRotate,
                InstructionalKey.ControllerRStickRotate,
                InstructionalKey.SymbolBusySpinner,
                InstructionalKey.SymbolPlus,
                InstructionalKey.SymbolArrowUp,
                InstructionalKey.SymbolArrowDown,
                InstructionalKey.SymbolArrowLeft,
                InstructionalKey.SymbolArrowRight,
                InstructionalKey.SymbolArrowUpDown,
                InstructionalKey.SymbolArrowLeftRight,
                InstructionalKey.SymbolArrowAll,
            };
        }
    }
}
