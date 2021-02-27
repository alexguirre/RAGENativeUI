namespace RAGENativeUI.Elements
{
    using System.Drawing;

    using Rage;

    public class UIMenuGridPanel : UIMenuPanel
    {
        public delegate void ValueChangedEvent(UIMenuGridPanel sender, Vector2 oldValue, Vector2 newValue);

        private const float Height = 0.034722f * 7 + (0.034722f * 0.25f);

        private Vector2 value = new Vector2(0.5f, 0.5f);
        private RectangleF gridBounds;
        private bool mousePressed;

        public Color GridColor { get; set; } = Color.FromArgb(205, 105, 105, 102);
        public Color DotColor { get; set; } = Color.FromArgb(255, 255, 255, 255);

        public Vector2 Value
        {
            get => value;
            set
            {
                value = new Vector2(MathHelper.Clamp(value.X, 0.0f, 1.0f),
                                    MathHelper.Clamp(value.Y, 0.0f, 1.0f));
                if (this.value != value)
                {
                    var oldValue = this.value;
                    this.value = value;
                    OnValueChanged(oldValue, value);
                }
            }
        }

        public event ValueChangedEvent ValueChanged;

        public override void Draw(float x, ref float y, float menuWidth)
        {
            DrawBackground(x, y, Height, menuWidth);

            DrawGrid(x, y, menuWidth);

            y += Height;
        }

        public override bool ProcessControl()
        {
            // tmp controls
            var controlX = N.GetControlNormal(2, GameControl.MoveLeftRight);
            var controlY = N.GetControlNormal(2, GameControl.MoveUpDown);
            var frameTime = Game.FrameTime;
            var newValue = Value;
            newValue.X += controlX * frameTime;
            newValue.Y += controlY * frameTime;
            Value = newValue;
            return controlX != 0.0f || controlY != 0.0f;
        }

        public override bool ProcessMouse(float mouseX, float mouseY)
        {
            N.DrawRect(gridBounds.X + gridBounds.Width * 0.5f, gridBounds.Y + gridBounds.Height * 0.5f,
                       gridBounds.Width, gridBounds.Height,
                       255, 0, 0, gridBounds.Contains(mouseX, mouseY) ? 220 : 100);
            N.DrawRect(mouseX, mouseY, 0.005f, 0.005f * N.GetAspectRatio(false), 0, 255, 0, 200);

            if (!mousePressed)
            {
                bool inBounds = gridBounds.Contains(mouseX, mouseY);
                if (inBounds && Game.IsControlPressed(2, GameControl.CursorAccept))
                {
                    mousePressed = true;
                    return true;
                }
            }
            else
            {
                if (Game.IsControlJustReleased(2, GameControl.CursorAccept))
                {
                    mousePressed = false;
                }
                else
                {
                    Value = new Vector2((mouseX - gridBounds.X) / gridBounds.Width,
                                        (mouseY - gridBounds.Y) / gridBounds.Height);
                }

                return true;
            }

            return false;
        }

        private void DrawGrid(float x, float y, float menuWidth)
        {
            const string Txd = "pause_menu_pages_char_mom_dad";
            const string Tex = "nose_grid";

            N.RequestStreamedTextureDict(Txd);
            if (!N.HasStreamedTextureDictLoaded(Txd))
            {
                return;
            }

            var aspectRatio = N.GetAspectRatio(false);
            var color = GridColor;
            float gridX = x + menuWidth * 0.5f;
            float gridY = y + Height * 0.5f;
            float gridWidth = Height * 0.4f;
            float gridHeight = gridWidth * aspectRatio;
            N.DrawSprite(
                Txd, Tex,
                gridX, gridY, gridWidth, gridHeight,
                0.0f,
                color.R, color.G, color.B, color.A);

            color = DotColor;
            float dotX = gridX - (gridWidth * 0.5f) + (gridWidth * Value.X);
            float dotY = gridY - (gridHeight * 0.5f) + (gridHeight * Value.Y);
            float dotWidth = 0.0065f;
            float dotHeight = dotWidth * aspectRatio;
            N.DrawRect(
                dotX, dotY, dotWidth, dotHeight,
                color.R, color.G, color.B, color.A);

            // calculate grid bounds
            {
                float x1 = gridX - gridWidth * 0.5f, y1 = gridY - gridHeight * 0.5f;
                float x2 = gridX + gridWidth * 0.5f, y2 = gridY + gridHeight * 0.5f;
                N.GetScriptGfxPosition(x1, y1, out x1, out y1);
                N.GetScriptGfxPosition(x2, y2, out x2, out y2);

                gridBounds = new RectangleF(x1, y1, x2 - x1, y2 - y1);
            }
        }

        protected virtual void OnValueChanged(Vector2 oldValue, Vector2 newValue)
            => ValueChanged?.Invoke(this, oldValue, newValue);
    }
}

