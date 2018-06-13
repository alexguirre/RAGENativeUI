namespace RAGENativeUI.Menus
{
    using System;
    using System.Linq;
    using System.Drawing;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Rage;
    using Rage.Native;
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Menus.Themes;

    /// <include file='..\Documentation\RAGENativeUI.Menus.Menu.xml' path='D/Menu/Doc/*' />
    public class Menu : IDisposable, INotifyPropertyChanged
    {
        public delegate void ForEachItemOnScreenDelegate(MenuItem item, int index);


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


        private bool isDisposed;
        private MenuTheme theme;
        private string title;
        private string subtitle;
        private MenuItemsCollection items;
        private string currentDescription;
        private string descriptionOverride;
        private int selectedIndex;
        private MenuControls controls = new MenuControls();
        private MenuSoundsSet soundsSet = new MenuSoundsSet();
        private bool disableControlsActions = true;
        private GameControl[] allowedControls = DefaultAllowedControls.ToArray();
        private int minItemOnScreenIndex;
        private int maxItemOnScreenIndex;
        private int maxItemsOnScreen = 10;
        private bool isVisible;
        private bool justOpened;

        private Menu currentParent, currentChild;

        public event PropertyChangedEventHandler PropertyChanged;
        public event TypedEventHandler<Menu, SelectedItemChangedEventArgs> SelectedItemChanged;
        public event TypedEventHandler<Menu, VisibleChangedEventArgs> VisibleChanged;

        public bool IsDisposed
        {
            get => isDisposed;
            private set
            {
                if(value != isDisposed)
                {
                    isDisposed = value;
                    OnPropertyChanged(nameof(IsDisposed));
                }
            }
        }

        public MenuTheme Theme
        {
            get => theme;
            set
            {
                Throw.IfNull(value, nameof(value));
                if (value != theme)
                {
                    theme = value;
                    OnPropertyChanged(nameof(Theme));
                }
            }
        }

        public string Title
        {
            get => title;
            set
            {
                Throw.IfNull(value, nameof(value));
                if (value != title)
                {
                    title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public string Subtitle   // TODO: in default theme, add Counter properties
        {
            get => subtitle;
            set
            {
                Throw.IfNull(value, nameof(value));
                if (value != subtitle)
                {
                    subtitle = value;
                    OnPropertyChanged(nameof(Subtitle));
                }
            }
        }

        public MenuItemsCollection Items
        {
            get
            {
                if(items == null)
                {
                    items = CreateItemsInstance();
                }
                return items;
            }
        }

        public string CurrentDescription
        {
            get => currentDescription;
            private set
            {
                if (value != currentDescription)
                {
                    currentDescription = value;
                    OnPropertyChanged(nameof(CurrentDescription));
                }
            }
        }

        public string DescriptionOverride
        {
            get => descriptionOverride;
            set
            {
                if (value != descriptionOverride)
                {
                    descriptionOverride = value;
                    CurrentDescription = descriptionOverride;
                    OnPropertyChanged(nameof(DescriptionOverride));
                }
            }
        }

        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                int newIndex = MathHelper.Clamp(value, 0, Items.Count);

                if (newIndex != selectedIndex)
                {
                    int oldIndex = selectedIndex;
                    selectedIndex = newIndex;
                    UpdateVisibleItemsIndices();
                    UpdateCurrentDescription();
                    OnPropertyChanged(nameof(SelectedIndex));
                    OnPropertyChanged(nameof(SelectedItem));
                    OnSelectedItemChanged(new SelectedItemChangedEventArgs(oldIndex, newIndex, Items[oldIndex], Items[newIndex]));
                }
            }
        }

        public MenuItem SelectedItem
        {
            get => (selectedIndex >= 0 && selectedIndex < Items.Count) ? Items[selectedIndex] : null;
            set => SelectedIndex = Items.IndexOf(value);
        }

        public MenuControls Controls
        {
            get => controls;
            set
            {
                if (value != controls)
                {
                    controls = value;
                    OnPropertyChanged(nameof(Controls));
                }
            }
        }

        public MenuSoundsSet SoundsSet
        {
            get => soundsSet;
            set
            {
                if (value != soundsSet)
                {
                    soundsSet = value;
                    OnPropertyChanged(nameof(SoundsSet));
                }
            }
        }

        public bool DisableControlsActions
        {
            get => disableControlsActions;
            set
            {
                if(value != disableControlsActions)
                {
                    disableControlsActions = value;
                    OnPropertyChanged(nameof(DisableControlsActions));
                }
            }
        }

        /// <include file='..\Documentation\RAGENativeUI.Menus.Menu.xml' path='D/Menu/Member[@name="AllowedControls"]/*' />
        public GameControl[] AllowedControls
        {
            get => allowedControls;
            set
            {
                if(value != allowedControls)
                {
                    allowedControls = value;
                    OnPropertyChanged(nameof(AllowedControls));
                }
            }
        }

        public int MinItemOnScreenIndex
        {
            get => minItemOnScreenIndex;
            private set
            {
                if (value != minItemOnScreenIndex)
                {
                    minItemOnScreenIndex = value;
                    OnPropertyChanged(nameof(MinItemOnScreenIndex));
                }
            }
        }

        public int MaxItemOnScreenIndex
        {
            get => maxItemOnScreenIndex;
            private set
            {
                if (value != maxItemOnScreenIndex)
                {
                    maxItemOnScreenIndex = value;
                    OnPropertyChanged(nameof(MaxItemOnScreenIndex));
                }
            }
        }

        public int MaxItemsOnScreen
        {
            get => maxItemsOnScreen;
            set
            {
                Throw.IfNegative(value, nameof(value));

                if (value != maxItemsOnScreen)
                {
                    maxItemsOnScreen = value;
                    UpdateVisibleItemsIndices();
                    OnPropertyChanged(nameof(MaxItemsOnScreen));
                }
            }
        }

        public bool IsAnyItemOnScreen => true;//IsVisible && Items.Count > 0;// && MaxItemsOnScreen != 0 && Items.Any(i => i.IsVisible);

        public bool IsVisible
        {
            get => isVisible;
            private set
            {
                if (value != isVisible)
                {
                    isVisible = value;
                    OnPropertyChanged(nameof(IsVisible));
                    OnVisibleChanged(new VisibleChangedEventArgs(isVisible));
                }
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

        public bool JustOpened
        {
            get => justOpened;
            private set
            {
                if (value != justOpened)
                {
                    justOpened = value;
                    OnPropertyChanged(nameof(JustOpened));
                }
            }
        }

        public dynamic Metadata { get; } = new Metadata();

        public Menu(string title, string subtitle)
        {
            Throw.IfNull(title, nameof(title));
            Throw.IfNull(subtitle, nameof(subtitle));

            Title = title;
            Subtitle = subtitle;
            Theme = new MenuDefaultTheme(this);

            MenusManager.AddMenu(this);
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
            
            for (int i = 0; i < Items.Count; i++)
            {
                MenuItem item = Items[i];
                if (item != null)
                {
                    item.IsSelected = i == SelectedIndex;
                    item.OnProcess();
                }
            }
        }

        protected virtual void DisableControls()
        {
            N.DisableAllControlActions(0);

            for (int i = 0; i < AllowedControls.Length; i++)
            {
                N.EnableControlAction(0, (int)AllowedControls[i], true);
            }
        }

        protected virtual void ProcessInput()
        {
            if (Controls != null && IsAnyItemOnScreen)
            {
                if (Controls.Up != null && Controls.Up.IsHeld())
                {
                    MenuItem item = SelectedItem;
                    if (item == null || item.OnMoveUp())
                    {
                        MoveUp();
                    }
                }

                if (Controls.Down != null && Controls.Down.IsHeld())
                {
                    MenuItem item = SelectedItem;
                    if (item == null || item.OnMoveDown())
                    {
                        MoveDown();
                    }
                }

                if (Controls.Right != null && Controls.Right.IsHeld())
                {
                    MenuItem item = SelectedItem;
                    if (item == null || (!item.IsDisabled && item.OnMoveRight()))
                    {
                        MoveRight();
                    }
                }

                if (Controls.Left != null && Controls.Left.IsHeld())
                {
                    MenuItem item = SelectedItem;
                    if (item == null || (!item.IsDisabled && item.OnMoveLeft()))
                    {
                        MoveLeft();
                    }
                }

                if (Controls.Accept != null && Controls.Accept.IsJustPressed())
                {
                    MenuItem item = SelectedItem;
                    if (item == null || (!item.IsDisabled && item.OnAccept()))
                    {
                        Accept();
                    }
                }

                if (Controls.Back != null && Controls.Back.IsJustPressed())
                {
                    MenuItem item = SelectedItem;
                    if (item == null || item.OnBack())
                    {
                        Back();
                    }
                }
            }
        }

        protected virtual void MoveUp()
        {
            int newIndex = SelectedIndex - 1;

            int min = GetMinItemWithInputIndex();

            // get previous if current isn't visible
            while (newIndex >= min && (!Items[newIndex].IsVisible || (Items[newIndex].IsDisabled && Items[newIndex].IsSkippedIfDisabled)))
                newIndex--;

            if (newIndex < min)
                newIndex = GetMaxItemWithInputIndex();

            SelectedIndex = newIndex;

            SoundsSet?.Up?.Play();
        }

        protected virtual void MoveDown()
        {
            int newIndex = SelectedIndex + 1;

            int max = GetMaxItemWithInputIndex();

            // get next if current isn't visible
            while (newIndex <= max && (!Items[newIndex].IsVisible || (Items[newIndex].IsDisabled && Items[newIndex].IsSkippedIfDisabled)))
                newIndex++;

            if (newIndex > max)
                newIndex = GetMinItemWithInputIndex();

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
            if (MaxItemsOnScreen == 0 || Items.All(i => !i.IsVisible))
            {
                MinItemOnScreenIndex = -1;
                MaxItemOnScreenIndex = -1;
                return;
            }
            else if (MaxItemsOnScreen >= Items.Count)
            {
                MinItemOnScreenIndex = 0;
                MaxItemOnScreenIndex = Items.Count - 1;
                return;
            }

            int index = SelectedIndex;

            if (index < MinItemOnScreenIndex)
            {
                MinItemOnScreenIndex = index;
                int count = 0;
                for (int i = MinItemOnScreenIndex; i < Items.Count; i++)
                {
                    if (Items[i].IsVisible)
                    {
                        count++;
                        if (count == MaxItemsOnScreen)
                        {
                            MaxItemOnScreenIndex = i;
                        }
                    }
                }
            }
            else if (index > MaxItemOnScreenIndex)
            {
                MaxItemOnScreenIndex = index;
                int count = 0;
                for (int i = MaxItemOnScreenIndex; i >= 0; i--)
                {
                    if (Items[i].IsVisible)
                    {
                        count++;
                        if (count == MaxItemsOnScreen)
                        {
                            MinItemOnScreenIndex = i;
                        }
                    }
                }
            }
            else
            {
                int count = 0;
                for (int i = MinItemOnScreenIndex; i < Items.Count; i++)
                {
                    if (Items[i].IsVisible)
                    {
                        count++;
                        if (count == MaxItemsOnScreen)
                        {
                            MaxItemOnScreenIndex = i;
                        }
                    }
                }
            }

            int min = GetMinVisibleItemIndex();
            int max = GetMaxVisibleItemIndex();

            if (MinItemOnScreenIndex < min)
                MinItemOnScreenIndex = min;
            if (MaxItemOnScreenIndex > max)
                MaxItemOnScreenIndex = max;

            Throw.InvalidOperationIf(MaxItemOnScreenIndex < MinItemOnScreenIndex, $"MaxVisibleItemIndex({MaxItemOnScreenIndex}) < MinVisibleItemIndex({MinItemOnScreenIndex}): this should never happen.");
        }

        internal void UpdateCurrentDescription()
        {
            if(DescriptionOverride == null)
            {
                MenuItem item = SelectedItem;
                if(item != null && item.IsVisible)
                {
                    CurrentDescription = item.Description;
                }
                else
                {
                    CurrentDescription = null;
                }
            }
        }

        /// <include file='..\Documentation\RAGENativeUI.Menus.Menu.xml' path='D/Menu/Member[@name="ForEachItemOnScreen"]/*' />
        public void ForEachItemOnScreen(ForEachItemOnScreenDelegate action)
        {
            if (MinItemOnScreenIndex == -1 || MaxItemOnScreenIndex == -1)
                return;

            if (Items.Count > 0 && IsAnyItemOnScreen)
            {
                for (int i = MinItemOnScreenIndex; i <= MaxItemOnScreenIndex; i++)
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
        
        private int GetMinItemWithInputIndex() => GetMinItemIndexForCondition(m => m.IsVisible && !(m.IsDisabled && m.IsSkippedIfDisabled));
        private int GetMaxItemWithInputIndex() => GetMaxItemIndexForCondition(m => m.IsVisible && !(m.IsDisabled && m.IsSkippedIfDisabled));
        private int GetMinVisibleItemIndex() => GetMinItemIndexForCondition(m => m.IsVisible);
        private int GetMaxVisibleItemIndex() => GetMaxItemIndexForCondition(m => m.IsVisible);

        private int GetMinItemIndexForCondition(Predicate<MenuItem> condition)
        {
            int min = 0;
            for (int i = 0; i < Items.Count; i++)
            {
                min = i;
                MenuItem item = Items[i];
                if (condition(item))
                    break;
            }

            return min;
        }

        private int GetMaxItemIndexForCondition(Predicate<MenuItem> condition)
        {
            int max = 0;
            for (int i = Items.Count - 1; i >= 0; i--)
            {
                max = i;
                MenuItem item = Items[i];
                if (condition(item))
                    break;
            }

            return max;
        }

        protected virtual MenuItemsCollection CreateItemsInstance()
        {
            return new MenuItemsCollection(this);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnSelectedItemChanged(SelectedItemChangedEventArgs e)
        {
            SelectedItemChanged?.Invoke(this, e);
        }

        protected virtual void OnVisibleChanged(VisibleChangedEventArgs e)
        {
            VisibleChanged?.Invoke(this, e);
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

