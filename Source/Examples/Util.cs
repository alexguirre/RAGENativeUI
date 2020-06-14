namespace RNUIExamples
{
    using System;
    using System.Linq;
    using RAGENativeUI;
    using RAGENativeUI.Elements;
    using Rage;
    using Rage.Native;

    internal static class Util
    {
        /// <summary>
        /// Creates a new menu item which displays the current string in the <see cref="UIMenuItem.RightLabel"/> which can be edited by selecting the item.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="description"></param>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        /// <returns></returns>
        public static UIMenuItem NewTextEditingItem(string text, string description, Func<string> getter, Action<string> setter)
        {
            const int MaxStringLength = 16;

            var item = new UIMenuItem(text, description);
            item.RightLabel = getter();
            item.Activated += (m, s) =>
            {
                Plugin.Pool.Draw();

                NativeFunction.Natives.DISPLAY_ONSCREEN_KEYBOARD(6, "", "", getter(), "", "", "", MaxStringLength);
                int state;
                while ((state = NativeFunction.Natives.UPDATE_ONSCREEN_KEYBOARD<int>()) == 0)
                {
                    GameFiber.Yield();
                    // keep drawing the menus but don't process inputs, since we are blocking the menu processing fiber
                    Plugin.Pool.Draw();
                }

                if (state == 1)
                {
                    string str = NativeFunction.Natives.GET_ONSCREEN_KEYBOARD_RESULT<string>();
                    setter(str);
                    item.RightLabel = str;
                }
            };

            return item;
        }

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
    }
}
