namespace RAGENativeUI.Menus
{
    using Graphics = Rage.Graphics;

    public interface IMenuComponent
    {
        Menu Menu { get; }

        void Process();
        void Draw(Graphics graphics, ref float x, ref float y);
    }

    public interface IDynamicHeightMenuComponent : IMenuComponent
    {
        float GetHeight();
    }
}

