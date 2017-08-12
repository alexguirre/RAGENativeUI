namespace RAGENativeUI.Elements
{
    using System;
    using System.Drawing;

    using Rage;
    using Rage.Native;

    public abstract class TimerBarBase
    {
        public bool IsVisible { get; set; }

        protected Sprite Background { get; set; }

        public TimerBarBase()
        {
            Background = new Sprite("timerbars", "all_black_bg", GameScreenRectangle.FromRelativeCoords(-1.0f, -1.0f, W, H), Color.FromArgb(140, 255, 255, 255)) { IsVisible = true };
        }


        protected const float W = 0.157f, H = 0.036f;
        public virtual void Draw(ref float y, float x) // y and x are where the bottom-right corner should be located, y should be set to the next timerbar bottom-right corner
        {
            if (!IsVisible)
                return;
            
            Background.Rectangle = GameScreenRectangle.FromRelativeCoords(x - W * 0.5f, y - H * 0.5f, W, H);
            Background.Draw();

            NativeFunction.Natives.HideHudComponentThisFrame(6); // VehicleName
            NativeFunction.Natives.HideHudComponentThisFrame(7); // AreaName
            NativeFunction.Natives.HideHudComponentThisFrame(9); // StreetName

            y -= H + 0.003f;
        }
    }

    public class LabeledTimerBar : TimerBarBase
    {
        /// <exception cref="ArgumentNullException">When setting the property to a null value.</exception>
        public string Label { get { return LabelElement.Caption; } set { LabelElement.Caption = value ?? throw new ArgumentNullException($"The timer bar {nameof(Label)} can't be null."); } }

        protected Text LabelElement { get; set; }

        public LabeledTimerBar(string label)
        {
            LabelElement = new Text(label, GameScreenPosition.FromRelativeCoords(-1.0f, -1.0f), 0.288f, Color.White) { IsVisible = true, Alignment = TextAlignment.Right };
        }
        
        public override void Draw(ref float y, float x)
        {
            if (!IsVisible)
                return;

            float labelX = (x - W * 0.5f) - 0.01f;
            float labelY = (y - H * 0.5f) - 0.01f;
            base.Draw(ref y, x);

            LabelElement.Position = GameScreenPosition.FromRelativeCoords(labelX, labelY);
            LabelElement.Draw();
        }
    }

    public class TextTimerBar : LabeledTimerBar
    {
        /// <exception cref="ArgumentNullException">When setting the property to a null value.</exception>
        public string Text { get { return TextElement.Caption; } set { TextElement.Caption = value ?? throw new ArgumentNullException($"The timer bar {nameof(Text)} can't be null."); } }

        protected Text TextElement { get; set; }

        public TextTimerBar(string label, string text) : base(label)
        {
            TextElement = new Text(text, GameScreenPosition.FromRelativeCoords(-1.0f, -1.0f), 0.5f, Color.White) { IsVisible = true, Alignment = TextAlignment.Right };
        }
        
        public override void Draw(ref float y, float x)
        {
            if (!IsVisible)
                return;

            float textX = (x - W * 0.5f) + 0.0755f;
            float textY = (y - H * 0.5f) - H / 2f;
            base.Draw(ref y, x);

            TextElement.Position = GameScreenPosition.FromRelativeCoords(textX, textY);
            TextElement.Draw();
        }
    }

    public class ProgressTimerBar : LabeledTimerBar
    {
        private float percentage;
        public float Percentage { get { return percentage; } set { percentage = MathHelper.Clamp(value, 0.0f, 1.0f); } }

        public Color BackColor { get { return BackRectangleElement.Color; } set { BackRectangleElement.Color = value; } }
        public Color ForeColor { get { return ForeRectangleElement.Color; } set { ForeRectangleElement.Color = value; } }

        protected Box BackRectangleElement { get; set; }
        protected Box ForeRectangleElement { get; set; }

        public ProgressTimerBar(string label, Color backColor, Color foreColor) : base(label)
        {
            BackRectangleElement = new Box(GameScreenRectangle.FromRelativeCoords(-1.0f, -1.0f, BarW, BarH), backColor) { IsVisible = true };
            ForeRectangleElement = new Box(GameScreenRectangle.FromRelativeCoords(-1.0f, -1.0f, BarW, BarH), foreColor) { IsVisible = true };
        }

        public ProgressTimerBar(string label) : this(label, Color.DarkRed, Color.Red)
        {
        }

        protected const float BarW = W / 2.25f, BarH = H / 3f; // TODO: set bar size the same as the game real timer bars
        public override void Draw(ref float y, float x)
        {
            if (!IsVisible)
                return;

            float barX = (x - W * 0.5f) + 0.04f;
            float barY = (y - H * 0.5f);
            base.Draw(ref y, x);

            BackRectangleElement.Rectangle = GameScreenRectangle.FromRelativeCoords(barX, barY, BarW, BarH);
            BackRectangleElement.Draw();
            float x2 = barX - BarW / 2f + BarW / 2f * percentage;
            float w = BarW * percentage;
            ForeRectangleElement.Rectangle = GameScreenRectangle.FromRelativeCoords(x2, barY, w, BarH);
            ForeRectangleElement.Draw();
        }
    }
}

