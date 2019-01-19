namespace RAGENativeUI.Elements.TimerBars
{
#if RPH1
    extern alias rph1;
    using Vector2 = rph1::Rage.Vector2;
#else
    /** REDACTED **/
#endif

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

            Vector2 p = Position;
            Vector2 s = Size;
            // TODO: fix LabeledTimerBar to accommodate the new changes in Text
            Text.Draw(Label, (p.X - s.X * 0.065f, p.Y - s.Y * 0.275f).Rel(), GetLabelScale(), Color, TextFont.ChaletLondon, TextAlignment.Right, 0.0f, false, false);
        }

        private float GetLabelScale() => (Size.Y * 0.288f) / DefaultHeight;
    }
}

