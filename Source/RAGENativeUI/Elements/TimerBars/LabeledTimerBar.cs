namespace RAGENativeUI.Elements.TimerBars
{
    public class LabeledTimerBar : TimerBar
    {
        private string label;

        public string Label { get { return label; } set { Throw.IfNull(value, nameof(value)); label = value; } }

        public LabeledTimerBar(string label)
        {
            Throw.IfNull(label, nameof(label));

            Label = label;
        }

        public override void Draw()
        {
            if (!IsVisible)
                return;

            base.Draw();

            ScreenRectangle r = Rectangle;
            Text.Draw(ScreenPosition.FromRelativeCoords(r.X - r.Width * 0.065f, r.Y - r.Height * 0.275f), Label, GetLabelScale(), Color, TextFont.ChaletLondon, TextAlignment.Right, 0.0f, false, false);
        }

        private float GetLabelScale() => (Rectangle.Height * 0.288f) / DefaultHeight;
    }
}

