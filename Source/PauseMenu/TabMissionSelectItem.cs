using System;
using System.Collections.Generic;
using System.Drawing;
using Rage;
using Rage.Native;
using RAGENativeUI.Elements;

namespace RAGENativeUI.PauseMenu
{
    public delegate void OnItemSelect(MissionInformation selectedItem);

    public class MissionInformation
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
            _maxItem = MaxItemsPerView;
            _minItem = 0;

            CanBeFocused = true;

            Heists = new List<MissionInformation>(list);
        }

        public event OnItemSelect OnItemSelect;

        public List<MissionInformation> Heists { get; set; }
        public int Index { get; set; }

        public bool DynamicMissionWidth { get; set; } = false;
        public int MinMissionWidth { get; set; } = 512;
        public int MaxMissionWidth { get; set; } = 1440;
        public int MaxLogoHeight { get; set; } = 512;
        public int MinLabelWidth { get; set; } = 100;
        public int MaxLabelWidth { get; set; } = 512;

        protected const int MaxItemsPerView = 15;
        protected int _minItem;
        protected int _maxItem;
        protected int baseLogoWidth = 512;
        protected Size logoSize;
        protected Size logoArea;
        protected Sprite _noLogo { get; set; }

        public override void ProcessControls()
        {
            if (!Focused || Heists.Count == 0) return;
            if (JustOpened)
            {
                JustOpened = false;
                return;
            }

            if (Common.IsDisabledControlJustPressed(0, GameControl.CellphoneSelect))
            {
                NativeFunction.CallByName<uint>("PLAY_SOUND_FRONTEND", -1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", 1);
                OnItemSelect?.Invoke(Heists[Index]);
            }

            if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendUp) || Common.IsDisabledControlJustPressed(0, GameControl.MoveUpOnly))
            {
                Index = (1000 - (1000 % Heists.Count) + Index - 1) % Heists.Count;
                NativeFunction.CallByName<uint>("PLAY_SOUND_FRONTEND", -1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", 1);

                if (Heists.Count <= MaxItemsPerView) return;

                if (Index < _minItem)
                {
                    _minItem--;
                    _maxItem--;
                }

                if (Index == Heists.Count - 1)
                {
                    _minItem = Heists.Count - MaxItemsPerView;
                    _maxItem = Heists.Count;
                }
            }

            else if (Common.IsDisabledControlJustPressed(0, GameControl.FrontendDown) || Common.IsDisabledControlJustPressed(0, GameControl.MoveDownOnly))
            {
                Index = (1000 - (1000 % Heists.Count) + Index + 1) % Heists.Count;
                NativeFunction.CallByName<uint>("PLAY_SOUND_FRONTEND", -1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", 1);

                if (Heists.Count <= MaxItemsPerView) return;

                if (Index >= _maxItem)
                {
                    _maxItem++;
                    _minItem++;
                }

                if (Index == 0)
                {
                    _minItem = 0;
                    _maxItem = MaxItemsPerView;
                }
            }
        }

        SizeF res;
        public override void Draw()
        {
            base.Draw();
            if (Heists.Count == 0) return;
            
            res = UIMenu.GetScreenResolutionMantainRatio();
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

            var curHeist = Heists[Index];
            var logo = curHeist.Logo;
            bool logoLoaded = curHeist.Logo?.IsTextureLoaded ?? false;
            
            float heistDescHeight = 40 + (40 * curHeist.ValueList.Count);
            if (!string.IsNullOrWhiteSpace(curHeist.Description))
            {
                int lineCount = TextCommands.GetLineCount(curHeist.Description, new TextStyle(TextFont.ChaletLondon, Color.White, 0.35f, TextJustification.Left, 0, baseLogoWidth / 1920f), 0f, 0f);
                heistDescHeight += (lineCount * 25) + 20;
                // this is necessary to reset an issue with GetLineCount breaking the next legitimate text rendering if a description is present
                ResText.Draw("", Point.Empty, 0.01f, Color.FromArgb(0, 0, 0, 0), Common.EFont.ChaletLondon, false);
            }

            float logoRatio = logo?.Ratio ?? 2f; // width to height ratio of actual texture being rendered this frame
            float logoHeight = Math.Min(Math.Min(baseLogoWidth / logoRatio, activeHeight - heistDescHeight), MaxLogoHeight);
            float logoWidth = logoHeight * logoRatio;
            logoSize = new Size((int)logoWidth, (int)logoHeight);
            logoArea = new Size(baseLogoWidth, (int)logoHeight);

            var itemSize = new Size((int)activeWidth - logoArea.Width - 3, 40);

            var alpha = Focused ? 120 : 30;
            var blackAlpha = Focused ? 200 : 100;
            var fullAlpha = Focused ? 255 : 150;

            // mission list rendering
            var counter = 0;
            for (int i = _minItem; i < Math.Min(Heists.Count, _maxItem); i++)
            {
                string listItemName = Heists[i].Name;
                var nameLength = ResText.MeasureStringWidth(listItemName, Common.EFont.ChaletLondon, 0.35f);
                if(nameLength > itemSize.Width - 10)
                {
                    listItemName = listItemName.Substring(0, Math.Max(Math.Min(listItemName.Length, 10), (int)((itemSize.Width - 20) / nameLength * listItemName.Length))).Trim() + "...";
                }
                ResRectangle.Draw(SafeSize.AddPoints(new Point(0, (itemSize.Height + 3) * counter)), itemSize, (Index == i && Focused) ? Color.FromArgb(fullAlpha, Color.White) : Color.FromArgb(blackAlpha, Color.Black));
                ResText.Draw(listItemName, SafeSize.AddPoints(new Point(6, 5 + (itemSize.Height + 3) * counter)), 0.35f, Color.FromArgb(fullAlpha, (Index == i && Focused) ? Color.Black : Color.White), Common.EFont.ChaletLondon, false);
                counter++;
            }

            var logoPaddingLeft = (int)Math.Floor(0.5f * (logoArea.Width - logoSize.Width));
            var logoPaddingRight = (int)Math.Ceiling(0.5f * (logoArea.Width - logoSize.Width));
            var missionPoint = new Point(TopLeft.X + activeWidth - logoArea.Width, TopLeft.Y);

            // texture/logo rendering
            if (Heists[Index].Logo == null || !logoLoaded || (Heists[Index].Logo.Sprite == null && Heists[Index].Logo.Texture == null))
            {
                drawTexture = false;
                _noLogo.Size = logoSize;
                _noLogo.Position = missionPoint.AddPoints(new Point(logoPaddingLeft, 0));
                _noLogo.Color = Color.FromArgb(blackAlpha, 0, 0, 0);
                _noLogo.Draw();
            }
            else if (Heists[Index].Logo != null && Heists[Index].Logo.Texture != null && !Heists[Index].Logo.IsGameSprite)
            {
                drawTexture = true;
            }
            else if (Heists[Index].Logo != null && Heists[Index].Logo.Sprite != null && Heists[Index].Logo.IsGameSprite)
            {
                drawTexture = false;
                Sprite sprite = Heists[Index].Logo.Sprite;
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
            ResRectangle.Draw(missionPoint, new Size(logoPaddingLeft, logoArea.Height), Color.FromArgb(blackAlpha, Color.Black));
            ResRectangle.Draw(missionPoint.AddPoints(new Point(logoArea.Width - logoPaddingRight, 0)), new Size(logoPaddingRight, logoArea.Height), Color.FromArgb(blackAlpha, Color.Black));

            // description text and background rendering
            ResRectangle.Draw(missionPoint.AddPoints(new Point(0, logoArea.Height)), new Size(logoArea.Width, 40), Color.FromArgb(fullAlpha, Color.Black));
            ResText.Draw(Heists[Index].Name, missionPoint.AddPoints(new Point(logoArea.Width - 4, logoArea.Height + 4)), 0.5f, Color.FromArgb(fullAlpha, Color.White), Common.EFont.HouseScript, ResText.Alignment.Right, false, false, Size.Empty);

            // objective items rendering
            for (int i = 0; i < Heists[Index].ValueList.Count; i++)
            {
                ResRectangle.Draw(missionPoint.AddPoints(new Point(0, logoArea.Height + 40 * (i + 1))), new Size(logoArea.Width, 40), i % 2 == 0 ? Color.FromArgb(alpha, 0, 0, 0) : Color.FromArgb(blackAlpha, 0, 0, 0));
                var text = Heists[Index].ValueList[i].Item1;
                var label = Heists[Index].ValueList[i].Item2;

                ResText.Draw(text, missionPoint.AddPoints(new Point(6, logoArea.Height + 6 + 40 * (i + 1))), 0.35f, Color.FromArgb(fullAlpha, Color.White), Common.EFont.ChaletLondon, false);
                ResText.Draw(label, missionPoint.AddPoints(new Point(logoArea.Width - 6, logoArea.Height + 6 + 40 * (i + 1))), 0.35f, Color.FromArgb(fullAlpha, Color.White), Common.EFont.ChaletLondon, ResText.Alignment.Right, false, false, Size.Empty);
            }

            // description rendering
            if (!string.IsNullOrEmpty(Heists[Index].Description))
            {
                var propLen = Heists[Index].ValueList.Count;
                var propBuffer = 40 * (1 + propLen);

                ResRectangle.Draw(missionPoint.AddPoints(new Point(0, logoArea.Height + 2 + propBuffer)), new Size(logoArea.Width, 2), Color.FromArgb(fullAlpha, Color.White));
                ResText.Draw(Heists[Index].Description, missionPoint.AddPoints(new Point(4, logoArea.Height + 9 + propBuffer)), 0.35f, Color.FromArgb(fullAlpha, Color.White), Common.EFont.ChaletLondon, ResText.Alignment.Left, false, false, new Size((logoArea.Width - 4), 0));

                int lineCount = TextCommands.GetLineCount(Heists[Index].Description, new TextStyle(TextFont.ChaletLondon, Color.White, 0.35f, TextJustification.Left, 0, logoArea.Width / 1920f), 0f, 0f);
                ResRectangle.Draw(missionPoint.AddPoints(new Point(0, logoArea.Height + 4 + propBuffer)), new Size(logoArea.Width, 25 * lineCount + 20), Color.FromArgb(blackAlpha, 0, 0, 0));
            }
        }

        private bool drawTexture = false;
        public override void DrawTextures(Rage.Graphics g)
        {
            if (drawTexture && Heists[Index].Logo?.Texture != null)
            {
                int logoPadding = (int)(0.5f * (logoArea.Width - logoSize.Width));
                var pos = new Point((int)res.Width - SafeSize.X - logoArea.Width + logoPadding, SafeSize.Y);
                Sprite.DrawTexture(Heists[Index].Logo.Texture, pos, logoSize, g);
            }
        }
    }
}

