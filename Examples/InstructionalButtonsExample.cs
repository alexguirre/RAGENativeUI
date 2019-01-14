namespace Examples
{
#if RPH1
    extern alias rph1;
    using GameFiber = rph1::Rage.GameFiber;
    using ConsoleCommandAttribute = rph1::Rage.Attributes.ConsoleCommandAttribute;
    using GameControl = rph1::Rage.GameControl;
    using MathHelper = rph1::Rage.MathHelper;
#else
    /** REDACTED **/
#endif

    using System;
    using System.Drawing;

    using RAGENativeUI;
    using RAGENativeUI.Scaleforms;

    internal static class InstructionalButtonsExample
    {
        [ConsoleCommand(Name = "InstructionalButtonsExample", Description = "Example showing the BigMessage class.")]
        private static void Command()
        {
            InstructionalButtons instructionalButtons = new InstructionalButtons();
            instructionalButtons.AddSlot("RappelJump 0", 0, GameControl.RappelJump);
            instructionalButtons.AddSlot("RappelJump 1", 1, GameControl.RappelJump);
            instructionalButtons.AddSlot("RappelJump 2", 2, GameControl.RappelJump);
            instructionalButtons.AddSlot("Label", "Text");
            instructionalButtons.AddSlot("Key", "K", InstructionalButtonStyle.Key);
            instructionalButtons.AddSlot("MultiCharKey", "KEY", InstructionalButtonStyle.MultiCharKey);
            instructionalButtons.AddSlot("WideKey", "WIDE", InstructionalButtonStyle.WideKey);
            instructionalButtons.AddSlot("Reload", GameControl.Reload);
            instructionalButtons.AddSlot("MoveUpDown", GameControl.MoveUpDown);
            instructionalButtons.AddSlot("MoveLeftRight", GameControl.MoveLeftRight);
            foreach (GameControl control in Enum.GetValues(typeof(GameControl)))
            {
                if (control.ToString().StartsWith("Frontend"))
                {
                    instructionalButtons.AddSlot(control.ToString(), control);
                }
            }

            GameFiber.StartNew(() =>
            {
                while (true)
                {
                    GameFiber.Yield();
                    
                    instructionalButtons.Draw();

                    if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.Y))
                    {
                        if (instructionalButtons.Layout == InstructionalButtons.LayoutType.Horizontal)
                            instructionalButtons.Layout = InstructionalButtons.LayoutType.Vertical;
                        else
                            instructionalButtons.Layout = InstructionalButtons.LayoutType.Horizontal;
                    }

                    if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.U))
                    {
                        instructionalButtons.BackgroundColor = Color.FromArgb(MathHelper.GetRandomInteger(0, 255), MathHelper.GetRandomInteger(0, 255), MathHelper.GetRandomInteger(0, 255), MathHelper.GetRandomInteger(0, 255));
                    }

                    if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.I))
                    {
                        instructionalButtons.BackgroundColor = Color.FromArgb(80, 0, 0, 0);
                    }

                }
            });
        }
    }
}

