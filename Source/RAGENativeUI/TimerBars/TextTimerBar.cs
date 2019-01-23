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
        // Constants from the game scripts
        private const float TextYOffset = ((((-0.01f - 0.005f) + 0.004f) - 0.001f) + 0.001f);
        private const float TextWrapEnd = (((((0.95f - 0.047f) + 0.001f) + 0.047f) - 0.002f) + 0.001f);
        private const float TextScale = 0.332f;
        private const float TextSize = ((((((0.469f + 0.096f) - 0.017f) + 0.022f) - 0.062f) - 0.001f) - 0.013f);


        private string text;

        public string Text { get { return text; } set { Throw.IfNull(value, nameof(value)); text = value; } }
        public Color TextColor { get; set; } = HudColor.White.GetColor();
        public TimerBarIcon Icon { get; set; }

        public TextTimerBar(string label, string text) : base(label)
        {
            Throw.IfNull(text, nameof(text));

            Text = text;
        }

        public override void Draw(Vector2 position)
        {

            if (!IsVisible)
                return;
            
            base.Draw(position);

            float wrapEnd = TextWrapEnd;

            if (Icon != null)
            {
                if (!Icon.Texture.Dictionary.IsLoaded) Icon.Texture.Dictionary.Load();

                Vector2 iconPos = position + new Vector2(TimerBarIcon.XOffset, TimerBarIcon.YOffset);
                Color c = Icon.Color;
                N.DrawSprite(Icon.Texture.Dictionary, Icon.Texture.Name, iconPos.X, iconPos.Y, Icon.Size.X, Icon.Size.Y, 0.0f, c.R, c.G, c.B, c.A);
                wrapEnd -= Icon.Size.X;
            }

            position.Y += TextYOffset;

            N.SetTextFont(0);
            N.SetTextWrap(0.0f, wrapEnd);
            N.SetTextScale(TextScale, TextSize);
            N.SetTextColour(TextColor.R, TextColor.G, TextColor.B, TextColor.A);
            N.SetTextJustification((int)TextAlignment.Right);

            N.BeginTextCommandDisplayText("STRING");
            N.AddTextComponentSubstringPlayerName(Text);
            N.EndTextCommandDisplayText(position.X, position.Y);
        }
    }
}

