namespace RAGENativeUI.Drawing
{
#if RPH1
    extern alias rph1;
    using Blip = rph1::Rage.Blip;
#else
    /** REDACTED **/
#endif

    using System;

    /// <summary>
    /// Base class for text components. 
    /// </summary>
    public abstract class TextComponent
    {
        public abstract void Push();
    }

    /// <summary>
    /// Component that replaces '~1~' from a label. 
    /// </summary>
    /// <seealso href="http://dev-c.com/nativedb/func/info/e7dcb5b874bcd96e#fntrg-e7dcb5b874bcd96e">
    /// ADD_TEXT_COMPONENT_FLOAT
    /// </seealso>
    public sealed class TextComponentFloat : TextComponent
    {
        public float Value { get; set; }
        public int DecimalPlaces { get; set; }

        public override void Push()
        {
            N.AddTextComponentFloat(Value, DecimalPlaces);
        }
    }

    /// <summary>
    /// Component that replaces '~a~' from a label. 
    /// </summary>
    /// <seealso href="http://dev-c.com/nativedb/func/info/0e4c749ff9de9cc4#fntrg-0e4c749ff9de9cc4">
    /// ADD_TEXT_COMPONENT_FORMATTED_INTEGER
    /// </seealso>
    public sealed class TextComponentFormattedInteger : TextComponent
    {
        public int Value { get; set; }
        public bool CommaSeparated { get; set; }

        public override void Push()
        {
            N.AddTextComponentFormattedInteger(Value, CommaSeparated);
        }
    }

    /// <summary>
    /// Component that replaces '~1~' from a label. 
    /// </summary>
    /// <seealso href="http://dev-c.com/nativedb/func/info/03b504cf259931bc#fntrg-03b504cf259931bc">
    /// ADD_TEXT_COMPONENT_INTEGER
    /// </seealso>
    public sealed class TextComponentInteger : TextComponent
    {
        public int Value { get; set; }

        public override void Push()
        {
            N.AddTextComponentInteger(Value);
        }
    }

    /// <summary>
    /// Component that replaces '~a~' from a label.
    /// </summary>
    /// <seealso href="http://dev-c.com/nativedb/func/info/1115f16b8ab9e8bf#fntrg-1115f16b8ab9e8bf">
    /// ADD_TEXT_COMPONENT_SUBSTRING_TIME
    /// </seealso>
    /// <seealso cref="TextComponentTimeOptions"/>
    public sealed class TextComponentTime : TextComponent
    {
        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        public TimeSpan Time { get; set; }
        /// <summary>
        /// Gets or sets how the time is displayed.
        /// </summary>
        public TextComponentTimeOptions Options { get; set; }

        public override void Push()
        {
            N.AddTextComponentSubstringTime(unchecked((int)Time.TotalMilliseconds), (int)Options);
        }
    }

    /// <summary>
    /// Specifies flags for <see cref="TextComponentTime"/> options.
    /// </summary>
    /// <seealso cref="TextComponentTime"/>
    [Flags]
    public enum TextComponentTimeOptions : int
    {
        /// <summary>
        /// Shows the milliseconds component.
        /// </summary>
        Milliseconds = 1 << 0,
        /// <summary>
        /// Shows the seconds component.
        /// </summary>
        Seconds = 1 << 1,
        /// <summary>
        /// Shows the minutes component.
        /// </summary>
        Minutes = 1 << 2,
        /// <summary>
        /// Shows the hours component.
        /// </summary>
        Hours = 1 << 3,
        /// <summary>
        /// Shows the days component.
        /// </summary>
        Days = 1 << 4,
        /// <summary>
        /// Removes the leading components that are equal to 0.
        /// </summary>
        NoLeadingEmptyComponents = 1 << 5,
        /// <summary>
        /// Removes the leading zeros of the highest unit shown.
        /// </summary>
        NoLeadingZeros = 1 << 6,
        /// <summary>
        /// Uses the time unit symbols ('d', 'h', 'm', 's' and 'ms') instead of colons (':')
        /// to separate each component. 
        /// </summary>
        UseUnitSymbols = 1 << 7,
        /// <summary>
        /// If <seealso cref="UseUnitSymbols"/> is set, removes the symbol of the smallest unit shown.
        /// </summary>
        NoTrailingSymbol = 1 << 8,
        /// <summary>
        /// If <seealso cref="Milliseconds"/> is set, replaces the milliseconds component with the hundredths of a second equivalent.
        /// </summary>
        HundredthsOfSecond = 1 << 9,
        /// <summary>
        /// If <seealso cref="Milliseconds"/> is set, replaces the milliseconds component with the tenths of a second equivalent.
        /// </summary>
        TenthsOfSecond = 1 << 10,
        /// <summary>
        /// If <seealso cref="Milliseconds"/> is set, replaces the colon (':') separating the milliseconds with a dot ('.').
        /// </summary>
        UseDotAsMillisecondsSeparator = 1 << 11,
    }

    /// <summary>
    /// Component that replaces '~a~' from a label.
    /// </summary>
    /// <seealso href="http://dev-c.com/nativedb/func/info/c63cd5d2920acbe7#fntrg-c63cd5d2920acbe7">
    /// ADD_TEXT_COMPONENT_SUBSTRING_TEXT_LABEL
    /// </seealso>
    public sealed class TextComponentLabel : TextComponent
    {
        public string LabelName { get; set; }

        public override void Push()
        {
            N.AddTextComponentSubstringTextLabel(LabelName);
        }
    }

    /// <summary>
    /// Component that replaces '~a~' from a label.
    /// </summary>
    /// <seealso href="http://dev-c.com/nativedb/func/info/17299b63c7683a2b#fntrg-17299b63c7683a2b">
    /// ADD_TEXT_COMPONENT_SUBSTRING_TEXT_LABEL_HASH_KEY
    /// </seealso>
    public sealed class TextComponentLabelHash : TextComponent
    {
        public uint LabelHash { get; set; }

        public override void Push()
        {
            N.AddTextComponentSubstringTextLabelHashKey(LabelHash);
        }
    }

    /// <summary>
    /// Component that replaces '~a~' from a label.
    /// </summary>
    /// <seealso href="http://dev-c.com/nativedb/func/info/80ead8e2e1d5d52e#fntrg-80ead8e2e1d5d52e">
    /// ADD_TEXT_COMPONENT_SUBSTRING_BLIP_NAME
    /// </seealso>
    public sealed class TextComponentBlipName : TextComponent
    {
        public Blip Blip { get; set; }

        public override void Push()
        {
            N.AddTextComponentSubstringBlipName(Blip ? Blip.Handle : 0);
        }
    }

    /// <summary>
    /// Component that replaces '~a~' from a label.
    /// </summary>
    /// <seealso href="http://dev-c.com/nativedb/func/info/6c188be134e074aa#fntrg-6c188be134e074aa">
    /// ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME
    /// </seealso>
    public sealed class TextComponentString : TextComponent
    {
        public string Value { get; set; }

        public override void Push()
        {
            N.AddTextComponentSubstringPlayerName(Value);
        }
    }

    public static class TextComponentsExtensions
    {
        private static T AddComponent<T>(this Text text, T comp) where T : TextComponent
        {
            text.Components.Add(comp);
            return comp;
        }

        public static TextComponentFloat AddComponentFloat(this Text text, float value, int decimalPlaces)
            => text.AddComponent(new TextComponentFloat { Value = value, DecimalPlaces = decimalPlaces });

        public static TextComponentFormattedInteger AddComponentFormattedInteger(this Text text, int value, bool commaSeparated)
            => text.AddComponent(new TextComponentFormattedInteger { Value = value, CommaSeparated = commaSeparated });

        public static TextComponentInteger AddComponentInteger(this Text text, int value)
            => text.AddComponent(new TextComponentInteger { Value = value });

        public static TextComponentTime AddComponentTime(this Text text, TimeSpan time, TextComponentTimeOptions options)
            => text.AddComponent(new TextComponentTime { Time = time, Options = options });

        public static TextComponentLabel AddComponentLabel(this Text text, string labelName)
            => text.AddComponent(new TextComponentLabel { LabelName = labelName });

        public static TextComponentLabelHash AddComponentLabel(this Text text, uint labelHash)
            => text.AddComponent(new TextComponentLabelHash { LabelHash = labelHash });

        public static TextComponentBlipName AddComponentBlipName(this Text text, Blip blip)
            => text.AddComponent(new TextComponentBlipName { Blip = blip });

        public static TextComponentString AddComponentString(this Text text, string value)
            => text.AddComponent(new TextComponentString { Value = value });
    }
}

