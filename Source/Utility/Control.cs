namespace RAGENativeUI.Utility
{
    using System.Windows.Forms;

    using Rage;
    using Rage.Native;

    public class Control
    {
        public Keys? Key { get; set; }
        public ControllerButtons? Button { get; set; }
        public GameControl? NativeControl { get; set; }
        public uint HeldCooldown { get; set; } = 200;

        protected uint NextHeldTime { get; set; }

        public Control(Keys? key = null, ControllerButtons? button = null, GameControl? nativeControl = null)
        {
            Key = key;
            Button = button;
            NativeControl = nativeControl;
        }

        public Control(GameControl? nativeControl = null) : this(null, null, nativeControl)
        {
        }

        public bool IsJustPressed()
        {
            if (Key.HasValue && Game.IsKeyDown(Key.Value))
                return true;

            if (Button.HasValue && Game.IsControllerButtonDown(Button.Value))
                return true;

            if (NativeControl.HasValue && (NativeFunction.Natives.IsControlJustPressed<bool>(0, (int)NativeControl.Value) || NativeFunction.Natives.IsDisabledControlJustPressed<bool>(0, (int)NativeControl.Value)))
                return true;

            NextHeldTime = 0;
            return false;
        }

        public bool IsPressed()
        {
            if (Key.HasValue && Game.IsKeyDownRightNow(Key.Value))
                return true;

            if (Button.HasValue && Game.IsControllerButtonDownRightNow(Button.Value))
                return true;

            if (NativeControl.HasValue && (NativeFunction.Natives.IsControlPressed<bool>(0, (int)NativeControl.Value) || NativeFunction.Natives.IsDisabledControlPressed<bool>(0, (int)NativeControl.Value)))
                return true;

            NextHeldTime = 0;
            return false;
        }

        public bool IsHeld()
        {
            if(Game.GameTime <= NextHeldTime)
            {
                return false;
            }

            if (IsPressed())
            {
                NextHeldTime = Game.GameTime + HeldCooldown;
                return true;
            }

            NextHeldTime = 0;
            return false;
        }
    }
}

