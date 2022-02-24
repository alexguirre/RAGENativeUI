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
                    if (res.X > 4 && res.Y > 4) // GetTextureResolution returns (4, 4, 0) if texture isn't loaded
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

        public bool DynamicMissionWidth { get; set; } = true;
        public int MinMissionWidth { get; set; } = 512;
        public int MinLabelWidth { get; set; } = 100;

        protected const int MaxItemsPerView = 15;
        protected int _minItem;
        protected int _maxItem;
        protected Size logoSize = new Size(512, 256);
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
            var activeWidth = res.Width - SafeSize.X * 2;
            var activeHeight = res.Height - SafeSize.Y * 2;

            if(DynamicMissionWidth)
            {
                float maxLabelWidth = 30;
                float maxItemDescHeight = 40;

                foreach (var heist in Heists)
                {
                    maxLabelWidth = Math.Max(maxLabelWidth, ResText.MeasureStringWidth(heist.Name, Common.EFont.ChaletLondon, 0.35f));

                    float heistDescHeight = 40 + (40 * heist.ValueList.Count);
                    if (!string.IsNullOrWhiteSpace(heist.Description))
                    {
                        heistDescHeight += TextCommands.GetLineCount(heist.Description, 0.0f, 0.0f) + 4;
                    }
                    maxItemDescHeight = Math.Max(maxItemDescHeight, heistDescHeight);
                }

                // ensure logo height does not exceed safe zone, logo width is over min but optimized to text width
                maxLabelWidth = Math.Max(MinLabelWidth, maxLabelWidth + 20);
                var logoWidth = Math.Max(activeWidth - maxLabelWidth, MinMissionWidth);
                var logoHeight = Math.Min(0.5f * logoWidth, activeHeight - maxItemDescHeight);
                logoWidth = logoHeight * 2.0f;
                logoSize = new Size((int)logoWidth, (int)logoHeight);
            }
            
            var itemSize = new Size((int)activeWidth - logoSize.Width - 3, 40);

            var alpha = Focused ? 120 : 30;
            var blackAlpha = Focused ? 200 : 100;
            var fullAlpha = Focused ? 255 : 150;

            var counter = 0;
            for (int i = _minItem; i < Math.Min(Heists.Count, _maxItem); i++)
            {
                ResRectangle.Draw(SafeSize.AddPoints(new Point(0, (itemSize.Height + 3) * counter)), itemSize, (Index == i && Focused) ? Color.FromArgb(fullAlpha, Color.White) : Color.FromArgb(blackAlpha, Color.Black));
                ResText.Draw(Heists[i].Name, SafeSize.AddPoints(new Point(6, 5 + (itemSize.Height + 3) * counter)), 0.35f, Color.FromArgb(fullAlpha, (Index == i && Focused) ? Color.Black : Color.White), Common.EFont.ChaletLondon, false);
                counter++;
            }

            if (Heists[Index].Logo == null || (Heists[Index].Logo.Sprite == null && Heists[Index].Logo.Texture == null))
            {
                drawTexture = false;
                _noLogo.Size = logoSize;
                _noLogo.Position = new Point((int)res.Width - SafeSize.X - logoSize.Width, SafeSize.Y);
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
                sprite.Position = new Point((int)res.Width - SafeSize.X - logoSize.Width, SafeSize.Y);
                sprite.Size = logoSize;
                sprite.Color = Color.FromArgb(blackAlpha, 255, 255, 255);
                sprite.Draw();
            }
            else
            {
                drawTexture = false;
            }

            ResRectangle.Draw(new Point((int)res.Width - SafeSize.X - logoSize.Width, SafeSize.Y + logoSize.Height), new Size(logoSize.Width, 40), Color.FromArgb(fullAlpha, Color.Black));
            ResText.Draw(Heists[Index].Name, new Point((int)res.Width - SafeSize.X - 4, SafeSize.Y + (logoSize.Height + 4)), 0.5f, Color.FromArgb(fullAlpha, Color.White), Common.EFont.HouseScript, ResText.Alignment.Right, false, false, Size.Empty);

            for (int i = 0; i < Heists[Index].ValueList.Count; i++)
            {
                ResRectangle.Draw(new Point((int)res.Width - SafeSize.X - logoSize.Width, SafeSize.Y + logoSize.Height + 40 + (40 * i)), new Size(logoSize.Width, 40), i % 2 == 0 ? Color.FromArgb(alpha, 0, 0, 0) : Color.FromArgb(blackAlpha, 0, 0, 0));
                var text = Heists[Index].ValueList[i].Item1;
                var label = Heists[Index].ValueList[i].Item2;


                ResText.Draw(text, new Point((int)res.Width - SafeSize.X - (logoSize.Width - 6), SafeSize.Y + (logoSize.Height + 4) + 42 + (40 * i)), 0.35f, Color.FromArgb(fullAlpha, Color.White), Common.EFont.ChaletLondon, false);
                ResText.Draw(label, new Point((int)res.Width - SafeSize.X - 6, SafeSize.Y + (logoSize.Height + 4) + 42 + (40 * i)), 0.35f, Color.FromArgb(fullAlpha, Color.White), Common.EFont.ChaletLondon, ResText.Alignment.Right, false, false, Size.Empty);
            }

            if (!string.IsNullOrEmpty(Heists[Index].Description))
            {
                var propLen = Heists[Index].ValueList.Count;
                ResRectangle.Draw(new Point((int)res.Width - SafeSize.X - logoSize.Width, SafeSize.Y + logoSize.Height + 42 + 40 * propLen), new Size(logoSize.Width, 2), Color.FromArgb(fullAlpha, Color.White));
                ResText.Draw(Heists[Index].Description, new Point((int)res.Width - SafeSize.X - (logoSize.Width - 4), SafeSize.Y + logoSize.Height + 45 + 40 * propLen + 4), 0.35f, Color.FromArgb(fullAlpha, Color.White), Common.EFont.ChaletLondon, ResText.Alignment.Left, false, false, new Size(508, 0));

                int lineCount = TextCommands.GetLineCount(Heists[Index].Description, 0.0f, 0.0f);
                ResRectangle.Draw(new Point((int)res.Width - SafeSize.X - logoSize.Width, SafeSize.Y + logoSize.Height + 44 + 40 * propLen), new Size(logoSize.Width, 45 * lineCount), Color.FromArgb(blackAlpha, 0, 0, 0));
            }
        }

        private bool drawTexture = false;
        public override void DrawTextures(Rage.Graphics g)
        {
            if (drawTexture && Heists[Index].Logo != null)
            {
                Sprite.DrawTexture(Heists[Index].Logo.Texture, new Point((int)res.Width - SafeSize.X - logoSize.Width, SafeSize.Y), logoSize, g);
            }
        }
    }
}

