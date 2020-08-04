namespace RNUIExamples
{
    using System;
    using System.Collections.Generic;

    using Rage;
    using Rage.Native;

    using RAGENativeUI;
    using RAGENativeUI.Elements;

    internal static class MenuExtensions
    {
        /// <summary>
        /// Increases the speed at which the left/right controls are triggered when any of <paramref name="items"/> is selected.
        /// This should called only once per menu with all the items that need fast scrolling.
        /// </summary>
        public static UIMenu WithFastScrollingOn(this UIMenu menu, params UIMenuItem[] items) => menu.WithFastScrollingOn((IEnumerable<UIMenuItem>)items);

        /// <summary>
        /// Increases the speed at which the left/right controls are triggered when any of <paramref name="items"/> is selected.
        /// This should called only once per menu with all the items that need fast scrolling.
        /// </summary>
        public static UIMenu WithFastScrollingOn(this UIMenu menu, IEnumerable<UIMenuItem> items)
        {
            var itemsSet = new HashSet<UIMenuItem>(items);

            void UpdateAcceleration(UIMenu menu, UIMenuItem selectedItem)
            {
                var accel = itemsSet.Contains(selectedItem) ? new[]
                {
                    new UIMenu.AccelerationStep(0, 300),
                    new UIMenu.AccelerationStep(600, 10),
                } : null;

                menu.SetKeyAcceleration(Common.MenuControls.Left, accel);
                menu.SetKeyAcceleration(Common.MenuControls.Right, accel);
            }

            menu.OnIndexChange += (m, i) => UpdateAcceleration(m, m.MenuItems[i]);

            if (menu.MenuItems.Count > 0)
            {
                // in case the currently selected item is one of the items that need fast scrolling
                UpdateAcceleration(menu, menu.MenuItems[menu.CurrentSelection]);
            }

            return menu;
        }

        /// <summary>
        /// Allows to edit a string by selecting the item. The current string is displayed in the item's <see cref="UIMenuItem.RightLabel"/>.
        /// </summary>
        /// <param name="getter">Gets the string to display to the user.</param>
        /// <param name="setter">Takes the string edited by the user.</param>
        /// <param name="maxLength">The maximum length of the string.</param>
        /// <param name="maxLengthInItem">
        /// The maximum length of the string when set to the <see cref="UIMenuItem.RightLabel"/> property.
        /// If the string length exceeds this value, the string is cut and "..." is appended.
        /// </param>
        public static UIMenuItem WithTextEditing(this UIMenuItem item, Func<string> getter, Action<string> setter, int maxLengthInItem = 16, int maxLength = 32)
        {
            getter = getter ?? throw new ArgumentNullException(nameof(getter));
            setter = setter ?? throw new ArgumentNullException(nameof(setter));

            WithTextEditingBase(item, maxLength,
                getter,
                str =>
                {
                    TrimAndSetRightLabel(item, str, maxLengthInItem);
                    setter(str);
                });

            TrimAndSetRightLabel(item, getter(), maxLengthInItem);
            return item;

            static void TrimAndSetRightLabel(UIMenuItem item, string str, int maxLength)
                => item.RightLabel = str.Length > maxLength ? (str.Substring(0, maxLength) + "...") : str;
        }

        /// <summary>
        /// Allows to input a new value by selecting the item.
        /// </summary>
        public static UIMenuNumericScrollerItem<float> WithTextEditing(this UIMenuNumericScrollerItem<float> item, int maxLength = 32)
            => WithTextEditing(item, maxLength, float.TryParse, "real number");

        /// <summary>
        /// Allows to input a new value by selecting the item.
        /// </summary>
        public static UIMenuNumericScrollerItem<int> WithTextEditing(this UIMenuNumericScrollerItem<int> item, int maxLength = 32)
            => WithTextEditing(item, maxLength, int.TryParse, "integer number");

        /// <summary>
        /// Allows to input a new value by selecting the item.
        /// </summary>
        public static UIMenuNumericScrollerItem<uint> WithTextEditing(this UIMenuNumericScrollerItem<uint> item, int maxLength = 32)
            => WithTextEditing(item, maxLength, uint.TryParse, "unsigned integer number");

        private delegate bool TryParseDelegate<T>(string s, out T result);

        private static UIMenuNumericScrollerItem<T> WithTextEditing<T>(this UIMenuNumericScrollerItem<T> item, int maxLength, TryParseDelegate<T> tryParse, string name)
            where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
            => WithTextEditingBase(item, maxLength,
                () => item.OptionText,
                str =>
                {
                    if (tryParse(str, out var newValue))
                    {
                        if (newValue.CompareTo(item.Maximum) > 0)
                        {
                            NotifyIncorrectInput($"the maximum value is {item.Formatter(item.Maximum)}.");
                        }
                        else if (newValue.CompareTo(item.Minimum) < 0)
                        {
                            NotifyIncorrectInput($"the minimum value is {item.Formatter(item.Minimum)}.");
                        }
                        else
                        {
                            item.Value = newValue;
                        }
                    }
                    else
                    {
                        NotifyIncorrectInput($"'{str}' is not a valid {name}.");
                    }

                    static void NotifyIncorrectInput(string msg) => Game.DisplayNotification($"~r~Incorrect input~s~: {msg}");
                });

        private static T WithTextEditingBase<T>(T item, int maxLength, Func<string> strGetter, Action<string> resultCallback) where T : UIMenuItem
        {
            item = item ?? throw new ArgumentNullException(nameof(item));
            if (maxLength < 0)
            {
                throw new ArgumentOutOfRangeException("Length cannot be negative", nameof(maxLength));
            }

            item.Activated += (m, s) =>
            {
                Plugin.Pool.Draw();

                NativeFunction.Natives.DISPLAY_ONSCREEN_KEYBOARD(6, "", "", strGetter(), "", "", "", maxLength);
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
                    resultCallback(str);
                }
            };

            return item;
        }
    }
}
