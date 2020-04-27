namespace RAGENativeUI.Elements
{
    using System;
    using System.Drawing;

    // TODO: how should it behave when empty (OptionCount == 0)?
    public abstract class UIMenuScrollerItem : UIMenuItem
    {
        private int index;

        /// <summary>
        /// Gets or sets the index of the selected option.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <c>value</c> is less than zero.
        /// -or-
        /// <c>value</c> is equal to or greater than <see cref="OptionCount"/>.
        /// </exception>
        public virtual int Index 
        {
            get => index;
            set
            {
                if (value != index)
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), "value < 0");
                    }

                    if (value >= OptionCount)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), "value >= OptionCount");
                    }

                    int oldIndex = index;
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
        /// Gets the text displayed for the selected option.
        /// </summary>
        public abstract string OptionText { get; }

        /// <summary>
        /// Occurs when the value of <see cref="Index"/> changes, either programmatically or when the user interacts with the item.
        /// </summary>
        public event ItemScrollerEvent IndexChanged;

        /// <summary>
        /// Enables or disables scrolling by holding the key
        /// </summary>
        public bool ScrollingEnabled { get; set; } = true;

        /// <inheritdoc/>
        public override string RightLabel { get => base.RightLabel; set => throw new Exception($"{nameof(UIMenuListItem)} cannot have a right label."); }

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
            int newIndex = Index - 1;

            if (newIndex < 0) // wrap around
                newIndex = OptionCount - 1;

            Index = newIndex;
        }

        /// <inheritdoc/>
        public override void Draw(float x, float y, float width, float height)
        {
            base.Draw(x, y, width, height);

            string selectedOption = OptionText;

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
