namespace RAGENativeUI.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Rage;

    public class UIMenuColorListPanel : UIMenuPanel
    {
        // TODO: maybe ScrollableListBase can be used here
        public delegate void SelectionChangedEvent(UIMenuColorListPanel sender, int oldIndex, int newIndex);

        private static readonly TextStyle BaseLabelStyle = TextStyle.Default.With(font: TextFont.ChaletLondon, scale: 0.35f);
        private const float BorderMargin = 0.0046875f;

        private IList<Color> colors = new List<Color>();
        private int minItem = -1;
        private int maxItem = -1;
        private int currentItem = -1;
        private int maxVisibleColors;

        public override float Height => 0.034722f * 2 * 1.45f; // TODO: use correct size

        public string Title { get; set; } = "Title";
        public TextStyle TitleStyle { get; set; } = TextStyle.Default.With(font: TextFont.ChaletLondon, scale: 0.35f, justification: TextJustification.Center);

        public IList<Color> Colors
        {
            get => colors;
            set => colors = value ?? throw new ArgumentNullException(nameof(value));
        }

        public event SelectionChangedEvent SelectionChanged;

        public UIMenuColorListPanel()
        {
            // TODO: correct instructional buttons
            InstructionalButtons.Add(new InstructionalButtonDynamic("Change Color", new InstructionalButtonId[] { InstructionalKey.PageDown, InstructionalKey.PageUp }, new InstructionalButtonId[] { InstructionalKey.PageDown, InstructionalKey.PageUp }));
        }

        protected override void DrawContents(float x, float y, float menuWidth)
        {
            DrawPalette(x, y, menuWidth);
            DrawTitle(x, y, menuWidth);
        }

        private void DrawPalette(float x, float y, float menuWidth)
        {
            const float SquareWidth = (UIMenu.DefaultWidth - BorderMargin * 2.0f) / 9;

            //var squareWidth = /*SquareWidth;*/(menuWidth - BorderMargin * 2.0f) / 9;
            //Game.LogTrivial($"squareWidth = {squareWidth}");

            maxVisibleColors = (int)(menuWidth * 9 / UIMenu.DefaultWidth);
            // TODO: calculate min/max
            //minItem = 0;
            //maxItem = Math.Min(Colors.Count, maxVisibleColors) - 1;
            UpdateVisibleItemsIndices();

            var squareHeight = SquareWidth * N.GetAspectRatio(false);
            var paletteWidth = (maxItem - minItem + 1) * SquareWidth;
            var paletteX = x + menuWidth * 0.5f - paletteWidth * 0.5f; // center the palette in the middle of the panel
            var paletteY = y + BorderMargin * 10.0f;

            for (int i = minItem; i <= maxItem; i++)
            {
                var c = Colors[i];
                var ix = paletteX + SquareWidth * (i - minItem) + SquareWidth * 0.5f;
                var iy = paletteY + squareHeight * 0.5f;
                
                if (currentItem == i)
                {
                    // highlight
                    var highlightHeight = squareHeight / 6.0f;
                    N.DrawRect(ix, paletteY - highlightHeight * 0.5f, SquareWidth, highlightHeight, 255, 255, 255, 255);
                }
                
                N.DrawRect(ix, iy, SquareWidth, squareHeight, c.R, c.G, c.B, c.A);
            }
        }

        private void DrawTitle(float x, float y, float menuWidth)
        {
            if (!string.IsNullOrEmpty(Title))
            {
                var titleCharHeight = TitleStyle.CharacterHeight;
                var titleStyle = TitleStyle.WithWrap(x + BorderMargin, x + menuWidth - BorderMargin);
                var centerX = x + menuWidth * 0.5f;
                TextCommands.Display(Title, titleStyle, centerX, y + BorderMargin * 6.5f - titleCharHeight);
            }
        }

        public override bool ProcessControl()
        {
            // TODO: implement ProcessControl
            var oldItem = currentItem;
            if (Game.IsControlJustPressed(2, GameControl.FrontendRight))
                currentItem = Common.Wrap(currentItem + 1, 0, Colors.Count);
            else if (Game.IsControlJustPressed(2, GameControl.FrontendLeft))
                currentItem = Common.Wrap(currentItem - 1, 0, Colors.Count);

            if (oldItem != currentItem)
                SelectionChanged?.Invoke(this, oldItem, currentItem);

            return false;
        }

        public override bool ProcessMouse(float mouseX, float mouseY)
        {
            // TODO: implement ProcessMouse

            return base.ProcessMouse(mouseX, mouseY);
        }

        private void UpdateVisibleItemsIndices()
        {
            if (maxVisibleColors >= Colors.Count)
            {
                minItem = 0;
                maxItem = Colors.Count - 1;
                return;
            }

            if (currentItem == -1 || minItem == -1 || maxItem == -1) // if no selection or no previous selection
            {
                minItem = 0;
                maxItem = Math.Min(Colors.Count, maxVisibleColors) - 1;
            }
            else if (currentItem < minItem) // moved selection left, out of current visible item
            {
                minItem = currentItem;
                maxItem = currentItem + Math.Min(maxVisibleColors, Colors.Count) - 1;
            }
            else if (currentItem > maxItem) // moved selection right, out of current visible item
            {
                minItem = currentItem - Math.Min(maxVisibleColors, Colors.Count) + 1;
                maxItem = currentItem;
            }
            else if (maxItem - minItem + 1 != maxVisibleColors) // maxVisibleColors changed
            {
                if (maxItem == currentItem)
                {
                    minItem = maxItem - Math.Min(maxVisibleColors, Colors.Count) + 1;
                }
                else
                {
                    maxItem = minItem + Math.Min(maxVisibleColors, Colors.Count) - 1;
                    if (maxItem >= Colors.Count)
                    {
                        int diff = maxItem - Colors.Count + 1;
                        maxItem -= diff;
                        minItem -= diff;
                    }
                }
            }
        }
    }
}
