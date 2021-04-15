namespace RAGENativeUI.Elements
{
    using System.Collections.Generic;
    using System.Drawing;

    using Rage;

    public class UIMenuStatsPanel : UIMenuPanel
    {

        public IList<Stat> Stats { get; } = new List<Stat>();

        private float CalculateHeight()
        {
            return 0.00138888f * (27.0f * Stats.Count);
        }

        public override void Draw(float x, ref float y, float menuWidth)
        {
            var height = CalculateHeight();

            DrawBackground(x, y, height, menuWidth);

            float statY = y;
            foreach (var s in Stats)
            {
                s.Draw(x, ref statY, menuWidth);
            }

            y += height;
        }

        public class Stat
        {
            public string Text { get; set; }
            public TextStyle TextStyle { get; set; } = TextStyle.Default.With(font: UIMenuItem.DefaultTextFont, scale: UIMenuItem.DefaultTextScale);
            public float Percentage { get; set; }
            public float Upgrade { get; set; }

            public Stat(string text, float percentage, float upgrade) => (Text, Percentage, Upgrade) = (text, percentage, upgrade);

            public virtual void Draw(float x, ref float y, float menuWidth)
            {
                const float OffsetX = 0.0046875f;
                const float OffsetY = 0.00277776f * 2.95f;

                TextStyle.Apply();
                TextCommands.Display(Text, x + OffsetX, y + OffsetY);

                var foreColor = HudColor.White.GetColor();
                var backColor = Color.FromArgb(76, foreColor);

                const float BarOffsetY = 0.00277776f * 6.25f;
                var sectionsX = x + menuWidth - (0.00078125f * 150f);
                var sectionsY = y + BarOffsetY;
                DrawSections(sectionsX, sectionsY, 1.0f, backColor);
                float percentage = Percentage;
                if (Upgrade != 0.0f)
                {
                    float upgradePercentage;
                    Color upgradeColor;
                    if (Upgrade < 0.0f)
                    {
                        upgradePercentage = percentage;
                        percentage += Upgrade;
                        upgradeColor = HudColor.Red.GetColor();
                    }
                    else
                    {
                        upgradePercentage = percentage + Upgrade;
                        upgradeColor = HudColor.Blue.GetColor();
                    }
                    DrawSections(sectionsX, sectionsY, upgradePercentage, upgradeColor);
                }
                DrawSections(sectionsX, sectionsY, percentage, foreColor);

                y += 0.034722f;
            }
        }

        private static void DrawSections(float x, float y, float percentage, Color color)
        {
            const int NumSections = 5;
            const float Padding = 2.0f;
            const float TotalWidth = 125.0f * 0.00078125f;
            const float TotalWidthNoPadding = TotalWidth - (Padding * (NumSections - 1) * 0.00078125f);

            const float SectionPercent = 1.0f / NumSections;
            const float SectionWidth = TotalWidth * SectionPercent;
            const float SectionHeight = 0.00138888f * 6f;

            for (int i = 0; i < NumSections; i++)
            {
                float p = System.Math.Min(percentage, SectionPercent);
                if (p > 0f)
                {
                    DrawRect(x, y, TotalWidthNoPadding * p, SectionHeight, color);
                }

                percentage -= SectionPercent;
                x += SectionWidth;
            }
        }

        private static void DrawRect(float x, float y, float w, float h, Color color)
            => N.DrawRect(x + (w * 0.5f), y + (h * 0.5f), w, h, color.R, color.G, color.B, color.A);
    }
}

