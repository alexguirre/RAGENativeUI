using System;
using System.Drawing;
using System.ComponentModel;

namespace RAGENativeUI.Elements
{
    /// <summary>
    /// Simple item with a label.
    /// </summary>
    public class UIMenuItem
    {
        public static readonly Color DefaultBackColor = Color.Empty,
                                     DefaultHighlightedBackColor = Color.White,
                                     DefaultForeColor = Color.WhiteSmoke,
                                     DefaultHighlightedForeColor = Color.Black,
                                     DefaultDisabledForeColor = Color.FromArgb(163, 159, 148);
        public static readonly float DefaultTextScale = 0.35f;
        public static readonly TextFont DefaultTextFont = TextFont.ChaletLondon;

        private float leftBadgeOffset = 0.0f;
        private float rightBadgeOffset = 0.0f;

        /// <summary>
        /// Called when user selects the current item.
        /// </summary>
        public event ItemActivatedEvent Activated;

        public Color BackColor { get; set; } = DefaultBackColor;
        public Color HighlightedBackColor { get; set; } = DefaultHighlightedBackColor;

        public Color ForeColor { get; set; } = DefaultForeColor;
        public Color HighlightedForeColor { get; set; } = DefaultHighlightedForeColor;
        public Color DisabledForeColor { get; set; } = DefaultDisabledForeColor;

        /// <summary>
        /// Gets the current foreground color based on the state of the <see cref="UIMenuItem"/>.
        /// </summary>
        public Color CurrentForeColor
        {
            get
            {
                if (Enabled)
                {
                    if (Selected)
                    {
                        return HighlightedForeColor;
                    }
                    else
                    {
                        return ForeColor;
                    }
                }
                else
                {
                    return DisabledForeColor;
                }
            }
        }

        /// <summary>
        /// Basic menu button.
        /// </summary>
        /// <param name="text">Button label.</param>
        public UIMenuItem(string text) : this(text, "")
        {
        }

        /// <summary>
        /// Basic menu button.
        /// </summary>
        /// <param name="text">Button label.</param>
        /// <param name="description">Description.</param>
        public UIMenuItem(string text, string description)
        {
            Enabled = true;

            Text = text;
            Description = description;
        }


        /// <summary>
        /// Whether this item is currently selected.
        /// </summary>
        public virtual bool Selected { get; set; }

        /// <summary>
        /// Whether this item is skipped.
        /// </summary>
        public virtual bool Skipped { get; set; } = false;


        /// <summary>
        /// Whether this item is currently being hovered on with a mouse.
        /// </summary>
        public virtual bool Hovered { get; set; }


        /// <summary>
        /// This item's description.
        /// </summary>
        public virtual string Description { get; set; }


        /// <summary>
        /// Whether this item is enabled or disabled (text is greyed out and you cannot select it).
        /// </summary>
        public virtual bool Enabled { get; set; }

        /// <summary>
        /// Activates this item.
        /// </summary>
        /// <param name="menu">The <see cref="UIMenu"/> that is calling this method, or <c>null</c> if no menu is involved.</param>
        public void Activate(UIMenu menu = null)
        {
            menu?.ItemSelect(this, menu.MenuItems.IndexOf(this));
            ItemActivate(menu);
        }

        internal virtual void ItemActivate(UIMenu sender)
        {
            Activated?.Invoke(sender, this);
        }

        /// <summary>
        /// Set item's position.
        /// </summary>
        /// <param name="y"></param>
        [Obsolete("Use UIMenuItem.SetVerticalPosition instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void Position(int y)
        {
            SetVerticalPosition(y);
        }

        /// <summary>
        /// Set item's vertical position.
        /// </summary>
        /// <param name="y"></param>
        [Obsolete("It is no longer allowed to change the position of a menu item. The position will be calculated by the menu when drawing the item."), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void SetVerticalPosition(int y)
        {
        }

        /// <summary>
        /// Draw this item.
        /// </summary>
        [Obsolete("Use UIMenuItem.Draw(float, float, float, float)"), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void Draw()
        {
        }

        public virtual void Draw(float x, float y, float width, float height)
        {
            float rectWidth = width;
            float rectHeight = height;
            float rectX = x + rectWidth * 0.5f;
            float rectY = y + rectHeight * 0.5f;

            if (Selected)
            {
                UIMenu.DrawSprite(UIMenu.CommonTxd, UIMenu.NavBarTextureName,
                                  rectX, rectY,
                                  rectWidth, rectHeight,
                                  HighlightedBackColor);
            }
            else if (BackColor != Color.Empty)
            {
                UIMenu.DrawRect(rectX, rectY,
                                rectWidth, rectHeight,
                                BackColor);
            }

            if (Hovered && !Selected)
            {
                Color hoveredColor = Color.FromArgb(25, 255, 255, 255);
                UIMenu.DrawRect(rectX, rectY - 0.00138888f * 0.5f,
                                rectWidth, rectHeight - 0.00138888f,
                                hoveredColor);
            }

            if (LeftBadgeInfo != null)
            {
                DrawBadge(LeftBadgeInfo, true, x, y, width, height, out leftBadgeOffset);
            }
            else
            {
                leftBadgeOffset = 0.0f;
            }

            if (RightBadgeInfo != null)
            {
                DrawBadge(RightBadgeInfo, false, x, y, width, height, out rightBadgeOffset);
            }
            else
            {
                rightBadgeOffset = 0.0f;
            }

            SetTextCommandOptions();
            TextCommands.Display(Text, x + 0.0046875f + leftBadgeOffset, y + 0.00277776f);

            if (!String.IsNullOrEmpty(RightLabel))
            {
                SetTextCommandOptions(false);
                float labelWidth = TextCommands.GetWidth(RightLabel);

                float labelX = x + width - 0.00390625f - labelWidth - rightBadgeOffset;
                float labelY = y + 0.00277776f;

                SetTextCommandOptions(false);
                TextCommands.Display(RightLabel, labelX, labelY);
            }
        }

        /// <summary>
        /// Gets the offset by which to move the item contents when badges are set.
        /// </summary>
        protected void GetBadgeOffsets(out float left, out float right)
        {
            left = leftBadgeOffset;
            right = rightBadgeOffset;
        }

        internal void SetTextCommandOptions(bool left = true, bool disabledColor = false)
        {
            TextStyle s = left ? TextStyle : RightLabelStyle;
            s.Color = disabledColor ? DisabledForeColor : CurrentForeColor;

            s.Apply();
        }

        private void DrawBadge(BadgeInfo badge, bool left, float itemX, float itemY, float itemW, float itemH, out float offsetX)
        {
            // Badges don't look exactly like how game menus do it, but close enough

            // use checkbox texture to have a constant size, since different badges have different texture resolution
            UIMenu.GetTextureDrawSize(UIMenu.CommonTxd, UIMenu.CheckboxTickTextureName, out float badgeW, out float badgeH);

            const float Offset = 0.00078125f * 2.5f;
            float badgeOffset = (badgeW * 0.5f) + Offset;
            offsetX = badgeOffset + (0.00078125f * 8f);
            if (!badge.IsBlank)
            {
                Color c = badge.Color ?? CurrentForeColor;
                float sizeMult = badge.SizeMultiplier;
                float badgeX = left ?
                                itemX + badgeOffset :
                                itemX + itemW - badgeOffset;
                badge.GetTexture(Selected, out string txd, out string tex);
                if (badge.Style == BadgeStyle.Custom)
                {
                    // built-in badges have textures from 'commonmenu' txd which is already loaded by UIMenu,
                    // in the case of custom badges we don't know which txd is used so request it
                    N.RequestStreamedTextureDict(txd);
                    if (!N.HasStreamedTextureDictLoaded(txd))
                    {
                        return;
                    }
                }

                UIMenu.DrawSprite(
                    txd, tex,
                    badgeX,
                    itemY + (itemH * 0.5f),
                    badgeW * sizeMult,
                    badgeH * sizeMult,
                    c);
            }
        }

        /// <summary>
        /// Handles an input event when this item is selected.
        /// </summary>
        /// <param name="menu">The source <see cref="UIMenu"/> for this event.</param>
        /// <param name="control">The input control to handle.</param>
        /// <returns><c>true</c> if the input was consumed, such that the source <paramref name="menu"/> may not use it again; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="menu"/> is <c>null</c>.</exception>
        protected internal virtual bool OnInput(UIMenu menu, Common.MenuControls control)
        {
            if (menu == null)
            {
                throw new ArgumentNullException(nameof(menu));
            }

            bool consumed = false;
            switch (control)
            {
                case Common.MenuControls.Select:
                    consumed = true;
                    if (!Enabled)
                    {
                        Common.PlaySound(menu.AUDIO_ERROR, menu.AUDIO_LIBRARY);
                        break;
                    }

                    Common.PlaySound(menu.AUDIO_SELECT, menu.AUDIO_LIBRARY);

                    Activate(menu);
                    menu.OpenChildMenu(this);
                    break;
            }

            return consumed;
        }

        /// <summary>
        /// Defines the left mouse button states for <see cref="OnMouseInput(UIMenu, RectangleF, PointF, MouseInput)"/>.
        /// </summary>
        /// <seealso cref="OnMouseInput(UIMenu, RectangleF, PointF, MouseInput)"/>
        protected internal enum MouseInput { JustPressed, JustReleased, Pressed, PressedRepeat, Released }

        /// <summary>
        /// Handles a mouse input event when this item is selected.
        /// </summary>
        /// <param name="menu">The source <see cref="UIMenu"/> for this event.</param>
        /// <param name="itemBounds">The position and size of this item on screen, in relative coordinates.</param>
        /// <param name="mousePos">The position of the mouse, in relative coordinates.</param>
        /// <param name="input">The state of the left mouse button (control <see cref="Rage.GameControl.CursorAccept"/>).</param>
        /// <returns><c>true</c> if the input was consumed, such that the source <paramref name="menu"/> may not use it again; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="menu"/> is <c>null</c>.</exception>
        protected internal virtual bool OnMouseInput(UIMenu menu, RectangleF itemBounds, PointF mousePos, MouseInput input)
        {
            if (menu == null)
            {
                throw new ArgumentNullException(nameof(menu));
            }

            bool consumed = false;
            if (input == MouseInput.JustReleased && Selected && Hovered)
            {
                consumed = true;
                OnInput(menu, Common.MenuControls.Select);
            }

            return consumed;
        }

        /// <summary>
        /// This item's offset.
        /// </summary>
        [Obsolete("It is no longer allowed to change the position of a menu item. The position will be calculated by the menu when drawing the item."), EditorBrowsable(EditorBrowsableState.Never)]
        public Point Offset { get; set; }

        /// <summary>
        /// Returns this item's label.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the text style for this item's label. Note, the property <see cref="TextStyle.Color"/> is ignored, instead the color returned by <see cref="CurrentForeColor"/> is used.
        /// </summary>
        public TextStyle TextStyle { get; set; } = TextStyle.Default.With(font: DefaultTextFont,scale: DefaultTextScale);

        /// <summary>
        /// Returns the menu this item is in.
        /// </summary>
        public UIMenu Parent { get; set; }

        /// <summary>
        /// Set the left badge. Set it to None to remove the badge.
        /// </summary>
        /// <param name="badge"></param>
        [Obsolete("Use UIMenuItem.LeftBadge setter instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void SetLeftBadge(BadgeStyle badge)
        {
            LeftBadge = badge;
        }

        /// <summary>
        /// Set the right badge. Set it to None to remove the badge.
        /// </summary>
        /// <param name="badge"></param>
        [Obsolete("Use UIMenuItem.RightBadge setter instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void SetRightBadge(BadgeStyle badge)
        {
            RightBadge = badge;
        }

        /// <summary>
        /// Set the right label.
        /// </summary>
        /// <param name="text">Text as label. Set it to "" to remove the label.</param>
        [Obsolete("Use UIMenuItem.RightLabel setter instead."), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void SetRightLabel(string text)
        {
            RightLabel = text;
        }

        /// <summary>
        /// Gets or sets the current right label.
        /// </summary>
        public virtual string RightLabel { get; set; }

        /// <summary>
        /// Gets or sets the text style for the right label. Used by <see cref="UIMenuListItem"/> selected option text as well.
        /// Note, the property <see cref="TextStyle.Color"/> is ignored, instead the color returned by <see cref="CurrentForeColor"/> is used.
        /// </summary>
        public TextStyle RightLabelStyle { get; set; } = TextStyle.Default.With(font: DefaultTextFont, scale: DefaultTextScale);

        /// <summary>
        /// Gets or sets the current left badge. Set it to <see cref="BadgeStyle.None"/> to remove the badge.
        /// </summary>
        public virtual BadgeStyle LeftBadge
        {
            get => LeftBadgeInfo == null ? BadgeStyle.None : LeftBadgeInfo.Style;
            set => LeftBadgeInfo = value == BadgeStyle.None ? null : BadgeInfo.FromStyle(value);
        }

        /// <summary>
        /// Gets or sets the current left badge. Set it to <c>null</c> to remove the badge.
        /// </summary>
        public virtual BadgeInfo LeftBadgeInfo { get; set; }

        /// <summary>
        /// Gets or sets the current right badge. Set it to <see cref="BadgeStyle.None"/> to remove the badge.
        /// </summary>
        public virtual BadgeStyle RightBadge
        {
            get => RightBadgeInfo == null ? BadgeStyle.None : RightBadgeInfo.Style;
            set => RightBadgeInfo = value == BadgeStyle.None ? null : BadgeInfo.FromStyle(value);
        }

        /// <summary>
        /// Gets or sets the current right badge. Set it to <c>null</c> to remove the badge.
        /// </summary>
        public virtual BadgeInfo RightBadgeInfo { get; set; }

        public enum BadgeStyle
        {
            None,
            BronzeMedal,
            GoldMedal,
            SilverMedal,
            Alert,
            Crown,
            Ammo,
            Armour,
            Barber,
            Clothes,
            Franklin,
            Bike,
            Car,
            Gun,
            Heart,
            Makeup,
            Mask,
            Michael,
            Star,
            Tatoo,
            Trevor,
            Lock,
            Tick,
            CardSuitClubs,
            CardSuitDiamonds,
            CardSuitHearts,
            CardSuitSpades,
            Art,
            Blank,
            Custom,
        }

        public class BadgeInfo
        {
            private string textureDictionary;
            private string textureName;
            private string selectedTextureDictionary;
            private string selectedTextureName;
            private Color? color;
            private float sizeMultiplier;

            /// <summary>
            /// Gets the style of this badge. For user-created <see cref="BadgeInfo"/>s, returns <see cref="BadgeStyle.Custom"/>.
            /// </summary>
            public BadgeStyle Style { get; private set; }

            /// <summary>
            /// Gets or sets the texture dictionary that contains the texture.
            /// </summary>
            public string TextureDictionary
            {
                get => textureDictionary;
                set
                {
                    textureDictionary = value;
                    Style = BadgeStyle.Custom;
                }
            }

            /// <summary>
            /// Gets or sets the name of the texture.
            /// </summary>
            public string TextureName
            {
                get => textureName;
                set
                {
                    textureName = value;
                    Style = BadgeStyle.Custom;
                }
            }

            /// <summary>
            /// Gets or sets the alternative texture dictionary used when the <see cref="UIMenuItem"/> is selected.
            /// If <c>null</c>, <see cref="TextureDictionary"/> and <see cref="TextureName"/> are used always.
            /// </summary>
            public string SelectedTextureDictionary
            {
                get => selectedTextureDictionary;
                set
                {
                    selectedTextureDictionary = value;
                    Style = BadgeStyle.Custom;
                }
            }

            /// <summary>
            /// Gets or sets the alternative texture name used when the <see cref="UIMenuItem"/> is selected.
            /// If <c>null</c>, <see cref="TextureDictionary"/> and <see cref="TextureName"/> are used always.
            /// </summary>
            public string SelectedTextureName
            {
                get => selectedTextureName;
                set
                {
                    selectedTextureName = value;
                    Style = BadgeStyle.Custom;
                }
            }

            /// <summary>
            /// Gets or sets the color used when drawing the badge. If <c>null</c> the foreground color of the <see cref="UIMenuItem"/> is used.
            /// </summary>
            public Color? Color
            {
                get => color;
                set
                {
                    color = value;
                    Style = BadgeStyle.Custom;
                }
            }

            /// <summary>
            /// Gets or sets the size of the badge.
            /// </summary>
            public float SizeMultiplier
            {
                get => sizeMultiplier;
                set
                {
                    sizeMultiplier = value;
                    Style = BadgeStyle.Custom;
                }
            }

            /// <summary>
            /// Gets a value indicating whether the badge is blank. If <c>true</c>, no texture will be drawn but the <see cref="UIMenuItem"/> contents still get indented.
            /// A badge is considered blank when <see cref="TextureDictionary"/> or <see cref="TextureName"/> are null.
            /// </summary>
            public bool IsBlank => TextureDictionary == null || TextureName == null;

            /// <summary>
            /// Constructor for built-in badges.
            /// </summary>
            private BadgeInfo(BadgeStyle style,
                              string textureDictionary, string textureName,
                              Color? color,
                              string selectedTextureDictionary = null, string selectedTextureName = null,
                              float sizeMultiplier = 1.0f)
            {
                TextureDictionary = textureDictionary;
                TextureName = textureName;
                SelectedTextureDictionary = selectedTextureDictionary;
                SelectedTextureName = selectedTextureName;
                Color = color;
                SizeMultiplier = sizeMultiplier;
                Style = style; // set the style last because the properties setters change it to Custom
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="BadgeInfo"/> class with <see cref="Style"/> set to <see cref="BadgeStyle.Custom"/>.
            /// </summary>
            /// <param name="textureDictionary">The texture dictionary that contains the texture.</param>
            /// <param name="textureName">The name of the texture.</param>
            /// <param name="selectedTextureDictionary">
            /// Alternative texture dictionary used when the <see cref="UIMenuItem"/> is selected.
            /// If <c>null</c>, <paramref name="textureDictionary"/> and <paramref name="textureName"/> are used always.
            /// </param>
            /// <param name="selectedTextureName">
            /// Alternative texture name used when the <see cref="UIMenuItem"/> is selected.
            /// If <c>null</c>, <paramref name="textureDictionary"/> and <paramref name="textureName"/> are used always.
            /// </param>
            /// <param name="color">
            /// Determines the color used when drawing this badge. If <c>null</c>, the foreground color of the <see cref="UIMenuItem"/> is used.
            /// </param>
            /// <param name="sizeMultiplier">
            /// Determines the size of the badge.
            /// </param>
            public BadgeInfo(string textureDictionary, string textureName,
                             Color? color,
                             string selectedTextureDictionary = null, string selectedTextureName = null,
                             float sizeMultiplier = 1.0f)
                : this(BadgeStyle.Custom, textureDictionary, textureName, color, selectedTextureDictionary, selectedTextureName, sizeMultiplier)
            {
            }

            internal void GetTexture(bool selected, out string txd, out string tex)
            {
                if (selected && SelectedTextureDictionary != null && SelectedTextureName != null)
                {
                    txd = SelectedTextureDictionary;
                    tex = SelectedTextureName;
                }
                else
                {
                    txd = TextureDictionary;
                    tex = TextureName;
                }
            }

            private const string Txd = UIMenu.CommonTxd; // just an alias to reduce typing
            private static readonly Color White = System.Drawing.Color.White;

            // built-in badges
            public static BadgeInfo Blank => new BadgeInfo(BadgeStyle.Blank, null, null, null);
            public static BadgeInfo BronzeMedal => new BadgeInfo(BadgeStyle.BronzeMedal, Txd, "mp_medal_bronze", White);
            public static BadgeInfo GoldMedal => new BadgeInfo(BadgeStyle.GoldMedal, Txd, "mp_medal_gold", White);
            public static BadgeInfo SilverMedal => new BadgeInfo(BadgeStyle.SilverMedal, Txd, "medal_silver", White, sizeMultiplier: 0.5f);
            public static BadgeInfo Alert => new BadgeInfo(BadgeStyle.Alert, Txd, "mp_alerttriangle", White);
            public static BadgeInfo Crown => new BadgeInfo(BadgeStyle.Crown, Txd, "mp_hostcrown", null, sizeMultiplier: 0.5f);
            public static BadgeInfo Ammo => new BadgeInfo(BadgeStyle.Ammo, Txd, "shop_ammo_icon_a", White, Txd, "shop_ammo_icon_b");
            public static BadgeInfo Armour => new BadgeInfo(BadgeStyle.Armour, Txd, "shop_armour_icon_a", White, Txd, "shop_armour_icon_b");
            public static BadgeInfo Barber => new BadgeInfo(BadgeStyle.Barber, Txd, "shop_barber_icon_a", White, Txd, "shop_barber_icon_b");
            public static BadgeInfo Clothes => new BadgeInfo(BadgeStyle.Clothes, Txd, "shop_clothing_icon_a", White, Txd, "shop_clothing_icon_b");
            public static BadgeInfo Franklin => new BadgeInfo(BadgeStyle.Franklin, Txd, "shop_franklin_icon_a", White, Txd, "shop_franklin_icon_b");
            public static BadgeInfo Bike => new BadgeInfo(BadgeStyle.Bike, Txd, "shop_garage_bike_icon_a", White, Txd, "shop_garage_bike_icon_b");
            public static BadgeInfo Car => new BadgeInfo(BadgeStyle.Car, Txd, "shop_garage_icon_a", White, Txd, "shop_garage_icon_b");
            public static BadgeInfo Gun => new BadgeInfo(BadgeStyle.Gun, Txd, "shop_gunclub_icon_a", White, Txd, "shop_gunclub_icon_b");
            public static BadgeInfo Heart => new BadgeInfo(BadgeStyle.Heart, Txd, "shop_health_icon_a", White, Txd, "shop_health_icon_b");
            public static BadgeInfo Makeup => new BadgeInfo(BadgeStyle.Makeup, Txd, "shop_makeup_icon_a", White, Txd, "shop_makeup_icon_b");
            public static BadgeInfo Mask => new BadgeInfo(BadgeStyle.Mask, Txd, "shop_mask_icon_a", White, Txd, "shop_mask_icon_b");
            public static BadgeInfo Michael => new BadgeInfo(BadgeStyle.Michael, Txd, "shop_michael_icon_a", White, Txd, "shop_michael_icon_b");
            public static BadgeInfo Star => new BadgeInfo(BadgeStyle.Star, Txd, "shop_new_star", White);
            public static BadgeInfo Tatoo => new BadgeInfo(BadgeStyle.Tatoo, Txd, "shop_tattoos_icon_a", White, Txd, "shop_tattoos_icon_b");
            public static BadgeInfo Trevor => new BadgeInfo(BadgeStyle.Trevor, Txd, "shop_trevor_icon_a", White, Txd, "shop_trevor_icon_b");
            public static BadgeInfo Lock => new BadgeInfo(BadgeStyle.Lock, Txd, "shop_lock", null);
            public static BadgeInfo Tick => new BadgeInfo(BadgeStyle.Tick, Txd, "shop_tick_icon", null);
            public static BadgeInfo CardSuitClubs => new BadgeInfo(BadgeStyle.CardSuitClubs, Txd, "card_suit_clubs", null);
            public static BadgeInfo CardSuitDiamonds => new BadgeInfo(BadgeStyle.CardSuitDiamonds, Txd, "card_suit_diamonds", null);
            public static BadgeInfo CardSuitHearts => new BadgeInfo(BadgeStyle.CardSuitHearts, Txd, "card_suit_hearts", null);
            public static BadgeInfo CardSuitSpades => new BadgeInfo(BadgeStyle.CardSuitSpades, Txd, "card_suit_spades", null);
            public static BadgeInfo Art => new BadgeInfo(BadgeStyle.Art, Txd, "shop_art_icon_a", White, Txd, "shop_art_icon_b");

            public static BadgeInfo FromStyle(BadgeStyle style) => style switch
            {
                BadgeStyle.Blank => Blank,
                BadgeStyle.BronzeMedal => BronzeMedal,
                BadgeStyle.GoldMedal => GoldMedal,
                BadgeStyle.SilverMedal => SilverMedal,
                BadgeStyle.Alert => Alert,
                BadgeStyle.Crown => Crown,
                BadgeStyle.Ammo => Ammo,
                BadgeStyle.Armour => Armour,
                BadgeStyle.Barber => Barber,
                BadgeStyle.Clothes => Clothes,
                BadgeStyle.Franklin => Franklin,
                BadgeStyle.Bike => Bike,
                BadgeStyle.Car => Car,
                BadgeStyle.Gun => Gun,
                BadgeStyle.Heart => Heart,
                BadgeStyle.Makeup => Makeup,
                BadgeStyle.Mask => Mask,
                BadgeStyle.Michael => Michael,
                BadgeStyle.Star => Star,
                BadgeStyle.Tatoo => Tatoo,
                BadgeStyle.Trevor => Trevor,
                BadgeStyle.Lock => Lock,
                BadgeStyle.Tick => Tick,
                BadgeStyle.CardSuitClubs => CardSuitClubs,
                BadgeStyle.CardSuitDiamonds => CardSuitDiamonds,
                BadgeStyle.CardSuitHearts => CardSuitHearts,
                BadgeStyle.CardSuitSpades => CardSuitSpades,
                BadgeStyle.Art => Art,
                _ => throw new ArgumentException($"No built-in {nameof(BadgeInfo)} for style '{style}'", nameof(style))
            };
        }
    }
}

