namespace RAGENativeUI.Menus
{
    using System;
    using System.Linq;
    using System.Drawing;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Rage;
    using Rage.Native;
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Menus.Styles;

    /// <include file='..\Documentation\RAGENativeUI.Menus.Menu.xml' path='D/Menu/Doc/*' />
    public class Menu : IDisposable
    {
        public delegate void ForEachOnScreenItemDelegate(MenuItem item, int index);
        public delegate void SelectedIndexChangedEventHandler(Menu sender, int oldIndex, int newIndex);
        public delegate void VisibleChangedEventHandler(Menu sender, bool visible);
        

        public static bool IsAnyMenuVisible => MenusManager.IsAnyMenuVisible;
        public static readonly ReadOnlyCollection<GameControl> DefaultAllowedControls = Array.AsReadOnly(new[]
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
        });


        private IMenuStyle style;
        private MenuItemsCollection items;
        private int selectedIndex;
        private int maxItemsOnScreen = 10;
        private bool isVisible;
        private Menu currentParent, currentChild;

        public event SelectedIndexChangedEventHandler SelectedIndexChanged;
        public event VisibleChangedEventHandler VisibleChanged;

        public bool IsDisposed { get; private set; }
        public PointF Location { get; set; }
        public IMenuStyle Style { get { return style; } set { style = value ?? throw new ArgumentNullException($"The menu {nameof(Style)} can't be null."); } }
        public MenuBanner Banner { get; set; }
        public MenuSubtitle Subtitle { get; set; }
        public MenuBackground Background { get; set; }
        public MenuItemsCollection Items { get { return items; } set { items = value ?? throw new ArgumentNullException($"The menu {nameof(Items)} can't be null."); } }
        public MenuUpDownDisplay UpDownDisplay { get; set; }
        public MenuDescription Description { get; set; }

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
                yield return Description;
            }
        }

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
                    UpdateVisibleItemsIndices();
                    OnSelectedIndexChanged(oldIndex, newIndex);
                }
            }
        }
        public MenuItem SelectedItem { get { return (selectedIndex >= 0 && selectedIndex < Items.Count) ? Items[selectedIndex] : null; } set { SelectedIndex = Items.IndexOf(value); } }
        public MenuControls Controls { get; set; }
        public MenuSoundsSet SoundsSet { get; set; }
        public bool DisableControlsActions { get; set; } = true;
        /// <include file='..\Documentation\RAGENativeUI.Menus.Menu.xml' path='D/Menu/Member[@name="AllowedControls"]/*' />
        public GameControl[] AllowedControls { get; set; } = DefaultAllowedControls.ToArray();
        protected int MinVisibleItemIndex { get; set; }
        protected int MaxVisibleItemIndex { get; set; }
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
        public bool IsAnyItemOnScreen { get { return IsVisible && Items.Count > 0 && MaxItemsOnScreen != 0 && Items.Any(i => i.IsVisible); } }
        public bool IsVisible
        {
            get { return isVisible; }
            private set
            {
                if (value == isVisible)
                    return;
                isVisible = value;
                OnVisibleChanged(isVisible);
            }
        }
        // returns true if this menu is visible or any child menu in the hierarchy is visible
        public bool IsAnyChildMenuVisible
        {
            get
            {
                return IsVisible || (currentChild != null && currentChild.IsAnyChildMenuVisible);
            }
        }
        public dynamic Metadata { get; } = new Metadata();
        public bool JustOpened { get; private set; }

        public Menu(string title, string subtitle, MenuStyle style)
        {
            MenusManager.AddMenu(this);

            Style = style ?? throw new ArgumentNullException($"The menu {nameof(Style)} can't be null.");
            Location = Style.InitialMenuLocation;
            Banner = new MenuBanner(this, title);
            Subtitle = new MenuSubtitle(this, subtitle);
            Background = new MenuBackground(this);
            Items = new MenuItemsCollection(this);
            UpDownDisplay = new MenuUpDownDisplay(this);
            Description = new MenuDescription(this);

            Controls = new MenuControls();
            SoundsSet = new MenuSoundsSet();
        }

        public Menu(string title, string subtitle) : this(title, subtitle, MenuStyle.Default)
        {
        }

        public void Show() => Show(null);
        public void Show(Menu parent)
        {
            currentParent = parent;

            if (parent != null)
            {
                parent.currentChild = this;
                parent.IsVisible = false;
            }

            IsVisible = true;
            JustOpened = true;
        }

        // hides child menus too
        public void Hide() => Hide(false);
        public void Hide(bool showParent)
        {
            if (currentChild != null)
                currentChild.Hide(false);

            if (showParent && currentParent != null)
            {
                currentParent.Show(currentParent.currentParent);
            }
            
            currentParent = null;
            currentChild = null;
            IsVisible = false;
        }

        // only called if the Menu is visible
        protected internal virtual void OnProcess()
        {
            // don't process in the tick the menu was opened
            if (JustOpened)
            {
                JustOpened = false;
                return;
            }

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
            if (Controls != null && IsAnyItemOnScreen)
            {
                if (Controls.Up != null && Controls.Up.IsHeld())
                {
                    if (SelectedItem == null || SelectedItem.OnMoveUp(this))
                    {
                        MoveUp();
                    }
                }

                if (Controls.Down != null && Controls.Down.IsHeld())
                {
                    if (SelectedItem == null || SelectedItem.OnMoveDown(this))
                    {
                        MoveDown();
                    }
                }

                if (Controls.Right != null && Controls.Right.IsHeld())
                {
                    if (SelectedItem == null || SelectedItem.OnMoveRight(this))
                    {
                        MoveRight();
                    }
                }

                if (Controls.Left != null && Controls.Left.IsHeld())
                {
                    if (SelectedItem == null || SelectedItem.OnMoveLeft(this))
                    {
                        MoveLeft();
                    }
                }

                if (Controls.Accept != null && Controls.Accept.IsJustPressed())
                {
                    if (SelectedItem == null || SelectedItem.OnAccept(this))
                    {
                        Accept();
                    }
                }

                if (Controls.Back != null && Controls.Back.IsJustPressed())
                {
                    if (SelectedItem == null || SelectedItem.OnBack(this))
                    {
                        Back();
                    }
                }
            }
        }

        protected virtual void MoveUp()
        {
            int newIndex = SelectedIndex - 1;

            int min = GetMinVisibleItemIndex();

            // get previous if current isn't visible
            while (newIndex >= min && !Items[newIndex].IsVisible)
                newIndex--;

            if (newIndex < min)
                newIndex = GetMaxVisibleItemIndex();

            SelectedIndex = newIndex;

            SoundsSet?.Up?.Play();
        }

        protected virtual void MoveDown()
        {
            int newIndex = SelectedIndex + 1;

            int max = GetMaxVisibleItemIndex();

            // get next if current isn't visible
            while (newIndex <= max && !Items[newIndex].IsVisible)
                newIndex++;

            if (newIndex > max)
                newIndex = GetMinVisibleItemIndex();

            SelectedIndex = newIndex;

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

        protected virtual void Back()
        {
            Hide(true);
            SoundsSet?.Back?.Play();
        }

        internal void UpdateVisibleItemsIndices()
        {
            if (MaxItemsOnScreen == 0)
            {
                MinVisibleItemIndex = -1;
                MaxVisibleItemIndex = -1;
                return;
            }
            else if (MaxItemsOnScreen >= Items.Count)
            {
                MinVisibleItemIndex = 0;
                MaxVisibleItemIndex = Items.Count - 1;
                return;
            }

            int index = SelectedIndex;

            if (index < MinVisibleItemIndex)
            {
                MinVisibleItemIndex = index;
                int count = 0;
                for (int i = MinVisibleItemIndex; i < Items.Count; i++)
                {
                    if (Items[i].IsVisible)
                    {
                        count++;
                        if (count == MaxItemsOnScreen)
                        {
                            MaxVisibleItemIndex = i;
                        }
                    }
                }
            }
            else if (index > MaxVisibleItemIndex)
            {
                MaxVisibleItemIndex = index;
                int count = 0;
                for (int i = MaxVisibleItemIndex; i >= 0; i--)
                {
                    if (Items[i].IsVisible)
                    {
                        count++;
                        if (count == MaxItemsOnScreen)
                        {
                            MinVisibleItemIndex = i;
                        }
                    }
                }
            }
            else
            {
                int count = 0;
                for (int i = MinVisibleItemIndex; i < Items.Count; i++)
                {
                    if (Items[i].IsVisible)
                    {
                        count++;
                        if (count == MaxItemsOnScreen)
                        {
                            MaxVisibleItemIndex = i;
                        }
                    }
                }
            }

            int min = GetMinVisibleItemIndex();
            int max = GetMaxVisibleItemIndex();

            if (MinVisibleItemIndex < min)
                MinVisibleItemIndex = min;
            if (MaxVisibleItemIndex > max)
                MaxVisibleItemIndex = max;

            if (MaxVisibleItemIndex < MinVisibleItemIndex)
                throw new InvalidOperationException($"MaxVisibleItemIndex({MaxVisibleItemIndex}) < MinVisibleItemIndex({MinVisibleItemIndex}): this shouldn't happen!");
        }

        // only called if the Menu is visible
        protected internal virtual void OnDraw(Graphics graphics)
        {
            float x = Location.X, y = Location.Y;

            foreach (IMenuComponent c in Components)
            {
                c?.Draw(graphics, ref x, ref y);
            }
        }

        /// <include file='..\Documentation\RAGENativeUI.Menus.Menu.xml' path='D/Menu/Member[@name="ForEachItemOnScreen"]/*' />
        public void ForEachItemOnScreen(ForEachOnScreenItemDelegate action)
        {
            if (Items.Count > 0 && IsAnyItemOnScreen)
            {
                for (int i = MinVisibleItemIndex; i <= MaxVisibleItemIndex; i++)
                {
                    MenuItem item = Items[i];

                    if (item != null)
                    {
                        if (!item.IsVisible)
                        {
                            continue;
                        }

                        action?.Invoke(item, i);
                    }
                }
            }
        }

        public int GetOnScreenItemsCount()
        {
            int count = 0;
            ForEachItemOnScreen((item, index) => count++);
            return count;
        }

        private int GetMinVisibleItemIndex()
        {
            int min = 0;
            for (int i = 0; i < Items.Count; i++)
            {
                min = i;
                if (Items[i].IsVisible)
                    break;
            }

            return min;
        }

        private int GetMaxVisibleItemIndex()
        {
            int max = 0;
            for (int i = Items.Count - 1; i >= 0; i--)
            {
                max = i;
                if (Items[i].IsVisible)
                    break;
            }

            return max;
        }

        protected virtual void OnSelectedIndexChanged(int oldIndex, int newIndex)
        {
            SelectedIndexChanged?.Invoke(this, oldIndex, newIndex);
        }

        protected virtual void OnVisibleChanged(bool visible)
        {
            VisibleChanged?.Invoke(this, visible);
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    MenusManager.RemoveMenu(this);
                }

                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }

    /// <include file='..\Documentation\RAGENativeUI.Menus.Menu.xml' path='D/MenuControls/Doc/*' />
    public class MenuControls
    {
        public Control Up { get; set; } = new Control(GameControl.FrontendUp);
        public Control Down { get; set; } = new Control(GameControl.FrontendDown);
        public Control Right { get; set; } = new Control(GameControl.FrontendRight);
        public Control Left { get; set; } = new Control(GameControl.FrontendLeft);
        public Control Accept { get; set; } = new Control(GameControl.FrontendAccept);
        public Control Back { get; set; } = new Control(GameControl.FrontendCancel);
    }

    /// <include file='..\Documentation\RAGENativeUI.Menus.Menu.xml' path='D/MenuSoundsSet/Doc/*' />
    public class MenuSoundsSet
    {
        public FrontendSound Up { get; set; } = new FrontendSound("HUD_FRONTEND_DEFAULT_SOUNDSET", "NAV_UP_DOWN");
        public FrontendSound Down { get; set; } = new FrontendSound("HUD_FRONTEND_DEFAULT_SOUNDSET", "NAV_UP_DOWN");
        public FrontendSound Right { get; set; } = new FrontendSound("HUD_FRONTEND_DEFAULT_SOUNDSET", "NAV_LEFT_RIGHT");
        public FrontendSound Left { get; set; } = new FrontendSound("HUD_FRONTEND_DEFAULT_SOUNDSET", "NAV_LEFT_RIGHT");
        public FrontendSound Accept { get; set; } = new FrontendSound("HUD_FRONTEND_DEFAULT_SOUNDSET", "SELECT");
        public FrontendSound Back { get; set; } = new FrontendSound("HUD_FRONTEND_DEFAULT_SOUNDSET", "BACK");
        public FrontendSound Error { get; set; } = new FrontendSound("HUD_FRONTEND_DEFAULT_SOUNDSET", "ERROR");
    }
}

