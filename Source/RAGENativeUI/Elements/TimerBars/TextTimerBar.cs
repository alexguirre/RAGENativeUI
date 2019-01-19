namespace RAGENativeUI.Elements.TimerBars
{
#if RPH1
    extern alias rph1;
    using Vector2 = rph1::Rage.Vector2;
#else
    /** REDACTED **/
#endif

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

            Vector2 p = Position;
            Vector2 s = Size;
            // TODO: fix TextTimerBar to accommodate the new changes in Text
            Elements.Text.Draw(Text, (p.X + s.X * 0.48f, p.Y - s.Y * 0.5f).Rel(), GetTextScale(), Color, TextFont.ChaletLondon, TextAlignment.Right, 0.0f, false, false);
        }

        private float GetTextScale() => (Size.Y * 0.5f) / DefaultHeight;
    }
}

