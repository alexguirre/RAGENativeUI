namespace RAGENativeUI.Elements
{
    using System;

    using Rage;
    using Rage.Native;

    public class InstructionalButtons
    {
        public Scaleform Scaleform { get; }
        public InstructionalButtonsCollection Buttons { get; }

        public InstructionalButtons()
        {
            Scaleform = new Scaleform(0);
            Scaleform.Load("instructional_buttons");

            Buttons = new InstructionalButtonsCollection();
            Buttons.ItemAdded += (c, i) => Update();
            Buttons.ItemRemoved += (c, i) => Update();
            Buttons.ItemModified += (c, o, n) => Update();
            Buttons.Cleared += (c) => Update();
        }

        public void Draw()
        {
            Scaleform.Render2D();
        }

        public void Update()
        {
            Scaleform.CallFunction("CLEAR_ALL");
            Scaleform.CallFunction("TOGGLE_MOUSE_BUTTONS", 0);
            Scaleform.CallFunction("CREATE_CONTAINER");

            int dateSlotIndex = 0;
            for (int i = 0; i < Buttons.Count; i++)
            {
                InstructionalButton b = Buttons[i];
                if (b.CanBeDisplayed == null || b.CanBeDisplayed.Invoke(b))
                {
                    Scaleform.CallFunction("SET_DATA_SLOT", dateSlotIndex++, b.GetButtonId(), b.Text);
                }
            }
            Scaleform.CallFunction("DRAW_INSTRUCTIONAL_BUTTONS", -1);
        }


        public class InstructionalButtonsCollection : BaseCollection<InstructionalButton>
        {
        }
    }

    public class InstructionalButton
    {
        public string Text { get; set; }

        public Predicate<InstructionalButton> CanBeDisplayed { get; set; }

        [Obsolete("ItemBind is obsolete. Check BindToItem(UIMenuItem) for details.")]
        public UIMenuItem ItemBind { get; private set; }

        private readonly string buttonString;
        private readonly GameControl buttonControl;
        private readonly bool usingControls;

        /// <summary>
        /// Add a dynamic button to the instructional buttons array.
        /// Changes whether the controller is being used and changes depending on keybinds.
        /// </summary>
        /// <param name="control">Rage.GameControl that gets converted into a button.</param>
        /// <param name="text">Help text that goes with the button.</param>
        public InstructionalButton(GameControl control, string text)
        {
            Text = text;
            buttonControl = control;
            usingControls = true;
        }

        /// <summary>
        /// Adds a keyboard button to the instructional buttons array.
        /// </summary>
        /// <param name="keystring">Custom keyboard button, like "I", or "O", or "F5".</param>
        /// <param name="text">Help text that goes with the button.</param>
        public InstructionalButton(string keystring, string text)
        {
            Text = text;
            buttonString = keystring;
            usingControls = false;
        }

        /// <summary>
        /// Bind this button to an item, so it's only shown when that item is selected.
        /// </summary>
        /// <param name="item">Item to bind to.</param>
        [Obsolete("BindToItem(UIMenuItem) is obsolete. Use CanBeDisplayed predicate instead, checking for UIMenuItem.Selected.")]
        public void BindToItem(UIMenuItem item)
        {
            ItemBind = item;
            CanBeDisplayed = (i) => item.Selected;
        }

        public string GetButtonId()
        {
            return usingControls ? GetButtonId(buttonControl) : GetButtonId(buttonString);
        }

        public static string GetButtonId(GameControl control)
        {
            return NativeFunction.Natives.GET_CONTROL_INSTRUCTIONAL_BUTTON<string>(2, (int)control, 0);
        }

        public static string GetButtonId(string keyString)
        {
            return "t_" + keyString;
        }
    }
}

