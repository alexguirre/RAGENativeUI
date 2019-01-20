namespace RAGENativeUI.TimerBars
{
#if RPH1
    extern alias rph1;
    using Vector2 = rph1::Rage.Vector2;
#else
    /** REDACTED **/
#endif

    using RAGENativeUI.Drawing;

    public class LabeledTimerBar : TimerBar
    {
        private string label;

        public string Label { get { return label; } set { Throw.IfNull(value, nameof(value)); label = value; } }

        public LabeledTimerBar(string label)
        {
            Throw.IfNull(label, nameof(label));

            Label = label;
        }

        public override void Draw(int index)
        {

            if (!IsVisible)
                return;

            base.Draw(index);

            // Constants from the game scripts
            const float InitialWrapEnd = ((((0.88f - 0.062f) + 0.026f) + 0.027f) + 0.03f) - 0.034f;
            const float ScaleScale = 0.202f;
            const float ScaleSize = 0.288f;

            Vector2 pos = Position(index);

            float wrapEnd = InitialWrapEnd;
            if (!N.GetIsWidescreen())
            {
                wrapEnd -= 0.02f;
            }
            wrapEnd = wrapEnd - (0.03f * WrapEndMultiplier);

            N.SetTextFont(0);
            N.SetTextWrap(0.0f, wrapEnd);
            N.SetTextScale(ScaleScale, ScaleSize);
            N.SetTextColour(240, 240, 240, 255);
            N.SetTextJustification((int)TextAlignment.Right);

            N.BeginTextCommandDisplayText("STRING");
            N.AddTextComponentSubstringPlayerName(Label);
            N.EndTextCommandDisplayText(pos.X, pos.Y);
        }

        private static float WrapEndMultiplier
        {
            get
            {
                float aspectRatio = N.GetAspectRatio(false);
                N.GetActiveScreenResolution(out int screenWidth, out int screenHeight);
                float screenRatio = (float)screenWidth / screenHeight;
                aspectRatio = System.Math.Min(aspectRatio, screenRatio);
                if (screenRatio > 3.5f && aspectRatio > 1.7f)
                {
                    return 0.4f;
                }
                else if (aspectRatio > 1.7f)
                {
                    return 0.0f;
                }
                else if (aspectRatio > 1.5f)
                {
                    return 0.2f;
                }
                else if (aspectRatio > 1.3f)
                {
                    return 0.3f;
                }
                else
                {
                    return 0.4f;
                }
            }
        }
    }
}

