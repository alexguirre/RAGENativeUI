namespace RAGENativeUI.Elements
{
    using System.Collections.Generic;

    public class TimerBarPool : BaseCollection<TimerBarBase>
    {
        public List<TimerBarBase> ToList()
        {
            return InternalList;
        }

        // added for backwards compatibility, BaseCollection.Remove returns a bool and this one doesn't return anything
        public new void Remove(TimerBarBase item)
        {
            base.Remove(item);
        }

        public void Draw()
        {
            if (InternalList.Count > 0)
            {
                N.RequestStreamedTextureDict(TB.BgTextureDictionary);
                if (!N.HasStreamedTextureDictLoaded(TB.BgTextureDictionary))
                {
                    return;
                }

                N.SetScriptGfxAlign('R', 'B');
                N.SetScriptGfxAlignParams(0.0f, 0.0f, 0.952f, 0.949f);

                float x = TB.InitialX, y = TB.InitialY - (N.BusySpinnerIsOn() ? TB.LoadingPromptYOffset : 0.0f);
                for (int i = 0; i < InternalList.Count; i++)
                {
                    TimerBarBase b = InternalList[i];
                    b.Draw(x, y);
                    y -= b.SmallHeight ? TB.SmallHeightWithGap : TB.DefaultHeightWithGap;
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

