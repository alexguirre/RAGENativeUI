namespace RAGENativeUI.Menus.Styles
{
    using System.Drawing;

    using Graphics = Rage.Graphics;
    
    public interface IMenuStyle
    {
        PointF InitialMenuLocation { get; set; }

        float MenuWidth { get; set; }

        float BannerHeight { get; set; }
        float SubtitleHeight { get; set; }
        float ItemHeight { get; set; }
        float UpDownDisplayHeight { get; set; }

        void DrawBackground(Graphics graphics, MenuBackground background, ref float x, ref float y);
        void DrawBanner(Graphics graphics, MenuBanner banner, ref float x, ref float y);
        void DrawDescription(Graphics graphics, MenuDescription description, ref float x, ref float y);
        void DrawSubtitle(Graphics graphics, MenuSubtitle subtitle, ref float x, ref float y);
        void DrawUpDownDisplay(Graphics graphics, MenuUpDownDisplay upDownDisplay, ref float x, ref float y);
        void DrawItem(Graphics graphics, MenuItem item, ref float x, ref float y);

        string FormatDescriptionText(MenuDescription description, string text, out SizeF textMeasurement);
    }
}

