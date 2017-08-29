namespace RAGENativeUI.ImGui
{
    using System;
    using System.Drawing;
    using System.Diagnostics;
    using System.Windows.Forms;
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
                state.Graphics = e.Graphics;

                Delegate[] delegates = Do.GetInvocationList();
                for (int i = 0; i < delegates.Length; i++)
                {
                    state.IsMouseEnabled = false;
                    state.ScreenContainer = new Container(null, new RectangleF(0f, 0f, Game.Resolution.Width, Game.Resolution.Height));
                    state.CurrentContainer = state.ScreenContainer;

                    ((GuiEventHandler)delegates[i]).Invoke();
                }

                if (state.HasMouseBeenCalled)
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
        
        public static RectangleF BeginWindow(RectangleF position, string title)
        {
            EnsureCall();
            uint id = state.Id(true, false);

            const float TitleBarHeight = 30f;


            Container container = state.CurrentContainer;
            Vector2 drawPos = container.ConvertToRootCoords(new Vector2(position.Location.X, position.Location.Y));

            state.PushContainer(new RectangleF(position.X, position.Y + TitleBarHeight, position.Width, position.Height - TitleBarHeight));

            RectangleF titleBarRect = new RectangleF(drawPos.X, drawPos.Y, position.Width, TitleBarHeight);
            RectangleF windRect = new RectangleF(drawPos.X, drawPos.Y + TitleBarHeight, position.Width, position.Height - TitleBarHeight);

            state.Graphics.DrawRectangle(titleBarRect, Color.FromArgb(215, 15, 15, 15));
            state.Graphics.DrawRectangle(windRect, Color.FromArgb(150, 45, 45, 45));

            DrawText(titleBarRect, title, 20f);

            DrawTextDebug(titleBarRect.Location, $"Window {id.ToString("X8")}", 18.0f);

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
                        newX = MathHelper.Clamp(newX, 0f, container.DrawArea.Width - position.Width);
                        newY = MathHelper.Clamp(newY, 0f, container.DrawArea.Height - position.Height);
                        position.Location = new PointF(newX, newY);
                    }
                }
                else if(!state.IsDraggingAny() && titleBarRect.Contains(state.MousePosition.X, state.MousePosition.Y) && Game.IsKeyDown(Keys.LButton))
                {
                    state.Drag(id);
                }
            }

            return position;
        }

        public static void EndWindow()
        {
            state.PopContainer();
        }

        public static bool Button(RectangleF position, string text)
        {
            EnsureCall();
            uint id = state.Id();

            RectangleF drawPos = state.CurrentContainer.ConvertToRootCoords(position);

            bool hovered = false;
            bool down = false;

            if (state.IsMouseEnabled)
            {
                hovered = drawPos.Contains(state.MousePosition.X, state.MousePosition.Y);
                if(hovered)
                {
                    down = Game.IsKeyDown(Keys.LButton);
                }
            }

            state.Graphics.DrawRectangle(drawPos, hovered ? down ? Color.FromArgb(200, 10, 10, 10) : Color.FromArgb(175, 20, 20, 20) : Color.FromArgb(160, 25, 25, 25));

            state.Graphics.DrawLine(new Vector2(drawPos.X, drawPos.Y), new Vector2(drawPos.X + drawPos.Width, drawPos.Y), Color.Black);
            state.Graphics.DrawLine(new Vector2(drawPos.X, drawPos.Y), new Vector2(drawPos.X , drawPos.Y + drawPos.Height), Color.Black);
            state.Graphics.DrawLine(new Vector2(drawPos.X + drawPos.Width, drawPos.Y), new Vector2(drawPos.X + drawPos.Width, drawPos.Y + drawPos.Height), Color.Black);
            state.Graphics.DrawLine(new Vector2(drawPos.X, drawPos.Y + drawPos.Height), new Vector2(drawPos.X + drawPos.Width, drawPos.Y + drawPos.Height), Color.Black);

            DrawText(drawPos, text);

            DrawTextDebug(drawPos.Location, $"Button {id.ToString("X8")}", 18.0f);

            return down;
        }

        public static bool Toggle(RectangleF position, string text, bool value)
        {
            EnsureCall();
            uint id = state.Id();

            RectangleF drawPos = state.CurrentContainer.ConvertToRootCoords(position);

            bool hovered = false;
            bool down = false;

            if (state.IsMouseEnabled)
            {
                hovered = drawPos.Contains(state.MousePosition.X, state.MousePosition.Y);
                if (hovered)
                {
                    if (Game.IsKeyDown(Keys.LButton))
                    {
                        down = true;
                        value = !value;
                    }
                }
            }

            RectangleF boxRect = new RectangleF(drawPos.X, drawPos.Y, drawPos.Height, drawPos.Height);
            state.Graphics.DrawRectangle(boxRect, Color.FromArgb(230, 10, 10, 10));
            if (value)
            {
                state.Graphics.DrawRectangle(new RectangleF(boxRect.X + 3f, boxRect.Y + 3f, boxRect.Width - 6f, boxRect.Height - 6f), hovered ? down ? Color.FromArgb(240, 95, 95, 95) : Color.FromArgb(240, 70, 70, 70) : Color.FromArgb(240, 55, 55, 55));
            }

            state.Graphics.DrawLine(new Vector2(boxRect.X, boxRect.Y), new Vector2(boxRect.X + boxRect.Width, boxRect.Y), Color.Black);
            state.Graphics.DrawLine(new Vector2(boxRect.X, boxRect.Y), new Vector2(boxRect.X, boxRect.Y + boxRect.Height), Color.Black);
            state.Graphics.DrawLine(new Vector2(boxRect.X + boxRect.Width, boxRect.Y), new Vector2(boxRect.X + boxRect.Width, boxRect.Y + boxRect.Height), Color.Black);
            state.Graphics.DrawLine(new Vector2(boxRect.X, boxRect.Y + boxRect.Height), new Vector2(boxRect.X + boxRect.Width, boxRect.Y + boxRect.Height), Color.Black);

            DrawText(drawPos, text, 15.0f, TextHorizontalAligment.Right, TextVerticalAligment.Center);

            DrawTextDebug(drawPos.Location, $"Toggle {id.ToString("X8")}", 18.0f);

            return value;
        }

        public static void Label(RectangleF rectangle, string text, float fontSize = 15.0f, TextHorizontalAligment hAlign = TextHorizontalAligment.Left, TextVerticalAligment vAlign = TextVerticalAligment.Center)
        {
            EnsureCall();
            uint id = state.Id();

            RectangleF drawPos = state.CurrentContainer.ConvertToRootCoords(rectangle);
            RectangleF clip = state.CurrentContainer.ConvertToRootCoords(state.CurrentContainer.ClipLocalRectangle(rectangle));

            DrawText(drawPos, clip, text, fontSize, hAlign, vAlign);

            DrawTextDebug(drawPos.Location, $"Label {id.ToString("X8")}", 18.0f);
            DrawRectangleDebug(drawPos);
        }

        public static float HorizontalSlider(RectangleF rectangle, float value, float minValue, float maxValue)
        {
            EnsureCall();
            uint id = state.Id();

            RectangleF drawPos = state.CurrentContainer.ConvertToRootCoords(rectangle);
            float handleSize = drawPos.Height - 6;
            float handleRelativePos = (value - minValue) / (maxValue - minValue);
            RectangleF handleRect = new RectangleF(handleRelativePos * (drawPos.Width - handleSize - 6) + 3 + drawPos.Location.X, drawPos.Location.Y + 3, handleSize, handleSize);

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
                            value = MathHelper.Clamp(value + (offset * (maxValue - minValue) / drawPos.Width), minValue, maxValue);
                        }
                    }
                }
                else if (!state.IsDraggingAny() && handleRect.Contains(state.MousePosition.X, state.MousePosition.Y))
                {
                    hovered = true;
                    if (Game.IsKeyDown(Keys.LButton))
                    {
                        down = true;
                        state.Drag(id);
                    }
                }
            }

            state.Graphics.DrawRectangle(drawPos, Color.FromArgb(230, 10, 10, 10));
            state.Graphics.DrawRectangle(handleRect, hovered ? down ? Color.FromArgb(240, 95, 95, 95) : Color.FromArgb(240, 70, 70, 70) : Color.FromArgb(240, 55, 55, 55));

            DrawTextDebug(drawPos.Location, $"HSlider {id.ToString("X8")}", 18.0f);

            return value;
        }

        public static float VerticalSlider(RectangleF rectangle, float value, float minValue, float maxValue)
        {
            EnsureCall();
            uint id = state.Id();

            RectangleF drawPos = state.CurrentContainer.ConvertToRootCoords(rectangle);
            float handleSize = drawPos.Width - 6;
            float handleRelativePos = (value - minValue) / (maxValue - minValue);
            RectangleF handleRect = new RectangleF(drawPos.Location.X + 3, handleRelativePos * (drawPos.Height - handleSize - 6) + 3 + drawPos.Location.Y, handleSize, handleSize);

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
                            value = MathHelper.Clamp(value + (offset * (maxValue - minValue) / drawPos.Height), minValue, maxValue);
                        }
                    }
                }
                else if (!state.IsDraggingAny() && handleRect.Contains(state.MousePosition.X, state.MousePosition.Y))
                {
                    hovered = true;
                    if (Game.IsKeyDown(Keys.LButton))
                    {
                        down = true;
                        state.Drag(id);
                    }
                }
            }

            state.Graphics.DrawRectangle(drawPos, Color.FromArgb(230, 10, 10, 10));
            state.Graphics.DrawRectangle(handleRect, hovered ? down ? Color.FromArgb(240, 95, 95, 95) : Color.FromArgb(240, 70, 70, 70) : Color.FromArgb(240, 55, 55, 55));

            DrawTextDebug(drawPos.Location, $"VSlider {id.ToString("X8")}", 18.0f);

            return value;
        }

        public static int HorizontalSlider(RectangleF rectangle, int value, int minValue, int maxValue)
        {
            return (int)HorizontalSlider(rectangle, (float)value, (float)minValue, (float)maxValue);
        }

        public static int VerticalSlider(RectangleF rectangle, int value, int minValue, int maxValue)
        {
            return (int)VerticalSlider(rectangle, (float)value, (float)minValue, (float)maxValue);
        }

        // TODO: implement clipping on other controls, currently only implemented on the Label, mainly to allow them to be used in the ScrollView
        public static Vector2 BeginScrollView(RectangleF position, Vector2 scrollPosition, SizeF viewSize, bool horizontalScrollbar = true, bool verticalScrollbar = true)
        {
            const float ScrollbarsSize = 17.0f;

            float x = horizontalScrollbar ? HorizontalSlider(new RectangleF(position.X, position.Bottom - ScrollbarsSize, position.Width - ScrollbarsSize, ScrollbarsSize), scrollPosition.X, 0f, viewSize.Width) : scrollPosition.X;
            float y = verticalScrollbar ? VerticalSlider(new RectangleF(position.Right - ScrollbarsSize, position.Y, ScrollbarsSize, position.Height - ScrollbarsSize), scrollPosition.Y, 0f, viewSize.Height) : scrollPosition.Y;

            state.Graphics.DrawRectangle(state.CurrentContainer.ConvertToRootCoords(new RectangleF(position.X, position.Y, position.Width - (horizontalScrollbar ? ScrollbarsSize : 0.0f), position.Height - (verticalScrollbar ? ScrollbarsSize : 0.0f))), Color.FromArgb(180, 25, 25, 25));
            
            state.PushContainer(new RectangleF(position.X, position.Y, position.Width - (horizontalScrollbar ? ScrollbarsSize : 0.0f), position.Height - (verticalScrollbar ? ScrollbarsSize : 0.0f)), new PointF(-scrollPosition.X, -scrollPosition.Y));

            return new Vector2(x, y);
        }

        public static void EndScrollView()
        {
            state.PopContainer();
        }


        private static void DrawText(RectangleF rectangle, string text, float fontSize = 15.0f, TextHorizontalAligment hAlign = TextHorizontalAligment.Center, TextVerticalAligment vAlign = TextVerticalAligment.Center)
        {
            DrawText(rectangle, rectangle, text, fontSize, hAlign, vAlign);
        }

        private static void DrawText(RectangleF rectangle, RectangleF clipRectangle, string text, float fontSize = 15.0f, TextHorizontalAligment hAlign = TextHorizontalAligment.Center, TextVerticalAligment vAlign = TextVerticalAligment.Center)
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

            state.Graphics.DrawText(text, "Consolas", fontSize, new PointF(x, y), Color.White, clipRectangle);
        }

        [Conditional("DEBUG")]
        private static void DrawTextDebug(PointF position, string text, float fontSize = 15.0f)
        {
            if (Game.IsShiftKeyDownRightNow)
            {
                state.Graphics.DrawText(text, "Consolas", fontSize, position, Color.Red);
            }
        }

        [Conditional("DEBUG")]
        private static void DrawRectangleDebug(RectangleF position)
        {
            if (Game.IsShiftKeyDownRightNow)
            {
                state.Graphics.DrawRectangle(position, Color.FromArgb(50, 255, 0, 0));
            }
        }
    }
}

