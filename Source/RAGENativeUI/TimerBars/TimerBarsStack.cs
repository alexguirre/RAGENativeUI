namespace RAGENativeUI.TimerBars
{
#if RPH1
    extern alias rph1;
    using Vector2 = rph1::Rage.Vector2;
#else
    /** REDACTED **/
#endif

    public class TimerBarsStack : BaseCollection<TimerBar>
    {
        public Vector2? OriginPosition { get; set; }

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
                    Vector2 s = b.Size;
                    b.Position = (x - s.X * 0.5f, y - s.Y * 0.5f).Rel();
                    b.Size = (s.X, s.Y).Rel();
                    b.Draw();
                    y -= s.Y + 0.003f;
                }
            }
        }
    }
}

