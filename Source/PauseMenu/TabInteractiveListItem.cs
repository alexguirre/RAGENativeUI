using System;
using System.Collections.Generic;
using System.Drawing;
using Rage;
using RAGENativeUI.Elements;

namespace RAGENativeUI.PauseMenu
{
    public class TabInteractiveListItem : TabItem
    {
        public TabInteractiveListItem(string name, IEnumerable<UIMenuItem> items) : base(name)
        {
            DrawBg = false;
            CanBeFocused = true;
            Items = new List<UIMenuItem>(items);
            IsInList = true;
            minItem = 0;
            maxItem = MaxItemsPerView;
        }

        public List<UIMenuItem> Items { get; set; }
        public int Index { get; set; }
        public bool IsInList { get; set; }
        protected const int MaxItemsPerView = 15;
        protected int minItem;
        protected int maxItem;
        private bool focused;

        public void MoveDown()
        {
            Index = (1000 - (1000 % Items.Count) + Index + 1) % Items.Count;

            if (Items.Count <= MaxItemsPerView) return;

            if (Index >= maxItem)
            {
                maxItem++;
                minItem++;
            }

            if (Index == 0)
            {
                minItem = 0;
                maxItem = MaxItemsPerView;
            }
        }

        public void MoveUp()
        {
            Index = (1000 - (1000 % Items.Count) + Index - 1) % Items.Count;

            if (Items.Count <= MaxItemsPerView) return;

            if (Index < maxItem)
            {
                maxItem--;
                minItem--;
            }

            if (Index == Items.Count - 1)
            {
                minItem = Items.Count - MaxItemsPerView;
                maxItem = Items.Count;
            }
        }


        public void RefreshIndex()
        {
            Index = 0;
            minItem = 0;
            maxItem = MaxItemsPerView;
        }

        public override void ProcessControls()
        {
            if (!Visible)
                return;
            if (JustOpened)
            {
                JustOpened = false;
                return;
            }

            if (!Focused)
                return;

            if (Items.Count == 0)
                return;


            if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendAccept) && Focused && Items[Index] is UIMenuCheckboxItem)
            {
                Common.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                UIMenuCheckboxItem cb = (UIMenuCheckboxItem)Items[Index];
                cb.Checked = !cb.Checked;
                cb.CheckboxEventTrigger();
            }
            else if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendAccept) && Focused)
            {
                Common.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                Items[Index].ItemActivate(null);
            }


            if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendLeft) && Focused && Items[Index] is UIMenuListItem)
            {
                var it = (UIMenuListItem)Items[Index];
                it.Index--;
                Common.PlaySound("NAV_LEFT_RIGHT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                it.ListChangedTrigger(it.Index);
            }


            if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendRight) && Focused && Items[Index] is UIMenuListItem)
            {
                var it = (UIMenuListItem)Items[Index];
                it.Index++;
                Common.PlaySound("NAV_LEFT_RIGHT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                it.ListChangedTrigger(it.Index);
            }


            if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendUp) || Common.IsDisabledControlJustPressed(0, GameControl.MoveUpOnly) || Common.IsDisabledControlJustPressed(0, GameControl.CursorScrollUp))
            {
                Common.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                MoveUp();
            }
            else if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendDown) || Common.IsDisabledControlJustPressed(0, GameControl.MoveDownOnly) || Common.IsDisabledControlJustPressed(0, GameControl.CursorScrollDown))
            {
                Common.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                MoveDown();
            }
        }

        public override void Draw()
        {
            if (!Visible)
                return;

            base.Draw();
            
            int blackAlpha = Focused ? 200 : 100;
            int fullAlpha = Focused ? 255 : 150;

            int subMenuWidth = (BottomRight.X - TopLeft.X);
            Size itemSize = new Size(subMenuWidth, 40);

            int i = 0;
            for (int c = minItem; c < Math.Min(Items.Count, maxItem); c++)
            {
                //bool hovering = UIMenu.IsMouseInBounds(SafeSize.AddPoints(new Point(0, (itemSize.Height + 3) * i)), itemSize);

                bool hasLeftBadge = Items[c].LeftBadge != UIMenuItem.BadgeStyle.None;
                bool hasRightBadge = Items[c].RightBadge != UIMenuItem.BadgeStyle.None;

                bool hasBothBadges = hasRightBadge && hasLeftBadge;
                bool hasAnyBadge = hasRightBadge || hasLeftBadge;

                ResRectangle.Draw(SafeSize.AddPoints(new Point(0, (itemSize.Height + 3) * i)), itemSize, (Index == c && Focused) ? Color.FromArgb(fullAlpha, Color.White) : /*Focused && hovering ? Color.FromArgb(100, 50, 50, 50) :*/ Color.FromArgb(blackAlpha, Color.Black));
                ResText.Draw(Items[c].Text, SafeSize.AddPoints(new Point((hasBothBadges ? 60 : hasAnyBadge ? 30 : 6), 5 + (itemSize.Height + 3) * i)), 0.35f, Color.FromArgb(fullAlpha, (Index == c && Focused) ? Color.Black : Color.White), Common.EFont.ChaletLondon, false);

                if (hasLeftBadge && !hasRightBadge)
                {
                    Sprite.Draw(UIMenuItem.BadgeToSpriteLib(Items[c].LeftBadge), 
                                UIMenuItem.BadgeToSpriteName(Items[c].LeftBadge, (Index == c && Focused)), SafeSize.AddPoints(new Point(-2, 1 + (itemSize.Height + 3) * i)), new Size(40, 40), 0f,
                                UIMenuItem.BadgeToColor(Items[c].LeftBadge, (Index == c && Focused)));
                }

                if (!hasLeftBadge && hasRightBadge)
                {
                    Sprite.Draw(UIMenuItem.BadgeToSpriteLib(Items[c].RightBadge),
                                UIMenuItem.BadgeToSpriteName(Items[c].RightBadge, (Index == c && Focused)), SafeSize.AddPoints(new Point(-2, 1 + (itemSize.Height + 3) * i)), new Size(40, 40), 0f,
                                UIMenuItem.BadgeToColor(Items[c].RightBadge, (Index == c && Focused)));
                }

                if (hasLeftBadge && hasRightBadge)
                {
                    Sprite.Draw(UIMenuItem.BadgeToSpriteLib(Items[c].LeftBadge),
                                UIMenuItem.BadgeToSpriteName(Items[c].LeftBadge, (Index == c && Focused)), SafeSize.AddPoints(new Point(-2, 1 + (itemSize.Height + 3) * i)), new Size(40, 40), 0f,
                                UIMenuItem.BadgeToColor(Items[c].LeftBadge, (Index == c && Focused)));

                    Sprite.Draw(UIMenuItem.BadgeToSpriteLib(Items[c].RightBadge),
                                UIMenuItem.BadgeToSpriteName(Items[c].RightBadge, (Index == c && Focused)), SafeSize.AddPoints(new Point(25, 1 + (itemSize.Height + 3) * i)), new Size(40, 40), 0f,
                                UIMenuItem.BadgeToColor(Items[c].RightBadge, (Index == c && Focused)));
                }

                if (!String.IsNullOrEmpty(Items[c].RightLabel))
                {
                    ResText.Draw(Items[c].RightLabel,
                                SafeSize.AddPoints(new Point(BottomRight.X - SafeSize.X - 5, 5 + (itemSize.Height + 3) * i)),
                                0.35f, Color.FromArgb(fullAlpha, (Index == c && Focused) ? Color.Black : Color.White),
                                Common.EFont.ChaletLondon, ResText.Alignment.Right, false, false, Size.Empty);
                }

                if (Items[c] is UIMenuCheckboxItem)
                {
                    string textureName;
                    if (c == Index && Focused)
                    {
                        textureName = ((UIMenuCheckboxItem)Items[c]).Checked ? "shop_box_tickb" : "shop_box_blankb";
                    }
                    else
                    {
                        textureName = ((UIMenuCheckboxItem)Items[c]).Checked ? "shop_box_tick" : "shop_box_blank";
                    }
                    Sprite.Draw("commonmenu", textureName, SafeSize.AddPoints(new Point(BottomRight.X - SafeSize.X - 60, -5 + (itemSize.Height + 3) * i)), new Size(50, 50), 0f, Color.White);
                }
                else if (Items[c] is UIMenuListItem)
                {
                    var convItem = (UIMenuListItem)Items[c];

                    var yoffset = 5;
                    var basePos =
                        SafeSize.AddPoints(new Point(BottomRight.X - SafeSize.X - 30, yoffset + (itemSize.Height + 3) * i));

                    var arrowLeft = new Sprite("commonmenu", "arrowleft", basePos, new Size(30, 30));
                    var arrowRight = new Sprite("commonmenu", "arrowright", basePos, new Size(30, 30));
                    var itemText = new ResText("", basePos, 0.35f, Color.White, Common.EFont.ChaletLondon, ResText.Alignment.Right);

                    string caption = (convItem.Collection == null ? convItem.IndexToItem(convItem.Index) : convItem.Collection[convItem.Index]).ToString();
                    int offset = StringMeasurer.MeasureString(caption);

                    var selected = c == Index && Focused;

                    itemText.Color = convItem.Enabled ? selected ? Color.Black : Color.WhiteSmoke : Color.FromArgb(163, 159, 148);

                    itemText.Caption = caption;

                    arrowLeft.Color = convItem.Enabled ? selected ? Color.Black : Color.WhiteSmoke : Color.FromArgb(163, 159, 148);
                    arrowRight.Color = convItem.Enabled ? selected ? Color.Black : Color.WhiteSmoke : Color.FromArgb(163, 159, 148);

                    arrowLeft.Position = SafeSize.AddPoints(new Point(BottomRight.X - SafeSize.X - 60 - offset, yoffset + (itemSize.Height + 3) * i));
                    if (selected)
                    {
                        arrowLeft.Draw();
                        arrowRight.Draw();
                        itemText.Position = SafeSize.AddPoints(new Point(BottomRight.X - SafeSize.X - 30, yoffset + (itemSize.Height + 3) * i));
                    }
                    else
                    {
                        itemText.Position = SafeSize.AddPoints(new Point(BottomRight.X - SafeSize.X - 5, yoffset + (itemSize.Height + 3) * i));
                    }

                    itemText.Draw();
                }

                //if (Focused && hovering && (Common.IsDisabledControlJustPressed(0, GameControl.CursorAccept) || Game.IsControlJustPressed(0, GameControl.CursorAccept)))
                //{
                //    bool open = Index == c;
                //    Index = (1000 - (1000 % Items.Count) + c) % Items.Count;
                //    if (!open)
                //        Common.PlaySound("NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                //    else
                //    {
                //        if (Items[Index] is UIMenuCheckboxItem)
                //        {
                //            Common.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                //            UIMenuCheckboxItem cb = (UIMenuCheckboxItem)Items[Index];
                //            cb.Checked = !cb.Checked;
                //            cb.CheckboxEventTrigger();
                //        }
                //        else
                //        {
                //            Common.PlaySound("SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET");
                //            Items[Index].ItemActivate(null);
                //        }
                //    }
                //}

                i++;
            }
        }
    }
}
