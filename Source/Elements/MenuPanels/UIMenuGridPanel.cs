namespace RAGENativeUI.Elements
{
    using System.Drawing;

    using Rage;

    public class UIMenuGridPanel : UIMenuPanel
    {
        private const float Height = 0.034722f * 7 + (0.034722f * 0.25f);

        private Vector2 value = new Vector2(0.5f, 0.5f);

        public Color GridColor { get; set; } = Color.FromArgb(205, 105, 105, 102);
        public Color DotColor { get; set; } = Color.FromArgb(255, 255, 255, 255);

        public Vector2 Value
        {
            get => value;
            set => this.value = new Vector2(MathHelper.Clamp(value.X, 0.0f, 1.0f),
                                            MathHelper.Clamp(value.Y, 0.0f, 1.0f));
        }

        public override void Draw(float x, ref float y, float menuWidth)
        {
            DrawBackground(x, y, Height, menuWidth);

            DrawGrid(x, y, menuWidth);

            y += Height;
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
            float gridWidth = menuWidth * 0.45f;
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



            // tmp controls
            var controlX = N.GetControlNormal(2, GameControl.ScriptRightAxisX);
            var controlY = N.GetControlNormal(2, GameControl.ScriptRightAxisY);
            var frameTime = Game.FrameTime;
            var newValue = Value;
            newValue.X += controlX * frameTime;
            newValue.Y += controlY * frameTime;
            Value = newValue;
        }
    }
}

