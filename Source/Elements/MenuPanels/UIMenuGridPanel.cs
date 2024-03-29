namespace RAGENativeUI.Elements
{
    using System.Collections.Generic;
    using System.Drawing;

    using Rage;

    public enum UIMenuGridPanelStyle
    {
        /// <summary>
        /// A 5x5 grid.
        /// </summary>
        Full,

        /// <summary>
        /// A single row with 5 cells. With this style, only the X component of <see cref="UIMenuGridPanel.Value"/> can be modified, the Y component is fixed to <c>0.5f</c>.
        /// </summary>
        SingleRow,

        /// <summary>
        /// A single column with 5 cells. With this style, only the Y component of <see cref="UIMenuGridPanel.Value"/> can be modified, the X component is fixed to <c>0.5f</c>.
        /// </summary>
        SingleColumn,
    }

    public class UIMenuGridPanel : UIMenuPanel
    {
        public delegate void ValueChangedEvent(UIMenuGridPanel sender, Vector2 oldValue, Vector2 newValue);

        private static readonly TextStyle BaseLabelStyle = TextStyle.Default.With(font: TextFont.ChaletLondon, scale: 0.35f);

        private Vector2 value = new Vector2(0.5f, 0.5f);
        private RectangleF gridBounds;
        private bool mousePressed;

        public override float Height => 0.034722f * 7 + (0.034722f * 0.25f);
        public Color GridColor { get; set; } = Color.FromArgb(205, 105, 105, 102);
        public Color DotColor { get; set; } = Color.FromArgb(255, 255, 255, 255);
        public UIMenuGridPanelStyle Style { get; set; }
        public string TopLabel { get; set; } = "Top (-Y)";
        public TextStyle TopLabelStyle { get; set; } = BaseLabelStyle.With(justification: TextJustification.Center);
        public string BottomLabel { get; set; } = "Bottom (+Y)";
        public TextStyle BottomLabelStyle { get; set; } = BaseLabelStyle.With(justification: TextJustification.Center);
        public string LeftLabel { get; set; } = "Left (-X)";
        public TextStyle LeftLabelStyle { get; set; } = BaseLabelStyle.With(justification: TextJustification.Right);
        public string RightLabel { get; set; } = "Right (+X)";
        public TextStyle RightLabelStyle { get; set; } = BaseLabelStyle.With(justification: TextJustification.Left);

        public Vector2 Value
        {
            get => value;
            set
            {
                value = new Vector2(Style == UIMenuGridPanelStyle.SingleColumn ? 0.5f : MathHelper.Clamp(value.X, 0.0f, 1.0f),
                                    Style == UIMenuGridPanelStyle.SingleRow ? 0.5f : MathHelper.Clamp(value.Y, 0.0f, 1.0f));
                if (this.value != value)
                {
                    var oldValue = this.value;
                    this.value = value;
                    OnValueChanged(oldValue, value);
                }
            }
        }

        public event ValueChangedEvent ValueChanged;

        public UIMenuGridPanel()
        {
            InstructionalButtons.Add(new InstructionalButtonDynamic("Fine Tune", InstructionalKey.Mouse, InstructionalKey.ControllerRStick));
        }

        public override bool ProcessControl()
        {
            if (UIMenu.IsUsingController)
            {
                N.SetInputExclusive(2, GameControl.ScriptRightAxisX);
                N.SetInputExclusive(2, GameControl.ScriptRightAxisY);
                var controlX = N.GetControlNormal(2, GameControl.ScriptRightAxisX);
                var controlY = N.GetControlNormal(2, GameControl.ScriptRightAxisY);
                var frameTime = Game.FrameTime;
                var newValue = Value;
                newValue.X += controlX * frameTime;
                newValue.Y += controlY * frameTime;
                Value = newValue;
                return controlX != 0.0f || controlY != 0.0f;
            }

            return false;
        }

        public override bool ProcessMouse(float mouseX, float mouseY)
        {
            const int CursorFinger = 5;

            if (!mousePressed)
            {
                bool inBounds = gridBounds.Contains(mouseX, mouseY);
                if (inBounds)
                {
                    N.SetMouseCursorSprite(CursorFinger);
                    if (Game.IsControlJustPressed(2, GameControl.CursorAccept))
                    {
                        mousePressed = true;
                        return true;
                    }
                }
            }
            else
            {
                N.SetMouseCursorSprite(CursorFinger);
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

            return base.ProcessMouse(mouseX, mouseY);
        }

        protected override void DrawContents(float x, float y, float menuWidth)
        {
            const string Txd = "pause_menu_pages_char_mom_dad";
            const string Tex = "nose_grid";
            const string DotTxd = "commonmenu";
            const string DotTex = "common_medal";

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
            switch (Style)
            {
                case UIMenuGridPanelStyle.SingleRow:
                    N.DrawSpriteUV(
                        Txd, Tex,
                        gridX, gridY, gridWidth, gridHeight / 5.0f,
                        0.0f, 0.4f, 1.0f, 0.6f,
                        0.0f,
                        color.R, color.G, color.B, color.A);
                    break;
                case UIMenuGridPanelStyle.SingleColumn:
                    N.DrawSpriteUV(
                        Txd, Tex,
                        gridX, gridY, gridWidth / 5.0f, gridHeight,
                        0.4f, 0.0f, 0.6f, 1.0f,
                        0.0f,
                        color.R, color.G, color.B, color.A);
                    break;
                default:
                    N.DrawSprite(
                        Txd, Tex,
                        gridX, gridY, gridWidth, gridHeight,
                        0.0f,
                        color.R, color.G, color.B, color.A);
                    break;
            }

            color = DotColor;
            float dotX = gridX - (gridWidth * 0.5f) + (gridWidth * Value.X);
            float dotY = gridY - (gridHeight * 0.5f) + (gridHeight * Value.Y);
            float dotWidth = 0.0065f * 3;
            float dotHeight = dotWidth * aspectRatio;
            N.DrawSprite(
                DotTxd, DotTex,
                dotX, dotY, dotWidth, dotHeight, 0.0f,
                color.R, color.G, color.B, color.A);

            // calculate grid bounds
            gridBounds = Common.GetScriptGfxRect(new RectangleF(gridX - gridWidth * 0.5f, gridY - gridHeight * 0.5f, gridWidth, gridHeight));

            // draw labels
            {
                const float Padding = 0.0046875f;

                var centerX = x + menuWidth * 0.5f;
                var centerY = y + Height * 0.5f;

                if (!string.IsNullOrEmpty(TopLabel))
                {
                    var topStyle = TopLabelStyle.WithWrap(x, x + menuWidth);
                    TextCommands.Display(TopLabel, topStyle, centerX, y + Padding);
                }

                if (!string.IsNullOrEmpty(BottomLabel))
                {
                    var gridBottom = gridY + gridHeight * 0.5f + Padding;
                    var bottomStyle = BottomLabelStyle.WithWrap(x, x + menuWidth);
                    TextCommands.Display(BottomLabel, bottomStyle, centerX, gridBottom);
                }

                if (!string.IsNullOrEmpty(LeftLabel))
                {
                    var gridLeft = gridX - gridWidth * 0.5f - Padding;
                    var leftCharHeight = LeftLabelStyle.CharacterHeight;
                    var leftStyle = LeftLabelStyle.WithWrap(x, gridLeft);
                    TextCommands.Display(LeftLabel, leftStyle, x, centerY - leftCharHeight * 0.6f);
                }

                if (!string.IsNullOrEmpty(RightLabel))
                {
                    var gridRight = gridX + gridWidth * 0.5f + Padding;
                    var rightCharHeight = RightLabelStyle.CharacterHeight;
                    var rightStyle = RightLabelStyle.WithWrap(gridRight, x + menuWidth);
                    TextCommands.Display(RightLabel, rightStyle, gridRight, centerY - rightCharHeight * 0.6f);
                }
            }
        }

        protected virtual void OnValueChanged(Vector2 oldValue, Vector2 newValue)
            => ValueChanged?.Invoke(this, oldValue, newValue);
    }
}
