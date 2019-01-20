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
            N.SetScriptGfxAlign('R', 'B');
            N.SetScriptGfxAlignParams(0.0f, 0.0f, 0.952f, 0.949f);
            for (int i = 0; i < InternalList.Count; i++)
            {
                TimerBar b = InternalList[i];
                if (b != null && b.IsVisible)
                {
                    b.Draw(i);
                }
            }
            N.ResetScriptGfxAlign();
        }
    }
}

