namespace RAGENativeUI.Elements
{
    using System;

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

        public T Value
        {
            get => selectedValue.Value;
            set
            {
                if (!value.Equals(selectedValue.Value))
                {
                    if (value.CompareTo(minimum.Value) < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), "value < minimum");
                    }

                    if (value.CompareTo(maximum.Value) > 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), "value > maximum");
                    }

                    Index = IndexOfValue(value);
                }
            }
        }

        public T Minimum
        {
            get => minimum;
            set
            {
                if (!value.Equals(minimum.Value))
                {
                    minimum = value;
                    if (minimum.Value.CompareTo(maximum.Value) > 0)
                    {
                        maximum = minimum;
                    }

                    SyncSelectedIndexToValue();
                }
            }
        }

        public T Maximum
        {
            get => maximum;
            set
            {
                if (!value.Equals(maximum.Value))
                {
                    maximum = value;
                    if (minimum.Value.CompareTo(maximum.Value) > 0)
                    {
                        minimum = maximum;
                    }

                    SyncSelectedIndexToValue();
                }
            }
        }

        public T Step
        {
            get => step;
            set
            {
                if (!value.Equals(step.Value))
                {
                    if (step.Value.CompareTo(default) < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), "negative");
                    }

                    step = value;

                    SyncSelectedIndexToValue();
                }
            }
        }

        /// <inheritdoc/>
        public override string OptionText => selectedValueText;
        
        /// <inheritdoc/>
        public override int OptionCount => ((maximum - minimum) / step).CastToInt() + 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIMenuNumericScrollerItem{T}"/> class.
        /// </summary>
        /// <param name="text">The <see cref="UIMenuNumericScrollerItem{T}"/>'s label.</param>
        /// <param name="description">The <see cref="UIMenuNumericScrollerItem{T}"/>'s description.</param>
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
            selectedValueText = n.Value.ToString(); // TODO: allow custom formatting
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

        private int IndexOfValue(T value)
        {
            return ((value - minimum) / step).CastToInt();
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
            public T Value { get; set; }

            public Number(T value) => Value = value;

            public int CastToInt()
            {
                if (typeof(T) == typeof(sbyte)) return (int)(sbyte)(object)Value;
                if (typeof(T) == typeof(byte)) return (int)(byte)(object)Value;
                if (typeof(T) == typeof(short)) return (int)(short)(object)Value;
                if (typeof(T) == typeof(ushort)) return (int)(ushort)(object)Value;
                if (typeof(T) == typeof(int)) return (int)(int)(object)Value;
                if (typeof(T) == typeof(uint)) return (int)(uint)(object)Value;
                if (typeof(T) == typeof(long)) return (int)(long)(object)Value;
                if (typeof(T) == typeof(ulong)) return (int)(ulong)(object)Value;
                if (typeof(T) == typeof(float)) return (int)(float)(object)Value;
                if (typeof(T) == typeof(double)) return (int)(double)(object)Value;
                if (typeof(T) == typeof(decimal)) return (int)(decimal)(object)Value;

                throw new InvalidOperationException();
            }

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

            public static implicit operator Number(T value) => new Number(value);
            public static implicit operator T(Number number) => number.Value;

            public static Number operator +(Number a, Number b)
            {
                if (typeof(T) == typeof(sbyte)) return (T)(object)((sbyte)(object)a.Value + (sbyte)(object)b.Value);
                if (typeof(T) == typeof(byte)) return (T)(object)((byte)(object)a.Value + (byte)(object)b.Value);
                if (typeof(T) == typeof(short)) return (T)(object)((short)(object)a.Value + (short)(object)b.Value);
                if (typeof(T) == typeof(ushort)) return (T)(object)((ushort)(object)a.Value + (ushort)(object)b.Value);
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
                if (typeof(T) == typeof(sbyte)) return (T)(object)((sbyte)(object)a.Value - (sbyte)(object)b.Value);
                if (typeof(T) == typeof(byte)) return (T)(object)((byte)(object)a.Value - (byte)(object)b.Value);
                if (typeof(T) == typeof(short)) return (T)(object)((short)(object)a.Value - (short)(object)b.Value);
                if (typeof(T) == typeof(ushort)) return (T)(object)((ushort)(object)a.Value - (ushort)(object)b.Value);
                if (typeof(T) == typeof(int)) return (T)(object)((int)(object)a.Value - (int)(object)b.Value);
                if (typeof(T) == typeof(uint)) return (T)(object)((uint)(object)a.Value - (uint)(object)b.Value);
                if (typeof(T) == typeof(long)) return (T)(object)((long)(object)a.Value - (long)(object)b.Value);
                if (typeof(T) == typeof(ulong)) return (T)(object)((ulong)(object)a.Value - (ulong)(object)b.Value);
                if (typeof(T) == typeof(float)) return (T)(object)((float)(object)a.Value - (float)(object)b.Value);
                if (typeof(T) == typeof(double)) return (T)(object)((double)(object)a.Value - (double)(object)b.Value);
                if (typeof(T) == typeof(decimal)) return (T)(object)((decimal)(object)a.Value - (decimal)(object)b.Value);

                throw new InvalidOperationException();
            }

            public static Number operator*(Number a, Number b)
            {
                if (typeof(T) == typeof(sbyte)) return (T)(object)((sbyte)(object)a.Value * (sbyte)(object)b.Value);
                if (typeof(T) == typeof(byte)) return (T)(object)((byte)(object)a.Value * (byte)(object)b.Value);
                if (typeof(T) == typeof(short)) return (T)(object)((short)(object)a.Value * (short)(object)b.Value);
                if (typeof(T) == typeof(ushort)) return (T)(object)((ushort)(object)a.Value * (ushort)(object)b.Value);
                if (typeof(T) == typeof(int)) return (T)(object)((int)(object)a.Value * (int)(object)b.Value);
                if (typeof(T) == typeof(uint)) return (T)(object)((uint)(object)a.Value * (uint)(object)b.Value);
                if (typeof(T) == typeof(long)) return (T)(object)((long)(object)a.Value * (long)(object)b.Value);
                if (typeof(T) == typeof(ulong)) return (T)(object)((ulong)(object)a.Value * (ulong)(object)b.Value);
                if (typeof(T) == typeof(float)) return (T)(object)((float)(object)a.Value * (float)(object)b.Value);
                if (typeof(T) == typeof(double)) return (T)(object)((double)(object)a.Value * (double)(object)b.Value);
                if (typeof(T) == typeof(decimal)) return (T)(object)((decimal)(object)a.Value * (decimal)(object)b.Value);

                throw new InvalidOperationException();
            }

            public static Number operator/(Number a, Number b)
            {
                if (typeof(T) == typeof(sbyte)) return (T)(object)((sbyte)(object)a.Value / (sbyte)(object)b.Value);
                if (typeof(T) == typeof(byte)) return (T)(object)((byte)(object)a.Value / (byte)(object)b.Value);
                if (typeof(T) == typeof(short)) return (T)(object)((short)(object)a.Value / (short)(object)b.Value);
                if (typeof(T) == typeof(ushort)) return (T)(object)((ushort)(object)a.Value / (ushort)(object)b.Value);
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
        }
    }
}
