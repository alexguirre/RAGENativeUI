namespace RAGENativeUI.Elements
{
    using System;
    using System.Drawing;
    using Rage;

    /// <summary>
    /// Implements the basic functionality of an item with multiple options to choose from through scrolling, with left/right arrows.
    /// </summary>
    public abstract class UIMenuScrollerItem : UIMenuItem
    {
        /// <summary>
        /// Defines the value of <see cref="Index"/> when <see cref="IsEmpty"/> is <c>true</c>.
        /// </summary>
        public const int EmptyIndex = -1;

        private int index = EmptyIndex;

        /// <summary>
        /// Gets or sets the index of the selected option. When <see cref="IsEmpty"/> is <c>true</c>, <see cref="EmptyIndex"/> is returned.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// <c>value</c> is not <see cref="EmptyIndex"/> when <see cref="IsEmpty"/> is <c>true</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <c>value</c> is negative.
        /// -or-
        /// <c>value</c> is equal to or greater than <see cref="OptionCount"/>.
        /// </exception>
        public virtual int Index 
        {
            get
            {
                if (IsEmpty)
                {
                    return EmptyIndex;
                }

                int idx = index;
                int count = OptionCount;
                if (idx < 0 || idx >= count)
                {
                    // in case OptionCount changed and index is now out of bounds or index has EmptyIndex and the scroller is no longer empty
                    int oldIndex = idx;
                    idx = idx == EmptyIndex ? 0 : (idx % count);
                    index = idx;
                    OnSelectedIndexChanged(oldIndex, idx);
                }

                return idx;
            }
            set
            {
                int oldIndex = Index;
                if (value != oldIndex)
                {
                    if (IsEmpty && value != EmptyIndex)
                    {
                        throw new ArgumentException(nameof(value), $"{nameof(IsEmpty)} is true and {nameof(value)} is not equal to {nameof(EmptyIndex)}");
                    }

                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} is negative");
                    }

                    if (value >= OptionCount)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} is equal or greater than {nameof(OptionCount)}");
                    }

                    index = value;
                    OnSelectedIndexChanged(oldIndex, value);
                }
            }
        }

        /// <summary>
        /// Gets the number of possible options.
        /// </summary>
        public abstract int OptionCount { get; }

        /// <summary>
        /// Gets the text to display as the currently selected option.
        /// </summary>
        /// <remarks>
        /// This property is also used when <see cref="IsEmpty"/> is <c>true</c>, so the implementation needs to take into account this
        /// state, for example it may return <c>null</c>.
        /// </remarks>
        public abstract string OptionText { get; }

        /// <summary>
        /// Gets whether any option is available.
        /// </summary>
        /// <returns>
        /// <c>true</c> when <see cref="OptionCount"/> is zero or negative; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEmpty => OptionCount <= 0;

        /// <summary>
        /// Occurs when the value of <see cref="Index"/> changes, either programmatically or when the user interacts with the item.
        /// </summary>
        public event ItemScrollerEvent IndexChanged;

        /// <summary>
        /// Gets or sets whether scrolling through the options is enabled.
        /// </summary>
        public bool ScrollingEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets whether scrolling through the options is enabled when <see cref="UIMenuItem.Enabled"/> is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// The property <see cref="ScrollingEnabled"/> still has to be <c>true</c> to enable scrolling when the item is disabled.
        /// </remarks>
        public bool ScrollingEnabledWhenDisabled { get; set; } = false;

        /// <summary>
        /// Gets or sets whether when the last item is selected, it can continue scrolling to the first item, and vice versa.
        /// The default value is <c>true</c>.
        /// </summary>
        public bool AllowWrapAround { get; set; } = true;

        /// <inheritdoc/>
        public override string RightLabel { get => base.RightLabel; set => throw new Exception($"{nameof(UIMenuScrollerItem)} cannot have a right label."); }

        public SliderStyleOptions? SliderStyle { get; set; }

        // temp until menu controls are refactored
        internal uint HoldTime;
        internal uint HoldTimeBeforeScroll = 200;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIMenuScrollerItem"/> class.
        /// </summary>
        /// <param name="text">The <see cref="UIMenuScrollerItem"/>'s label.</param>
        /// <param name="description">The <see cref="UIMenuScrollerItem"/>'s description.</param>
        public UIMenuScrollerItem(string text, string description) : base(text, description)
        {
            ScrollerProxy = new UIMenuScrollerProxy(this);
        }

        /// <summary>
        /// Scrolls to the next option following the selected option.
        /// </summary>
        /// <param name="menu">The <see cref="UIMenu"/> that is calling this method, or <c>null</c> if no menu is involved.</param>
        /// <returns><c>true</c> if <see cref="Index"/> changed; otherwise, <c>false</c>.</returns>
        public virtual bool ScrollToNextOption(UIMenu menu = null)
        {
            if (IsEmpty)
            {
                return false;
            }

            int oldIndex = Index;
            int newIndex = oldIndex + 1;

            if (newIndex > (OptionCount - 1)) // wrap around
            {
                if (!AllowWrapAround)
                {
                    return false;
                }

                newIndex = 0;
            }

            Index = newIndex;

            if (oldIndex != newIndex)
            {
                menu?.ScrollerChange(this, oldIndex, newIndex);
                return true;
            }
            else 
            {
                return false;
            }
        }

        /// <summary>
        /// Scrolls to the option previous to the selected option.
        /// </summary>
        /// <param name="menu">The <see cref="UIMenu"/> that is calling this method, or <c>null</c> if no menu is involved.</param>
        /// <returns><c>true</c> if <see cref="Index"/> changed; otherwise, <c>false</c>.</returns>
        public virtual bool ScrollToPreviousOption(UIMenu menu = null)
        {
            if (IsEmpty)
            {
                return false;
            }

            int oldIndex = Index;
            int newIndex = oldIndex - 1;

            if (newIndex < 0) // wrap around
            {
                if (!AllowWrapAround)
                {
                    return false;
                }

                newIndex = OptionCount - 1;
            }

            Index = newIndex;

            if (oldIndex != newIndex)
            {
                menu?.ScrollerChange(this, oldIndex, newIndex);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public override void Draw(float x, float y, float width, float height)
        {
            base.Draw(x, y, width, height);

            if (SliderStyle.HasValue)
            {
                DrawSlider(x, y, width, height);
            }
            else
            {
                DrawScroller(x, y, width, height);
            }
        }

        private void DrawScroller(float x, float y, float width, float height)
        {
            string selectedOption = OptionText ?? string.Empty;

            SetTextCommandOptions(false);
            float optTextWidth = TextCommands.GetWidth(selectedOption);

            GetBadgeOffsets(out _, out float badgeOffset);

            if (Selected && (Enabled || ScrollingEnabledWhenDisabled) && ScrollingEnabled)
            {
                Color arrowsColor = CurrentForeColor;
                if (!Enabled)
                {
                    arrowsColor = HighlightedForeColor;
                }

                float optTextX = x + width - (0.00390625f * 1.5f) - optTextWidth - (0.0046875f * 1.5f) - badgeOffset;
                float optTextY = y + 0.00277776f;

                SetTextCommandOptions(false);
                TextCommands.Display(selectedOption, optTextX, optTextY);

                {
                    UIMenu.GetTextureDrawSize(UIMenu.CommonTxd, UIMenu.ArrowRightTextureName, out float w, out float h);

                    float spriteX = x + width - (0.00390625f * 0.5f) - (w * 0.5f) - badgeOffset;
                    float spriteY = y + (0.034722f * 0.5f);

                    Color c = (!AllowWrapAround && Index == OptionCount - 1 || IsEmpty) ? DisabledForeColor : arrowsColor;
                    UIMenu.DrawSprite(UIMenu.CommonTxd, UIMenu.ArrowRightTextureName, spriteX, spriteY, w, h, c);
                }
                {
                    UIMenu.GetTextureDrawSize(UIMenu.CommonTxd, UIMenu.ArrowLeftTextureName, out float w, out float h);

                    float spriteX = x + width - (0.00390625f * 1.5f) - (w * 0.5f) - optTextWidth - (0.0046875f * 1.5f) - badgeOffset;
                    float spriteY = y + (0.034722f * 0.5f);

                    Color c = (!AllowWrapAround && Index == 0 || IsEmpty) ? DisabledForeColor : arrowsColor;
                    UIMenu.DrawSprite(UIMenu.CommonTxd, UIMenu.ArrowLeftTextureName, spriteX, spriteY, w, h, c);
                }
            }
            else
            {
                float optTextX = x + width - 0.00390625f - optTextWidth - badgeOffset;
                float optTextY = y + 0.00277776f;

                SetTextCommandOptions(false);
                if (!ScrollingEnabled)
                {
                    Internals.CTextStyle.ScriptStyle.Color = DisabledForeColor.ToArgb();
                }
                TextCommands.Display(selectedOption, optTextX, optTextY);
            }
        }

        // TODO: how should the slider look when ScrollingEnabled is false?
        private void DrawSlider(float x, float y, float width, float height)
        {
            float percentage = IsEmpty ? 0.0f : (OptionCount == 1 ? 1.0f : ((float)Index / (OptionCount - 1)));

            RectangleF r = GetSliderBarBounds(x, y, width, height);
            SliderStyleOptions s = SliderStyle.Value;

            UIMenu.DrawRect(r.X + r.Width * 0.5f, r.Y + r.Height * 0.5f, r.Width, r.Height, s.BackgroundColor);

            float fillWidth = r.Width * percentage;
            UIMenu.DrawRect(r.X + fillWidth * 0.5f, r.Y + r.Height * 0.5f, fillWidth, r.Height, s.ForegroundColor);
        }

        /// <inheritdoc/>
        protected internal override bool OnInput(UIMenu menu, Common.MenuControls control)
        {
            bool consumed = base.OnInput(menu, control);

            if (ScrollingEnabled && (Enabled || ScrollingEnabledWhenDisabled))
            {
                bool playSound = false;
                switch (control)
                {
                    case Common.MenuControls.Left:
                        consumed = true;
                        playSound = ScrollToPreviousOption(menu);
                        break;

                    case Common.MenuControls.Right:
                        consumed = true;
                        playSound = ScrollToNextOption(menu);
                        break;
                }

                if (playSound)
                {
                    Common.PlaySound(menu.AUDIO_LEFTRIGHT, menu.AUDIO_LIBRARY);
                }
            }

            return consumed;
        }

        /// <inheritdoc/>
        protected internal override bool OnMouseInput(UIMenu menu, RectangleF itemBounds, PointF mousePos, MouseInput input)
        {
            if (menu == null)
            {
                throw new ArgumentNullException(nameof(menu));
            }

            if (SliderStyle.HasValue)
            {
                return OnSliderMouseInput(menu, itemBounds, mousePos, input);
            }
            else
            {
                return OnScrollerMouseInput(menu, itemBounds, mousePos, input);
            }
        }

        private bool OnScrollerMouseInput(UIMenu menu, RectangleF itemBounds, PointF mousePos, MouseInput input)
        {
            if (!Selected || !Hovered)
            {
                return false;
            }

            bool consumed = false;
            bool inSelectBounds = false;
            float selectBoundsX = itemBounds.X + itemBounds.Width * 0.33333f;

            if (mousePos.X <= selectBoundsX)
            {
                inSelectBounds = true;
                // approximately hovering the label, first 1/3 of the item width
                // TODO: game shows cursor sprite 5 when hovering this part, but only if the item does something when selected.
                //       Here, we don't really know if the user does something when selected, maybe add some bool property in UIMenuListItem?
                if (input == MouseInput.JustReleased)
                {
                    consumed = true;
                    OnInput(menu, Common.MenuControls.Select);
                }
            }

            if (!inSelectBounds && ScrollingEnabled && (Enabled || ScrollingEnabledWhenDisabled) && input == MouseInput.PressedRepeat)
            {
                UIMenu.GetTextureDrawSize(UIMenu.CommonTxd, UIMenu.ArrowRightTextureName, out float rightW, out _);

                float rightX = (0.00390625f * 1.0f) + (rightW * 1.0f) + (0.0046875f * 0.75f);

                if (menu.ScaleWithSafezone)
                {
                    N.SetScriptGfxAlign('L', 'T');
                    N.SetScriptGfxAlignParams(-0.05f, -0.05f, 0.0f, 0.0f);
                }
                N.GetScriptGfxPosition(rightX, 0.0f, out rightX, out _);
                N.GetScriptGfxPosition(0.0f, 0.0f, out float borderX, out _);
                if (menu.ScaleWithSafezone)
                {
                    N.ResetScriptGfxAlign();
                }

                rightX = itemBounds.Right - rightX + borderX;

                // It does not check if the mouse in exactly on top of the arrow sprites intentionally:
                //  - If to the right of the right arrow's left border, go right
                //  - Anywhere else in the item, go left.
                // This is how the vanilla menus behave
                consumed = true;
                if (mousePos.X >= rightX)
                {
                    OnInput(menu, Common.MenuControls.Right);
                }
                else
                {
                    OnInput(menu, Common.MenuControls.Left);
                }
            }

            return consumed;
        }

        private bool mouseDownOnSlider = false;
        private bool OnSliderMouseInput(UIMenu menu, RectangleF itemBounds, PointF mousePos, MouseInput input)
        {
            if (!Selected)
            {
                return false;
            }

            RectangleF r = GetSliderBarBounds(itemBounds.X, itemBounds.Y, itemBounds.Width, itemBounds.Height);

            bool consumed = false;
            if (input == MouseInput.JustPressed && Hovered && mousePos.X >= r.X && mousePos.X <= (r.X + r.Width))
            {
                mouseDownOnSlider = true;
            }

            if (mouseDownOnSlider)
            {
                if (OptionCount > 1)
                {
                    consumed = true;
                    float percentage = (mousePos.X - r.X) / r.Width;
                    percentage = MathHelper.Clamp(percentage, 0.0f, 1.0f);
                    int newIndex = (int)Math.Round(percentage * (OptionCount - 1));
                    if (newIndex != Index)
                    {
                        Common.PlaySound(menu.AUDIO_LEFTRIGHT, menu.AUDIO_LIBRARY);
                    }
                    Index = newIndex;
                }

                if (input == MouseInput.JustReleased || input == MouseInput.Released)
                {
                    mouseDownOnSlider = false;
                }
            }
            else if (input == MouseInput.JustReleased && Hovered && mousePos.X < (r.X - 0.00390625f * 2.0f))
            {
                consumed = true;
                OnInput(menu, Common.MenuControls.Select);
            }

            return consumed;
        }

        private RectangleF GetSliderBarBounds(float x, float y, float width, float height)
        {
            GetBadgeOffsets(out float badgeLeftOffset, out float badgeRightOffset);

            const float BorderPadding = 0.00390625f;

            SliderStyleOptions s = SliderStyle.Value;
            float barWidth = (width - badgeLeftOffset - badgeRightOffset - BorderPadding * 2.0f) * s.Width;
            float barHeight = (height - BorderPadding * 2.0f) * s.Height;
            float barX = x + width - BorderPadding - badgeRightOffset - barWidth;
            float barY = y + height * 0.5f - barHeight * 0.5f;

            return new RectangleF(barX, barY, barWidth, barHeight);
        }

        /// <summary>
        /// Triggers the <see cref="IndexChanged"/> event.
        /// </summary>
        protected virtual void OnSelectedIndexChanged(int oldIndex, int newIndex)
        {
            IndexChanged?.Invoke(this, oldIndex, newIndex);
        }

        public struct SliderStyleOptions
        {
            public Color ForegroundColor { get; set; }
            public Color BackgroundColor { get; set; }
            public float Width { get; set; } // as a percentage of the item width
            public float Height { get; set; } // as a percentage of the item height

            public static readonly SliderStyleOptions Default = new SliderStyleOptions
            {
                ForegroundColor = HudColor.Blue.GetColor(),
                BackgroundColor = Color.FromArgb(120, HudColor.Blue.GetColor()),
                Width = 0.45f,
                Height = 0.275f,
            };
        }
    }

    /// <summary>
    /// Helper class to allow to share code between <see cref="UIMenuScrollerItem"/> and <see cref="UIMenuListItem"/>.
    /// </summary>
    internal sealed class UIMenuScrollerProxy
    {
        public delegate ref uint GetHoldTimeDelegate();

        public UIMenuItem Item { get; }
        public Func<bool> GetScrollingEnabled { get; }
        public Func<bool> GetScrollingEnabledWhenDisabled { get; }
        public Func<uint> GetHoldTimeBeforeScroll { get; }
        public GetHoldTimeDelegate GetHoldTime { get; }
        public Action<int> SetIndex { get; }

        public UIMenuScrollerProxy(UIMenuScrollerItem item)
        {
            Item = item;
            GetScrollingEnabled = () => item.ScrollingEnabled;
            GetScrollingEnabledWhenDisabled = () => item.ScrollingEnabledWhenDisabled;
            GetHoldTimeBeforeScroll = () => item.HoldTimeBeforeScroll;
            GetHoldTime = () => ref item.HoldTime;
            SetIndex = i => item.Index = i;
        }

        public UIMenuScrollerProxy(UIMenuListItem item)
        {
            Item = item;
            GetScrollingEnabled = () => item.ScrollingEnabled;
            GetScrollingEnabledWhenDisabled = () => false;
            GetHoldTimeBeforeScroll = () => item.HoldTimeBeforeScroll;
            GetHoldTime = () => ref item._holdTime;
            SetIndex = i => item.Index = i;
        }
    }
}
