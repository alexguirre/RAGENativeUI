using System;
using System.Drawing;

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
                                     DefaultHighlightedForeColor = Color.Black;

        /// <summary>
        /// Called when user selects the current item.
        /// </summary>
        public event ItemActivatedEvent Activated;


        public Color BackColor { get; set; } = DefaultBackColor;
        public Color HighlightedBackColor { get; set; } = DefaultHighlightedBackColor;

        public Color ForeColor { get; set; } = DefaultForeColor;
        public Color HighlightedForeColor { get; set; } = DefaultHighlightedForeColor;

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

        internal virtual void ItemActivate(UIMenu sender)
        {
            Activated?.Invoke(sender, this);
        }

        /// <summary>
        /// Set item's position.
        /// </summary>
        /// <param name="y"></param>
        [Obsolete("Use UIMenuItem.SetVerticalPosition instead.")]
        public virtual void Position(int y)
        {
            SetVerticalPosition(y);
        }

        /// <summary>
        /// Set item's vertical position.
        /// </summary>
        /// <param name="y"></param>
        [Obsolete("It is no longer allowed to change the position of a menu item. The position will be calculated by the menu when drawing the item.")]
        public virtual void SetVerticalPosition(int y)
        {
        }

        /// <summary>
        /// Draw this item.
        /// </summary>
        [Obsolete]
        public virtual void Draw()
        {
        }

        public virtual void Draw(float x, float y, float width, float height)
        {
            float rectWidth = width;
            float rectHeight = height;
            float rectX = x + rectWidth * 0.5f;
            float rectY = y + rectHeight * 0.5f;

            Color barColor = Selected ? HighlightedBackColor : BackColor;
            if (barColor != Color.Empty)
            {
                Parent.DrawSprite(UIMenu.CommonTxd, UIMenu.NavBarTextureName,
                                  rectX, rectY,
                                  rectWidth, rectHeight,
                                  barColor);
            }

            if (Hovered && !Selected)
            {
                Color hoveredColor = Color.FromArgb(25, 255, 255, 255);
                Parent.DrawRect(rectX, rectY - 0.00138888f * 0.5f,
                                rectWidth, rectHeight - 0.00138888f,
                                hoveredColor);
            }

            float leftBadgeOffset = 0.0f;
            float rightBadgeOffset = 0.0f;

            if (LeftBadge != BadgeStyle.None)
            {
                DrawBadge(LeftBadge, true, x, y, width, height, out leftBadgeOffset);
            }

            if (RightBadge != BadgeStyle.None)
            {
                DrawBadge(RightBadge, false, x, y, width, height, out rightBadgeOffset);
            }

            SetTextCommandOptions();
            N.BeginTextCommandDisplayText("STRING");
            N.AddTextComponentSubstringPlayerName(Text);
            N.EndTextCommandDisplayText(x + 0.0046875f + leftBadgeOffset, y + 0.00277776f);

            if (!String.IsNullOrEmpty(RightLabel))
            {
                SetTextCommandOptions();
                N.BeginTextCommandGetWidth("STRING");
                N.AddTextComponentSubstringPlayerName(RightLabel);
                float labelWidth = N.EndTextCommandGetWidth(true);

                float labelX = x + width - 0.00390625f - labelWidth - rightBadgeOffset;
                float labelY = y + 0.00277776f;;

                SetTextCommandOptions();
                N.BeginTextCommandDisplayText("STRING");
                N.AddTextComponentSubstringPlayerName(RightLabel);
                N.EndTextCommandDisplayText(labelX, labelY);
            }
        }

        internal void SetTextCommandOptions()
        {
            Color textColor = GetItemTextColor();
            N.SetTextColour(textColor.R, textColor.G, textColor.B, textColor.A);
            N.SetTextScale(0f, 0.35f);
            N.SetTextJustification(1);
            N.SetTextFont(0);
            N.SetTextWrap(0f, 1f);
            N.SetTextCentre(false);
            N.SetTextDropshadow(0, 0, 0, 0, 0);
            N.SetTextEdge(0, 0, 0, 0, 0);
        }

        private void DrawBadge(BadgeStyle badge, bool left, float itemX, float itemY, float itemW, float itemH, out float offsetX)
        {
            // Badges don't look exactly like how game menus do it, but close enough
            
            Color c = IsBagdeWhiteSprite(badge) ? GetItemTextColor() : Color.White;

            // use checkbox texture to have a constant size, since different badges have different texture resolution
            Parent.GetTextureDrawSize(UIMenu.CommonTxd, UIMenu.CheckboxTickTextureName, true, out float badgeW, out float badgeH, false);

            float sizeMult = BadgeToSizeMultiplier(badge);

            float badgeOffset = (badgeW * 0.5f) + BadgeToOffset(badge);
            float badgeX = left ?
                            itemX + badgeOffset :
                            itemX + itemW - badgeOffset;
            Parent.DrawSprite(BadgeToSpriteLib(badge), BadgeToSpriteName(badge, Selected),
                badgeX,
                itemY + (itemH * 0.5f),
                badgeW * sizeMult,
                badgeH * sizeMult,
                c);

            offsetX = badgeOffset + (0.00078125f * 8f);
        }

        internal Color GetItemTextColor()
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
                return Color.FromArgb(163, 159, 148);
            }
        }


        /// <summary>
        /// This item's offset.
        /// </summary>
        [Obsolete("It is no longer allowed to change the position of a menu item. The position will be calculated by the menu when drawing the item.")]
        public Point Offset { get; set; }


        /// <summary>
        /// Returns this item's label.
        /// </summary>
        public string Text { get; set; }


        /// <summary>
        /// Set the left badge. Set it to None to remove the badge.
        /// </summary>
        /// <param name="badge"></param>
        public virtual void SetLeftBadge(BadgeStyle badge)
        {
            LeftBadge = badge;
        }


        /// <summary>
        /// Set the right badge. Set it to None to remove the badge.
        /// </summary>
        /// <param name="badge"></param>
        public virtual void SetRightBadge(BadgeStyle badge)
        {
            RightBadge = badge;
        }


        /// <summary>
        /// Set the right label.
        /// </summary>
        /// <param name="text">Text as label. Set it to "" to remove the label.</param>
        public virtual void SetRightLabel(string text)
        {
            RightLabel = text;
        }

        /// <summary>
        /// Returns the current right label.
        /// </summary>
        public virtual string RightLabel { get; private set; }


        /// <summary>
        /// Returns the current left badge.
        /// </summary>
        public virtual BadgeStyle LeftBadge { get; private set; }


        /// <summary>
        /// Returns the current right badge.
        /// </summary>
        public virtual BadgeStyle RightBadge { get; private set; }

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
        }

        internal static string BadgeToSpriteLib(BadgeStyle badge)
        {
            switch (badge)
            {
                default:
                    return "commonmenu";
            }
        }

        internal static string BadgeToSpriteName(BadgeStyle badge, bool selected)
        {
            switch (badge)
            {
                case BadgeStyle.None:
                    return "";
                case BadgeStyle.BronzeMedal:
                    return "mp_medal_bronze";
                case BadgeStyle.GoldMedal:
                    return "mp_medal_gold";
                case BadgeStyle.SilverMedal:
                    return "medal_silver";
                case BadgeStyle.Alert:
                    return "mp_alerttriangle";
                case BadgeStyle.Crown:
                    return "mp_hostcrown";
                case BadgeStyle.Ammo:
                    return selected ? "shop_ammo_icon_b" : "shop_ammo_icon_a";
                case BadgeStyle.Armour:
                    return selected ? "shop_armour_icon_b" : "shop_armour_icon_a";
                case BadgeStyle.Barber:
                    return selected ? "shop_barber_icon_b" : "shop_barber_icon_a";
                case BadgeStyle.Clothes:
                    return selected ? "shop_clothing_icon_b" : "shop_clothing_icon_a";
                case BadgeStyle.Franklin:
                    return selected ? "shop_franklin_icon_b" : "shop_franklin_icon_a";
                case BadgeStyle.Bike:
                    return selected ? "shop_garage_bike_icon_b" : "shop_garage_bike_icon_a";
                case BadgeStyle.Car:
                    return selected ? "shop_garage_icon_b" : "shop_garage_icon_a";
                case BadgeStyle.Gun:
                    return selected ? "shop_gunclub_icon_b" : "shop_gunclub_icon_a";
                case BadgeStyle.Heart:
                    return selected ? "shop_health_icon_b" : "shop_health_icon_a";
                case BadgeStyle.Lock:
                    return "shop_lock";
                case BadgeStyle.Makeup:
                    return selected ? "shop_makeup_icon_b" : "shop_makeup_icon_a";
                case BadgeStyle.Mask:
                    return selected ? "shop_mask_icon_b" : "shop_mask_icon_a";
                case BadgeStyle.Michael:
                    return selected ? "shop_michael_icon_b" : "shop_michael_icon_a";
                case BadgeStyle.Star:
                    return "shop_new_star";
                case BadgeStyle.Tatoo:
                    return selected ? "shop_tattoos_icon_b" : "shop_tattoos_icon_a";
                case BadgeStyle.Tick:
                    return "shop_tick_icon";
                case BadgeStyle.Trevor:
                    return selected ? "shop_trevor_icon_b" : "shop_trevor_icon_a";
                case BadgeStyle.CardSuitClubs:
                    return "card_suit_clubs";
                case BadgeStyle.CardSuitDiamonds:
                    return "card_suit_diamonds";
                case BadgeStyle.CardSuitHearts:
                    return "card_suit_hearts";
                case BadgeStyle.CardSuitSpades:
                    return "card_suit_spades";
                default:
                    return "";
            }
        }

        internal static bool IsBagdeWhiteSprite(BadgeStyle badge)
        {
            switch (badge)
            {
                case BadgeStyle.Lock:
                case BadgeStyle.Tick:
                case BadgeStyle.Crown:
                case BadgeStyle.CardSuitClubs:
                case BadgeStyle.CardSuitDiamonds:
                case BadgeStyle.CardSuitHearts:
                case BadgeStyle.CardSuitSpades:
                    return true;
                default:
                    return false;
            }
        }

        internal static float BadgeToSizeMultiplier(BadgeStyle badge)
        {
            switch (badge)
            {
                case BadgeStyle.Crown:
                case BadgeStyle.SilverMedal:
                    return 0.5f;
                default:
                    return 1.0f;
            }
        }

        internal static float BadgeToOffset(BadgeStyle badge)
        {
            switch (badge)
            {
                case BadgeStyle.Ammo:
                case BadgeStyle.Armour:
                case BadgeStyle.Barber:
                case BadgeStyle.Clothes:
                case BadgeStyle.Franklin:
                case BadgeStyle.Bike:
                case BadgeStyle.Car:
                case BadgeStyle.Gun:
                case BadgeStyle.Heart:
                case BadgeStyle.Makeup:
                case BadgeStyle.Mask:
                case BadgeStyle.Michael:
                case BadgeStyle.Tatoo:
                case BadgeStyle.Trevor:
                    return (0.00078125f * 2.5f);
                default:
                    return -(0.00078125f * 1f);
            }
        }

        internal static Color BadgeToColor(BadgeStyle badge, bool selected)
        {
            if (IsBagdeWhiteSprite(badge))
            {
                return selected ? Color.FromArgb(255, 0, 0, 0) : Color.FromArgb(255, 255, 255, 255);
            }
            else
            {
                return Color.FromArgb(255, 255, 255, 255);
            }
        }

        /// <summary>
        /// Returns the menu this item is in.
        /// </summary>
        public UIMenu Parent { get; set; }
    }
}

