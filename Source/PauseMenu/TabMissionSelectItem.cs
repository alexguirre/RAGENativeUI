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

        protected const int MaxItemsPerView = 15;
        protected int _minItem;
        protected int _maxItem;
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
            var itemSize = new Size((int)activeWidth - 515, 40);

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
                _noLogo.Position = new Point((int)res.Width - SafeSize.X - 512, SafeSize.Y);
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
                sprite.Position = new Point((int)res.Width - SafeSize.X - 512, SafeSize.Y);
                sprite.Color = Color.FromArgb(blackAlpha, 0, 0, 0);
                sprite.Draw();
            }
            else
            {
                drawTexture = false;
            }

            ResRectangle.Draw(new Point((int)res.Width - SafeSize.X - 512, SafeSize.Y + 256), new Size(512, 40), Color.FromArgb(fullAlpha, Color.Black));
            ResText.Draw(Heists[Index].Name, new Point((int)res.Width - SafeSize.X - 4, SafeSize.Y + 260), 0.5f, Color.FromArgb(fullAlpha, Color.White), Common.EFont.HouseScript, ResText.Alignment.Right, false, false, Size.Empty);

            for (int i = 0; i < Heists[Index].ValueList.Count; i++)
            {
                ResRectangle.Draw(new Point((int)res.Width - SafeSize.X - 512, SafeSize.Y + 256 + 40 + (40 * i)), new Size(512, 40), i % 2 == 0 ? Color.FromArgb(alpha, 0, 0, 0) : Color.FromArgb(blackAlpha, 0, 0, 0));
                var text = Heists[Index].ValueList[i].Item1;
                var label = Heists[Index].ValueList[i].Item2;


                ResText.Draw(text, new Point((int)res.Width - SafeSize.X - 506, SafeSize.Y + 260 + 42 + (40 * i)), 0.35f, Color.FromArgb(fullAlpha, Color.White), Common.EFont.ChaletLondon, false);
                ResText.Draw(label, new Point((int)res.Width - SafeSize.X - 6, SafeSize.Y + 260 + 42 + (40 * i)), 0.35f, Color.FromArgb(fullAlpha, Color.White), Common.EFont.ChaletLondon, ResText.Alignment.Right, false, false, Size.Empty);
            }

            if (!string.IsNullOrEmpty(Heists[Index].Description))
            {
                var propLen = Heists[Index].ValueList.Count;
                ResRectangle.Draw(new Point((int)res.Width - SafeSize.X - 512, SafeSize.Y + 256 + 42 + 40 * propLen), new Size(512, 2), Color.FromArgb(fullAlpha, Color.White));
                ResText.Draw(Heists[Index].Description, new Point((int)res.Width - SafeSize.X - 508, SafeSize.Y + 256 + 45 + 40 * propLen + 4), 0.35f, Color.FromArgb(fullAlpha, Color.White), Common.EFont.ChaletLondon, ResText.Alignment.Left, false, false, new Size(508, 0));

                ResRectangle.Draw(new Point((int)res.Width - SafeSize.X - 512, SafeSize.Y + 256 + 44 + 40 * propLen), new Size(512, 45 * (StringMeasurer.MeasureString(Heists[Index].Description) / 500)), Color.FromArgb(blackAlpha, 0, 0, 0));
            }
        }

        private bool drawTexture = false;
        public override void DrawTextures(Rage.Graphics g)
        {
            if (drawTexture && Heists[Index].Logo != null)
            {
                Sprite.DrawTexture(Heists[Index].Logo.Texture, new Point((int)res.Width - SafeSize.X - 512, SafeSize.Y), new Size(512, 256), g);
            }
        }
    }
}
