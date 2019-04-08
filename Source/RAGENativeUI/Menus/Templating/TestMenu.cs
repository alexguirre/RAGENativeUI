namespace RAGENativeUI.Menus.Templating
{
    extern alias rph1;
    using GameControl = rph1::Rage.GameControl;

    [Menu(Title = "My Menu", Subtitle = "Subtitle for my menu")]
    public class TestMenu : MenuTemplate
    {
        private bool doSomething;
        private bool doSomeOtherThing;
        private float floatValue;
        private int intValue;
        private GameControl enumValue;

        [MenuItemCheckbox(Text = "Do Something?", Description = "Whether to do something or not.")]
        public bool DoSomething
        {
            get => doSomething;
            set => SetProperty(ref doSomething, value);
        }

        [MenuItemCheckbox(Text = "Do Some Other Thing?", Description = "Whether to do some other thing or not.")]
        public bool DoSomeOtherThing
        {
            get => doSomeOtherThing;
            set => SetProperty(ref doSomeOtherThing, value);
        }

        [MenuItemNumericScroller(Text = "Float Value")]
        public float FloatValue
        {
            get => floatValue;
            set => SetProperty(ref floatValue, value);
        }

        [MenuItemNumericScroller(Text = "Int Value as Dec", Increment = 1.0, DecimalPlaces = 0)]
        public int IntValue
        {
            get => intValue;
            set => SetProperty(ref intValue, value, nameof(IntValue), nameof(IntValueAsHex));
        }

        [MenuItemNumericScroller(Text = "Int Value as Hex", Increment = 1.0, DecimalPlaces = 0, Hexadecimal = true)]
        private int IntValueAsHex
        {
            get => IntValue;
            set => IntValue = value;
        }

        [MenuItemEnumScroller(Text = "Enum Value")]
        public GameControl EnumValue
        {
            get => enumValue;
            set => SetProperty(ref enumValue, value);
        }

        public void Display()
        {
            rph1::Rage.Game.DisplaySubtitle(
                $"DoSomething:{DoSomething}~n~" +
                $"DoSomeOtherThing:{DoSomeOtherThing}~n~" +
                $"FloatValue:{FloatValue}~n~" +
                $"IntValue:{IntValue}~n~" +
                $"EnumValue:{EnumValue}"
                );
        }
    }
}

