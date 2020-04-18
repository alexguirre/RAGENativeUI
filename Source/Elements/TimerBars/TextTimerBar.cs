namespace RAGENativeUI.Elements
{
    using System;
    using System.ComponentModel;
    using System.Drawing;

    public class TextTimerBar : TimerBarBase
    {
        public string Text { get; set; }

        public TextTimerBar(string label, string text) : base(label)
        {
            Text = text;
        }

        [Obsolete("Use TimerBarBase.Draw(float, float) instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public override void Draw(int interval)
        {
            SizeF res = UIMenu.GetScreenResolutionMantainRatio();
            Point safe = UIMenu.GetSafezoneBounds();

            base.Draw(interval);
            ResText.Draw(Text, new Point((int)res.Width - safe.X - 10, (int)res.Height - safe.Y - (42 + (4 * interval))), 0.5f, Color.White, Common.EFont.ChaletLondon, ResText.Alignment.Right, false, false, Size.Empty);
        }

        public override void Draw(float x, float y)
        {
            base.Draw(x, y);

            y += TB.TextYOffset;

            N.SetTextFont(0);
            N.SetTextWrap(0.0f, TB.TextWrapEnd);
            N.SetTextScale(TB.TextScale, TB.TextSize);
            N.GetHudColour(1, out int r, out int g, out int b, out int a); // TODO: TextColor property
            N.SetTextColour(r, g, b, a);
            N.SetTextJustification(2); // Right

            TextCommands.Display(Text, x, y);
        }
    }
}

