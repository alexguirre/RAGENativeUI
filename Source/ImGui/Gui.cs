namespace RAGENativeUI.ImGui
{
    using System;
    using System.Drawing;
    using System.Runtime.CompilerServices;

    using Rage;
    using Rage.Native;
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Rendering;

    public static class Gui
    {
        public delegate void GuiEventHandler();

        private static Graphics graphics;
        private static bool isMouseEnabled;
        private static MouseState lastMouseState;
        private static MouseState currentMouseState;
        private static Vector2 mousePosition;
        private static Texture mouseTexture;
        private static RectangleF currentParentContainer;
        private static bool firstMouseCall;

        public static event GuiEventHandler Do;

        static Gui()
        {
            Game.FrameRender += OnFrameRender;
        }

        private static void OnFrameRender(object sender, GraphicsEventArgs e)
        {
            if (Do != null)
            {
                firstMouseCall = true;
                bool drawMouse = false;
                graphics = e.Graphics;

                Delegate[] delegates = Do.GetInvocationList();
                for (int i = 0; i < delegates.Length; i++)
                {
                    ((GuiEventHandler)delegates[i]).Invoke();

                    if (isMouseEnabled)
                        drawMouse = true;

                    isMouseEnabled = false;
                    currentParentContainer = new RectangleF(0f, 0f, Game.Resolution.Width, Game.Resolution.Height);
                }

                if (drawMouse)
                {
                    if (mouseTexture == null)
                    {
                        mouseTexture = Game.CreateTextureFromFile("cursor_32_2.png");
                    }

                    graphics.DrawTexture(mouseTexture, mousePosition.X, mousePosition.Y, 32.0f, 32.0f);
                }
            }
            graphics = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EnsureCall()
        {
            if (graphics == null)
                throw new InvalidOperationException($"{nameof(Gui)} methods cannot be called from outside the {nameof(Gui)}.{nameof(Do)} event.");
        }

        public static Vector2 Mouse(bool disableGameControls = true)
        {
            EnsureCall();

            isMouseEnabled = true;
            if (firstMouseCall)
            {
                lastMouseState = currentMouseState;
                currentMouseState = Game.GetMouseState();
                firstMouseCall = false;
            }
            mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);

            if (disableGameControls)
            {
                NativeFunction.Natives.DisableAllControlActions(0);
            }

            return mousePosition;
        }

        public static RectangleF Window(RectangleF position, string title)
        {
            EnsureCall();

            const float TitleBarHeight = 30f;

            Vector2 drawPos = new Vector2(position.Location.X + currentParentContainer.X, position.Location.Y + currentParentContainer.Y);

            RectangleF windRect = new RectangleF(drawPos.X, drawPos.Y + TitleBarHeight, position.Width, position.Height - TitleBarHeight);
            RectangleF titleBarRect = new RectangleF(drawPos.X, drawPos.Y, position.Width, TitleBarHeight);

            graphics.DrawRectangle(titleBarRect, Color.FromArgb(215, 15, 15, 15));
            graphics.DrawRectangle(windRect, Color.FromArgb(150, 45, 45, 45));

            DrawText(titleBarRect, title, 20f);

            if (isMouseEnabled)
            {
                if (titleBarRect.Contains(mousePosition.X, mousePosition.Y) && currentMouseState.IsLeftButtonDown)
                {
                    Vector2 offset = new Vector2(currentMouseState.X - lastMouseState.X, currentMouseState.Y - lastMouseState.Y);
                    float newX = position.Location.X + offset.X;
                    float newY = position.Location.Y + offset.Y;
                    newX = MathHelper.Clamp(newX + currentParentContainer.X, currentParentContainer.X, currentParentContainer.Right - position.Width) - currentParentContainer.X;
                    newY = MathHelper.Clamp(newY + currentParentContainer.Y, currentParentContainer.Y, currentParentContainer.Bottom - position.Height) - currentParentContainer.Y;
                    position.Location = new PointF(newX, newY);
                }
            }

            currentParentContainer = windRect;

            return position;
        }

        public static bool Button(RectangleF position, string text)
        {
            EnsureCall();

            position.Location = new PointF(position.Location.X + currentParentContainer.X, position.Location.Y + currentParentContainer.Y);

            bool hovered = false;
            bool down = false;

            if (isMouseEnabled)
            {
                hovered = position.Contains(mousePosition.X, mousePosition.Y);
                if(hovered)
                {
                    down = Game.IsKeyDown(System.Windows.Forms.Keys.LButton);
                }
            }

            graphics.DrawRectangle(position, hovered ? down ? Color.FromArgb(200, 10, 10, 10) : Color.FromArgb(175, 20, 20, 20) : Color.FromArgb(160, 25, 25, 25));

            graphics.DrawLine(new Vector2(position.X, position.Y), new Vector2(position.X + position.Width, position.Y), Color.Black);
            graphics.DrawLine(new Vector2(position.X, position.Y), new Vector2(position.X , position.Y + position.Height), Color.Black);
            graphics.DrawLine(new Vector2(position.X + position.Width, position.Y), new Vector2(position.X + position.Width, position.Y + position.Height), Color.Black);
            graphics.DrawLine(new Vector2(position.X, position.Y + position.Height), new Vector2(position.X + position.Width, position.Y + position.Height), Color.Black);

            DrawText(position, text);

            return down;
        }

        public static void Label(RectangleF rectangle, string text, float fontSize = 15.0f, TextHorizontalAligment hAlign = TextHorizontalAligment.Left, TextVerticalAligment vAlign = TextVerticalAligment.Center)
        {
            EnsureCall();

            rectangle.Location = new PointF(rectangle.Location.X + currentParentContainer.X, rectangle.Location.Y + currentParentContainer.Y);

            DrawText(rectangle, text, fontSize, hAlign, vAlign);
        }




        private static void DrawText(RectangleF rectangle, string text, float fontSize = 15.0f, TextHorizontalAligment hAlign = TextHorizontalAligment.Center, TextVerticalAligment vAlign = TextVerticalAligment.Center)
        {
            SizeF textSize = Graphics.MeasureText(text, "Consolas", fontSize);
            float x = 0.0f, y = 0.0f;

            switch (hAlign)
            {
                case TextHorizontalAligment.Left:
                    x = rectangle.X;
                    break;
                case TextHorizontalAligment.Center:
                    x = rectangle.X + rectangle.Width * 0.5f - textSize.Width * 0.5f;
                    break;
                case TextHorizontalAligment.Right:
                    x = rectangle.Right - textSize.Width - 2.0f;
                    break;
            }

            switch (vAlign)
            {
                case TextVerticalAligment.Top:
                    y = rectangle.Y;
                    break;
                case TextVerticalAligment.Center:
                    y = rectangle.Y + rectangle.Height * 0.5f - textSize.Height * 0.8f;
                    break;
                case TextVerticalAligment.Down:
                    y = rectangle.Y + rectangle.Height - textSize.Height * 1.6f;
                    break;
            }

            graphics.DrawText(text, "Consolas", fontSize, new PointF(x, y), Color.White, rectangle);
        }
    }
}

