namespace RAGENativeUI.Elements
{
    // Based off https://github.com/Guad/NOOSE/blob/master/MissionPassedScreen.cs by Guad, original license: MIT License

    using System;
    using System.Drawing;

    using Rage;

    public class MissionPassedScreen
    {
        public enum MedalType
        {
            Bronze,
            Silver,
            Gold,
        }

        public delegate void MissionPassedScreenEventHandler(MissionPassedScreen sender);


        /// <summary>
        /// Occurs when the user presses <see cref="ContinueControl"/> while this <see cref="MissionPassedScreen"/> is shown.
        /// </summary>
        public event MissionPassedScreenEventHandler ContinueHit;

        /// <summary>
        /// Gets or sets the title, the big yellow text.
        /// </summary>
        /// <value>
        /// A <see cref="String"/> representing the text of the title.
        /// </value>
        public string Title { get; set; }
        /// <summary>
        /// Gets or sets the subtitle, the text below the <see cref="Title"/>.
        /// </summary>
        /// <value>
        /// A <see cref="String"/> representing the text of the subtitle.
        /// </value>
        public string Subtitle { get; set; }
        /// <summary>
        /// Gets or sets the value that will be displayed as the completion percentage.
        /// </summary>
        /// <value>
        /// A <see cref="int"/> representing the completion percentage.
        /// </value>
        public int CompletionPercentage { get; set; }
        /// <summary>
        /// Gets or sets the medal displayed next to the completion percentage.
        /// </summary>
        /// <value>
        /// A <see cref="MedalType"/> representing the medal displayed next to the completion percentage.
        /// </value>
        public MedalType Medal { get; set; }

        /// <summary>
        /// Gets the <see cref="MissionPassedScreenItem"/>s contained on this <see cref="MissionPassedScreen"/>.
        /// </summary>
        /// <value>
        /// A <see cref="MissionPassedScreenItemsCollection"/> instance.
        /// </value>
        public MissionPassedScreenItemsCollection Items { get; }

        /// <summary>
        /// Gets the instructional buttons displayed when this <see cref="MissionPassedScreen"/> is visible.
        /// </summary>
        /// <value>
        /// A <see cref="Elements.InstructionalButtons"/> instance.
        /// </value>
        public InstructionalButtons InstructionalButtons { get; }

        private GameControl continueControl = GameControl.FrontendAccept;
        private readonly InstructionalButton continueControlInstructionalButton;
        /// <summary>
        /// Gets or sets the game control that must be pressed to exit this <see cref="MissionPassedScreen"/>. By default <see cref="GameControl.FrontendAccept"/>.
        /// </summary>
        /// <value>
        /// A <see cref="GameControl"/>.
        /// </value>
        public GameControl ContinueControl
        {
            get { return continueControl; }
            set
            {
                if (value != continueControl)
                {
                    continueControl = value;

                    int index = InstructionalButtons.Buttons.IndexOf(continueControlInstructionalButton);
                    if (index != -1)
                    {
                        InstructionalButtons.Buttons[index].ButtonControl = continueControl;
                    }
                    InstructionalButtons.Update();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissionPassedScreen"/> class.
        /// </summary>
        /// <param name="title">The title, the big yellow text.</param>
        /// <param name="subtitle">The subtitle, the text below the <see cref="Title"/>.</param>
        /// <param name="completionPercentage">The value that will be displayed as the completion percentage.</param>
        /// <param name="medal">The medal type displayed next to the completion percentage.</param>
        public MissionPassedScreen(string title, string subtitle, int completionPercentage, MedalType medal)
        {
            Title = title;
            Subtitle = subtitle;
            CompletionPercentage = completionPercentage;
            Medal = medal;
            Items = new MissionPassedScreenItemsCollection();
            InstructionalButtons = new InstructionalButtons();
            continueControlInstructionalButton = new InstructionalButton(ContinueControl, "Continue");
            InstructionalButtons.Buttons.Add(continueControlInstructionalButton);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissionPassedScreen"/> class with the <see cref="Title"/> set to "mission passed".
        /// </summary>
        /// <param name="subtitle">The subtitle, the text below the <see cref="Title"/>.</param>
        /// <param name="completionPercentage">The value that will be displayed as the completion percentage.</param>
        /// <param name="medal">The medal type displayed next to the completion percentage.</param>
        public MissionPassedScreen(string subtitle, int completionPercentage, MedalType medal) : this("mission passed", subtitle, completionPercentage, medal)
        {
        }

        /// <summary>
        /// Shows this <see cref="MissionPassedScreen"/>. It will be visible until the user presses <see cref="ContinueControl"/>, or the returned <see cref="GameFiber"/> is aborted.
        /// </summary>
        /// <returns>The <see cref="GameFiber"/> in which this <see cref="MissionPassedScreen"/> is drawn.</returns>
        public virtual GameFiber Show()
        {
            return GameFiber.StartNew(() =>
            {
                InstructionalButtons.Update();

                while (true)
                {
                    GameFiber.Yield();

                    Draw();

                    if (Game.IsControlJustPressed(0, ContinueControl))
                    {
                        Common.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                        break;
                    }
                }

                OnContinueHit();
            }, "RAGENativeUI - MissionPassedScreen Fiber");
        }

        protected virtual void Draw()
        {
            SizeF res = UIMenu.GetScreenResolutionMantainRatio();
            int middle = (int)(res.Width / 2);

            Sprite.Draw("mpentry", "mp_modenotselected_gradient", new Point(0, 10), new Size((int)res.Width, 450 + (Items.Count * 40)), 0.0f, Color.FromArgb(200, 255, 255, 255));

            ResText.Draw(Title, new Point(middle, 100), 2.5f, Color.FromArgb(255, 199, 168, 87), Common.EFont.Pricedown, true);

            ResText.Draw(Subtitle, new Point(middle, 230), 0.5f, Color.White, Common.EFont.ChaletLondon, true);
            ResRectangle.Draw(new Point(middle - 300, 290), new Size(600, 2), Color.White);

            for (int i = 0; i < Items.Count; i++)
            {
                MissionPassedScreenItem item = Items[i];

                ResText.Draw(item.Label, new Point(middle - 230, 300 + (40 * i)), 0.35f, Color.White, Common.EFont.ChaletLondon, false);
                ResText.Draw(item.Status, new Point(item.Tickbox == MissionPassedScreenItem.TickboxState.None ? middle + 265 : middle + 230, 300 + (40 * i)), 0.35f, Color.White, Common.EFont.ChaletLondon, ResText.Alignment.Right, false, false, Size.Empty);

                if (item.Tickbox == MissionPassedScreenItem.TickboxState.None)
                    continue;

                string spriteName;
                if (item.Tickbox == MissionPassedScreenItem.TickboxState.Tick)
                    spriteName = "shop_box_tick";
                else if (item.Tickbox  == MissionPassedScreenItem.TickboxState.Cross)
                    spriteName = "shop_box_cross";
                else
                    spriteName = "shop_box_blank";

                Sprite.Draw("commonmenu", spriteName, new Point(middle + 230, 290 + (40 * i)), new Size(48, 48), 0.0f, Color.White);
            }

            ResRectangle.Draw(new Point(middle - 300, 300 + (40 * Items.Count)), new Size(600, 2), Color.White);

            ResText.Draw("Completion", new Point(middle - 150, 320 + (40 * Items.Count)), 0.4f, Color.White, Common.EFont.ChaletLondon, false);
            ResText.Draw(CompletionPercentage + "%", new Point(middle + 150, 320 + (40 * Items.Count)), 0.4f, Color.White, Common.EFont.ChaletLondon, ResText.Alignment.Right, false, false, Size.Empty);

            string medalSprite;
            if (Medal == MedalType.Silver)
                medalSprite = "silvermedal";
            else if (Medal == MedalType.Gold)
                medalSprite = "goldmedal";
            else
                medalSprite = "bronzemedal";

            Sprite.Draw("mpmissionend", medalSprite, new Point(middle + 150, 320 + (40 * Items.Count)), new Size(32, 32), 0.0f, Color.White);

            InstructionalButtons.Draw();
        }

        /// <summary>
        /// Called when this <see cref="MissionPassedScreen"/> is shown and the user presses <see cref="ContinueControl"/>.
        /// </summary>
        protected virtual void OnContinueHit()
        {
            ContinueHit?.Invoke(this);
        }


        public class MissionPassedScreenItemsCollection : BaseCollection<MissionPassedScreenItem>
        {
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


        /// <summary>
        /// Gets or sets the label, the text displayed at the left of this <see cref="MissionPassedScreenItem"/>.
        /// </summary>
        /// <value>
        /// A <see cref="String"/> representing the text displayed at the left of this <see cref="MissionPassedScreenItem"/>.
        /// </value>
        public string Label { get; set; }
        /// <summary>
        /// Gets or sets the status, the text displayed at the right of this <see cref="MissionPassedScreenItem"/>.
        /// </summary>
        /// <value>
        /// A <see cref="String"/> representing the text displayed at the right of this <see cref="MissionPassedScreenItem"/>.
        /// </value>
        public string Status { get; set; }
        /// <summary>
        /// Gets or sets how the tickbox at the right of this <see cref="MissionPassedScreenItem"/> is displayed.
        /// </summary>
        /// <value>
        /// A <see cref="TickboxState"/> representing how the tickbox at the right of this <see cref="MissionPassedScreenItem"/> is displayed.
        /// </value>
        public TickboxState Tickbox { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissionPassedScreenItem"/> class.
        /// </summary>
        /// <param name="label">The label, the text displayed at the left of this item.</param>
        /// <param name="status">The status, the text displayed at the right of this item.</param>
        /// <param name="tickbox">The <see cref="TickboxState"/> representing how the tickbox at the right of this <see cref="MissionPassedScreenItem"/> is displayed.</param>
        public MissionPassedScreenItem(string label, string status, TickboxState tickbox)
        {
            Label = label;
            Status = status;
            Tickbox = tickbox;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissionPassedScreenItem"/> class with <see cref="Tickbox"/> set to <see cref="TickboxState.None"/>.
        /// </summary>
        /// <param name="label">The label, the text displayed at the left of this item.</param>
        /// <param name="status">The status, the text displayed at the right of this item.</param>
        public MissionPassedScreenItem(string label, string status) : this(label, status, TickboxState.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissionPassedScreenItem"/> class with <see cref="Tickbox"/> set to <see cref="TickboxState.None"/> and <see cref="Status"/> set to an empty <see cref="String"/>.
        /// </summary>
        /// <param name="label">The label, the text displayed at the left of this item.</param>
        public MissionPassedScreenItem(string label) : this(label, "", TickboxState.None)
        {
        }
    }
}
