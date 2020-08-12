namespace RNUIExamples
{
    using System;

    using Rage;

    using RAGENativeUI;
    using RAGENativeUI.Elements;

    /// <summary>
    /// Example of a menu used to confirm an action, like the exit confirmation menu from Los Santos Customs.
    /// </summary>
    public class ConfirmationMenu : UIMenu
    {
        private readonly InstructionalButton confirmButton;
        private readonly InstructionalButton backButton;

        private GameControl confirmControl = GameControl.FrontendAccept;
        private GameControl backControl = GameControl.FrontendCancel;

        public GameControl ConfirmControl
        {
            get => confirmControl;
            set
            {
                confirmButton.Button = value;
                confirmControl = value;
            }
        }

        public GameControl BackControl
        {
            get => backControl;
            set
            {
                backButton.Button = value;
                backControl = value;
            }
        }

        public event EventHandler Confirm;
        public event EventHandler Back;

        public ConfirmationMenu(string title, string subtitle, string message) : base(title, subtitle)
        {
            DescriptionOverride = message;

            confirmButton = new InstructionalButton(ConfirmControl, "Confirm");
            backButton = new InstructionalButton(BackControl, "Back");

            InstructionalButtons.Buttons.Clear();
            InstructionalButtons.Buttons.Add(confirmButton, backButton);
        }

        protected virtual void OnConfirm(EventArgs args)
        {
            Close(openParentMenu: false);
            Confirm?.Invoke(this, args);
        }

        protected virtual void OnBack(EventArgs args)
        {
            Close(openParentMenu: false);
            Back?.Invoke(this, args);
        }

        public override void ProcessControl()
        {
            if (Game.IsControlJustPressed(2, ConfirmControl))
            {
                OnConfirm(EventArgs.Empty);
            }
            else if (Game.IsControlJustPressed(2, BackControl))
            {
                OnBack(EventArgs.Empty);
            }
        }
    }
}
