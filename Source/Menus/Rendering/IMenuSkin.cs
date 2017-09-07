namespace RAGENativeUI.Menus.Rendering
{
    using System.Drawing;

    using Graphics = Rage.Graphics;
    
    public interface IMenuSkin
    {
        void DrawBackground(Graphics graphics, MenuBackground background, float x, float y);
        void DrawBanner(Graphics graphics, MenuBanner banner, float x, float y);
        void DrawDescription(Graphics graphics, MenuDescription description, float x, float y);
        void DrawSubtitle(Graphics graphics, MenuSubtitle subtitle, float x, float y);
        void DrawUpDownDisplay(Graphics graphics, MenuUpDownDisplay upDownDisplay, float x, float y);
        void DrawItem(Graphics graphics, MenuItem item, float x, float y, bool selected);
        void DrawItemCheckbox(Graphics graphics, MenuItemCheckbox item, float x, float y, bool selected);
        void DrawItemScroller(Graphics graphics, MenuItemScroller item, float x, float y, bool selected);

        string FormatDescriptionText(MenuDescription description, string text, out SizeF textMeasurement);
    }
}

