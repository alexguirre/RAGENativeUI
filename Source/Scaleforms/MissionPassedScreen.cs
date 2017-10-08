namespace RAGENativeUI.Scaleforms
{
    using System;
    using System.Drawing;

    using Rage;
    using Rage.Native;

    using RAGENativeUI.Elements;

    public class MissionPassedScreen : IDisposable
    {
        private const int MaxItemsOnScreenCount = 14;


        private readonly int instructionalButtonContinueIndex, instructionalButtonExpandIndex;
        private State state;
        private string title;
        private string subtitle;
        private MissionPassedScreenItemsCollection items;
        private uint timer;

        public event TypedEventHandler<MissionPassedScreen, EventArgs> Continued;

        public BigMessage BigMessage { get; private set; }
        public InstructionalButtons InstructionalButtons { get; private set; }

        public GameControl ContinueControl
        {
            get { return (InstructionalButtons.Slots[instructionalButtonContinueIndex] as InstructionalButtonControlSlot).Control; }
            set { (InstructionalButtons.Slots[instructionalButtonContinueIndex] as InstructionalButtonControlSlot).Control = value; }
        }

        public GameControl ExpandControl
        {
            get { return (InstructionalButtons.Slots[instructionalButtonExpandIndex] as InstructionalButtonControlSlot).Control; }
            set { (InstructionalButtons.Slots[instructionalButtonExpandIndex] as InstructionalButtonControlSlot).Control = value; }
        }
        
        /// <exception cref="ArgumentNullException">When setting the property to a null value.</exception>
        public string Title { get { return title; } set { Throw.IfNull(value, nameof(value)); title = value; } }
        /// <exception cref="ArgumentNullException">When setting the property to a null value.</exception>
        public string Subtitle { get { return subtitle; } set { Throw.IfNull(value, nameof(value)); subtitle = value; } }
        /// <exception cref="ArgumentNullException">When setting the property to a null value.</exception>
        public MissionPassedScreenItemsCollection Items { get { return items; } set { Throw.IfNull(value, nameof(value)); items = value; } }

        public bool IsVisible { get { return state != State.None; } }

        public GameFiber Fiber { get; private set; }

        public bool IsCompletionVisible { get; set; }
        public string CompletionText { get; set; } = Game.GetLocalizedString("MTPHPER"); // Completion
        public int CompletionPercentage { get; set; } = 100;
        public Color CompletionMedalColor { get; set; } = HudColor.GOLD.GetColor();

        public PostFxAnimation ShownEffect { get; set; } = PostFxAnimation.GetByName("SuccessNeutral");
        public FrontendSound ButtonPressedSound { get; set; } = new FrontendSound("HUD_FRONTEND_DEFAULT_SOUNDSET", "CONTINUE");

        public MissionPassedScreen(string title, string subtitle)
        {
            Throw.IfNull(title, nameof(title));
            Throw.IfNull(subtitle, nameof(subtitle));

            BigMessage = new BigMessage();
            BigMessage.OutTransition = BigMessage.OutTransitionType.MoveUp;
            BigMessage.OutTransitionTime = 0.4f;
            InstructionalButtons = new InstructionalButtons();
            instructionalButtonContinueIndex = InstructionalButtons.AddSlot(Game.GetLocalizedString("ES_HELP"), GameControl.FrontendEndscreenAccept); // Continue
            instructionalButtonExpandIndex = InstructionalButtons.AddSlot(Game.GetLocalizedString("ES_XPAND"), GameControl.FrontendEndscreenExpand); // Expand

            Title = title;
            Subtitle = subtitle;
            Items = new MissionPassedScreenItemsCollection();

            Fiber = GameFiber.StartNew(UpdateLoop, $"RAGENativeUI - {nameof(MissionPassedScreen)} Update Fiber");
        }

        public void Show()
        {
            InstructionalButtons.Slots[instructionalButtonExpandIndex].IsVisible = true;
                                        // 277 hours, I guess nobody will stay with this screen open for that long :P
            BigMessage.CallMethodAndShow(999999999, "SHOW_MISSION_PASSED_MESSAGE", Title, Subtitle, 100, true, GetBigMessageNumStats(), true);
            // TODO: play some mission complete sound
            ShownEffect?.Start(1000, false);
            timer = 0;
            state = State.WaitForInputClosed;
        }

        public void Hide()
        {
            if (state == State.None || state >= State.CloseRollUp)
                return;

            if(state >= State.WaitForInputExpanded)
            {
                state = State.CloseRollUp;
            }
            else if(state >= State.WaitForInputClosed)
            {
                timer = 0;
                state = State.CloseHide;
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
            if (state == State.None)
                return;
            
            Draw();

            switch (state)
            {
                case State.WaitForInputClosed:
                    {
                        if (Game.IsControlJustPressed(0, ExpandControl))
                        {
                            ButtonPressedSound?.Play();
                            InstructionalButtons.Slots[instructionalButtonExpandIndex].IsVisible = false;
                            state = State.StartExpand;
                        }
                        else if (Game.IsControlJustPressed(0, ContinueControl))
                        {
                            ButtonPressedSound?.Play();
                            Hide();
                        }
                        break;
                    }

                case State.StartExpand:
                    {
                        BigMessage.CallMethod("TRANSITION_UP", 0.15f, true);
                        timer = Game.GameTime;
                        state = State.ExpandRollDown;
                        break;
                    }

                case State.ExpandRollDown:
                    {
                        if (Game.GameTime > timer + 150)
                        {
                            BigMessage.CallMethod("ROLL_DOWN_BACKGROUND");
                            state = State.WaitForInputExpanded;
                        }

                        break;
                    }

                case State.WaitForInputExpanded:
                    {
                        if (Game.IsControlJustPressed(0, ContinueControl))
                        {
                            ButtonPressedSound?.Play();
                            Hide();
                        }
                        break;
                    }

                case State.CloseRollUp:
                    {
                        BigMessage.CallMethod("ROLL_UP_BACKGROUND");
                        timer = Game.GameTime;
                        state = State.CloseHide;
                        break;
                    }

                case State.CloseHide:
                    {
                        if (Game.GameTime > timer + 200)
                        {
                            BigMessage.Hide();
                            timer = Game.GameTime;
                            state = State.WaitHide;
                        }
                        break;
                    }

                case State.WaitHide:
                    {
                        if (Game.GameTime > timer + 400)
                        {
                            state = State.Continue;
                        }
                        break;
                    }

                case State.Continue:
                    {
                        OnContinued(EventArgs.Empty);
                        BigMessage.Dismiss();
                        InstructionalButtons.Dismiss();
                        state = State.None;
                        break;
                    }
            }
        }

        private void Draw()
        {
            BigMessage.Draw();
            InstructionalButtons.Draw();

            if(state == State.WaitForInputExpanded)
            {
                Color white = HudColor.WHITE.GetColor();
                Box.Draw(ScreenRectangle.FromRelativeCoords(0.5f, 0.3275f, 0.27f, 0.0009259259f), white);

                // TODO: show items as the background rolls down, instead of showing them all at once
                float y = 0.3387495f;
                for (int i = 0; i < Math.Min(Items.Count, MaxItemsOnScreenCount - (IsCompletionVisible ? 1 : 0)); i++)
                {
                    MissionPassedScreenItem item = Items[i];
                    Text.Draw(ScreenPosition.FromRelativeCoords(0.37f, y), item.Label, 0.35f, white, TextFont.ChaletLondon, TextAlignment.Left, 0f, false, false);
                    if (item.Tickbox == MissionPassedScreenItem.TickboxState.None)
                    {
                        Text.Draw(ScreenPosition.FromRelativeCoords(0.63f, y), item.Status, 0.35f, white, TextFont.ChaletLondon, TextAlignment.Right, 0f, false, false);
                    }
                    else
                    {
                        Text.Draw(ScreenPosition.FromRelativeCoords(0.614f, y), item.Status, 0.35f, white, TextFont.ChaletLondon, TextAlignment.Right, 0f, false, false);

                        const float w = 0.00078125f * 24f * 2f * 0.65f;
                        const float h = 0.001388889f * 24f * 2f * 0.65f;
                        float spriteY = y + 0.014f;
                        Sprite.Draw("commonmenu", GetTickboxTextureName(item.Tickbox), ScreenRectangle.FromRelativeCoords(0.622f, spriteY, w, h), 0f, Color.White);
                    }
                    y += 125f * 0.2f * 0.001388889f;
                }

                y += 27.5f * 0.2f * 0.001388889f;

                Box.Draw(ScreenRectangle.FromRelativeCoords(0.5f, y, 0.27f, 0.0009259259f), white);

                if (IsCompletionVisible)
                {
                    y += 42f * 0.2f * 0.001388889f;

                    if (CompletionText != null)
                    {
                        Text.Draw(ScreenPosition.FromRelativeCoords(0.4175f, y), CompletionText, 0.4f, white, TextFont.ChaletLondon, TextAlignment.Left, 0f, false, false);
                    }
                    Text.Draw(ScreenPosition.FromRelativeCoords(0.56895f, y), $"{CompletionPercentage}%", 0.395f, white, TextFont.ChaletLondon, TextAlignment.Right, 0f, false, false);

                    y += 0.015715f;

                    const float w = 0.00078125f * 16f * 2f * 0.65f;
                    const float h = 0.001388889f * 16f * 2f * 0.65f;
                    Sprite.Draw("mphud", "missionpassedmedal", ScreenRectangle.FromRelativeCoords(0.57875f, y, w, h), 0f, CompletionMedalColor);
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

        public void Dispose()
        {
            Fiber?.Abort();
            Fiber = null;
            BigMessage?.Dismiss();
            BigMessage = null;
            InstructionalButtons?.Dismiss();
            InstructionalButtons = null;
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

        public string Label { get; set; }
        public string Status { get; set; }
        public TickboxState Tickbox { get; set; }

        public MissionPassedScreenItem(string label, string status, TickboxState tickbox)
        {
            Label = label;
            Status = status;
            Tickbox = tickbox;
        }

        public MissionPassedScreenItem(string label, string status) : this(label, status, TickboxState.None)
        {
        }

        public MissionPassedScreenItem(string label) : this(label, "", TickboxState.None)
        {
        }
    }

    public class MissionPassedScreenItemsCollection : BaseCollection<MissionPassedScreenItem>
    {
    }
}

