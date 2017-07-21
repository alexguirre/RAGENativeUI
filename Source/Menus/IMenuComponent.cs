namespace RAGENativeUI.Menus
{
    using System.Drawing;
    
    using Graphics = Rage.Graphics;

    public interface IMenuComponent
    {
        Menu Menu { get; }
        SizeF Size { get; set; }

        void Process();
        void Draw(Graphics graphics, ref float x, ref float y);
    }
}

