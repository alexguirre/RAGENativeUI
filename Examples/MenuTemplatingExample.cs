namespace Examples
{
#if RPH1
    extern alias rph1;
    using GameControl = rph1::Rage.GameControl;
    using Game = rph1::Rage.Game;
    using MathHelper = rph1::Rage.MathHelper;
    using Vector3 = rph1::Rage.Vector3;
    using ConsoleCommandAttribute = rph1::Rage.Attributes.ConsoleCommandAttribute;
#else
    /** REDACTED **/
#endif

    using System;
    using System.Linq;
    using System.Drawing;

    using RAGENativeUI;
    using RAGENativeUI.Menus;
    using RAGENativeUI.Menus.Themes;
    using RAGENativeUI.Menus.Templating;

    internal static class MenuTemplatingExample
    {
        [ConsoleCommand(Name = "MenuTemplatingExample", Description = "Example showing RAGENativeUI menu API")]
        private static void Command()
        {
            RPH.GameFiber.StartNew(() =>
            {
                TestMenu test = new TestMenu();
                Menu menu = test.Build();


                while (true)
                {
                    RPH.GameFiber.Yield();

                    test.Display();

                    if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.T))
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

                    if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.Y))
                    {
                        test.DoSomething = RPH.MathHelper.GetRandomInteger(0, 100) < 50;
                        test.DoSomeOtherThing = RPH.MathHelper.GetRandomInteger(0, 100) < 50;
                    }
                }
            });
        }
    }
}

