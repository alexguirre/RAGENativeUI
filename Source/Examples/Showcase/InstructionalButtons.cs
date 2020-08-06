namespace RNUIExamples.Showcase
{
    using RAGENativeUI;
    using RAGENativeUI.Elements;
    using Rage;
    using System.Drawing;
    using System.Linq;

    internal sealed class InstructionalButtons : UIMenu
    {
        private int slotId;

        public InstructionalButtons() : base(Plugin.MenuTitle, "INSTRUCTIONAL BUTTONS")
        {
            Plugin.Pool.Add(this);

            CreateMenuItems();
        }

        private void CreateMenuItems()
        {
            var bg = Util.NewColorsItem("Background", $"Modifies the ~b~{nameof(RAGENativeUI.Elements.InstructionalButtons)}.{nameof(RAGENativeUI.Elements.InstructionalButtons.BackgroundColor)}~s~ property.");
            bg.SelectedItem = HudColor.PanelLight;
            InstructionalButtons.BackgroundColor = HudColor.PanelLight.GetColor();
            bg.IndexChanged += (s, o, n) =>
            {
                Color c = bg.SelectedItem.GetColor();
                InstructionalButtons.BackgroundColor = c;
                InstructionalButtons.Update();
                foreach (UIMenu child in Children.Values)
                {
                    child.InstructionalButtons.BackgroundColor = c;
                    child.InstructionalButtons.Update();
                }
            };

            var maxWidth = new UIMenuNumericScrollerItem<float>("Max Width", $"Modifies the ~b~{nameof(RAGENativeUI.Elements.InstructionalButtons)}.{nameof(RAGENativeUI.Elements.InstructionalButtons.MaxWidth)}~s~ property.",
                                0.0f, 1.0f, 0.01f)
            {
                Formatter = v => v.ToString("0.00"),
            }.WithTextEditing();
            maxWidth.Value = InstructionalButtons.MaxWidth;
            maxWidth.IndexChanged += (s, o, n) =>
            {
                float v = maxWidth.Value;
                InstructionalButtons.MaxWidth = v;
                InstructionalButtons.Update();
                foreach (UIMenu child in Children.Values)
                {
                    child.InstructionalButtons.MaxWidth = v;
                    child.InstructionalButtons.Update();
                }
            };

            var addButton = new UIMenuItem("Add Button");
            addButton.Activated += (m, s) =>
            {
                var button = new InstructionalButton("A", "Slot #" + slotId++);
                var buttonMenu = new ButtonMenu(button);
                buttonMenu.InstructionalButtons.BackgroundColor = bg.SelectedItem.GetColor();
                buttonMenu.InstructionalButtons.MaxWidth = maxWidth.Value;
                buttonMenu.InstructionalButtons.Buttons.Clear();
                buttonMenu.InstructionalButtons.Buttons.AddRange(InstructionalButtons.Buttons);

                var bindItem = new UIMenuItem(button.Text);
                AddItem(bindItem);
                BindMenuToItem(buttonMenu, bindItem);

                InstructionalButtons.Buttons.Add(button);
                foreach (UIMenu child in Children.Values)
                {
                    child.InstructionalButtons.Buttons.Add(button);
                }
            };

            AddItems(bg, maxWidth, addButton);
            this.WithFastScrollingOn(maxWidth);
        }


        private class ButtonMenu : UIMenu
        {
            private readonly InstructionalButton button;
            private string buttonText = "A";
            private GameControl buttonControl = GameControl.Context;
            private uint buttonRawId = 0;
            private InstructionalKey buttonInstructionalKey = InstructionalKey.Unknown;

            public ButtonMenu(InstructionalButton button) : base(Plugin.MenuTitle, "INSTRUCTIONAL BUTTONS: " + button.Text.ToUpper())
            {
                Plugin.Pool.Add(this);

                this.button = button;

                var text = new UIMenuItem("Text", "") { Enabled = true }
                            .WithTextEditing(() => buttonText,
                                             s =>
                                             {
                                                 buttonText = s;
                                                 this.button.Button = s;
                                                 InstructionalButtons.Update();
                                             });

                var control = new UIMenuListScrollerItem<GameControl>("Control", "", (GameControl[])System.Enum.GetValues(typeof(GameControl)))
                {
                    Enabled = false,
                    SelectedItem = buttonControl
                };
                control.IndexChanged += (s, o, n) =>
                {
                    buttonControl = control.SelectedItem;
                    this.button.Button = control.SelectedItem;
                    InstructionalButtons.Update();
                };

                var rawId = new UIMenuNumericScrollerItem<uint>("Raw ID", "", 0, 2048, 1)
                {
                    Enabled = false,
                    Value = buttonRawId
                }.WithTextEditing();
                rawId.IndexChanged += (s, o, n) =>
                {
                    buttonRawId = rawId.Value;
                    this.button.Button = rawId.Value;
                    InstructionalButtons.Update();
                };

                var instructionalKey = new UIMenuListScrollerItem<InstructionalKey>("Instructional Key", "", ((InstructionalKey[])System.Enum.GetValues(typeof(InstructionalKey))).Distinct())
                {
                    Enabled = false,
                    SelectedItem = buttonInstructionalKey
                };
                instructionalKey.IndexChanged += (s, o, n) =>
                {
                    buttonInstructionalKey = instructionalKey.SelectedItem;
                    this.button.Button = instructionalKey.SelectedItem;
                    InstructionalButtons.Update();
                };

                var type = new UIMenuListScrollerItem<string>("Type", "", new[] { "Text", "Control", "Raw ID", "Instructional Key" });
                type.IndexChanged += (s, o, n) =>
                {
                    const int TextIdx = 0, ControlIdx = 1, RawIdIdx = 2, InstructionalKeyIdx = 3;

                    text.Enabled = n == TextIdx;
                    control.Enabled = n == ControlIdx;
                    rawId.Enabled = n == RawIdIdx;
                    instructionalKey.Enabled = n == InstructionalKeyIdx;

                    this.button.Button = n switch
                    {
                        TextIdx => buttonText,
                        ControlIdx => buttonControl,
                        RawIdIdx => buttonRawId,
                        InstructionalKeyIdx => buttonInstructionalKey,
                        _ => buttonText
                    };
                    InstructionalButtons.Update();
                };

                var remove = new UIMenuItem("Remove");
                remove.Activated += (m, s) =>
                {
                    UIMenu parentMenu = ParentMenu;
                    UIMenuItem parentItem = ParentItem;

                    Close();

                    parentMenu.ReleaseMenuFromItem(parentItem);
                    parentMenu.RemoveItemAt(parentMenu.MenuItems.IndexOf(parentItem));

                    Plugin.Pool.Remove(this);

                    parentMenu.InstructionalButtons.Buttons.Remove(button);
                    parentMenu.InstructionalButtons.Update();
                    foreach (UIMenu child in parentMenu.Children.Values)
                    {
                        child.InstructionalButtons.Buttons.Remove(this.button);
                        child.InstructionalButtons.Update();
                    }
                };

                AddItems(type, text, control, rawId, instructionalKey, remove);
            }
        }
    }
}
