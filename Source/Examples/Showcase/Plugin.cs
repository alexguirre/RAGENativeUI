using System.Reflection;
using System.Windows.Forms;
using System.Linq;
using Rage;
using RAGENativeUI;

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

            if (Game.IsKeyDown(Keys.F5) && !UIMenu.IsAnyMenuVisible)
            {
                ShowcaseMenu.Visible = true;
            }

            // process input and draw the visible menus
            Pool.ProcessMenus();
        }
    }
}
