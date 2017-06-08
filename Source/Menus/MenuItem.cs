namespace RAGENativeUI.Menus
{
    public class MenuItem
    {
        public delegate void MenuItemEventHandler(MenuItem sender);

        public string Text { get; set; }

        public event MenuItemEventHandler Activated;
    }
}

