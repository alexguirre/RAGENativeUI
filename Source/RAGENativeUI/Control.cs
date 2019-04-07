namespace RAGENativeUI
{
#if RPH1
    extern alias rph1;
    using GameControl = rph1::Rage.GameControl;
#else
    /** REDACTED **/
#endif

    using System;
    using System.Linq;
    using System.Collections.ObjectModel;
    using System.Windows.Forms;

    public class Control
    {
        // used if RepeatSteps is null or empty
        private const uint DefaultRepeatInterval = 180;
        private const uint DefaultHeldDownTime = 1000;

        public Keys? Key { get; set; }
        //public ControllerButtons? Button { get; set; }
        public GameControl? NativeControl { get; set; }
        public RepeatStep[] RepeatSteps { get; set; } // TODO: ensure that RepeatSteps is ordered by RepeatStep.Time
        public uint HeldDownTime { get; set; } = DefaultHeldDownTime;

        private uint repeatStartTime;
        private int currentRepeatStepIndex;
        private uint repeatInterval;
        private uint nextRepeatTime;

        private uint heldStartTime;
        private bool heldDownTimeReached;

        public Control(Keys? key = null, /*ControllerButtons? button = null,*/ GameControl? nativeControl = null)
        {
            Key = key;
            //Button = button;
            NativeControl = nativeControl;
            RepeatSteps = DefaultRepeatSteps.ToArray();
        }

        public Control(GameControl? nativeControl = null) : this(null, /*null,*/ nativeControl)
        {
        }

        /// <summary>
        /// Checks whether the <see cref="Control"/> was just pressed.
        /// </summary>
        /// <param name="repeat">
        /// <see langword="true"/> to return <see langword="true"/> on every interval defined by <see cref="RepeatSteps"/>,
        /// not only if the <see cref="Control"/> was just pressed; otherwise, <see langword="false"/>.
        /// </param>
        /// <returns><see langword="true"/> is the <see cref="Control"/> was just pressed; otherwise, <see langword="false"/>.</returns>
        public bool WasJustPressed(bool repeat = false)
        {
            if (repeat)
            {
                return WasJustPressedRepeated();
            }
            else
            {
                if (Key.HasValue && RPH.Game.WasKeyJustPressed(Key.Value))
                    return true;

                //if (Button.HasValue && Game.IsControllerButtonDown(Button.Value))
                //    return true;

                if (NativeControl.HasValue && RPH.Game.WasControlActionJustPressed(0, NativeControl.Value, true))
                    return true;

                ResetRepeat();
                return false;
            }
        }

        /// <summary>
        /// Checks whether the <see cref="Control"/> is currently down.
        /// </summary>
        /// <returns><see langword="true"/> is the <see cref="Control"/> is currently down; otherwise, <see langword="false"/>.</returns>
        public bool IsDown()
        {
            if (Key.HasValue && RPH.Game.IsKeyDown(Key.Value))
                return true;

            //if (Button.HasValue && Game.IsControllerButtonDownRightNow(Button.Value))
            //    return true;

            if (NativeControl.HasValue && RPH.Game.IsControlActionDown(0, NativeControl.Value, true))
                return true;

            ResetRepeat();
            return false;
        }

        public bool IsHeldDown() => IsHeldDown(out _);
        public bool IsHeldDown(out float percentage)
        {
            if (IsDown())
            {
                if (heldStartTime == 0)
                {
                    heldStartTime = RPH.Game.GameTime;
                }

                if (!heldDownTimeReached)
                {
                    uint currentTime = RPH.Game.GameTime;
                    percentage = RPH.MathHelper.Clamp((float)(currentTime - heldStartTime) / HeldDownTime, 0.0f, 1.0f);
                    heldDownTimeReached = currentTime >= (heldStartTime + HeldDownTime);
                    return heldDownTimeReached;
                }
                else
                {
                    percentage = 1.0f;
                    return false;
                }

            }

            heldStartTime = 0;
            heldDownTimeReached = false;
            percentage = 0.0f;
            return false;
        }

        private bool WasJustPressedRepeated()
        {
            if(RPH.Game.GameTime <= nextRepeatTime)
            {
                if(!IsDown())
                {
                    ResetRepeat();
                }

                return false;
            }

            if (IsDown())
            {
                UpdateRepeat();
                return true;
            }

            ResetRepeat();
            return false;
        }

        private void UpdateRepeat()
        {
            if (repeatStartTime == 0)
            {
                repeatStartTime = RPH.Game.GameTime;
            }

            if (RepeatSteps != null && RepeatSteps.Length > 0)
            {
                if (currentRepeatStepIndex != RepeatSteps.Length - 1)
                {
                    int newIndex = currentRepeatStepIndex + 1;
                    if ((RPH.Game.GameTime - repeatStartTime) >= RepeatSteps[newIndex].Time)
                        currentRepeatStepIndex = newIndex;
                }

                repeatInterval = RepeatSteps[currentRepeatStepIndex].Interval;
            }
            else
            {
                repeatInterval = DefaultRepeatInterval;
            }

            nextRepeatTime = RPH.Game.GameTime + repeatInterval;
        }

        private void ResetRepeat()
        {
            repeatStartTime = 0;
            currentRepeatStepIndex = 0;
            repeatInterval = 0;
            nextRepeatTime = 0;
        }

        /// <summary>
        /// Specifies the default <see cref="RepeatStep"/>s for the <see cref="RepeatSteps"/> property.
        /// </summary>
        public static readonly ReadOnlyCollection<RepeatStep> DefaultRepeatSteps = Array.AsReadOnly(new[]
        {
            new RepeatStep(0, 180),
            new RepeatStep(2000, 110),
            new RepeatStep(6000, 50),
        });

        /// <summary>
        /// Defines an <see cref="Interval"/> for a <see cref="Control"/> press repeats after
        /// the <see cref="Control"/> was down for a specific <see cref="Time"/>.
        /// </summary>
        public struct RepeatStep
        {
            /// <summary>
            /// Gets or sets the number of milliseconds that a <see cref="Control"/> must be down to
            /// use this <see cref="RepeatStep"/>.
            /// </summary>
            /// <value>The number of milliseconds that a <see cref="Control"/> must be down to use this <see cref="RepeatStep"/>.</value>
            public uint Time { get; }
            /// <summary>
            /// Gets or sets the number of milliseconds between a <see cref="Control"/> press repeats.
            /// </summary>
            /// <value>The number of milliseconds between a <see cref="Control"/> press repeats.</value>
            public uint Interval { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="RepeatStep"/> structure with the specified
            /// time and interval.
            /// </summary>
            /// <param name="time">The number of milliseconds that a <see cref="Control"/> must be down to use this <see cref="RepeatStep"/>.</param>
            /// <param name="interval">The number of milliseconds between a <see cref="Control"/> press repeats.</param>
            public RepeatStep(uint time, uint interval)
            {
                Time = time;
                Interval = interval;
            }
        }
    }
}

