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
                    Scaleform.CallFunction("SET_DATA_SLOT", dateSlotIndex++, b.GetButtonId(), b.Text ?? "");
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
        /// <summary>
        /// Gets or sets the text displayed next to the button.
        /// <para>
        /// If this <see cref="InstructionalButton"/> is contained in a <see cref="InstructionalButtons"/> scaleform, 
        /// <see cref="InstructionalButtons.Update"/> needs to be called to reflect the changes made.
        /// </para>
        /// </summary>
        /// <value>
        /// A <see cref="String"/> representing the text displayed next to the button.
        /// </value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the text displayed inside the button, mainly for displaying custom keyboard bindings, like "I", or "O", or "F5".
        /// <para>
        /// If <c>null</c>, <see cref="ButtonControl"/> is used instead.
        /// </para>
        /// <para>
        /// If this <see cref="InstructionalButton"/> is contained in a <see cref="InstructionalButtons"/> scaleform, 
        /// <see cref="InstructionalButtons.Update"/> needs to be called to reflect the changes made.
        /// </para>
        /// </summary>
        /// <value>
        /// A <see cref="String"/> representing the text displayed inside the button.
        /// </value>
        public string ButtonText { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="GameControl"/> displayed inside the button, it changes depending on keybinds and whether the user is using the controller or the keyboard and mouse.
        /// <para>
        /// If <c>null</c> and <see cref="ButtonText"/> is also <c>null</c> an empty button is displayed.
        /// </para>
        /// <para>
        /// If this <see cref="InstructionalButton"/> is contained in a <see cref="InstructionalButtons"/> scaleform, 
        /// <see cref="InstructionalButtons.Update"/> needs to be called to reflect the changes made.
        /// </para>
        /// </summary>
        /// <value>
        /// A <see cref="GameControl"/> representing the binding displayed inside the button.
        /// </value>
        public GameControl? ButtonControl { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Predicate{InstructionalButton}"/> delegate that defines the conditions for this <see cref="InstructionalButton"/> to be visible.
        /// <para>
        /// If this <see cref="InstructionalButton"/> is contained in a <see cref="InstructionalButtons"/> scaleform, 
        /// this predicate is only evaluated when <see cref="InstructionalButtons.Update"/> is called.
        /// </para>
        /// </summary>
        /// <value>
        /// A <see cref="Predicate{InstructionalButton}"/> delegate that defines the conditions for this <see cref="InstructionalButton"/> to be visible.
        /// </value>
        public Predicate<InstructionalButton> CanBeDisplayed { get; set; }

        [Obsolete("ItemBind is obsolete. Check BindToItem(UIMenuItem) for details.")]
        public UIMenuItem ItemBind { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionalButton"/> class.
        /// </summary>
        /// <param name="control">The <see cref="GameControl"/> displayed inside the button, it changes depending on keybinds and whether the user is using the controller or the keyboard and mouse.</param>
        /// <param name="text">The text displayed next to the button.</param>
        public InstructionalButton(GameControl control, string text)
        {
            Text = text;
            ButtonControl = control;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstructionalButton"/> class.
        /// </summary>
        /// <param name="buttonText">The text displayed inside the button, mainly for displaying custom keyboard bindings, like "I", or "O", or "F5".</param>
        /// <param name="text">The text displayed next to the button.</param>
        public InstructionalButton(string buttonText, string text)
        {
            Text = text;
            ButtonText = buttonText;
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
            return ButtonText == null ? (ButtonControl.HasValue ? GetButtonId(ButtonControl.Value) : GetButtonId("")) : GetButtonId(ButtonText);
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
