namespace Examples
{
    using System;
    using System.Drawing;

    using Rage;
    using Rage.Attributes;
    
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

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                    {
                        if (instructionalButtons.Layout == InstructionalButtons.LayoutType.Horizontal)
                            instructionalButtons.Layout = InstructionalButtons.LayoutType.Vertical;
                        else
                            instructionalButtons.Layout = InstructionalButtons.LayoutType.Horizontal;
                    }

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.U))
                    {
                        instructionalButtons.BackgroundColor = Color.FromArgb(MathHelper.GetRandomInteger(0, 255), MathHelper.GetRandomInteger(0, 255), MathHelper.GetRandomInteger(0, 255), MathHelper.GetRandomInteger(0, 255));
                    }

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.I))
                    {
                        instructionalButtons.BackgroundColor = Color.FromArgb(80, 0, 0, 0);
                    }

                }
            });
        }
    }
}

