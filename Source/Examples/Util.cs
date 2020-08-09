namespace RNUIExamples
{
    using System;
    using System.Linq;
    using RAGENativeUI;
    using RAGENativeUI.Elements;

    internal static class Util
    {
        /// <summary>
        /// Create new list menu item containing all the <see cref="HudColor"/>s which can be selected by the user.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static UIMenuListScrollerItem<HudColor> NewColorsItem(string text, string description)
            => new UIMenuListScrollerItem<HudColor>(text, description, (HudColor[])Enum.GetValues(typeof(HudColor)))
            {
                // custom formatter that adds whitespace between words (i.e. "RedDark" -> "Red Dark")
                Formatter = v => v.ToString().Aggregate("", (acc, c) => acc + (acc.Length > 0 && char.IsUpper(c) ? " " : "") + c)
            };

        public static UIMenuCheckboxItem NewTriStateCheckbox(string name, string description)
        {
            var cb = new UIMenuCheckboxItem(name, false, description);

            cb.Activated += (s, i) =>
            {
                if (cb.Checked)
                {
                    if (cb.Style == UIMenuCheckboxStyle.Tick)
                    {
                        cb.Style = UIMenuCheckboxStyle.Cross;
                        cb.Checked = !cb.Checked; // make the item checked
                    }
                    else
                    {
                        cb.Style = UIMenuCheckboxStyle.Tick;
                    }
                }
            };

            return cb;
        }
    }
}
