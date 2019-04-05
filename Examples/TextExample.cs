namespace Examples
{
#if RPH1
    extern alias rph1;
    using ConsoleCommandAttribute = rph1::Rage.Attributes.ConsoleCommandAttribute;
#else
    /** REDACTED **/
#endif

    using System;
    using System.Drawing;

    using RAGENativeUI;
    using RAGENativeUI.Text;
    using RAGENativeUI.Drawing;

    internal static class TextExample
    {
        [ConsoleCommand(Name = "TextExample", Description = "Example showing the Text class.")]
        private static void Command()
        {
            Text text1 = new Text() { Style = new TextStyle { Alignment = TextAlignment.Left, Color = Color.White, Scale = 0.5f } };
            text1.SetUnformattedString("Left");
            Text text2 = new Text() { Style = new TextStyle { Alignment = TextAlignment.Center, Color = Color.White, Scale = 0.5f } };
            text2.SetUnformattedString("Center");
            Text text3 = new Text() { Style = new TextStyle { Alignment = TextAlignment.Right, Color = Color.White, Scale = 0.5f } };
            text3.SetUnformattedString("Right");
            Text text4 = new Text("HUD_CASH") { Style = new TextStyle { Alignment = TextAlignment.Left, Color = Color.Green, Scale = 0.6f, Outline = true } };
            text4.AddComponentFloat(11500.4233f, 2);
            Text text5 = new Text("STRING") { Style = new TextStyle { Alignment = TextAlignment.Center, Color = Color.Green, Outline = true, Scale = 0.6f } };
            text5.AddComponentTime(TimeSpan.Zero, TextComponentTimeOptions.Hours | TextComponentTimeOptions.Milliseconds | TextComponentTimeOptions.UseDotAsMillisecondsSeparator);
            Text text6 = new Text { Style = new TextStyle { Alignment = TextAlignment.Center, Color = Color.Green, Outline = true, Scale = 0.6f } };
            text6.SetUnformattedString("Multiline~n~Text~n~Multiline~n~Text");

            RPH.GameFiber.StartNew(() =>
            {
                var v = 0.0f;
                while (true)
                {
                    RPH.GameFiber.Yield();

                    (text5.Components[0] as TextComponentTime).Time = DateTime.Now.TimeOfDay;

                    text1.Display((0.5f, 0.25f).Rel());
                    text2.Display((0.5f, 0.5f).Rel());
                    text3.Display((0.5f, 0.75f).Rel());
                    text4.Display((0.1f, 0.1f).Rel());

                    float width = text5.CalculateWidth();
                    float height = text5.Style.Height;
                    Rect.Draw((0.1f, 0.15f + height * 0.6f).Rel(), (width, height).Rel(), Color.FromArgb(150, Color.Red));
                    text5.Display((0.1f, 0.15f).Rel());

                    if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.Add))
                    {
                        text5.Style.Scale += RPH.Game.FrameTime;
                    }
                    else if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.Subtract))
                    {
                        text5.Style.Scale -= RPH.Game.FrameTime;
                    }

                    text5.DisplaySubtitle(0, true);


                    var pos = (0.7f, 0.15f).Rel();
                    int lines = text6.CalculateLineCount(pos);
                    width = text6.CalculateWidth();
                    height = text6.Style.Height;
                    Rect.Draw(pos + (height * lines * 0.6f).YRel(), (width, height * lines).Rel(), Color.FromArgb(150, Color.Red));
                    text6.Display((0.7f, 0.15f).Rel());

                    if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.Add))
                    {
                        text6.Style.Scale += RPH.Game.FrameTime;
                    }
                    else if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.Subtract))
                    {
                        text6.Style.Scale -= RPH.Game.FrameTime;
                    }
                }
            });
        }
    }
}

