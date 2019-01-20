namespace RAGENativeUI.TimerBars
{
#if RPH1
    extern alias rph1;
    using Vector2 = rph1::Rage.Vector2;
#else
    /** REDACTED **/
#endif

    using System.Drawing;

    using RAGENativeUI.Drawing;

    public class TextTimerBar : LabeledTimerBar
    {
        private string text;

        public string Text { get { return text; } set { Throw.IfNull(value, nameof(value)); text = value; } }
        public Color TextColor { get; set; } = HudColor.White.GetColor();

        public TextTimerBar(string label, string text) : base(label)
        {
            Throw.IfNull(text, nameof(text));

            Text = text;
        }

        public override void Draw(int index)
        {

            if (!IsVisible)
                return;
            
            base.Draw(index);

            // Constants from the game scripts
            const float YOffset = ((((-0.01f - 0.005f) + 0.004f) - 0.001f) + 0.001f);
            const float WrapEnd = (((((0.95f - 0.047f) + 0.001f) + 0.047f) - 0.002f) + 0.001f);
            const float ScaleScale = 0.332f;
            const float ScaleSize = ((((((0.469f + 0.096f) - 0.017f) + 0.022f) - 0.062f) - 0.001f) - 0.013f);

            Vector2 pos = Position(index);
            pos.Y += YOffset;

            N.SetTextFont(0);
            N.SetTextWrap(0.0f, WrapEnd);
            N.SetTextScale(ScaleScale, ScaleSize);
            N.SetTextColour(TextColor.R, TextColor.G, TextColor.B, TextColor.A);
            N.SetTextJustification((int)TextAlignment.Right);

            N.BeginTextCommandDisplayText("STRING");
            N.AddTextComponentSubstringPlayerName(Text);
            N.EndTextCommandDisplayText(pos.X, pos.Y);
        }
    }
}

