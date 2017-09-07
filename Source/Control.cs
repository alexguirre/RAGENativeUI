namespace RAGENativeUI
{
    using System;
    using System.Linq;
    using System.Collections.ObjectModel;
    using System.Windows.Forms;

    using Rage;
    using Rage.Native;

    public class Control
    {
        // used if HeldTimeSteps is null or empty
        private const uint DefaultHeldCooldown = 180;

        public Keys? Key { get; set; }
        public ControllerButtons? Button { get; set; }
        public GameControl? NativeControl { get; set; }
        public TimeStep[] HeldTimeSteps { get; set; }

        private uint heldStartTime;
        private int currentHeldStepIndex;
        private uint heldCooldown;
        private uint nextHeldTime;

        public Control(Keys? key = null, ControllerButtons? button = null, GameControl? nativeControl = null)
        {
            Key = key;
            Button = button;
            NativeControl = nativeControl;
            HeldTimeSteps = DefaultHeldTimeSteps.ToArray();
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

            ResetHeld();
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

            ResetHeld();
            return false;
        }

        // returns true every X time(defined by the HeldTimeSteps) while the key/button/control is pressed, instead of returning true every tick like IsPressed
        // TODO: maybe rename IsHeld to better reflect its behaviour
        public bool IsHeld()
        {
            if(Game.GameTime <= nextHeldTime)
            {
                if(!IsPressed())
                {
                    ResetHeld();
                }

                return false;
            }

            if (IsPressed())
            {
                if (heldStartTime == 0)
                {
                    heldStartTime = Game.GameTime;
                }
                UpdateHeldStep();
                nextHeldTime = Game.GameTime + heldCooldown;
                return true;
            }

            ResetHeld();
            return false;
        }

        private void UpdateHeldStep()
        {
            if (HeldTimeSteps != null && heldStartTime != 0 && HeldTimeSteps.Length > 0)
            {
                if (currentHeldStepIndex != HeldTimeSteps.Length - 1)
                {
                    int newIndex = currentHeldStepIndex + 1;
                    if ((Game.GameTime - heldStartTime) >= HeldTimeSteps[newIndex].Time)
                        currentHeldStepIndex = newIndex;
                }

                heldCooldown = HeldTimeSteps[currentHeldStepIndex].HeldCooldown;
            }
            else
            {
                heldCooldown = DefaultHeldCooldown;
            }
        }

        private void ResetHeld()
        {
            heldStartTime = 0;
            currentHeldStepIndex = 0;
            nextHeldTime = 0;
        }

        public static readonly ReadOnlyCollection<TimeStep> DefaultHeldTimeSteps = Array.AsReadOnly(new[]
        {
            new TimeStep(0, 180),
            new TimeStep(2000, 110),
            new TimeStep(6000, 50),
        });

        public struct TimeStep
        {
            public uint Time { get; }
            public uint HeldCooldown { get; }

            public TimeStep(uint time, uint heldCooldown)
            {
                Time = time;
                HeldCooldown = heldCooldown;
            }
        }
    }
}

