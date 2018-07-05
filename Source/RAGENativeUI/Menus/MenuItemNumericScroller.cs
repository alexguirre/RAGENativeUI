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
        private bool thousandsSeparator;
        private bool hexadecimal;
        private int decimalPlaces = 2;

        public decimal Value
        {
            get { return currentValue; }
            set
            {
                if (value != currentValue)
                {
                    Throw.IfOutOfRange(value, minimum, maximum, nameof(value), $"{nameof(Value)} can't be lower than {nameof(Minimum)} or higher than {nameof(Maximum)}.");

                    currentValue = value;

                    UpdateSelectedIndex();
                }
            }
        }

        public decimal Minimum
        {
            get => minimum;
            set
            {
                if (value != minimum)
                {
                    minimum = value;
                    if (minimum > maximum)
                    {
                        maximum = minimum;
                        OnPropertyChanged(nameof(Maximum));
                    }
                    OnPropertyChanged(nameof(Minimum));

                    Value = EnsureValue(Value);

                    UpdateSelectedIndex();
                }
            }
        }

        public decimal Maximum
        {
            get => maximum;
            set
            {
                if (value != maximum)
                {
                    maximum = value;
                    if (minimum > maximum)
                    {
                        minimum = maximum;
                        OnPropertyChanged(nameof(Minimum));
                    }
                    OnPropertyChanged(nameof(Maximum));

                    Value = EnsureValue(Value);

                    UpdateSelectedIndex();
                }
            }
        }

        public decimal Increment
        {
            get => increment;
            set
            {
                Throw.IfNegative(value, nameof(value));

                if (value != increment)
                {
                    increment = value;
                    OnPropertyChanged(nameof(Increment));
                    UpdateSelectedIndex();
                }
            }
        }

        public override int SelectedIndex
        {
            get => base.SelectedIndex;
            set
            {
                int newIndex = MathHelper.Clamp(value, 0, MathHelper.Max(0, GetOptionsCount() - 1));
                if (newIndex != SelectedIndex)
                {
                    currentValue = Minimum + newIndex * Increment;
                    base.SelectedIndex = newIndex;
                }
            }
        }

        public bool ThousandsSeparator
        {
            get => thousandsSeparator;
            set
            {
                if(value != thousandsSeparator)
                {
                    thousandsSeparator = value;
                    OnPropertyChanged(nameof(ThousandsSeparator));
                }
            }
        }

        public bool Hexadecimal
        {
            get => hexadecimal;
            set
            {
                if (value != hexadecimal)
                {
                    hexadecimal = value;
                    OnPropertyChanged(nameof(Hexadecimal));
                }
            }
        }

        public int DecimalPlaces
        {
            get => decimalPlaces;
            set
            {
                if (value != decimalPlaces)
                {
                    decimalPlaces = value;
                    OnPropertyChanged(nameof(DecimalPlaces));
                }
            }
        }

        public MenuItemNumericScroller(string text, string description) : base(text, description)
        {
            SelectedIndex = GetOptionsCount() / 2;
        }

        public MenuItemNumericScroller(string text) : this(text, String.Empty)
        {
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

        protected internal override bool OnMoveLeft()
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

            return base.OnMoveLeft();
        }

        protected internal override bool OnMoveRight()
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

            return base.OnMoveRight();
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(SelectedIndex))
            {
                OnPropertyChanged(nameof(Value));
            }
        }
    }
}

