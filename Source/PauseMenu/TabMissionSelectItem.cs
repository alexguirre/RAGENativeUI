using System;
using System.Collections.Generic;
using System.Drawing;
using Rage;
using Rage.Native;
using RAGENativeUI.Elements;

namespace RAGENativeUI.PauseMenu
{
    public delegate void OnItemSelect(MissionInformation selectedItem);

    public class MissionInformation : IScrollableListItem
    {
        public MissionInformation(string name, IEnumerable<Tuple<string, string>> info)
        {
            Name = name;
            ValueList = new List<Tuple<string, string>>(info);
        }

        public MissionInformation(string name, string description, IEnumerable<Tuple<string, string>> info)
        {
            Name = name;
            Description = description;
            ValueList = new List<Tuple<string, string>>(info);
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public MissionLogo Logo { get; set; }
        public List<Tuple<string, string>> ValueList { get; set; }
        public bool Selected { get; set; }
        public bool Skipped { get; set; }
    }

    public class MissionLogo
    {
        /// <summary>
        /// Create a logo from an external picture.
        /// </summary>
        /// <param name="filepath">Path to the picture</param>
        public MissionLogo(Texture texture)
        {
            Texture = texture;
            IsGameSprite = false;
        }

        /// <summary>
        /// Create a mission logo from a game texture.
        /// </summary>
        /// <param name="textureDict">Name of the texture dictionary</param>
        /// <param name="textureName">Name of the texture.</param>
        public MissionLogo(string textureDict, string textureName)
        {
            Sprite = new Sprite(textureDict, textureName, Point.Empty, Size.Empty);
            IsGameSprite = true;
        }

        internal readonly bool IsGameSprite;
        internal readonly Texture Texture;
        internal readonly Sprite Sprite;

        private Size spriteResolution;
        public Size Resolution
        {
            get
            {
                if (!IsGameSprite) return Texture.Size;

                if (spriteResolution == default)
                {
                    var res = N.GetTextureResolution(Sprite.TextureDictionary, Sprite.TextureName);
                    if (res.X != 4 || res.Y != 4) // GetTextureResolution returns (4, 4, 0) if texture isn't loaded
                    {
                        spriteResolution = new Size((int)res.X, (int)res.Y);
                    }
                }

                return spriteResolution;
            }
        }

        public float Ratio
        {
            get
            {
                var res = Resolution;
                if (res == default) return 2.0f;

                return (float)res.Width / (float)res.Height;
            }
        }

        public bool IsTextureLoaded
        {
            get
            {
                var res = Resolution;
                return (res.Width > 0 && res.Height > 0 && !(res.Width == 4 && res.Height == 4));
            }
        }
    }

    public class TabMissionSelectItem : TabItem
    {
        public TabMissionSelectItem(string name, IEnumerable<MissionInformation> list) : base(name)
        {
            base.FadeInWhenFocused = true;
            base.DrawBg = false;

            _noLogo = new Sprite("gtav_online", "rockstarlogo256", new Point(), new Size(512, 256));

            CanBeFocused = true;

            Heists = new List<MissionInformation>(list);
            RefreshIndex();
        }

        private class ScrollableMissionList : ScrollableListBase<MissionInformation>
        {
            protected override List<MissionInformation> Items { get; set; } = new List<MissionInformation>();
            public List<MissionInformation> Heists
            {
                get => Items;
                set => Items = value;
            }

            public override int MaxItemsOnScreen 
            { 
                get => TabView.MaxTabRowItemsOnScreen; 
                set { } 
            }
        }

        public event OnItemSelect OnItemSelect;

        private ScrollableMissionList missions = new ScrollableMissionList();
        public List<MissionInformation> Heists
        {
            get => missions.Heists;
            set => missions.Heists = value;
        }
        public int Index
        {
            get => missions.CurrentSelection;
            set => missions.CurrentSelection = value;
        }

        public float DefaultLogoRatio { get; set; } = 2f;
        public bool DynamicLogoRatio { get; set; } = true;
        public bool DynamicMissionWidth { get; set; } = false;
        public int MinMissionWidth { get; set; } = 512;
        public int MaxMissionWidth { get; set; } = 1440;
        public int MaxLogoHeight { get; set; } = 512;
        public int MinLabelWidth { get; set; } = 100;
        public int MaxLabelWidth { get; set; } = 512;

        [Obsolete("Item scrolling is now handled automatically", true)]
        protected int _minItem;
        [Obsolete("Item scrolling is now handled automatically", true)]
        protected int _maxItem;

        protected int baseLogoWidth = 512;
        protected Size logoSize;
        protected Size logoArea;
        protected int logoPaddingLeft;
        protected int logoPaddingRight;
        protected Sprite _noLogo { get; set; }
        protected Point missionPoint;

        public void RefreshIndex() => missions.RefreshIndex();

        public override void ProcessControls()
        {
            if (!Focused || Heists.Count == 0) return;
            if (JustOpened)
            {
                JustOpened = false;
                return;
            }

            // reset index if it's an invalid value
            if (Index < 0 || Index >= Heists.Count)
                missions.RefreshIndex();

            if (Common.IsDisabledControlJustPressed(0, GameControl.CellphoneSelect))
            {
                TabView.PlaySelectSound();
                OnItemSelect?.Invoke(Heists[Index]);
            }

            if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendUp) || Common.IsDisabledControlJustPressed(0, GameControl.MoveUpOnly))
            {
                missions.MoveToPreviousItem();
                TabView.PlayNavUpDownSound();
            }

            else if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendDown) || Common.IsDisabledControlJustPressed(0, GameControl.MoveDownOnly))
            {
                missions.MoveToNextItem();
                TabView.PlayNavUpDownSound();
            }
        }

        public override void Draw()
        {
            base.Draw();
            if (Heists.Count == 0) return;
            
            var res = UIMenu.GetScreenResolutionMantainRatio();
            var activeWidth = BottomRight.X - TopLeft.X;
            var activeHeight = BottomRight.Y - TopLeft.Y;

            if (DynamicMissionWidth)
            {
                // ensure logo height does not exceed safe zone, logo width is over min but optimized to text width

                float maxLabelWidth = 30;

                foreach (var heist in Heists)
                {
                    maxLabelWidth = Math.Max(maxLabelWidth, ResText.MeasureStringWidth(heist.Name, Common.EFont.ChaletLondon, 0.35f));
                }

                maxLabelWidth = Math.Min(MaxLabelWidth, Math.Max(MinLabelWidth, maxLabelWidth + 20));
                baseLogoWidth = (int)Math.Min(MaxMissionWidth, Math.Max(activeWidth - maxLabelWidth, MinMissionWidth));
            }

            var itemSize = new Size((int)activeWidth - baseLogoWidth - 3, 40);
            var alpha = Focused ? 120 : 30;
            var blackAlpha = Focused ? 200 : 100;
            var fullAlpha = Focused ? 255 : 150;

            // mission list rendering
            foreach ((int iterIndex, int heistIndex, MissionInformation heist, bool heistSelected) in missions.IterateVisibleItems())
            {
                string listItemName = heist.Name;
                var nameLength = ResText.MeasureStringWidth(listItemName, Common.EFont.ChaletLondon, 0.35f);
                if (nameLength > itemSize.Width - 10)
                {
                    listItemName = listItemName.Substring(0, Math.Max(Math.Min(listItemName.Length, 10), (int)((itemSize.Width - 20) / nameLength * listItemName.Length))).Trim() + "...";
                }
                ResRectangle.Draw(SafeSize.AddPoints(new Point(0, (itemSize.Height + 3) * iterIndex)), itemSize, (heistSelected && Focused) ? Color.FromArgb(fullAlpha, Color.White) : Color.FromArgb(blackAlpha, Color.Black));
                ResText.Draw(listItemName, SafeSize.AddPoints(new Point(6, 5 + (itemSize.Height + 3) * iterIndex)), 0.35f, Color.FromArgb(fullAlpha, (heistSelected && Focused) ? Color.Black : Color.White), Common.EFont.ChaletLondon, false);
            }

            if (Index >= 0 && Index < Heists.Count)
            {
                var curHeist = Heists[Index];
                var logo = curHeist.Logo;
                bool logoLoaded = curHeist.Logo?.IsTextureLoaded ?? false;

                int heistNameHeight = 40 * TextCommands.GetLineCount(curHeist.Name ?? " ", new TextStyle(TextFont.HouseScript, Color.White, 0.5f, TextJustification.Right, 0, baseLogoWidth / 1920f), 0f, 0f);
                int descLineHeight = string.IsNullOrWhiteSpace(curHeist.Description) ? 0 : 20 + 25 * TextCommands.GetLineCount(curHeist.Description, new TextStyle(TextFont.ChaletLondon, Color.White, 0.35f, TextJustification.Left, 0, baseLogoWidth / 1920f), 0f, 0f);
                int propListHeight = 40 * curHeist.ValueList.Count;

                float heistDescHeight = heistNameHeight + propListHeight + descLineHeight;

                // this is necessary to reset an issue with GetLineCount breaking the next legitimate text rendering if a description is present
                // ResText.Draw("", Point.Empty, 0.01f, Color.FromArgb(0, 0, 0, 0), Common.EFont.ChaletLondon, false);

                float logoRatio = (DynamicLogoRatio && logo != null) ? logo.Ratio : DefaultLogoRatio; // width to height ratio of actual texture being rendered this frame
                float logoHeight = Math.Min(Math.Min(baseLogoWidth / logoRatio, activeHeight - heistDescHeight), MaxLogoHeight);
                float logoWidth = logoHeight * logoRatio;
                logoSize = new Size((int)logoWidth, (int)logoHeight);
                logoArea = new Size(baseLogoWidth, (int)logoHeight);

                logoPaddingLeft = DynamicLogoRatio ? (int)Math.Floor(0.5f * (logoArea.Width - logoSize.Width)) : 0;
                logoPaddingRight = DynamicLogoRatio ? (int)Math.Ceiling(0.5f * (logoArea.Width - logoSize.Width)) : 0;
                missionPoint = new Point(TopLeft.X + activeWidth - logoArea.Width, TopLeft.Y);

                // texture/logo rendering
                if (curHeist.Logo == null || !logoLoaded || (curHeist.Logo.Sprite == null && curHeist.Logo.Texture == null))
                {
                    drawTexture = false;
                    _noLogo.Size = logoSize;
                    _noLogo.Position = missionPoint.AddPoints(new Point(logoPaddingLeft, 0));
                    _noLogo.Color = Color.FromArgb(blackAlpha, 0, 0, 0);
                    _noLogo.Draw();
                }
                else if (curHeist.Logo != null && curHeist.Logo.Texture != null && !curHeist.Logo.IsGameSprite)
                {
                    drawTexture = true;
                }
                else if (curHeist.Logo != null && curHeist.Logo.Sprite != null && curHeist.Logo.IsGameSprite)
                {
                    drawTexture = false;
                    Sprite sprite = curHeist.Logo.Sprite;
                    sprite.Position = missionPoint.AddPoints(new Point(logoPaddingLeft, 0));
                    sprite.Size = logoSize;
                    sprite.Color = Color.FromArgb(blackAlpha, 255, 255, 255);
                    sprite.Draw();
                }
                else
                {
                    drawTexture = false;
                }

                // padding around texture background
                if (DynamicLogoRatio)
                {
                    ResRectangle.Draw(missionPoint, new Size(logoPaddingLeft, logoArea.Height), Color.FromArgb(blackAlpha, Color.Black));
                    ResRectangle.Draw(missionPoint.AddPoints(new Point(logoArea.Width - logoPaddingRight, 0)), new Size(logoPaddingRight, logoArea.Height), Color.FromArgb(blackAlpha, Color.Black));
                }

                // description text and background rendering
                ResRectangle.Draw(missionPoint.AddPoints(new Point(0, logoArea.Height)), new Size(logoArea.Width, heistNameHeight), Color.FromArgb(fullAlpha, Color.Black));
                ResText.Draw(curHeist.Name, missionPoint.AddPoints(new Point(0, logoArea.Height + 4)), 0.5f, Color.FromArgb(fullAlpha, Color.White), Common.EFont.HouseScript, ResText.Alignment.Right, false, false, new Size((logoArea.Width - 4), 0));

                // objective items rendering
                for (int i = 0; i < curHeist.ValueList.Count; i++)
                {
                    int heightBuffer = logoArea.Height + heistNameHeight + 40 * i;
                    ResRectangle.Draw(missionPoint.AddPoints(new Point(0, heightBuffer)), new Size(logoArea.Width, 40), i % 2 == 0 ? Color.FromArgb(alpha, 0, 0, 0) : Color.FromArgb(blackAlpha, 0, 0, 0));
                    ResText.Draw(curHeist.ValueList[i].Item1, missionPoint.AddPoints(new Point(6, heightBuffer + 6)), 0.35f, Color.FromArgb(fullAlpha, Color.White), Common.EFont.ChaletLondon, false);
                    ResText.Draw(curHeist.ValueList[i].Item2, missionPoint.AddPoints(new Point(logoArea.Width - 6, heightBuffer + 6)), 0.35f, Color.FromArgb(fullAlpha, Color.White), Common.EFont.ChaletLondon, ResText.Alignment.Right, false, false, Size.Empty);
                }

                // description rendering
                if (!string.IsNullOrEmpty(curHeist.Description))
                {
                    int heightBuffer = heistNameHeight + propListHeight;

                    ResRectangle.Draw(missionPoint.AddPoints(new Point(0, logoArea.Height + 2 + heightBuffer)), new Size(logoArea.Width, 2), Color.FromArgb(fullAlpha, Color.White));
                    ResText.Draw(curHeist.Description, missionPoint.AddPoints(new Point(4, logoArea.Height + 9 + heightBuffer)), 0.35f, Color.FromArgb(fullAlpha, Color.White), Common.EFont.ChaletLondon, ResText.Alignment.Left, false, false, new Size((logoArea.Width - 4), 0));

                    ResRectangle.Draw(missionPoint.AddPoints(new Point(0, logoArea.Height + 4 + heightBuffer)), new Size(logoArea.Width, descLineHeight), Color.FromArgb(blackAlpha, 0, 0, 0));
                }
            } else
            {
                RefreshIndex();
            }
        }

        private bool drawTexture = false;
        public override void DrawTextures(Rage.Graphics g)
        {
            if (drawTexture && Index >= 0 && Index < Heists.Count && Heists[Index].Logo?.Texture != null)
            {
                var pos = missionPoint.AddPoints(new Point(logoPaddingLeft, 0));
                Sprite.DrawTexture(Heists[Index].Logo.Texture, pos, logoSize, g);
            }
        }
    }
}

