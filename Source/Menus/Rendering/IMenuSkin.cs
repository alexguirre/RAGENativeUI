namespace RAGENativeUI.Menus.Rendering
{
    using Rage;

    using RAGENativeUI.Rendering;

    public interface IMenuSkin : ISkin
    {
        Font TitleFont { get; }
        Font SubtitleFont { get; }
        Font ItemTextFont { get; }
        Font DescriptionFont { get; }

        void DrawBanner(Graphics graphics, float x, float y, float width, float height);
        void DrawBackground(Graphics graphics, float x, float y, float width, float height);
        void DrawSelectedGradient(Graphics graphics, float x, float y, float width, float height);
        void DrawCheckboxEmptyWhite(Graphics graphics, float x, float y, float width, float height);
        void DrawCheckboxEmptyBlack(Graphics graphics, float x, float y, float width, float height);
        void DrawCheckboxCrossWhite(Graphics graphics, float x, float y, float width, float height);
        void DrawCheckboxCrossBlack(Graphics graphics, float x, float y, float width, float height);
        void DrawCheckboxTickWhite(Graphics graphics, float x, float y, float width, float height);
        void DrawCheckboxTickBlack(Graphics graphics, float x, float y, float width, float height);
        void DrawArrowLeft(Graphics graphics, float x, float y, float width, float height);
        void DrawArrowRight(Graphics graphics, float x, float y, float width, float height);
        void DrawArrowsUpDown(Graphics graphics, float x, float y, float width, float height);
        void DrawArrowsUpDownBackground(Graphics graphics, float x, float y, float width, float height);
    }
}

