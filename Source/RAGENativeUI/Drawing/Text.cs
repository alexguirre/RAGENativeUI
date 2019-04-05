namespace RAGENativeUI.Drawing
{
#if RPH1
    extern alias rph1;
    using Vector2 = rph1::Rage.Vector2;
#else
    /** REDACTED **/
#endif

    using System;
    using System.Collections.Generic;

    public class Text
    {
        private string label;

        public string Label
        {
            get => label;
            set
            {
                Throw.IfNull(value, nameof(value));

                if (value == label)
                    return;
                label = value;
            }
        }
        public IList<TextComponent> Components { get; } = new List<TextComponent>();
        public TextStyle Style { get; set; }

        public Text(string label)
        {
            Throw.IfNull(label, nameof(label));

            Label = label;
        }

        public Text() : this(String.Empty)
        {
        }

        public void SetUnformattedString(string value)
        {
            Throw.IfNull(value, nameof(value));

            const string ShortStringFormat = "STRING";
            const string LongStringFormat = "CELL_EMAIL_BCON";
            const int MaxSubstringLength = 99;

            Components.Clear();
            if (value.Length <= MaxSubstringLength)
            {
                Label = ShortStringFormat;
                Components.Add(new TextComponentString { Value = value });
            }
            else
            {
                Label = LongStringFormat;
                for (int i = 0; i < value.Length; i += MaxSubstringLength)
                {
                    string str = value.Substring(i, Math.Min(MaxSubstringLength, value.Length - i));
                    Components.Add(new TextComponentString { Value = str });
                }
            }
        }
        // TODO: see if there are more text commands worth adding
        public void DisplaySubtitle(int duration, bool drawImmediately)
        {
            N.BeginTextCommandPrint(Label);
            PushComponents();
            N.EndTextCommandPrint(duration, drawImmediately);
        }

        public void Display(Vector2 position)
        {
            Style?.Apply(position);
            N.BeginTextCommandDisplayText(Label);
            PushComponents();
            N.EndTextCommandDisplayText(position.X, position.Y);
        }

        public float CalculateWidth()
        {
            Style?.Apply();
            N.BeginTextCommandGetWidth(Label);
            PushComponents();
            return N.EndTextCommandGetWidth(true);
        }

        public int CalculateLineCount(Vector2 position)
        {
            Style?.Apply(position);
            N.BeginTextCommandGetLineCount(Label);
            PushComponents();
            return N.EndTextCommandGetLineCount(position.X, position.Y);
        }

        public void PushComponents()
        {
            if (Components.Count > 0)
            {
                foreach (TextComponent comp in Components)
                {
                    comp?.Push();
                }
            }
        }
    }
}

