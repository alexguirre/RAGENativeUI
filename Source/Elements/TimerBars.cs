namespace RAGENativeUI.Elements
{
    using System;
    using System.Drawing;

    using Rage;
    using Rage.Native;

    public abstract class TimerBarBase
    {
        public static readonly float DefaultWidth = 0.157f;
        public static readonly float DefaultHeight = 0.036f;

        public bool IsVisible { get; set; } = true;

        protected Sprite Background { get; set; }

        // in relative coords
        public float Width { get; set; } = DefaultWidth;
        public float Height { get; set; } = DefaultHeight;

        public TimerBarBase()
        {
            Background = new Sprite("timerbars", "all_black_bg", ScreenRectangle.FromRelativeCoords(-1.0f, -1.0f, Width, Height), Color.FromArgb(140, 255, 255, 255)) { IsVisible = true };
        }


        public virtual void Draw(ref float y, float x) // y and x are where the bottom-right corner should be located, y should be set to the next timerbar bottom-right corner
        {
            if (!IsVisible)
                return;
            
            Background.Rectangle = ScreenRectangle.FromRelativeCoords(x - Width * 0.5f, y - Height * 0.5f, Width, Height);
            Background.Draw();

            NativeFunction.Natives.HideHudComponentThisFrame(6); // VehicleName
            NativeFunction.Natives.HideHudComponentThisFrame(7); // AreaName
            NativeFunction.Natives.HideHudComponentThisFrame(9); // StreetName

            y -= Height + 0.003f;
        }
    }

    public class LabeledTimerBar : TimerBarBase
    {
        /// <exception cref="ArgumentNullException">When setting the property to a null value.</exception>
        public string Label { get { return LabelElement.Caption; } set { Throw.IfNull(value, nameof(value)); LabelElement.Caption = value; } }

        protected Text LabelElement { get; set; }

        public LabeledTimerBar(string label)
        {
            Throw.IfNull(label, nameof(label));

            LabelElement = new Text(label, ScreenPosition.FromRelativeCoords(-1.0f, -1.0f), 0.288f, Color.White) { IsVisible = true, Alignment = TextAlignment.Right };
        }
        
        public override void Draw(ref float y, float x)
        {
            if (!IsVisible)
                return;

            float labelX = (x - Width * 0.5f) - 0.01f;
            float labelY = (y - Height * 0.5f) - 0.01f;
            base.Draw(ref y, x);

            LabelElement.Position = ScreenPosition.FromRelativeCoords(labelX, labelY);
            LabelElement.Draw();
        }
    }

    public class TextTimerBar : LabeledTimerBar
    {
        /// <exception cref="ArgumentNullException">When setting the property to a null value.</exception>
        public string Text { get { return TextElement.Caption; } set { Throw.IfNull(value, nameof(value)); TextElement.Caption = value; } }

        protected Text TextElement { get; set; }

        public TextTimerBar(string label, string text) : base(label)
        {
            Throw.IfNull(text, nameof(text));

            TextElement = new Text(text, ScreenPosition.FromRelativeCoords(-1.0f, -1.0f), 0.5f, Color.White) { IsVisible = true, Alignment = TextAlignment.Right };
        }
        
        public override void Draw(ref float y, float x)
        {
            if (!IsVisible)
                return;

            float textX = (x - Width * 0.5f) + 0.0755f;
            float textY = (y - Height * 0.5f) - Height * 0.5f;
            base.Draw(ref y, x);

            TextElement.Position = ScreenPosition.FromRelativeCoords(textX, textY);
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
            BackRectangleElement = new Box(ScreenRectangle.FromRelativeCoords(-1.0f, -1.0f, 0f, 0f), backColor) { IsVisible = true };
            ForeRectangleElement = new Box(ScreenRectangle.FromRelativeCoords(-1.0f, -1.0f, 0f, 0f), foreColor) { IsVisible = true };
        }

        public ProgressTimerBar(string label) : this(label, Color.DarkRed, Color.Red)
        {
        }
        
        public override void Draw(ref float y, float x)
        {
            if (!IsVisible)
                return;

            float barX = (x - Width * 0.5f) + 0.04f;
            float barY = (y - Height * 0.5f);
            float barW = Width / 2.25f;
            float barH = Height / 3f;
            base.Draw(ref y, x);

            BackRectangleElement.Rectangle = ScreenRectangle.FromRelativeCoords(barX, barY, barW, barH);
            BackRectangleElement.Draw();
            float x2 = barX - barW * 0.5f + barW * 0.5f * percentage;
            float w = barW * percentage;
            ForeRectangleElement.Rectangle = ScreenRectangle.FromRelativeCoords(x2, barY, w, barH);
            ForeRectangleElement.Draw();
        }
    }
}

