namespace RAGENativeUI.Rendering
{
    using System.Drawing;

    using Rage;
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Utility;
    using Font = RAGENativeUI.Utility.Font;

    public interface ISkin
    {
        Texture Image { get; }

        void DrawText(Graphics graphics, string text, string fontName, float fontSize, RectangleF rectangle, Color color, TextHorizontalAligment horizontalAligment = TextHorizontalAligment.Left, TextVerticalAligment verticalAligment = TextVerticalAligment.Center);
        void DrawText(Graphics graphics, string text, Font font, RectangleF rectangle, Color color, TextHorizontalAligment horizontalAligment = TextHorizontalAligment.Left, TextVerticalAligment verticalAligment = TextVerticalAligment.Center);
    }
}

