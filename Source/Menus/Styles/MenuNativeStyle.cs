using System.Drawing;
using Rage;

namespace RAGENativeUI.Menus.Styles
{
    // TODO: implement MenuNativeStyle
    // this class will use natives to draw the menu
    internal class MenuNativeStyle : IMenuStyle
    {
        public PointF InitialMenuLocation { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public float MenuWidth { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public float BannerHeight { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public float SubtitleHeight { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public float ItemHeight { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public float UpDownDisplayHeight { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public void DrawBackground(Rage.Graphics graphics, MenuBackground background, ref float x, ref float y)
        {
            throw new System.NotImplementedException();
        }

        public void DrawBanner(Rage.Graphics graphics, MenuBanner banner, ref float x, ref float y)
        {
            throw new System.NotImplementedException();
        }

        public void DrawDescription(Rage.Graphics graphics, MenuDescription description, ref float x, ref float y)
        {
            throw new System.NotImplementedException();
        }

        public void DrawItem(Rage.Graphics graphics, MenuItem item, ref float x, ref float y)
        {
            throw new System.NotImplementedException();
        }

        public void DrawItemCheckbox(Rage.Graphics graphics, MenuItemCheckbox item, ref float x, ref float y)
        {
            throw new System.NotImplementedException();
        }

        public void DrawItemScroller(Rage.Graphics graphics, MenuItemScroller item, ref float x, ref float y)
        {
            throw new System.NotImplementedException();
        }

        public void DrawSubtitle(Rage.Graphics graphics, MenuSubtitle subtitle, ref float x, ref float y)
        {
            throw new System.NotImplementedException();
        }

        public void DrawUpDownDisplay(Rage.Graphics graphics, MenuUpDownDisplay upDownDisplay, ref float x, ref float y)
        {
            throw new System.NotImplementedException();
        }

        public string FormatDescriptionText(MenuDescription description, string text, out SizeF textMeasurement)
        {
            throw new System.NotImplementedException();
        }
    }
}

