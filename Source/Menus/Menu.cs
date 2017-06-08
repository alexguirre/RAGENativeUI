namespace RAGENativeUI.Menus
{
    public class Menu
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }

        public MenuItemsCollection Items { get; }


    }


    public class MenuItemsCollection : Utility.BaseCollection<MenuItem>
    {
    }
}

