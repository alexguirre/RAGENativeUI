namespace RAGENativeUI.Menus
{
    using System;
    using System.Linq;
    using System.Drawing;

    using Rage;
    using Rage.Native;
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Menus.Rendering;
    using RAGENativeUI.Utility;

    public class Menu
    {
        public PointF Location { get; set; } = new PointF(30, 23);// 17

        private MenuItemsCollection items;
        public MenuItemsCollection Items { get { return items; } set { items = value ?? throw new InvalidOperationException($"The menu {nameof(Items)} can't be null."); } }

        private MenuSkin skin;
        public MenuSkin Skin { get { return skin; } set { skin = value ?? throw new InvalidOperationException($"The menu {nameof(Skin)} can't be null."); } }

        private MenuBanner banner;
        public MenuBanner Banner { get { return banner; } set { banner = value; } }

        private MenuSubtitle subtitle;
        public MenuSubtitle Subtitle { get { return subtitle; } set { subtitle = value; } }

        private int selectedIndex;
        public int SelectedIndex { get { return selectedIndex; } set { selectedIndex = MathHelper.Clamp(value, 0, Items.Count); } }
        public MenuItem SelectedItem { get { return (selectedIndex >= 0 && selectedIndex < Items.Count) ? Items[selectedIndex] : null; } set { selectedIndex = Items.IndexOf(value); } }

        private MenuControls controls;
        public MenuControls Controls { get { return controls; } set { controls = value ?? throw new InvalidOperationException($"The menu {nameof(Controls)} can't be null."); } }

        private float width;
        public float Width
        {
            get { return width; }
            set
            {
                if (value == width)
                    return;
                width = value;
                Banner.Size = new SizeF(width, Banner.Size.Height);
                Subtitle.Size = new SizeF(width, Subtitle.Size.Height);
                for (int i = 0; i < Items.Count; i++)
                {
                    MenuItem item = Items[i];
                    item.Size = new SizeF(width, item.Size.Height);
                }
            }
        }
        
        public bool DisableControlsActions { get; set; } = true;
        /// <summary>
        /// Gets or sets the controls that aren't disabled when <see cref="DisableControlsActions"/> is <c>true</c>.
        /// </summary>
        /// <value>
        /// A <see cref="GameControl"/>s array.
        /// </value>
        public GameControl[] AllowedControls { get; set; } = DefaultAllowedControls;

        public bool IsVisible { get; set; } = false;

        public Menu(string title, string subtitle)
        {
            Items = new MenuItemsCollection(this);
            Skin = MenuSkin.DefaultSkin;
            Banner = new MenuBanner();
            Subtitle = new MenuSubtitle();
            Controls = new MenuControls();

            Banner.Title = title;
            Subtitle.Text = subtitle;

            Width = 432.0f;
        }

        public virtual void Process()
        {
            if (!IsVisible)
                return;

            if (DisableControlsActions)
            {
                DisableControls();
            }

            ProcessInput();
        }

        protected virtual void DisableControls()
        {
            NativeFunction.Natives.DisableAllControlActions(0);

            for (int i = 0; i < AllowedControls.Length; i++)
            {
                NativeFunction.Natives.EnableControlAction(0, (int)AllowedControls[i], true);
            }
        }

        protected virtual void ProcessInput()
        {
            if (Controls.Up.IsHeld())
            {
                if (SelectedItem == null || SelectedItem.OnPreviewMoveUp(this))
                {
                    MoveUp();
                }
            }

            if (Controls.Down.IsHeld())
            {
                if (SelectedItem == null || SelectedItem.OnPreviewMoveDown(this))
                {
                    MoveDown();
                }
            }

            if (Controls.Right.IsHeld())
            {
                if (SelectedItem == null || SelectedItem.OnPreviewMoveRight(this))
                {
                    MoveRight();
                }
            }

            if (Controls.Left.IsHeld())
            {
                if (SelectedItem == null || SelectedItem.OnPreviewMoveLeft(this))
                {
                    MoveLeft();
                }
            }

            if (Controls.Accept.IsJustPressed())
            {
                if (SelectedItem == null || SelectedItem.OnPreviewAccept(this))
                {
                    Accept();
                }
            }

            if (Controls.Cancel.IsJustPressed())
            {
                if (SelectedItem == null || SelectedItem.OnPreviewCancel(this))
                {
                    Cancel();
                }
            }
        }

        protected virtual void MoveUp()
        {
            int newIndex = SelectedIndex - 1;

            if (newIndex < 0)
                newIndex = Items.Count - 1;

            SelectedIndex = newIndex;
        }

        protected virtual void MoveDown()
        {
            int newIndex = SelectedIndex + 1;

            if (newIndex > (Items.Count - 1))
                newIndex = 0;

            SelectedIndex = newIndex;
        }

        protected virtual void MoveRight()
        {
        }

        protected virtual void MoveLeft()
        {
        }

        protected virtual void Accept()
        {
        }

        protected virtual void Cancel()
        {
        }

        public virtual void Draw(Graphics graphics)
        {
            if (!IsVisible)
                return;

            float x = Location.X, y = Location.Y;

#if DEBUG
            bool debugDrawing = Game.IsKeyDownRightNow(System.Windows.Forms.Keys.D0);
            if (debugDrawing) Banner?.DebugDraw(graphics, skin, x, y);
#endif
            Banner?.Draw(graphics, skin, ref x, ref y);

#if DEBUG
            if (debugDrawing) Subtitle?.DebugDraw(graphics, skin, x, y);
#endif
            Subtitle?.Draw(graphics, skin, ref x, ref y);

            skin.DrawBackground(graphics, x, y - 1, Items.Max(m => m.Size.Width), Items.Sum(m => m.Size.Height));

            for (int i = 0; i < Items.Count; i++)
            {
                MenuItem item = Items[i];

#if DEBUG
                if (debugDrawing) item?.DebugDraw(graphics, skin, i == SelectedIndex, x, y);
#endif
                item?.Draw(graphics, skin, i == SelectedIndex, ref x, ref y);
            }
        }


        protected static readonly GameControl[] DefaultAllowedControls =
        {
            GameControl.MoveUpDown,
            GameControl.MoveLeftRight,
            GameControl.Sprint,
            GameControl.Jump,
            GameControl.Enter,
            GameControl.VehicleExit,
            GameControl.VehicleAccelerate,
            GameControl.VehicleBrake,
            GameControl.VehicleMoveLeftRight,
            GameControl.VehicleFlyYawLeft,
            GameControl.ScriptedFlyLeftRight,
            GameControl.ScriptedFlyUpDown,
            GameControl.VehicleFlyYawRight,
            GameControl.VehicleHandbrake,
        };
    }


    public class MenuItemsCollection : Utility.BaseCollection<MenuItem>
    {
        protected internal Menu Menu { get; }

        public override MenuItem this[int index]
        {
            get { return base[index]; }
            set
            {
                base[index] = value;
                value.Size = new SizeF(Menu.Width, value.Size.Height);
            }

        }
        public MenuItemsCollection(Menu menu)
        {
            Menu = menu;
        }

        public override void Add(MenuItem item)
        {
            base.Add(item);
            item.Size = new SizeF(Menu.Width, item.Size.Height);
        }

        public override void Insert(int index, MenuItem item)
        {
            base.Insert(index, item);
            item.Size = new SizeF(Menu.Width, item.Size.Height);
        }
    }

    public class MenuControls
    {
        public Control Up { get; set; } = new Control(GameControl.FrontendUp);
        public Control Down { get; set; } = new Control(GameControl.FrontendDown);
        public Control Right { get; set; } = new Control(GameControl.FrontendRight);
        public Control Left { get; set; } = new Control(GameControl.FrontendLeft);
        public Control Accept { get; set; } = new Control(GameControl.FrontendAccept);
        public Control Cancel { get; set; } = new Control(GameControl.FrontendCancel);
    }
}

