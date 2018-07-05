namespace RAGENativeUI.Scaleforms
{
    using System;
    using System.Drawing;

    using Rage;
    using Rage.Native;
    
    public class InstructionalButtons : Scaleform
    {
        private InstructionalButtonsSlots slots;

        /// <exception cref="ArgumentNullException">When setting the property to a null value.</exception>
        public InstructionalButtonsSlots Slots { get { return slots; } set { Throw.IfNull(value, nameof(value)); slots = value; } }
        public LayoutType Layout { get; set; } = LayoutType.Horizontal;
        public Color BackgroundColor { get; set; } = Color.FromArgb(80, 0, 0, 0);

        public InstructionalButtons() : base("instructional_buttons")
        {
            Slots = new InstructionalButtonsSlots();
        }
        
        private void Update()
        {
            CallMethod("SET_DATA_SLOT_EMPTY");

            CallMethod("TOGGLE_MOUSE_BUTTONS", false);

            int j = 0;
            for (int i = 0; i < Slots.Count; i++)
            {
                InstructionalButtonSlot s = Slots[i];
                if (s.IsVisible)
                {
                    CallMethod("SET_DATA_SLOT", j++, s.GetButtonId(), s.Label);
                }
            }

            CallMethod("DRAW_INSTRUCTIONAL_BUTTONS", (int)Layout);

            CallMethod("SET_BACKGROUND_COLOUR", BackgroundColor.R, BackgroundColor.G, BackgroundColor.B, BackgroundColor.A);
        }

        public override void Draw(Color color)
        {
            Update();
            base.Draw(color);
        }

        public override void Draw(ScreenRectangle rectangle, Color color)
        {
            Update();
            base.Draw(rectangle, color);
        }

        public override void Draw3D(Vector3 position, Rotator rotation, Vector3 scale)
        {
            Update();
            base.Draw3D(position, rotation, scale);
        }

        public int AddSlot(InstructionalButtonSlot slot)
        {
            int slotIndex = Slots.Count;
            Slots.Add(slot);
            return slotIndex;
        }

        public int AddSlot(string label, int index, GameControl control) => AddSlot(new InstructionalButtonControlSlot(label, index, control));
        public int AddSlot(string label, GameControl control) => AddSlot(new InstructionalButtonControlSlot(label, control));
        public int AddSlot(string label, string text, InstructionalButtonStyle style) => AddSlot(new InstructionalButtonTextSlot(label, text, style));
        public int AddSlot(string label, string text) => AddSlot(new InstructionalButtonTextSlot(label, text));


        public class InstructionalButtonsSlots : BaseCollection<InstructionalButtonSlot>
        {
        }

        public enum LayoutType
        {
            Horizontal = -1,
            Vertical = 1,
        }
    }

    public abstract class InstructionalButtonSlot
    {
        public bool IsVisible { get; set; } = true;
        public string Label { get; set; }

        public InstructionalButtonSlot(string label)
        {
            Throw.IfNull(label, nameof(label));

            Label = label;
        }

        public abstract string GetButtonId();
    }

    public class InstructionalButtonControlSlot : InstructionalButtonSlot
    {
        public int InputGroup { get; set; }
        public GameControl Control { get; set; }

        public InstructionalButtonControlSlot(string label, int inputGroup, GameControl control) : base(label)
        {
            InputGroup = inputGroup;
            Control = control;
        }

        public InstructionalButtonControlSlot(string label, GameControl control) : this(label, 0, control)
        {
        }

        public override string GetButtonId()
        {
            return N.GetControlInstructionalButton(InputGroup, (int)Control, 0);
        }
    }

    public class InstructionalButtonTextSlot : InstructionalButtonSlot
    {
        public string Text { get; set; }
        public InstructionalButtonStyle Style { get; set; }

        public InstructionalButtonTextSlot(string label, string text, InstructionalButtonStyle style) : base(label)
        {
            Throw.IfNull(text, nameof(text));

            Text = text;
            Style = style;
        }

        public InstructionalButtonTextSlot(string label, string text) : this(label, text, InstructionalButtonStyle.Key)
        {
        }

        public override string GetButtonId()
        {
            return GetStyleButtonIdPrefix(Style) + Text;
        }

        public static string GetStyleButtonIdPrefix(InstructionalButtonStyle style)
        {
            switch (style)
            {
                case InstructionalButtonStyle.Key: return "t_";
                case InstructionalButtonStyle.MultiCharKey: return "T_";
                case InstructionalButtonStyle.WideKey: return "w_";
            }

            throw new ArgumentOutOfRangeException(nameof(style));
        }
    }

    public enum InstructionalButtonStyle
    {
        Key,
        MultiCharKey,
        WideKey,
    }
}

