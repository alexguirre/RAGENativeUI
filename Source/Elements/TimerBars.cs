using System;
using System.Collections.Generic;
using System.Drawing;
using Rage;
using Rage.Native;

namespace RAGENativeUI.Elements
{
    // TimerBar Constants
    internal static class TB
    {
        // Most constants are from the game scripts

        internal const float InitialX = 0.795f;
        internal const float InitialY = 0.925f - 0.002f;
        internal const float LoadingPromptYOffset = 0.036f;

        internal const string BgTextureDictionary = "timerbars";
        internal const string BgTextureName = "all_black_bg";
        internal const string BgHighlightTextureName = "all_white_bg";
        internal const float BgXOffset = 0.079f;
        internal const float BgDefaultYOffset = 0.008f;
        internal const float BgSmallYOffset = 0.012f;
        internal const float BgWidth = 0.157f;
        internal const float BgDefaultHeight = 0.036f;
        internal const float BgSmallHeight = 0.028f;
        internal const float DefaultHeightWithGap = ((0.025f + 0.006f) + 0.0009f) + 0.008f;
        internal const float SmallHeightWithGap = ((0.025f + 0.006f) + 0.0009f);

        internal const float LabelInitialWrapEnd = ((((0.88f - 0.062f) + 0.026f) + 0.027f) + 0.03f) - 0.034f;
        internal const float LabelScale = 0.202f;
        internal const float LabelSize = 0.288f;

        internal const float TextYOffset = ((((-0.01f - 0.005f) + 0.004f) - 0.001f) + 0.001f);
        internal const float TextWrapEnd = (((((0.95f - 0.047f) + 0.001f) + 0.047f) - 0.002f) + 0.001f);
        internal const float TextScale = 0.332f;
        internal const float TextSize = ((((((0.469f + 0.096f) - 0.017f) + 0.022f) - 0.062f) - 0.001f) - 0.013f);

        internal const float BarXOffset = 0.118f; // == (((((0.919f - 0.081f) + 0.028f) + 0.05f) - 0.001f) - 0.002f) - TimerBarManager.InitialX
        internal const float BarYOffset = ((((0.013f - 0.002f) + 0.001f) + 0.001f) - 0.001f);
        internal const float BarWidth = 0.069f;
        internal const float BarHeight = 0.011f;
    }

    public abstract class TimerBarBase
    {
        // TODO: HighlightColor property
        // TODO: public SmallHeight property
        internal bool SmallHeight => this is BarTimerBar;

        public string Label { get; set; }

        public TimerBarBase(string label)
        {
            Label = label;
        }

        [Obsolete]
        public virtual void Draw(int interval)
        {
            SizeF res = UIMenu.GetScreenResolutionMantainRatio();
            Point safe = UIMenu.GetSafezoneBounds();

            ResText.Draw(Label, new Point((int)res.Width - safe.X - 180, (int)res.Height - safe.Y - (30 + (4 * interval))), 0.3f, Color.White, Common.EFont.ChaletLondon, ResText.Alignment.Right, false, false, Size.Empty);
            Sprite.Draw("timerbars", "all_black_bg", new Point((int)res.Width - safe.X - 298, (int)res.Height - safe.Y - (40 + (4 * interval))), new Size(300, 37), 0f, Color.FromArgb(180, 255, 255, 255));

        }

        public virtual void Draw(float x, float y)
        {
            DrawBackground(x, y);
            DrawLabel(x, y);
        }

        private void DrawBackground(float x, float y)
        {
            x += TB.BgXOffset;
            y += SmallHeight ? TB.BgSmallYOffset : TB.BgDefaultYOffset;

            float w = TB.BgWidth;
            float h = SmallHeight ? TB.BgSmallHeight : TB.BgDefaultHeight;

            N.DrawSprite(TB.BgTextureDictionary, TB.BgTextureName, x, y, w, h, 0.0f, 255, 255, 255, 140);
        }

        private void DrawLabel(float x, float y)
        {

            float wrapEnd = TB.LabelInitialWrapEnd;
            if (!N.GetIsWidescreen())
            {
                wrapEnd -= 0.02f;
            }
            wrapEnd = wrapEnd - (0.03f * WrapEndMultiplier);

            N.SetTextFont(0);
            N.SetTextWrap(0.0f, wrapEnd);
            N.SetTextScale(TB.LabelScale, TB.LabelSize);
            N.GetHudColour(1, out int r, out int g, out int b, out int a); // TODO: LabelColor property
            N.SetTextColour(r, g, b, a);
            N.SetTextJustification(2); // Right

            TextCommands.Display(Label, x, y);
        }

        private static float WrapEndMultiplier
        {
            get
            {
                float aspectRatio = N.GetAspectRatio(false);
                N.GetActiveScreenResolution(out int screenWidth, out int screenHeight);
                float screenRatio = (float)screenWidth / screenHeight;
                aspectRatio = Math.Min(aspectRatio, screenRatio);
                if (screenRatio > 3.5f && aspectRatio > 1.7f)
                {
                    return 0.4f;
                }
                else if (aspectRatio > 1.7f)
                {
                    return 0.0f;
                }
                else if (aspectRatio > 1.5f)
                {
                    return 0.2f;
                }
                else if (aspectRatio > 1.3f)
                {
                    return 0.3f;
                }
                else
                {
                    return 0.4f;
                }
            }
        }
    }

    public class TextTimerBar : TimerBarBase
    {
        public string Text { get; set; }

        public TextTimerBar(string label, string text) : base(label)
        {
            Text = text;
        }

        [Obsolete]
        public override void Draw(int interval)
        {
            SizeF res = UIMenu.GetScreenResolutionMantainRatio();
            Point safe = UIMenu.GetSafezoneBounds();

            base.Draw(interval);
            ResText.Draw(Text, new Point((int)res.Width - safe.X - 10, (int)res.Height - safe.Y - (42 + (4 * interval))), 0.5f, Color.White, Common.EFont.ChaletLondon, ResText.Alignment.Right, false, false, Size.Empty);
        }

        public override void Draw(float x, float y)
        {
            base.Draw(x, y);

            y += TB.TextYOffset;

            N.SetTextFont(0);
            N.SetTextWrap(0.0f, TB.TextWrapEnd);
            N.SetTextScale(TB.TextScale, TB.TextSize);
            N.GetHudColour(1, out int r, out int g, out int b, out int a); // TODO: TextColor property
            N.SetTextColour(r, g, b, a);
            N.SetTextJustification(2); // Right

            TextCommands.Display(Text, x, y);
        }
    }

    public class BarTimerBar : TimerBarBase
    {
        // TODO: markers (see timerbar_lines)

        /// <summary>
        /// Bar percentage. Goes from 0 to 1.
        /// </summary>
        public float Percentage { get; set; }

        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }

        public BarTimerBar(string label) : base(label)
        {
            N.GetHudColour(8, out int bR, out int bG, out int bB, out int bA); // HUD_COLOUR_REDDARK
            N.GetHudColour(6, out int fR, out int fG, out int fB, out int fA); // HUD_COLOUR_RED

            BackgroundColor = Color.FromArgb(bA, bR, bG, bB);
            ForegroundColor = Color.FromArgb(fA, fR, fG, fB);
        }

        [Obsolete]
        public override void Draw(int interval)
        {
            SizeF res = UIMenu.GetScreenResolutionMantainRatio();
            Point safe = UIMenu.GetSafezoneBounds();

            base.Draw(interval);

            var start = new Point((int)res.Width - safe.X - 160, (int)res.Height - safe.Y - (28 + (4 * interval)));

            ResRectangle.Draw(start, new Size(150, 15), BackgroundColor);
            ResRectangle.Draw(start, new Size((int)(150 * Percentage), 15), ForegroundColor);
        }

        public override void Draw(float x, float y)
        {
            base.Draw(x, y);

            x += TB.BarXOffset;
            y += TB.BarYOffset;

            float w = TB.BarWidth;
            float h = TB.BarHeight;

            N.DrawRect(x, y, w, h, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B, BackgroundColor.A);

            float fillW = w * Percentage;
            float fillX = x - w * 0.5f + fillW * 0.5f;
            N.DrawRect(fillX, y, fillW, h, ForegroundColor.R, ForegroundColor.G, ForegroundColor.B, ForegroundColor.A);
        }
    }

    public class TimerBarPool : BaseCollection<TimerBarBase>
    {
        public List<TimerBarBase> ToList()
        {
            return InternalList;
        }

        // added for backwards compatibility, BaseCollection.Remove returns a bool and this one doesn't return anything
        public new void Remove(TimerBarBase item) 
        {
            base.Remove(item);
        }

        public void Draw()
        {
            if (InternalList.Count > 0)
            {
                N.RequestStreamedTextureDict(TB.BgTextureDictionary);
                if (!N.HasStreamedTextureDictLoaded(TB.BgTextureDictionary))
                {
                    return;
                }

                N.SetScriptGfxAlign('R', 'B');
                N.SetScriptGfxAlignParams(0.0f, 0.0f, 0.952f, 0.949f);

                float x = TB.InitialX, y = TB.InitialY - (N.BusySpinnerIsOn() ? TB.LoadingPromptYOffset : 0.0f);
                for (int i = 0; i < InternalList.Count; i++)
                {
                    TimerBarBase b = InternalList[i];
                    b.Draw(x, y);
                    y -= b.SmallHeight ? TB.SmallHeightWithGap : TB.DefaultHeightWithGap;
                }

                N.ResetScriptGfxAlign();

                N.HideHudComponentThisFrame(6); // VehicleName
                N.HideHudComponentThisFrame(7); // AreaName
                N.HideHudComponentThisFrame(8); // ?
                N.HideHudComponentThisFrame(9); // StreetName
            }
        }
    }
}

