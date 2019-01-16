namespace RAGENativeUI.Elements.TimerBars
{
    public class TextTimerBar : LabeledTimerBar
    {
        private string text;

        public string Text { get { return text; } set { Throw.IfNull(value, nameof(value)); text = value; } }

        public TextTimerBar(string label, string text) : base(label)
        {
            Throw.IfNull(text, nameof(text));

            Text = text;
        }

        public override void Draw()
        {
            if (!IsVisible)
                return;
            
            base.Draw();

            ScreenRectangle r = Rectangle;
            // TODO: fix TextTimerBar to accommodate the new changes in Text
            Elements.Text.Draw(Text, ScreenPosition.FromRelativeCoords(r.X + r.Width * 0.48f, r.Y - r.Height * 0.5f), GetTextScale(), Color, TextFont.ChaletLondon, TextAlignment.Right, 0.0f, false, false);
        }

        private float GetTextScale() => (Rectangle.Height * 0.5f) / DefaultHeight;
    }
}

