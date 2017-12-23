using System.Collections.Generic;
using System.Drawing;
using Rage;
using Rage.Native;

namespace RAGENativeUI.Elements
{
    public abstract class TimerBarBase
    {
        public string Label { get; set; }

        public TimerBarBase(string label)
        {
            Label = label;
        }

        public virtual void Draw(int interval)
        {
            SizeF res = UIMenu.GetScreenResolutionMantainRatio();
            Point safe = UIMenu.GetSafezoneBounds();

            ResText.Draw(Label, new Point((int)res.Width - safe.X - 180, (int)res.Height - safe.Y - (30 + (4 * interval))), 0.3f, Color.White, Common.EFont.ChaletLondon, ResText.Alignment.Right, false, false, Size.Empty);
            Sprite.Draw("timerbars", "all_black_bg", new Point((int)res.Width - safe.X - 298, (int)res.Height - safe.Y - (40 + (4 * interval))), new Size(300, 37), 0f, Color.FromArgb(180, 255, 255, 255));
           
            NativeFunction.Natives.HideHudComponentThisFrame(7);//AreaName
            NativeFunction.Natives.HideHudComponentThisFrame(9);//StreetName
            NativeFunction.Natives.HideHudComponentThisFrame(6);//VehicleName
        }
    }

    public class TextTimerBar : TimerBarBase
    {
        public string Text { get; set; }

        public TextTimerBar(string label, string text) : base(label)
        {
            Text = text;
        }

        public override void Draw(int interval)
        {
            SizeF res = UIMenu.GetScreenResolutionMantainRatio();
            Point safe = UIMenu.GetSafezoneBounds();

            base.Draw(interval);
            ResText.Draw(Text, new Point((int)res.Width - safe.X - 10, (int)res.Height - safe.Y - (42 + (4 * interval))), 0.5f, Color.White, Common.EFont.ChaletLondon, ResText.Alignment.Right, false, false, Size.Empty);
        }
    }

    public class BarTimerBar : TimerBarBase
    {
        /// <summary>
        /// Bar percentage. Goes from 0 to 1.
        /// </summary>
        public float Percentage { get; set; }

        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }

        public BarTimerBar(string label) : base(label)
        {
            BackgroundColor = Color.DarkRed;
            ForegroundColor = Color.Red;
        }

        public override void Draw(int interval)
        {
            SizeF res = UIMenu.GetScreenResolutionMantainRatio();
            Point safe = UIMenu.GetSafezoneBounds();

            base.Draw(interval);

            var start = new Point((int)res.Width - safe.X - 160, (int)res.Height - safe.Y - (28 + (4 * interval)));

            ResRectangle.Draw(start, new Size(150, 15), BackgroundColor);
            ResRectangle.Draw(start, new Size((int)(150 * Percentage), 15), ForegroundColor);
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
            for (int i = 0; i < InternalList.Count; i++)
            {
                InternalList[i].Draw(i * 10);
            }
        }
    }
}
