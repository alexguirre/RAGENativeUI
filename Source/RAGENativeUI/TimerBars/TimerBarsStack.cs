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
            if (InternalList.Count > 0)
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
                
                N.HideHudComponentThisFrame(6); // VehicleName
                N.HideHudComponentThisFrame(7); // AreaName
                N.HideHudComponentThisFrame(8); // ?
                N.HideHudComponentThisFrame(9); // StreetName
            }
        }
    }
}

