namespace RAGENativeUI.Elements.TimerBars
{
    public class TimerBarsStack : BaseCollection<TimerBar>
    {
        public ScreenPosition? OriginPosition { get; set; }

        public void ShowAll()
        {
            foreach (TimerBar t in InternalList)
            {
                t.IsVisible = true;
            }
        }

        public void HideAll()
        {
            foreach (TimerBar t in InternalList)
            {
                t.IsVisible = false;
            }
        }

        public void Draw()
        {
            float x, y;

            if (OriginPosition.HasValue)
            {
                x = OriginPosition.Value.X;
                y = OriginPosition.Value.Y;
            }
            else
            {
                x = y = 0.5f + (N.GetSafeZoneSize() / 2f); // safezone bottom-right corner
            }

            for (int i = 0; i < InternalList.Count; i++)
            {
                TimerBar b = InternalList[i];
                if (b != null && b.IsVisible)
                {
                    ScreenRectangle r = b.Rectangle;
                    b.Rectangle = ScreenRectangle.FromRelativeCoords(x - r.Width * 0.5f, y - r.Height * 0.5f, r.Width, r.Height);
                    b.Draw();
                    y -= r.Height + 0.003f;
                }
            }
        }
    }
}

