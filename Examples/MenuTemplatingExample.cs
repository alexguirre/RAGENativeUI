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
                test.DoSomething = false;
                test.DoSomeOtherThing = true;
                test.FloatValue = 42.5f;
                test.IntValue = 99;
                test.BuildMenu();


                while (true)
                {
                    RPH.GameFiber.Yield();

                    test.Display();

                    if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.T))
                    {
                        if (test.Menu.IsAnyChildMenuVisible)
                        {
                            test.Menu.Hide();
                        }
                        else
                        {
                            test.Menu.Show();
                        }
                    }

                    if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.Y))
                    {
                        test.DoSomething = RPH.MathHelper.GetRandomInteger(0, 100) < 50;
                        test.DoSomeOtherThing = RPH.MathHelper.GetRandomInteger(0, 100) < 50;
                        test.FloatValue = RPH.MathHelper.GetRandomSingle(0.0f, 100.0f);
                        test.IntValue = RPH.MathHelper.GetRandomInteger(0, 100);
                        test.EnumValue = RPH.MathHelper.Choose((GameControl[])Enum.GetValues(typeof(GameControl)));
                    }
                }
            });
        }

        [Menu(Title = "My Menu", Subtitle = "Subtitle for my menu")]
        private class TestMenu : MenuTemplate
        {
            private bool doSomething;
            private bool doSomeOtherThing;
            private float floatValue;
            private int intValue;
            private GameControl enumValue;

            [MenuItem(Text = "Basic Item")]
            private Action BasicItem => () =>
            {
                Game.DisplayNotification("Basic Item Activated");
            };

            [MenuItem(Text = "Basic Item 2")]
            private Action<MenuItem> BasicItem2 => (i) =>
            {
                Game.DisplayNotification("Basic Item 2 Activated");
                i.Metadata.TimesActivated = i.Metadata.ContainsKey("TimesActivated") ? (i.Metadata.TimesActivated + 1) : 1;
                i.Text = "Times Activated = " + i.Metadata.TimesActivated;
            };


            [MenuItem(Text = "Item With Custom Class")]
            private InvokableClass ItemWithCustomClass { get; } = new InvokableClass();

            [MenuItemCheckbox(Text = "Do Something?", Description = "Whether to do something or not.")]
            public bool DoSomething
            {
                get => doSomething;
                set => SetProperty(ref doSomething, value);
            }

            [MenuItemCheckbox(Text = "Do Some Other Thing?", Description = "Whether to do some other thing or not.")]
            public bool DoSomeOtherThing
            {
                get => doSomeOtherThing;
                set => SetProperty(ref doSomeOtherThing, value);
            }

            [MenuItemNumericScroller(Text = "Float Value")]
            public float FloatValue
            {
                get => floatValue;
                set => SetProperty(ref floatValue, value);
            }

            [MenuItemNumericScroller(Text = "Int Value as Dec", Increment = 1.0, DecimalPlaces = 0)]
            public int IntValue
            {
                get => intValue;
                set => SetProperty(ref intValue, value, nameof(IntValue), nameof(IntValueAsHex));
            }

            [MenuItemNumericScroller(Text = "Int Value as Hex", Increment = 1.0, DecimalPlaces = 0, Hexadecimal = true)]
            private int IntValueAsHex
            {
                get => IntValue;
                set => IntValue = value;
            }

            [MenuItemEnumScroller(Text = "Enum Value")]
            public GameControl EnumValue
            {
                get => enumValue;
                set => SetProperty(ref enumValue, value);
            }

            public void Display()
            {
                Game.DisplaySubtitle(
                    $"DoSomething:{DoSomething}~n~" +
                    $"DoSomeOtherThing:{DoSomeOtherThing}~n~" +
                    $"FloatValue:{FloatValue}~n~" +
                    $"IntValue:{IntValue}~n~" +
                    $"EnumValue:{EnumValue}"
                    );
            }

            [MenuItemActivatedHandler(nameof(IntValue), nameof(FloatValue))]
            private void OnIntOrFloatValueActivated(MenuItem sender, ActivatedEventArgs e)
            {
                Game.DisplayNotification(sender.Text + " -> Activated");
            }

            private class InvokableClass
            {
                int times;

                public void Invoke()
                {
                    Game.DisplayNotification("InvokableClass: Number of times activated -> " + ++times);
                }
            }
        }
    }
}

