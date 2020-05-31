namespace RAGENativeUI.Elements
{
    using System.Collections.Generic;
    using Rage;
    using Rage.Native;
    using RAGENativeUI.Internals;

    public class TimerBarPool : BaseCollection<TimerBarBase>
    {
        private float lastY = TB.InitialY;

        public List<TimerBarBase> ToList()
        {
            return InternalList;
        }

        // added for backwards compatibility, BaseCollection.Remove returns a bool and this one doesn't return anything
        public new void Remove(TimerBarBase item)
        {
            base.Remove(item);
        }

        public unsafe void Draw()
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

                int buttonsRows = GetInstructionalButtonsRows();

                float x = TB.InitialX, y = TB.InitialY - (TB.LoadingPromptYOffset * buttonsRows);

                ref float totalHeight = ref x; // dummy assignment
                bool hasTotalHeight = false;
                if (Game.Console.IsOpen)
                {
                    y = lastY;
                }
                else if (ScriptGlobals.TimersBarsTotalHeightAvailable && N.GetNumberOfReferencesOfScript(0xC45650F0 /* ingamehud */) > 0)
                {
                    ref float timerBarsTotalHeight = ref ScriptGlobals.TimerBarsTotalHeight;
                    ref float timerBarsPrevTotalHeight = ref ScriptGlobals.TimerBarsPrevTotalHeight;

                    totalHeight = ref (timerBarsTotalHeight > timerBarsPrevTotalHeight ? ref timerBarsTotalHeight : ref timerBarsPrevTotalHeight);
                    if (totalHeight > 0.0f)
                    {
                        y -= totalHeight + 0.0075f;
                    }
                    else
                    {
                        totalHeight = -0.0075f;
                    }

                    hasTotalHeight = true;
                }
                // TODO: fallback for when `ingamehud` script is terminated

                lastY = y;

                for (int i = 0; i < InternalList.Count; i++)
                {
                    TimerBarBase b = InternalList[i];
                    b.Draw(x, y);
                    y -= b.Thin ? TB.SmallHeightWithGap : TB.DefaultHeightWithGap;
                }

                if (hasTotalHeight)
                {
                    // update the totalHeight so other TimerBarPools don't overlap
                    totalHeight += lastY - y;
                }

                N.ResetScriptGfxAlign();

                N.HideHudComponentThisFrame(6); // VehicleName
                N.HideHudComponentThisFrame(7); // AreaName
                N.HideHudComponentThisFrame(8); // ?
                N.HideHudComponentThisFrame(9); // StreetName
            }
        }

        private static int GetInstructionalButtonsRows()
        {
            if (CBusySpinner.Available)
            {
                foreach (int sf in CBusySpinner.InstructionalButtons)
                {
                    if (CScaleformMgr.IsMovieRendering(sf))
                    {
                        ref GFxMovieView v = ref CScaleformMgr.GetRawMovieView(sf);

                        return (int)v.GetVariableDouble("TIMELINE.backgrounds.length");
                    }
                }
            }

            return N.BusySpinnerIsOn() ? 1 : 0;
        }
    }
}

