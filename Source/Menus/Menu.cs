namespace RAGENativeUI.Menus
{
    using System;
    using System.Text;
    using System.Linq;
    using System.Drawing;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text.RegularExpressions;

    using Rage;
    using Rage.Native;
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Menus.Rendering;

    /// <include file='..\Documentation\RAGENativeUI.Menus.Menu.xml' path='D/Menu/Doc/*' />
    public class Menu
    {
        public delegate void ForEachOnScreenItemDelegate(MenuItem item, int index);

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
                    UpdateVisibleItemsIndices();
                    OnSelectedIndexChanged(oldIndex, newIndex);
                }
            }
        }
        public MenuItem SelectedItem { get { return (selectedIndex >= 0 && selectedIndex < Items.Count) ? Items[selectedIndex] : null; } set { SelectedIndex = Items.IndexOf(value); } }
        
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
        /// <include file='..\Documentation\RAGENativeUI.Menus.Menu.xml' path='D/Menu/Member[@name="AllowedControls"]/*' />
        public GameControl[] AllowedControls { get; set; } = DefaultAllowedControls.ToArray();

        protected int MinVisibleItemIndex { get; set; }
        protected int MaxVisibleItemIndex { get; set; }
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
        public bool IsAnyItemOnScreen { get { return IsVisible && Items.Count > 0 && MaxItemsOnScreen != 0 && Items.Any(i => i.IsVisible); } }

        private bool isVisible = false;
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

        /// <include file='..\Documentation\RAGENativeUI.Menus.Menu.xml' path='D/Menu/Member[@name="Owner"]/*' />
        public Menu Owner { get; private set; }
        public dynamic Metadata { get; } = new Metadata();

        public Menu(string title, string subtitle)
        {
            Skin = MenuSkin.DefaultSkin;
            Banner = new MenuBanner(this);
            Subtitle = new MenuSubtitle(this);
            Background = new MenuBackground(this);
            Items = new MenuItemsCollection(this);
            UpDownDisplay = new MenuUpDownDisplay(this);
            Description = new MenuDescription(this);

            Controls = new MenuControls();
            SoundsSet = new MenuSoundsSet();

            Banner.Title = title;
            Subtitle.Text = subtitle;

            Width = DefaultWidth;
        }

        public void Show(Menu owner = null)
        {
            Owner = owner;
            if (Owner != null)
                Owner.isVisible = false;
            IsVisible = true;
        }

        public void Hide(bool showOwner = true)
        {
            if (showOwner && Owner != null)
                Owner.IsVisible = true;
            Owner = null;
            IsVisible = false;
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
            if (Controls != null && IsAnyItemOnScreen)
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

                if (Controls.Back != null && Controls.Back.IsJustPressed())
                {
                    if (SelectedItem == null || SelectedItem.OnPreviewBack(this))
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
            Hide();
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
            else if(MaxItemsOnScreen >= Items.Count)
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
                        if(count == MaxItemsOnScreen)
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


        public const float DefaultWidth = 432.0f;
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
    }


    /// <include file='..\Documentation\RAGENativeUI.Menus.Menu.xml' path='D/MenuItemsCollection/Doc/*' />
    public class MenuItemsCollection : BaseCollection<MenuItem>, IMenuComponent
    {
        public Menu Menu { get; }
        public SizeF Size
        {
            get
            {
                float w = 0f, h = 0f;
                Menu.ForEachItemOnScreen((item, index) =>
                {
                    if (item.Size.Width > w)
                        w = item.Size.Width;

                    h += item.Size.Height;
                });

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
            float currentX = x, currentY = y;
            Menu.ForEachItemOnScreen((item, index) =>
            {
                item.Draw(g, Menu, index == Menu.SelectedIndex, ref currentX, ref currentY);
            });
            x = currentX;
            y = currentY;
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

        public MenuItem FindByText(string regexSearchPattern) => FindByText(regexSearchPattern, 0, Count - 1); 
        public MenuItem FindByText(string regexSearchPattern, int startIndex, int endIndex)
        {
            return FindByInternal(regexSearchPattern, startIndex, endIndex, (m) => m.Text);
        }

        public MenuItem FindByDescription(string regexSearchPattern) => FindByText(regexSearchPattern, 0, Count - 1);
        public MenuItem FindByDescription(string regexSearchPattern, int startIndex, int endIndex)
        {
            return FindByInternal(regexSearchPattern, startIndex, endIndex, (m) => m.Description);
        }

        private MenuItem FindByInternal(string regexSearchPattern, int startIndex, int endIndex, Func<MenuItem, string> getInput)
        {
            return FindAllByInternal(regexSearchPattern, startIndex, endIndex, getInput).FirstOrDefault();
        }

        public IEnumerable<MenuItem> FindAllByText(string regexSearchPattern) => FindAllByText(regexSearchPattern, 0, Count - 1);
        public IEnumerable<MenuItem> FindAllByText(string regexSearchPattern, int startIndex, int endIndex)
        {
            return FindAllByInternal(regexSearchPattern, startIndex, endIndex, (m) => m.Text);
        }

        public IEnumerable<MenuItem> FindAllByDescription(string regexSearchPattern) => FindAllByText(regexSearchPattern, 0, Count - 1);
        public IEnumerable<MenuItem> FindAllByDescription(string regexSearchPattern, int startIndex, int endIndex)
        {
            return FindAllByInternal(regexSearchPattern, startIndex, endIndex, (m) => m.Description);
        }

        private IEnumerable<MenuItem> FindAllByInternal(string regexSearchPattern, int startIndex, int endIndex, Func<MenuItem, string> getInput)
        {
            if (regexSearchPattern == null)
                throw new ArgumentNullException(nameof(regexSearchPattern));
            if (getInput == null)
                throw new ArgumentNullException(nameof(getInput));

            for (int i = startIndex; i < endIndex; i++)
            {
                if (i > 0 && i < Count)
                {
                    MenuItem item = this[i];
                    string input = getInput(item);
                    if (input != null && Regex.IsMatch(getInput(item), regexSearchPattern, RegexOptions.IgnoreCase))
                    {
                        yield return item;
                    }
                }
            }
        }
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

