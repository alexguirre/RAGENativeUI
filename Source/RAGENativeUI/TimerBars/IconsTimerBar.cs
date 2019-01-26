namespace RAGENativeUI.TimerBars
{
#if RPH1
    extern alias rph1;
    using Vector2 = rph1::Rage.Vector2;
#else
    /** REDACTED **/
#endif

    using System.Drawing;
    using System.Collections.Generic;

    public class IconsTimerBar : LabeledTimerBar
    {
        public IList<TimerBarIcon> Icons { get; } = new List<TimerBarIcon>();
        public float Spacing { get; set; } = TimerBarIcon.DefaultWidth * 0.75f;

        /// <exception cref="ArgumentNullException"><paramref name="label"/> is <see langword="null"/>.</exception>
        public IconsTimerBar(string label) : base(label)
        {
        }

        public override void Draw(Vector2 position)
        {

            if (!IsVisible)
                return;

            base.Draw(position);

            if (Icons.Count > 0)
            {
                position.X += TimerBarIcon.XOffset;
                position.Y += TimerBarIcon.YOffset;

                for (int i = 0; i < Icons.Count; i++)
                {
                    TimerBarIcon icon = Icons[i];

                    if (!icon.Texture.Dictionary.IsLoaded) icon.Texture.Dictionary.Load();

                    Color c = icon.Color;
                    N.DrawSprite(icon.Texture.Dictionary, icon.Texture.Name, position.X, position.Y, icon.Size.X, icon.Size.Y, 0.0f, c.R, c.G, c.B, c.A);
                    position.X -= Spacing;
                }
            }
        }
    }
}

