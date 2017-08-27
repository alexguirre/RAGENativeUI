namespace RAGENativeUI.ImGui
{
    using System;
    using System.Drawing;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    using Rage;
    using Rage.Native;
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Rendering;

    public static partial class Gui
    {
        public delegate void GuiEventHandler();
        
        private static State state;

        private static Texture mouseTexture;

        public static event GuiEventHandler Do;

        static Gui()
        {
            Game.FrameRender += OnFrameRender;
        }

        private static void OnFrameRender(object sender, GraphicsEventArgs e)
        {
            if (Do != null)
            {
                state.ResetIds();
                
                state.HasMouseBeenCalled = false;
                bool drawMouse = false;
                state.Graphics = e.Graphics;

                Delegate[] delegates = Do.GetInvocationList();
                for (int i = 0; i < delegates.Length; i++)
                {
                    ((GuiEventHandler)delegates[i]).Invoke();

                    if (state.IsMouseEnabled)
                        drawMouse = true;

                    state.IsMouseEnabled = false;
                    state.CurrentParentContainer = new RectangleF(0f, 0f, Game.Resolution.Width, Game.Resolution.Height);
                }

                if (drawMouse)
                {
                    if (mouseTexture == null)
                    {
                        mouseTexture = Game.CreateTextureFromFile("cursor_32_2.png");
                    }

                    state.Graphics.DrawTexture(mouseTexture, state.MousePosition.X, state.MousePosition.Y, 32.0f, 32.0f);
                }
            }
            state.Graphics = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EnsureCall()
        {
            if (state.Graphics == null)
                throw new InvalidOperationException($"{nameof(Gui)} methods cannot be called from outside the {nameof(Gui)}.{nameof(Do)} event.");
        }

        public static Vector2 Mouse(bool disableGameControls = true)
        {
            EnsureCall();

            state.IsMouseEnabled = true;
            if (!state.HasMouseBeenCalled)
            {
                state.LastMouseState = state.CurrentMouseState;
                state.CurrentMouseState = Game.GetMouseState();
                state.HasMouseBeenCalled = true;
            }
            state.MousePosition = new Vector2(state.CurrentMouseState.X, state.CurrentMouseState.Y);

            if (disableGameControls)
            {
                NativeFunction.Natives.DisableAllControlActions(0);
            }

            return state.MousePosition;
        }

        public static RectangleF Window(RectangleF position, string title)
        {
            EnsureCall();
            uint id = state.Id(true, false);

            const float TitleBarHeight = 30f;

            Vector2 drawPos = new Vector2(position.Location.X + state.CurrentParentContainer.X, position.Location.Y + state.CurrentParentContainer.Y);

            RectangleF windRect = new RectangleF(drawPos.X, drawPos.Y + TitleBarHeight, position.Width, position.Height - TitleBarHeight);
            RectangleF titleBarRect = new RectangleF(drawPos.X, drawPos.Y, position.Width, TitleBarHeight);

            state.Graphics.DrawRectangle(titleBarRect, Color.FromArgb(215, 15, 15, 15));
            state.Graphics.DrawRectangle(windRect, Color.FromArgb(150, 45, 45, 45));

            DrawText(titleBarRect, title, 20f);

            if (state.IsMouseEnabled)
            {
                if (state.IsDragging(id))
                {
                    if (state.CurrentMouseState.IsLeftButtonUp)
                    {
                        state.Drop();
                    }
                    else
                    {
                        Vector2 offset = state.DragOffset();
                        float newX = position.Location.X + offset.X;
                        float newY = position.Location.Y + offset.Y;
                        newX = MathHelper.Clamp(newX + state.CurrentParentContainer.X, state.CurrentParentContainer.X, state.CurrentParentContainer.Right - position.Width) - state.CurrentParentContainer.X;
                        newY = MathHelper.Clamp(newY + state.CurrentParentContainer.Y, state.CurrentParentContainer.Y, state.CurrentParentContainer.Bottom - position.Height) - state.CurrentParentContainer.Y;
                        position.Location = new PointF(newX, newY);
                    }
                }
                else if(!state.IsDraggingAny() && titleBarRect.Contains(state.MousePosition.X, state.MousePosition.Y) && state.CurrentMouseState.IsLeftButtonDown)
                {
                    state.Drag(id);
                }
            }

            state.CurrentParentContainer = windRect;

            DrawTextDebug(windRect.Location, $"Window {id.ToString("X8")}", 18.0f);

            return position;
        }

        public static bool Button(RectangleF position, string text)
        {
            EnsureCall();
            uint id = state.Id();

            position.Location = new PointF(position.Location.X + state.CurrentParentContainer.X, position.Location.Y + state.CurrentParentContainer.Y);

            bool hovered = false;
            bool down = false;

            if (state.IsMouseEnabled)
            {
                hovered = position.Contains(state.MousePosition.X, state.MousePosition.Y);
                if(hovered)
                {
                    down = Game.IsKeyDown(System.Windows.Forms.Keys.LButton);
                }
            }

            state.Graphics.DrawRectangle(position, hovered ? down ? Color.FromArgb(200, 10, 10, 10) : Color.FromArgb(175, 20, 20, 20) : Color.FromArgb(160, 25, 25, 25));

            state.Graphics.DrawLine(new Vector2(position.X, position.Y), new Vector2(position.X + position.Width, position.Y), Color.Black);
            state.Graphics.DrawLine(new Vector2(position.X, position.Y), new Vector2(position.X , position.Y + position.Height), Color.Black);
            state.Graphics.DrawLine(new Vector2(position.X + position.Width, position.Y), new Vector2(position.X + position.Width, position.Y + position.Height), Color.Black);
            state.Graphics.DrawLine(new Vector2(position.X, position.Y + position.Height), new Vector2(position.X + position.Width, position.Y + position.Height), Color.Black);

            DrawText(position, text);

            DrawTextDebug(position.Location, $"Button {id.ToString("X8")}", 18.0f);

            return down;
        }

        public static void Label(RectangleF rectangle, string text, float fontSize = 15.0f, TextHorizontalAligment hAlign = TextHorizontalAligment.Left, TextVerticalAligment vAlign = TextVerticalAligment.Center)
        {
            EnsureCall();
            uint id = state.Id();

            rectangle.Location = new PointF(rectangle.Location.X + state.CurrentParentContainer.X, rectangle.Location.Y + state.CurrentParentContainer.Y);

            DrawText(rectangle, text, fontSize, hAlign, vAlign);

            DrawTextDebug(rectangle.Location, $"Label {id.ToString("X8")}", 18.0f);
        }

        public static float HorizontalSlider(RectangleF rectangle, float value, float minValue, float maxValue)
        {
            EnsureCall();
            uint id = state.Id();

            rectangle.Location = new PointF(rectangle.Location.X + state.CurrentParentContainer.X, rectangle.Location.Y + state.CurrentParentContainer.Y);
            float handleSize = rectangle.Height - 6;
            float handleRelativePos = (value - minValue) / (maxValue - minValue);
            RectangleF handleRect = new RectangleF(handleRelativePos * (rectangle.Width - handleSize - 6) + 3 + rectangle.Location.X, rectangle.Location.Y + 3, handleSize, handleSize);

            bool hovered = false;
            bool down = false;

            if (state.IsMouseEnabled)
            {
                if (state.IsDragging(id))
                {
                    if (state.CurrentMouseState.IsLeftButtonUp)
                    {
                        state.Drop();
                    }
                    else
                    {
                        down = true;
                        hovered = true;

                        float handlePos = handleRect.X;

                        float offset = state.DragOffset().X;

                        if (offset != 0)
                        {
                            value = MathHelper.Clamp(value + (offset * (maxValue - minValue) / rectangle.Width), minValue, maxValue);
                        }
                    }
                }
                else if (!state.IsDraggingAny() && handleRect.Contains(state.MousePosition.X, state.MousePosition.Y) && state.CurrentMouseState.IsLeftButtonDown)
                {
                    down = true;
                    hovered = true;
                    state.Drag(id);
                }
            }

            state.Graphics.DrawRectangle(rectangle, Color.FromArgb(230, 10, 10, 10));
            state.Graphics.DrawRectangle(handleRect, hovered ? down ? Color.FromArgb(240, 95, 95, 95) : Color.FromArgb(240, 70, 70, 70) : Color.FromArgb(240, 55, 55, 55));

            DrawTextDebug(rectangle.Location, $"HSlider {id.ToString("X8")}", 18.0f);

            return value;
        }

        public static float VerticalSlider(RectangleF rectangle, float value, float minValue, float maxValue)
        {
            EnsureCall();
            uint id = state.Id();

            rectangle.Location = new PointF(rectangle.Location.X + state.CurrentParentContainer.X, rectangle.Location.Y + state.CurrentParentContainer.Y);
            float handleSize = rectangle.Width - 6;
            float handleRelativePos = (value - minValue) / (maxValue - minValue);
            RectangleF handleRect = new RectangleF(rectangle.Location.X + 3, handleRelativePos * (rectangle.Height - handleSize - 6) + 3 + rectangle.Location.Y, handleSize, handleSize);

            bool hovered = false;
            bool down = false;

            if (state.IsMouseEnabled)
            {
                if (state.IsDragging(id))
                {
                    if (state.CurrentMouseState.IsLeftButtonUp)
                    {
                        state.Drop();
                    }
                    else
                    {
                        down = true;
                        hovered = true;

                        float handlePos = handleRect.Y;

                        float offset = state.DragOffset().Y;

                        if (offset != 0)
                        {
                            value = MathHelper.Clamp(value + (offset * (maxValue - minValue) / rectangle.Height), minValue, maxValue);
                        }
                    }
                }
                else if (!state.IsDraggingAny() && handleRect.Contains(state.MousePosition.X, state.MousePosition.Y) && state.CurrentMouseState.IsLeftButtonDown)
                {
                    down = true;
                    hovered = true;
                    state.Drag(id);
                }
            }

            state.Graphics.DrawRectangle(rectangle, Color.FromArgb(230, 10, 10, 10));
            state.Graphics.DrawRectangle(handleRect, hovered ? down ? Color.FromArgb(240, 95, 95, 95) : Color.FromArgb(240, 70, 70, 70) : Color.FromArgb(240, 55, 55, 55));

            DrawTextDebug(rectangle.Location, $"VSlider {id.ToString("X8")}", 18.0f);

            return value;
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

            state.Graphics.DrawText(text, "Consolas", fontSize, new PointF(x, y), Color.White, rectangle);
        }

        [Conditional("DEBUG")]
        private static void DrawTextDebug(PointF position, string text, float fontSize = 15.0f)
        {
            state.Graphics.DrawText(text, "Consolas", fontSize, position, Color.Red);
        }
    }
}

