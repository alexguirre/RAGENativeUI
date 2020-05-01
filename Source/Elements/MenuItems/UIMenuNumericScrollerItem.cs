namespace RAGENativeUI.Elements
{
    using System;

    /// <summary>
    /// Represents a scroller item that displays numeric values within a range.
    /// </summary>
    /// <typeparam name="T">
    /// The numeric type to use. Possible types are: <see cref="sbyte"/>, <see cref="byte"/>, <see cref="short"/>, <see cref="ushort"/>,
    /// <see cref="int"/>, <see cref="uint"/>, <see cref="long"/>, <see cref="ulong"/>, <see cref="float"/>, <see cref="double"/> and <see cref="decimal"/>.
    /// <para>
    /// If some other type that meets the constraints is used, a <see cref="InvalidOperationException"/> will be thrown at runtime.
    /// </para>
    /// </typeparam>
    public class UIMenuNumericScrollerItem<T> : UIMenuScrollerItem
        // As close as we can get to limit T to numeric types at compile time. If a type meets 
        // these constraints and is not a numeric type, an exception will be thrown at runtime
        where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
    {
        private Number selectedValue;
        private string selectedValueText;
        private Number step;
        private Number minimum;
        private Number maximum;
        private bool allowSync;
        private string format;
        private IFormatProvider formatProvider;

        /// <summary>
        /// Gets or sets the currently selected value. When changing its value, the new value is rounded to nearest possible
        /// value depending on the <see cref="Minimum"/>, <see cref="Maximum"/> and <see cref="Step"/> values.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <c>value</c> is less than <see cref="Minimum"/>.
        /// -or-
        /// <c>value</c> is greater than <see cref="Maximum"/>.
        /// </exception>
        public T Value
        {
            get => selectedValue;
            set
            {
                if (!value.Equals(selectedValue))
                {
                    if (value.CompareTo(minimum) < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} is less than {nameof(Minimum)}");
                    }

                    if (value.CompareTo(maximum) > 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} is greater than {nameof(Maximum)}");
                    }

                    Index = IndexOfValue(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum value of the numeric scroller, inclusive.
        /// </summary>
        public T Minimum
        {
            get => minimum;
            set
            {
                if (!value.Equals(minimum))
                {
                    minimum = value;
                    if (minimum.Value.CompareTo(maximum) > 0)
                    {
                        maximum = minimum;
                    }

                    SyncSelectedIndexToValue();
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum value of the numeric scroller, inclusive.
        /// </summary>
        public T Maximum
        {
            get => maximum;
            set
            {
                if (!value.Equals(maximum))
                {
                    maximum = value;
                    if (maximum.Value.CompareTo(minimum) < 0)
                    {
                        minimum = maximum;
                    }

                    SyncSelectedIndexToValue();
                }
            }
        }

        /// <summary>
        /// Gets or sets the value to increment or decrement when scrolling.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <c>value</c> is negative or zero.
        /// </exception>
        public T Step
        {
            get => step;
            set
            {
                if (!value.Equals(step))
                {
                    if (value.CompareTo(default) <= 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} is negative or zero");
                    }

                    step = value;

                    SyncSelectedIndexToValue();
                }
            }
        }

        /// <inheritdoc/>
        public override string OptionText => selectedValueText; // don't need to handle IsEmpty case, since it can never be empty, Min and Max are inclusive and always have some value

        /// <inheritdoc/>
        public override int OptionCount
            => (int)Math.Round((maximum.ToDecimal() - minimum.ToDecimal()) / step.ToDecimal()) + 1; // cast to decimal to avoid overflows on smaller numeric types

        // TODO: provide some sane default format for floating-point types?
        // otherwise their display can vary from "1" to "0.25" to "0.0100000000002183"
        /// <summary>
        /// Gets or sets the format to use for displaying the current value. If <c>null</c>, the default format defined for <typeparamref name="T"/> is used.
        /// </summary>
        public string Format
        {
            get => format;
            set
            {
                if (value != format)
                {
                    format = value;
                    SyncSelectedTextToValue();
                }
            }
        }

        /// <summary>
        /// Gets or sets the provider to use to format the current value. If <c>null</c>, the numeric format information from
        /// the current locale setting of the operating system is used.
        /// </summary>
        public IFormatProvider FormatProvider
        {
            get => formatProvider;
            set
            {
                if (value != formatProvider)
                {
                    formatProvider = value;
                    SyncSelectedTextToValue();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIMenuNumericScrollerItem{T}"/> class.
        /// </summary>
        /// <param name="text">The <see cref="UIMenuNumericScrollerItem{T}"/>'s label.</param>
        /// <param name="description">The <see cref="UIMenuNumericScrollerItem{T}"/>'s description.</param>
        /// <param name="minimum">The minimum value.</param>
        /// <param name="maximum">The maximum value.</param>
        /// <param name="step">The value to increment or decrement when scrolling.</param>
        public UIMenuNumericScrollerItem(string text, string description, T minimum, T maximum, T step) : base(text, description)
        {
            if (!Number.IsTypeValid())
            {
                throw new InvalidOperationException($"{typeof(T)} is not a numeric type");
            }

            allowSync = false;
            Step = step;
            Minimum = minimum;
            Maximum = maximum;
            allowSync = true;

            Index = OptionCount / 2;
        }

        /// <inheritdoc/>
        protected override void OnSelectedIndexChanged(int oldIndex, int newIndex)
        {
            SyncSelectedValueToIndex();

            base.OnSelectedIndexChanged(oldIndex, newIndex);
        }

        /// <summary>
        /// Sets <see cref="selectedValue"/> without trigering any event.
        /// </summary>
        private void SetSelectedValueRaw(Number n)
        {
            selectedValue = n;
            SyncSelectedTextToValue();
        }

        private void SyncSelectedValueToIndex()
        {
            if (!allowSync)
            {
                return;
            }

            SetSelectedValueRaw(Minimum + Number.FromInt(Index) * Step);
        }

        private void SyncSelectedIndexToValue()
        {
            if (!allowSync)
            {
                return;
            }

            Index = IndexOfValue(ClampValue(Value));
        }

        private void SyncSelectedTextToValue()
        {
            if (!allowSync)
            {
                return;
            }

            selectedValueText = selectedValue.Value.ToString(Format, FormatProvider);
        }

        private int IndexOfValue(T value)
        {
            // cast to decimal to avoid overflows on smaller numeric types
            return (int)Math.Round((new Number(value).ToDecimal() - minimum.ToDecimal()) / step.ToDecimal());
        }

        private T ClampValue(T value)
        {
            if (value.CompareTo(minimum.Value) < 0)
                value = minimum;

            if (value.CompareTo(maximum.Value) > 0)
                value = maximum;

            return value;
        }

        /// <summary>
        /// Helper struct to perform arithmetic operations independent of the generic type.
        /// Once JITted, it compiles down to pretty much the same code as using the type directly.
        /// </summary>
        private struct Number
        {
            public T Value { get; }

            public Number(T value) => Value = value;

            public static Number FromInt(int n)
            {
                if (typeof(T) == typeof(sbyte)) return (T)(object)(sbyte)n;
                if (typeof(T) == typeof(byte)) return (T)(object)(byte)n;
                if (typeof(T) == typeof(short)) return (T)(object)(short)n;
                if (typeof(T) == typeof(ushort)) return (T)(object)(ushort)n;
                if (typeof(T) == typeof(int)) return (T)(object)(int)n;
                if (typeof(T) == typeof(uint)) return (T)(object)(uint)n;
                if (typeof(T) == typeof(long)) return (T)(object)(long)n;
                if (typeof(T) == typeof(ulong)) return (T)(object)(ulong)n;
                if (typeof(T) == typeof(float)) return (T)(object)(float)n;
                if (typeof(T) == typeof(double)) return (T)(object)(double)n;
                if (typeof(T) == typeof(decimal)) return (T)(object)(decimal)n;

                throw new InvalidOperationException();
            }

            public decimal ToDecimal()
            {
                if (typeof(T) == typeof(sbyte)) return (sbyte)(object)Value;
                if (typeof(T) == typeof(byte)) return (byte)(object)Value;
                if (typeof(T) == typeof(short)) return (short)(object)Value;
                if (typeof(T) == typeof(ushort)) return (ushort)(object)Value;
                if (typeof(T) == typeof(int)) return (int)(object)Value;
                if (typeof(T) == typeof(uint)) return (uint)(object)Value;
                if (typeof(T) == typeof(long)) return (long)(object)Value;
                if (typeof(T) == typeof(ulong)) return (ulong)(object)Value;
                if (typeof(T) == typeof(float)) return (decimal)(float)(object)Value;
                if (typeof(T) == typeof(double)) return (decimal)(double)(object)Value;
                if (typeof(T) == typeof(decimal)) return (decimal)(object)Value;

                throw new InvalidOperationException();
            }

            public static implicit operator Number(T value) => new Number(value);
            public static implicit operator T(Number number) => number.Value;

            public static Number operator +(Number a, Number b)
            {
                if (typeof(T) == typeof(sbyte)) return (T)(object)(sbyte)((sbyte)(object)a.Value + (sbyte)(object)b.Value);
                if (typeof(T) == typeof(byte)) return (T)(object)(byte)((byte)(object)a.Value + (byte)(object)b.Value);
                if (typeof(T) == typeof(short)) return (T)(object)(short)((short)(object)a.Value + (short)(object)b.Value);
                if (typeof(T) == typeof(ushort)) return (T)(object)(ushort)((ushort)(object)a.Value + (ushort)(object)b.Value);
                if (typeof(T) == typeof(int)) return (T)(object)((int)(object)a.Value + (int)(object)b.Value);
                if (typeof(T) == typeof(uint)) return (T)(object)((uint)(object)a.Value + (uint)(object)b.Value);
                if (typeof(T) == typeof(long)) return (T)(object)((long)(object)a.Value + (long)(object)b.Value);
                if (typeof(T) == typeof(ulong)) return (T)(object)((ulong)(object)a.Value + (ulong)(object)b.Value);
                if (typeof(T) == typeof(float)) return (T)(object)((float)(object)a.Value + (float)(object)b.Value);
                if (typeof(T) == typeof(double)) return (T)(object)((double)(object)a.Value + (double)(object)b.Value);
                if (typeof(T) == typeof(decimal)) return (T)(object)((decimal)(object)a.Value + (decimal)(object)b.Value);

                throw new InvalidOperationException();
            }

            public static Number operator -(Number a, Number b)
            {
                if (typeof(T) == typeof(sbyte)) return (T)(object)(sbyte)((sbyte)(object)a.Value - (sbyte)(object)b.Value);
                if (typeof(T) == typeof(byte)) return (T)(object)(byte)((byte)(object)a.Value - (byte)(object)b.Value);
                if (typeof(T) == typeof(short)) return (T)(object)(short)((short)(object)a.Value - (short)(object)b.Value);
                if (typeof(T) == typeof(ushort)) return (T)(object)(ushort)((ushort)(object)a.Value - (ushort)(object)b.Value);
                if (typeof(T) == typeof(int)) return (T)(object)((int)(object)a.Value - (int)(object)b.Value);
                if (typeof(T) == typeof(uint)) return (T)(object)((uint)(object)a.Value - (uint)(object)b.Value);
                if (typeof(T) == typeof(long)) return (T)(object)((long)(object)a.Value - (long)(object)b.Value);
                if (typeof(T) == typeof(ulong)) return (T)(object)((ulong)(object)a.Value - (ulong)(object)b.Value);
                if (typeof(T) == typeof(float)) return (T)(object)((float)(object)a.Value - (float)(object)b.Value);
                if (typeof(T) == typeof(double)) return (T)(object)((double)(object)a.Value - (double)(object)b.Value);
                if (typeof(T) == typeof(decimal)) return (T)(object)((decimal)(object)a.Value - (decimal)(object)b.Value);

                throw new InvalidOperationException();
            }

            public static Number operator *(Number a, Number b)
            {
                if (typeof(T) == typeof(sbyte)) return (T)(object)(sbyte)((sbyte)(object)a.Value * (sbyte)(object)b.Value);
                if (typeof(T) == typeof(byte)) return (T)(object)(byte)((byte)(object)a.Value * (byte)(object)b.Value);
                if (typeof(T) == typeof(short)) return (T)(object)(short)((short)(object)a.Value * (short)(object)b.Value);
                if (typeof(T) == typeof(ushort)) return (T)(object)(ushort)((ushort)(object)a.Value * (ushort)(object)b.Value);
                if (typeof(T) == typeof(int)) return (T)(object)((int)(object)a.Value * (int)(object)b.Value);
                if (typeof(T) == typeof(uint)) return (T)(object)((uint)(object)a.Value * (uint)(object)b.Value);
                if (typeof(T) == typeof(long)) return (T)(object)((long)(object)a.Value * (long)(object)b.Value);
                if (typeof(T) == typeof(ulong)) return (T)(object)((ulong)(object)a.Value * (ulong)(object)b.Value);
                if (typeof(T) == typeof(float)) return (T)(object)((float)(object)a.Value * (float)(object)b.Value);
                if (typeof(T) == typeof(double)) return (T)(object)((double)(object)a.Value * (double)(object)b.Value);
                if (typeof(T) == typeof(decimal)) return (T)(object)((decimal)(object)a.Value * (decimal)(object)b.Value);

                throw new InvalidOperationException();
            }

            public static Number operator /(Number a, Number b)
            {
                if (typeof(T) == typeof(sbyte)) return (T)(object)(sbyte)((sbyte)(object)a.Value / (sbyte)(object)b.Value);
                if (typeof(T) == typeof(byte)) return (T)(object)(byte)((byte)(object)a.Value / (byte)(object)b.Value);
                if (typeof(T) == typeof(short)) return (T)(object)(short)((short)(object)a.Value / (short)(object)b.Value);
                if (typeof(T) == typeof(ushort)) return (T)(object)(ushort)((ushort)(object)a.Value / (ushort)(object)b.Value);
                if (typeof(T) == typeof(int)) return (T)(object)((int)(object)a.Value / (int)(object)b.Value);
                if (typeof(T) == typeof(uint)) return (T)(object)((uint)(object)a.Value / (uint)(object)b.Value);
                if (typeof(T) == typeof(long)) return (T)(object)((long)(object)a.Value / (long)(object)b.Value);
                if (typeof(T) == typeof(ulong)) return (T)(object)((ulong)(object)a.Value / (ulong)(object)b.Value);
                if (typeof(T) == typeof(float)) return (T)(object)((float)(object)a.Value / (float)(object)b.Value);
                if (typeof(T) == typeof(double)) return (T)(object)((double)(object)a.Value / (double)(object)b.Value);
                if (typeof(T) == typeof(decimal)) return (T)(object)((decimal)(object)a.Value / (decimal)(object)b.Value);

                throw new InvalidOperationException();
            }

            public static bool IsTypeValid()
            {
                return typeof(T) == typeof(sbyte) ||
                       typeof(T) == typeof(byte) ||
                       typeof(T) == typeof(short) ||
                       typeof(T) == typeof(ushort) ||
                       typeof(T) == typeof(int) ||
                       typeof(T) == typeof(uint) ||
                       typeof(T) == typeof(long) ||
                       typeof(T) == typeof(ulong) ||
                       typeof(T) == typeof(float) ||
                       typeof(T) == typeof(double) ||
                       typeof(T) == typeof(decimal);
            }

            public override string ToString() => Value.ToString();
        }
    }
}
