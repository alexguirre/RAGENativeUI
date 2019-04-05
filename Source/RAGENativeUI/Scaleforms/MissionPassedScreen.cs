namespace RAGENativeUI.Scaleforms
{
#if RPH1
    extern alias rph1;
    using GameFiber = rph1::Rage.GameFiber;
    using GameControl = rph1::Rage.GameControl;
#else
    /** REDACTED **/
#endif

    using System;
    using System.Drawing;
    using System.Collections.ObjectModel;

    using RAGENativeUI.Text;
    using RAGENativeUI.Drawing;

    public class MissionPassedScreen : IDisposable
    {
        private const int MaxItemsOnScreenCount = 14;
        private const int DefaultCompletionPercentage = 100;


        private readonly int instructionalButtonContinueIndex, instructionalButtonExpandIndex;
        private State currentState;
        private uint currentStateStartTime = RPH.Game.GameTime;
        private string title;
        private string subtitle;
        private MissionPassedScreenItemsCollection items;

        public event TypedEventHandler<MissionPassedScreen, EventArgs> Continued;

        public BigMessage BigMessage { get; private set; }
        public InstructionalButtons InstructionalButtons { get; private set; }

        public GameControl ContinueControl
        {
            get => (InstructionalButtons.Slots[instructionalButtonContinueIndex] as InstructionalButtonControlSlot).Control;
            set => (InstructionalButtons.Slots[instructionalButtonContinueIndex] as InstructionalButtonControlSlot).Control = value;
        }

        public GameControl ExpandControl
        {
            get => (InstructionalButtons.Slots[instructionalButtonExpandIndex] as InstructionalButtonControlSlot).Control;
            set => (InstructionalButtons.Slots[instructionalButtonExpandIndex] as InstructionalButtonControlSlot).Control = value;
        }
        
        /// <exception cref="ArgumentNullException">When setting the property to a null value.</exception>
        public string Title { get => title; set { Throw.IfNull(value, nameof(value)); title = value; } }
        /// <exception cref="ArgumentNullException">When setting the property to a null value.</exception>
        public string Subtitle { get => subtitle; set { Throw.IfNull(value, nameof(value)); subtitle = value; } }
        /// <exception cref="ArgumentNullException">When setting the property to a null value.</exception>
        public MissionPassedScreenItemsCollection Items { get => items; set { Throw.IfNull(value, nameof(value)); items = value; } }

        public bool IsVisible => CurrentState != State.None;

        public GameFiber Fiber { get; private set; }

        public bool IsCompletionVisible { get; set; }
        public Text CompletionText { get; } = new Text("MTPHPER"/*Completion*/) { Style = CreateCompletionTextStyle() };
        public Text CompletionPercentageText { get; } = new Text("PERCENTAGE"/*~1~%*/) { Style = CreateCompletionPercentageTextStyle() };
        public int CompletionPercentage
        {
            get
            {
                Text text = CompletionPercentageText;
                if (text.Components.Count > 0 && text.Components[0] is TextComponentInteger comp)
                {
                    return comp.Value;
                }
                else
                {
                    return Int32.MinValue; // TODO: throw exception?
                }
            }
            set
            {
                Text text = CompletionPercentageText;
                if (text.Components.Count > 0 && text.Components[0] is TextComponentInteger comp)
                {
                    comp.Value = value;
                }
                else
                {
                    // TODO: throw exception?
                }
            }
        }
        public Color CompletionMedalColor { get; set; } = HudColor.Gold.GetColor();

        public PostFxAnimation ShownEffect { get; set; } = PostFxAnimation.GetByName("SuccessNeutral");
        public FrontendSound ButtonPressedSound { get; set; } = new FrontendSound("HUD_FRONTEND_DEFAULT_SOUNDSET", "CONTINUE");

        private State CurrentState
        {
            get => currentState;
            set
            {
                if (value != currentState)
                {
                    currentState = value;
                    currentStateStartTime = RPH.Game.GameTime;
                }
            }
        }

        public MissionPassedScreen(string title, string subtitle)
        {
            Throw.IfNull(title, nameof(title));
            Throw.IfNull(subtitle, nameof(subtitle));

            BigMessage = new BigMessage();
            BigMessage.OutTransition = BigMessage.OutTransitionType.MoveUp;
            BigMessage.OutTransitionTime = 0.4f;
            InstructionalButtons = new InstructionalButtons();
            instructionalButtonContinueIndex = InstructionalButtons.AddSlot(RPH.Game.GetLocalizedString("ES_HELP"), GameControl.FrontendEndscreenAccept); // Continue
            instructionalButtonExpandIndex = InstructionalButtons.AddSlot(RPH.Game.GetLocalizedString("ES_XPAND"), GameControl.FrontendEndscreenExpand); // Expand

            Title = title;
            Subtitle = subtitle;
            Items = new MissionPassedScreenItemsCollection();

            CompletionPercentageText.AddComponentInteger(DefaultCompletionPercentage);

            Fiber = GameFiber.StartNew(UpdateLoop, $"RAGENativeUI - {nameof(MissionPassedScreen)} Update Fiber");

        }

        public void Show()
        {
            InstructionalButtons.Slots[instructionalButtonExpandIndex].IsVisible = true;
            BigMessage.CallMethodAndShow(Int32.MaxValue, "SHOW_MISSION_PASSED_MESSAGE", Title, Subtitle, 100, true, GetBigMessageNumStats(), true);
            // TODO: play some mission complete sound
            ShownEffect?.Start(1000, false);
            CurrentState = State.WaitForInputClosed;
        }

        public void Hide()
        {
            if (CurrentState == State.None || CurrentState >= State.CloseRollUp)
                return;

            if(CurrentState >= State.WaitForInputExpanded)
            {
                CurrentState = State.CloseRollUp;
            }
            else if(CurrentState >= State.WaitForInputClosed)
            {
                CurrentState = State.CloseHide;
            }
        }

        private void UpdateLoop()
        {
            while (true)
            {
                GameFiber.Yield();
                Update();
            }
        }

        private void Update()
        {
            if (CurrentState == State.None)
                return;
            
            Draw();

            switch (CurrentState)
            {
                case State.WaitForInputClosed:
                    {
                        if (RPH.Game.WasControlActionJustPressed(0, ExpandControl))
                        {
                            ButtonPressedSound?.Play();
                            InstructionalButtons.Slots[instructionalButtonExpandIndex].IsVisible = false;
                            CurrentState = State.StartExpand;
                        }
                        else if (RPH.Game.WasControlActionJustPressed(0, ContinueControl))
                        {
                            ButtonPressedSound?.Play();
                            Hide();
                        }
                        break;
                    }

                case State.StartExpand:
                    {
                        BigMessage.CallMethod("TRANSITION_UP", 0.15f, true);
                        CurrentState = State.ExpandRollDown;
                        break;
                    }

                case State.ExpandRollDown:
                    {
                        if (HaveElapsedSinceStateStart(150))
                        {
                            BigMessage.CallMethod("ROLL_DOWN_BACKGROUND");
                            CurrentState = State.WaitForInputExpanded;
                        }

                        break;
                    }

                case State.WaitForInputExpanded:
                    {
                        if (RPH.Game.WasControlActionJustPressed(0, ContinueControl))
                        {
                            ButtonPressedSound?.Play();
                            Hide();
                        }
                        break;
                    }

                case State.CloseRollUp:
                    {
                        BigMessage.CallMethod("ROLL_UP_BACKGROUND");
                        CurrentState = State.CloseHide;
                        break;
                    }

                case State.CloseHide:
                    {
                        if (HaveElapsedSinceStateStart(200))
                        {
                            BigMessage.Hide();
                            CurrentState = State.WaitHide;
                        }
                        break;
                    }

                case State.WaitHide:
                    {
                        if (HaveElapsedSinceStateStart(400))
                        {
                            CurrentState = State.Continue;
                        }
                        break;
                    }

                case State.Continue:
                    {
                        OnContinued(EventArgs.Empty);
                        BigMessage.Dismiss();
                        InstructionalButtons.Dismiss();
                        CurrentState = State.None;
                        break;
                    }
            }
        }

        private void Draw()
        {
            BigMessage.Draw();
            InstructionalButtons.Draw();

            if(CurrentState == State.WaitForInputExpanded && HaveElapsedSinceStateStart(300))
            {
                Color white = HudColor.White.GetColor();
                Rect.Draw((0.5f, 0.3275f).Rel(), (0.27f, 0.0009259259f).Rel(), white);

                float y = 0.3387495f;
                for (int i = 0; i < Math.Min(Items.Count, MaxItemsOnScreenCount - (IsCompletionVisible ? 1 : 0)); i++)
                {
                    MissionPassedScreenItem item = Items[i];
                    item.Label.Display((0.37f, y).Rel());
                    if (item.Tickbox == MissionPassedScreenItem.TickboxState.None)
                    {
                        item.Status.Display((0.63f, y).Rel());
                    }
                    else
                    {
                        item.Status.Display((0.614f, y).Rel());

                        const float w = 0.00078125f * 24f * 2f * 0.65f;
                        const float h = 0.001388889f * 24f * 2f * 0.65f;
                        float spriteY = y + 0.014f;
                        Sprite.Draw("commonmenu", GetTickboxTextureName(item.Tickbox), (0.622f, spriteY).Rel(), (w, h).Rel(), 0f, Color.White);
                    }
                    y += 125f * 0.2f * 0.001388889f;
                }

                y += 27.5f * 0.2f * 0.001388889f;

                Rect.Draw((0.5f, y).Rel(), (0.27f, 0.0009259259f).Rel(), white);

                if (IsCompletionVisible)
                {
                    y += 42f * 0.2f * 0.001388889f;

                    CompletionText.Display((0.4175f, y).Rel());
                    CompletionPercentageText.Display((0.56895f, y).Rel());

                    y += 0.015715f;

                    const float w = 0.00078125f * 16f * 2f * 0.65f;
                    const float h = 0.001388889f * 16f * 2f * 0.65f;
                    Sprite.Draw("mphud", "missionpassedmedal", (0.57875f, y).Rel(), (w, h).Rel(), 0f, CompletionMedalColor);
                }
            }
        }

        protected virtual void OnContinued(EventArgs e)
        {
            Continued?.Invoke(this, e);
        }

        private int GetBigMessageNumStats()
        {
            int count = Math.Min(Items.Count + (IsCompletionVisible ? 1 : 0), 13);

            return count;
        }

        private string GetTickboxTextureName(MissionPassedScreenItem.TickboxState tickbox)
        {
            switch (tickbox)
            {
                case MissionPassedScreenItem.TickboxState.Empty: return "shop_box_blank";
                case MissionPassedScreenItem.TickboxState.Tick: return "shop_box_tick";
                case MissionPassedScreenItem.TickboxState.Cross: return "shop_box_cross";
            }

            return null;
        }

        private bool HaveElapsedSinceStateStart(uint milliseconds)
        {
            return RPH.Game.GameTime > (currentStateStartTime + milliseconds);
        }

        public void Dispose()
        {
            Fiber?.Abort();
            Fiber = null;
            BigMessage?.Dismiss();
            BigMessage = null;
            InstructionalButtons?.Dismiss();
            InstructionalButtons = null;
        }
        
        private static TextStyle CreateCompletionTextStyle()
        {
            return new TextStyle
            {
                Scale = 0.4f,
                Color = HudColor.White.GetColor(),
                Font = TextFont.ChaletLondon,
                Alignment = TextAlignment.Left,
            };
        }

        private static TextStyle CreateCompletionPercentageTextStyle()
        {
            return new TextStyle
            {
                Scale = 0.395f,
                Color = HudColor.White.GetColor(),
                Font = TextFont.ChaletLondon,
                Alignment = TextAlignment.Right,
            };
        }

        private enum State
        {
            None,

            WaitForInputClosed,

            StartExpand,
            ExpandRollDown,

            WaitForInputExpanded,

            CloseRollUp,
            CloseHide,
            WaitHide,

            Continue,
        }
    }

    public class MissionPassedScreenItem
    {
        public enum TickboxState
        {
            None,
            Empty,
            Tick,
            Cross,
        }

        public Text Label { get; }
        public Text Status { get; }
        public TickboxState Tickbox { get; set; }

        public MissionPassedScreenItem(string label, string status, TickboxState tickbox)
        {
            Label = new Text() { Style = CreateLabelTextStyle() };
            Status = new Text() { Style = CreateStatusTextStyle() };
            Tickbox = tickbox;

            Label.SetUnformattedString(label);
            Status.SetUnformattedString(status);
        }

        public MissionPassedScreenItem(string label, string status) : this(label, status, TickboxState.None)
        {
        }

        public MissionPassedScreenItem(string label) : this(label, "", TickboxState.None)
        {
        }

        private static TextStyle CreateLabelTextStyle()
        {
            return new TextStyle
            {
                Scale = 0.35f,
                Color = HudColor.White.GetColor(),
                Font = TextFont.ChaletLondon,
                Alignment = TextAlignment.Left,
            };
        }

        private static TextStyle CreateStatusTextStyle()
        {
            return new TextStyle
            {
                Scale = 0.35f,
                Color = HudColor.White.GetColor(),
                Font = TextFont.ChaletLondon,
                Alignment = TextAlignment.Right,
            };
        }
    }

    public class MissionPassedScreenItemsCollection : Collection<MissionPassedScreenItem>
    {
    }
}

