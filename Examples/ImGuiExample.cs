namespace Examples
{
#if RPH1
    extern alias rph1;
    using GameFiber = rph1::Rage.GameFiber;
    using Game = rph1::Rage.Game;
    using Vector2 = rph1::Rage.Vector2;
    using Vehicle = rph1::Rage.Vehicle;
    using ConsoleCommandAttribute = rph1::Rage.Attributes.ConsoleCommandAttribute;
#else
    /** REDACTED **/
#endif

    using System.Drawing;
    
    using RAGENativeUI.ImGui;

    internal static class ImGuiExample
    {
        [ConsoleCommand(Name = "ImGuiExample", Description = "Example showing the ImGui.Gui class.")]
        private static void Command()
        {
            RectangleF windowRect1 = new RectangleF(10, 10, 350, 585);
            RectangleF windowRect2 = new RectangleF(900, 10, 650, 650);
            RectangleF windowRect3 = new RectangleF(5, 50, 320, 320);
            RectangleF windowRect4 = new RectangleF(5, 25, 180, 180);
            float slider1Value = 5.0f;
            float slider2Value = 50.0f;
            int slider3Value = 100;
            float slider4Value = 0.5f;
            bool toggle1Value = false;
            bool toggle2Value = false;
            Vector2 scrollView1Pos = new Vector2(0f, 0f);
            Vector2 scrollView2Pos = new Vector2(0f, 0f);
            string[] modelNames = { "RHINO", "POLICE", "FBI", "ZENTORNO", "ADDER" };

            Gui.Do += () =>
            {
                Gui.Mouse(); // enable mouse

                // main window
                Gui.BeginWindow(ref windowRect1, "Test");

                Gui.Label(new RectangleF(5, 5, 100, 40), "Label");

                if (Gui.Button(new RectangleF(5, 50, 125, 50), "Button"))
                {
                    Game.DisplayNotification("Button Pressed!");
                }

                Gui.Label(new RectangleF(5, 110, 300, 25), $"{slider1Value.ToString("0.0000")}");
                Gui.HorizontalSlider(new RectangleF(5, 145, 320, 30), ref slider1Value, 0.0f, 10.0f);

                Gui.Label(new RectangleF(5, 180, 300, 25), $"{slider2Value.ToString("0.0000")}");
                Gui.VerticalSlider(new RectangleF(5, 215, 30, 320), ref slider2Value, -1000.0f, 1000.0f);

                Gui.Label(new RectangleF(110, 225, 200, 30), $"{slider3Value}");
                Gui.HorizontalSlider(new RectangleF(110, 260, 160, 20), ref slider3Value, 0, 255);

                Gui.Toggle(new RectangleF(180, 200, 140, 25), $"Toggle: {toggle1Value}", ref toggle1Value);

                Gui.BeginScrollView(new RectangleF(80, 300, 250, 250), ref scrollView1Pos, new SizeF(400, 400));
                Gui.Label(new RectangleF(5f, 5f, 200f, 20f), "Scroll View");
                Gui.Button(new RectangleF(5f, 30f, 200f, 20f), "Button");
                Gui.Toggle(new RectangleF(5f, 55f, 200f, 20f), "Toggle", ref toggle2Value);
                Gui.HorizontalSlider(new RectangleF(5f, 80f, 200f, 20f), ref slider4Value, 0.0f, 1.0f);
                Gui.VerticalSlider(new RectangleF(5f, 105f, 20f, 200f), ref slider4Value, 0.0f, 1.0f);
                Gui.EndScrollView();

                Gui.EndWindow();


                // nested windows
                Gui.BeginWindow(ref windowRect2, "Test 2");

                if (Gui.Button(new RectangleF(5, 5, 120, 35), "Button 1"))
                {
                    Game.DisplayNotification("First button pressed from second window!");
                }

                Gui.BeginScrollView(new RectangleF(5, 130, 200, 250), ref scrollView2Pos, new SizeF(200, 400));
                for (int i = 0; i < modelNames.Length; i++)
                {
                    if (Gui.Button(new RectangleF(1, 1 + 20 * i, 198, 20), modelNames[i]))
                    {
                        string model = modelNames[i];
                        GameFiber.StartNew(() => { new Vehicle(model, Game.LocalPlayer.Character.Position).IsPersistent = false; });
                    }
                }
                Gui.EndScrollView();

                Gui.BeginWindow(ref windowRect3, "Winception");
                if (Gui.Button(new RectangleF(5, 45, 120, 35), "Button 2"))
                {
                    Game.DisplayNotification("Second button pressed from second window!");
                }

                Gui.BeginWindow(ref windowRect4, "Winception");
                if (Gui.Button(new RectangleF(5, 45, 120, 35), "Button 3"))
                {
                    Game.DisplayNotification("Third button pressed from second window!");
                }

                Gui.EndWindow();
                Gui.EndWindow();
                Gui.EndWindow();


                // fixed window
                Gui.BeginWindow(new RectangleF(Game.Resolution.Width - 175, Game.Resolution.Height - 175, 150, 150), "Fixed Window");

                if(Gui.Button(new RectangleF(5f, 5f, 140f, 110f), "A button"))
                {
                    Game.DisplayNotification("Button pressed from fixed window!");
                }

                Gui.EndWindow();

                // without window
                Gui.Label(new RectangleF(5f, 5f, 300f, 70f), "Label without window", 18.5f);
                Gui.Button(new RectangleF(5f, 75f, 200f, 80f), "Button without window");
            };
        }
    }
}

