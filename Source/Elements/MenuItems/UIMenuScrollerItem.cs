namespace RAGENativeUI.Elements
{
    using System;
    using System.Drawing;

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
        /// Enables or disables scrolling through the options.
        /// </summary>
        public bool ScrollingEnabled { get; set; } = true;

        /// <inheritdoc/>
        public override string RightLabel { get => base.RightLabel; set => throw new Exception($"{nameof(UIMenuScrollerItem)} cannot have a right label."); }

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
        protected internal virtual void ScrollToNextOption()
        {
            if (IsEmpty)
            {
                return;
            }

            int newIndex = Index + 1;

            if (newIndex > (OptionCount - 1)) // wrap around
                newIndex = 0;

            Index = newIndex;
        }

        /// <summary>
        /// Scrolls to the option previous to the selected option.
        /// </summary>
        protected internal virtual void ScrollToPreviousOption()
        {
            if (IsEmpty)
            {
                return;
            }

            int newIndex = Index - 1;

            if (newIndex < 0) // wrap around
                newIndex = OptionCount - 1;

            Index = newIndex;
        }

        /// <inheritdoc/>
        public override void Draw(float x, float y, float width, float height)
        {
            base.Draw(x, y, width, height);

            string selectedOption = OptionText ?? string.Empty;

            SetTextCommandOptions(false);
            float optTextWidth = TextCommands.GetWidth(selectedOption);

            GetBadgeOffsets(out _, out float badgeOffset);

            if (Selected && Enabled && ScrollingEnabled)
            {
                Color textColor = CurrentForeColor;
                float optTextX = x + width - 0.00390625f - optTextWidth - (0.0046875f * 1.5f) - badgeOffset;
                float optTextY = y + 0.00277776f;

                SetTextCommandOptions(false);
                TextCommands.Display(selectedOption, optTextX, optTextY);

                {
                    UIMenu.GetTextureDrawSize(UIMenu.CommonTxd, UIMenu.ArrowRightTextureName, out float w, out float h);
                    w *= 0.65f;
                    h *= 0.65f;

                    float spriteX = x + width - (0.00390625f * 1.0f) - (w * 0.5f) - badgeOffset;
                    float spriteY = y + (0.034722f * 0.5f);

                    UIMenu.DrawSprite(UIMenu.CommonTxd, UIMenu.ArrowRightTextureName, spriteX, spriteY, w, h, textColor);
                }
                {
                    UIMenu.GetTextureDrawSize(UIMenu.CommonTxd, UIMenu.ArrowLeftTextureName, out float w, out float h);
                    w *= 0.65f;
                    h *= 0.65f;

                    float spriteX = x + width - (0.00390625f * 1.0f) - (w * 0.5f) - optTextWidth - (0.0046875f * 1.5f) - badgeOffset;
                    float spriteY = y + (0.034722f * 0.5f);

                    UIMenu.DrawSprite(UIMenu.CommonTxd, UIMenu.ArrowLeftTextureName, spriteX, spriteY, w, h, textColor);
                }
            }
            else
            {
                float optTextX = x + width - 0.00390625f - optTextWidth - badgeOffset;
                float optTextY = y + 0.00277776f;// + 0.00416664f;

                SetTextCommandOptions(false);
                if (!ScrollingEnabled)
                {
                    Internals.Variables.ScriptTextStyle.Color = DisabledForeColor.ToArgb();
                }
                TextCommands.Display(selectedOption, optTextX, optTextY);
            }
        }

        /// <summary>
        /// Triggers <see cref="IndexChanged"/> event.
        /// </summary>
        protected virtual void OnSelectedIndexChanged(int oldIndex, int newIndex)
        {
            IndexChanged?.Invoke(this, oldIndex, newIndex);
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
        public Func<uint> GetHoldTimeBeforeScroll { get; }
        public GetHoldTimeDelegate GetHoldTime { get; }
        public Action<int> SetIndex { get; }

        public UIMenuScrollerProxy(UIMenuScrollerItem item)
        {
            Item = item;
            GetScrollingEnabled = () => item.ScrollingEnabled;
            GetHoldTimeBeforeScroll = () => item.HoldTimeBeforeScroll;
            GetHoldTime = () => ref item.HoldTime;
            SetIndex = i => item.Index = i;
        }

        public UIMenuScrollerProxy(UIMenuListItem item)
        {
            Item = item;
            GetScrollingEnabled = () => item.ScrollingEnabled;
            GetHoldTimeBeforeScroll = () => item.HoldTimeBeforeScroll;
            GetHoldTime = () => ref item._holdTime;
            SetIndex = i => item.Index = i;
        }
    }
}
