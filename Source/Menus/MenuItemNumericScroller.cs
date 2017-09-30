namespace RAGENativeUI.Menus
{
    using System;
    using System.Globalization;

    using Rage;

    public class MenuItemNumericScroller : MenuItemScroller
    {
        private decimal currentValue;
        private decimal minimum = 0.0m;
        private decimal maximum = 100.0m;
        private decimal increment = 0.5m;

        public decimal Value
        {
            get { return currentValue; }
            set
            {
                if (value != currentValue)
                {
                    if (value < minimum || value > maximum)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Value), $"{nameof(Value)} can't be lower than {nameof(Minimum)} or higher than {nameof(Maximum)}.");
                    }

                    currentValue = value;

                    UpdateSelectedIndex();
                }
            }
        }


        public decimal Minimum
        {
            get { return minimum; }
            set
            {
                minimum = value;
                if (minimum > maximum)
                {
                    maximum = minimum;
                }

                Value = EnsureValue(Value);

                UpdateSelectedIndex();
            }
        }

        public decimal Maximum
        {
            get { return maximum; }
            set
            {
                maximum = value;
                if (minimum > maximum)
                {
                    minimum = maximum;
                }

                Value = EnsureValue(Value);

                UpdateSelectedIndex();
            }
        }

        public decimal Increment
        {
            get { return increment; }
            set
            {
                if (value < 0.0m)
                {
                    throw new ArgumentOutOfRangeException(nameof(Increment), $"{nameof(Increment)} can't be lower than zero.");
                }

                increment = value;

                UpdateSelectedIndex();
            }
        }

        public override int SelectedIndex
        {
            get { return base.SelectedIndex; }
            set
            {
                int newIndex = MathHelper.Clamp(value, 0, MathHelper.Max(0, GetOptionsCount() - 1));
                currentValue = Minimum + newIndex * Increment;
                base.SelectedIndex = newIndex;
            }
        }

        public bool ThousandsSeparator { get; set; }
        public bool Hexadecimal { get; set; }
        public int DecimalPlaces { get; set; } = 2;

        public MenuItemNumericScroller(string text) : base(text)
        {
            SelectedIndex = GetOptionsCount() / 2;
        }

        private decimal EnsureValue(decimal value)
        {
            if (value < minimum)
                value = minimum;

            if (value > maximum)
                value = maximum;

            return value;

        }

        private void UpdateSelectedIndex()
        {
            SelectedIndex = (int)((currentValue - Minimum) / Increment);
        }

        public override int GetOptionsCount()
        {
            return (int)((Maximum - Minimum) / Increment) + 1;
        }

        public override string GetSelectedOptionText()
        {
            string text;

            if (Hexadecimal)
            {
                text = ((Int64)currentValue).ToString("X", CultureInfo.InvariantCulture);
            }
            else
            {
                text = currentValue.ToString((ThousandsSeparator ? "N" : "F") + DecimalPlaces.ToString(CultureInfo.CurrentCulture), CultureInfo.CurrentCulture);
            }

            return text;
        }

        protected internal override bool OnMoveLeft(Menu origin)
        {
            decimal newValue = currentValue;

            try
            {
                newValue -= Increment;

                if (newValue < minimum)
                    newValue = minimum;
            }
#if DEBUG
            catch (OverflowException ex)
            {
                Game.LogTrivial("MenuItemNumericScroller.OnPreviewMoveLeft: OverflowException");
                Game.LogTrivial(ex.ToString());

                newValue = minimum;
            }
#else
            catch (OverflowException)
            {
                newValue = minimum;
            }
#endif
            Value = newValue;

            return base.OnMoveLeft(origin);
        }

        protected internal override bool OnMoveRight(Menu origin)
        {
            decimal newValue = currentValue;

            try
            {
                newValue += Increment;

                if (newValue > maximum)
                    newValue = maximum;
            }
#if DEBUG
            catch (OverflowException ex)
            {
                Game.LogTrivial("MenuItemNumericScroller.OnPreviewMoveLeft: OverflowException");
                Game.LogTrivial(ex.ToString());

                newValue = maximum;
            }
#else
            catch (OverflowException)
            {
                newValue = maximum;
            }
#endif
            Value = newValue;

            return base.OnMoveRight(origin);
        }
    }
}

