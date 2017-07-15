namespace RAGENativeUI.Elements
{
    using System;
    using System.Drawing;

    using Rage;
    using Rage.Native;
    
    using RAGENativeUI.Utility;

    public abstract class TimerBarBase
    {
        public bool IsVisible { get; set; }

        protected Sprite Background { get; set; }

        public TimerBarBase()
        {
            Background = new Sprite("timerbars", "all_black_bg", GameScreenRectangle.FromRelativeCoords(X, -1.0f, W, H), Color.FromArgb(140, 255, 255, 255)) { IsVisible = true };
        }


        protected const float X = 0.9068f, W = 0.157f, H = 0.036f;
        public virtual void Draw(ref float y)
        {
            if (!IsVisible)
                return;
            
            Background.Rectangle = GameScreenRectangle.FromRelativeCoords(X, y, W, H);
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
            LabelElement = new Text(label, GameScreenPosition.FromRelativeCoords(LabelX, -1.0f), 0.288f, Color.White) { IsVisible = true, Alignment = TextAlignment.Right };
        }

        protected const float LabelX = X - 0.0069f;
        public override void Draw(ref float y)
        {
            if (!IsVisible)
                return;

            float labelY = y - 0.01f;
            base.Draw(ref y);

            LabelElement.Position = GameScreenPosition.FromRelativeCoords(LabelX, labelY);
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
            TextElement = new Text(text, GameScreenPosition.FromRelativeCoords(TextX, -1.0f), 0.5f, Color.White) { IsVisible = true, Alignment = TextAlignment.Right };
        }

        protected const float TextX = X + 0.0755f;
        public override void Draw(ref float y)
        {
            if (!IsVisible)
                return;

            float textY = y - H / 2f;
            base.Draw(ref y);

            TextElement.Position = GameScreenPosition.FromRelativeCoords(TextX, textY);
            TextElement.Draw();
        }
    }

    public class ProgressTimerBar : LabeledTimerBar
    {
        private float percentage;
        public float Percentage { get { return percentage; } set { percentage = MathHelper.Clamp(value, 0.0f, 1.0f); } }

        public Color BackColor { get { return BackRectangleElement.Color; } set { BackRectangleElement.Color = value; } }
        public Color ForeColor { get { return ForeRectangleElement.Color; } set { ForeRectangleElement.Color = value; } }

        protected Rectangle BackRectangleElement { get; set; }
        protected Rectangle ForeRectangleElement { get; set; }

        public ProgressTimerBar(string label, Color backColor, Color foreColor) : base(label)
        {
            BackRectangleElement = new Rectangle(GameScreenRectangle.FromRelativeCoords(BarX, -1.0f, BarW, BarH), backColor) { IsVisible = true };
            ForeRectangleElement = new Rectangle(GameScreenRectangle.FromRelativeCoords(BarX, -1.0f, BarW, BarH), foreColor) { IsVisible = true };
        }

        public ProgressTimerBar(string label) : this(label, Color.DarkRed, Color.Red)
        {
        }

        protected const float BarX = X + 0.04f, BarW = W / 2.25f, BarH = H / 3f; // TODO: set bar size the same as the game real timer bars
        public override void Draw(ref float y)
        {
            if (!IsVisible)
                return;

            float rectY = y;
            base.Draw(ref y);

            BackRectangleElement.ScreenRectangle = GameScreenRectangle.FromRelativeCoords(BarX, rectY, BarW, BarH);
            BackRectangleElement.Draw();
            float x = BarX - BarW / 2f + BarW / 2f * percentage;
            float w = BarW * percentage;
            ForeRectangleElement.ScreenRectangle = GameScreenRectangle.FromRelativeCoords(x, rectY, w, BarH);
            ForeRectangleElement.Draw();
        }
    }
}

