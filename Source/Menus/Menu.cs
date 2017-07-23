namespace RAGENativeUI.Menus
{
    using System;
    using System.Drawing;
    using System.Collections.Generic;

    using Rage;
    using Rage.Native;
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Menus.Rendering;
    using RAGENativeUI.Utility;

    public class Menu
    {
        public delegate void SelectedIndexChangedEventHandler(Menu sender, int oldIndex, int newIndex);
        public delegate void VisibleChangedEventHandler(Menu sender, bool visible);


        public event SelectedIndexChangedEventHandler SelectedIndexChanged;
        public event VisibleChangedEventHandler VisibleChanged;

        public PointF Location { get; set; } = new PointF(30, 23);

        private IMenuSkin skin;
        /// <exception cref="ArgumentNullException">When setting the property to a null value.</exception>
        public IMenuSkin Skin { get { return skin; } set { skin = value ?? throw new ArgumentNullException($"The menu {nameof(Skin)} can't be null."); } }
        
        public MenuBanner Banner { get; set; }
        public MenuSubtitle Subtitle { get; set; }
        public MenuBackground Background { get; set; }

        private MenuItemsCollection items;
        /// <exception cref="ArgumentNullException">When setting the property to a null value.</exception>
        public MenuItemsCollection Items { get { return items; } set { items = value ?? throw new ArgumentNullException($"The menu {nameof(Items)} can't be null."); } }
        
        public MenuUpDownDisplay UpDownDisplay { get; set; }

        public IEnumerable<IMenuComponent> Components
        {
            get
            {
                // returned in draw order
                yield return Banner;
                yield return Subtitle;
                yield return Background;
                yield return Items;
                yield return UpDownDisplay;
            }
        }

        private int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                int newIndex = MathHelper.Clamp(value, 0, Items.Count);

                if (newIndex != selectedIndex)
                {
                    int oldIndex = selectedIndex;
                    selectedIndex = newIndex;
                    OnSelectedIndexChanged(oldIndex, newIndex);
                }
            }
        }
        public MenuItem SelectedItem { get { return (selectedIndex >= 0 && selectedIndex < Items.Count) ? Items[selectedIndex] : null; } set { selectedIndex = Items.IndexOf(value); } }
        
        public MenuControls Controls { get; set; }
        public MenuSoundsSet SoundsSet { get; set; }

        private float width;
        public float Width
        {
            get { return width; }
            set
            {
                if (value == width)
                    return;
                width = value;

                foreach (IMenuComponent c in Components)
                {
                    if (c != null)
                    {
                        c.Size = new SizeF(width, c.Size.Height);
                    }
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

        internal int MinVisibleItemIndex { get; private set; }
        internal int MaxVisibleItemIndex { get; private set; }
        private int maxItemsOnScreen = 10;
        public int MaxItemsOnScreen
        {
            get { return maxItemsOnScreen; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException($"{nameof(MaxItemsOnScreen)} can't be a negative value.");
                maxItemsOnScreen = value;
                UpdateVisibleItemsIndices();
            }
        }
        public bool IsAnyItemOnScreen { get { return IsVisible && Items.Count > 0 && MaxItemsOnScreen != 0; } }

        private bool isVisible = false;
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (value == isVisible)
                    return;
                isVisible = value;
                OnVisibleChanged(isVisible);
            }
        }


        public Menu(string title, string subtitle)
        {
            Skin = MenuSkin.DefaultSkin;
            Banner = new MenuBanner(this);
            Subtitle = new MenuSubtitle(this);
            Background = new MenuBackground(this);
            Items = new MenuItemsCollection(this);
            UpDownDisplay = new MenuUpDownDisplay(this);

            Controls = new MenuControls();
            SoundsSet = new MenuSoundsSet();

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


            foreach (IMenuComponent c in Components)
            {
                c?.Process();
            }
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
            if (Controls != null)
            {
                if (Controls.Up != null && Controls.Up.IsHeld())
                {
                    if (SelectedItem == null || SelectedItem.OnPreviewMoveUp(this))
                    {
                        MoveUp();
                    }
                }

                if (Controls.Down != null && Controls.Down.IsHeld())
                {
                    if (SelectedItem == null || SelectedItem.OnPreviewMoveDown(this))
                    {
                        MoveDown();
                    }
                }

                if (Controls.Right != null && Controls.Right.IsHeld())
                {
                    if (SelectedItem == null || SelectedItem.OnPreviewMoveRight(this))
                    {
                        MoveRight();
                    }
                }

                if (Controls.Left != null && Controls.Left.IsHeld())
                {
                    if (SelectedItem == null || SelectedItem.OnPreviewMoveLeft(this))
                    {
                        MoveLeft();
                    }
                }

                if (Controls.Accept != null && Controls.Accept.IsJustPressed())
                {
                    if (SelectedItem == null || SelectedItem.OnPreviewAccept(this))
                    {
                        Accept();
                    }
                }

                if (Controls.Cancel != null && Controls.Cancel.IsJustPressed())
                {
                    if (SelectedItem == null || SelectedItem.OnPreviewCancel(this))
                    {
                        Cancel();
                    }
                }
            }
        }

        protected virtual void MoveUp()
        {
            int newIndex = SelectedIndex - 1;

            if (newIndex < 0)
                newIndex = Items.Count - 1;

            SelectedIndex = newIndex;
            UpdateVisibleItemsIndices();

            SoundsSet?.Up?.Play();
        }

        protected virtual void MoveDown()
        {
            int newIndex = SelectedIndex + 1;

            if (newIndex > (Items.Count - 1))
                newIndex = 0;

            SelectedIndex = newIndex;
            UpdateVisibleItemsIndices();
            
            SoundsSet?.Down?.Play();
        }

        protected virtual void MoveRight()
        {
            SoundsSet?.Right?.Play();
        }

        protected virtual void MoveLeft()
        {
            SoundsSet?.Left?.Play();
        }

        protected virtual void Accept()
        {
            SoundsSet?.Accept?.Play();
        }

        protected virtual void Cancel()
        {
            SoundsSet?.Cancel?.Play();
        }

        internal void UpdateVisibleItemsIndices()
        {
            if (MaxItemsOnScreen == 0)
            {
                MinVisibleItemIndex = -1;
                MaxVisibleItemIndex = -1;
                return;
            }
            else if(MaxItemsOnScreen >= Items.Count)
            {
                MinVisibleItemIndex = 0;
                MaxVisibleItemIndex = Items.Count - 1;
                return;
            }

            int index = SelectedIndex;
            if (MinVisibleItemIndex > index)
            {
                int diff = index - MinVisibleItemIndex;
                MaxVisibleItemIndex += diff;
                MinVisibleItemIndex += diff;
            }
            else if (MaxVisibleItemIndex < index)
            {
                int diff = index - MaxVisibleItemIndex;
                MaxVisibleItemIndex += diff;
                MinVisibleItemIndex += diff;
            }

            if ((MaxVisibleItemIndex - MinVisibleItemIndex) + 1 != MaxItemsOnScreen)
            {
                MaxVisibleItemIndex = MinVisibleItemIndex + MaxItemsOnScreen - 1;
            }

            if (MaxVisibleItemIndex < 0)
                MaxVisibleItemIndex = 0;
            if (MaxVisibleItemIndex >= Items.Count)
                MaxVisibleItemIndex = Items.Count - 1;

            if (MaxVisibleItemIndex < MinVisibleItemIndex)
                throw new InvalidOperationException($"MaxVisibleItemIndex({MaxVisibleItemIndex}) < MinVisibleItemIndex({MinVisibleItemIndex}): this shouldn't happen!");
        }

        public virtual void Draw(Graphics graphics)
        {
            if (!IsVisible)
                return;
            
            float x = Location.X, y = Location.Y;

            foreach (IMenuComponent c in Components)
            {
                c?.Draw(graphics, ref x, ref y);
            }
        }

        protected virtual void OnSelectedIndexChanged(int oldIndex, int newIndex)
        {
            SelectedIndexChanged?.Invoke(this, oldIndex, newIndex);
        }

        protected virtual void OnVisibleChanged(bool visible)
        {
            VisibleChanged?.Invoke(this, visible);
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


    public class MenuItemsCollection : BaseCollection<MenuItem>, IMenuComponent
    {
        public Menu Menu { get; }
        public SizeF Size
        {
            get
            {
                float w = 0f, h = 0f;
                if (Count > 0 && Menu.IsAnyItemOnScreen)
                {
                    for (int i = Menu.MinVisibleItemIndex; i <= Menu.MaxVisibleItemIndex; i++)
                    {
                        MenuItem item = this[i];

                        if (item.Size.Width > w)
                            w = item.Size.Width;

                        h += item.Size.Height;
                    }
                }

                return new SizeF(w, h);
            }
            set
            {
                for (int i = 0; i < Count; i++)
                {
                    MenuItem item = this[i];
                    item.Size = new SizeF(value.Width, item.Size.Height);
                }
            }
        }

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

        public void Process()
        {
            for (int i = 0; i < Count; i++)
            {
                MenuItem item = this[i];
                item?.Process(Menu, i == Menu.SelectedIndex);
            }
        }

        public void Draw(Graphics g, ref float x, ref float y)
        {
            if (Menu.IsAnyItemOnScreen)
            {
                for (int i = Menu.MinVisibleItemIndex; i <= Menu.MaxVisibleItemIndex; i++)
                {
                    MenuItem item = this[i];

                    item?.Draw(g, Menu, i == Menu.SelectedIndex, ref x, ref y);
                }
            }
        }

        public override void Add(MenuItem item)
        {
            base.Add(item);
            item.Size = new SizeF(Menu.Width, item.Size.Height);
            Menu.UpdateVisibleItemsIndices();
        }

        public override void Insert(int index, MenuItem item)
        {
            base.Insert(index, item);
            item.Size = new SizeF(Menu.Width, item.Size.Height);
            Menu.UpdateVisibleItemsIndices();
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

    public class MenuSoundsSet
    {
        public FrontendSound Up { get; set; } = new FrontendSound("HUD_FRONTEND_DEFAULT_SOUNDSET", "NAV_UP_DOWN");
        public FrontendSound Down { get; set; } = new FrontendSound("HUD_FRONTEND_DEFAULT_SOUNDSET", "NAV_UP_DOWN");
        public FrontendSound Right { get; set; } = new FrontendSound("HUD_FRONTEND_DEFAULT_SOUNDSET", "NAV_LEFT_RIGHT");
        public FrontendSound Left { get; set; } = new FrontendSound("HUD_FRONTEND_DEFAULT_SOUNDSET", "NAV_LEFT_RIGHT");
        public FrontendSound Accept { get; set; } = new FrontendSound("HUD_FRONTEND_DEFAULT_SOUNDSET", "SELECT");
        public FrontendSound Cancel { get; set; } = new FrontendSound("HUD_FRONTEND_DEFAULT_SOUNDSET", "BACK");
        public FrontendSound Error { get; set; } = new FrontendSound("HUD_FRONTEND_DEFAULT_SOUNDSET", "ERROR");
    }
}

