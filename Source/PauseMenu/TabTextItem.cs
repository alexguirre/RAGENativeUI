using System.Drawing;
using RAGENativeUI.Elements;

namespace RAGENativeUI.PauseMenu
{
    public class TabTextItem : TabItem
    {
        public string TextTitle { get; set; }
        public string Text { get; set; }
        public int WordWrap { get; set; }

        public TabTextItem(string name, string title) : base(name)
        {
            TextTitle = title;
        }

        public TabTextItem(string name, string title, string text) : base(name)
        {
            TextTitle = title;
            Text = text;
        }

        public override void Draw()
        {
            base.Draw();
            
            var alpha = (Focused || !CanBeFocused) ? 255 : 200;

            int width = BottomRight.X - TopLeft.X - 80;
            int titleLines = TextCommands.GetLineCount(TextTitle ?? "", new TextStyle(TextFont.ChaletLondon, Color.White, 1.5f, TextJustification.Left, 0, width / 1920f), 0, 0);

            if (!string.IsNullOrEmpty(TextTitle))
            {
                ResText.Draw(TextTitle, SafeSize.AddPoints(new Point(40, 20)), 1.5f, Color.FromArgb(alpha, Color.White), Common.EFont.ChaletLondon, ResText.Alignment.Left, false, false, new Size(width, 0));
            }

            if (!string.IsNullOrEmpty(Text))
            {
                var ww = WordWrap == 0 ? BottomRight.X - TopLeft.X - 40 : WordWrap;
                ResText.Draw(Text, SafeSize.AddPoints(new Point(40, 40 + 110 * titleLines)), 0.4f, Color.FromArgb(alpha, Color.White), Common.EFont.ChaletLondon, ResText.Alignment.Left, false, false, new Size((int)ww, 0));
            }
        }
    }
}

