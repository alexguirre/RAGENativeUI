namespace RAGENativeUI.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// Represents a timer bar containing a list of checkpoints.
    /// <para>
    /// Each checkpoint is drawn as a circle which is either greyed out when not completed, black when failed or of a specific color when completed.
    /// Additionally, it can be crossed out.
    /// </para>
    /// <para>
    /// The state of the checkpoints is defined using the <see cref="Checkpoint"/> class and the <see cref="Checkpoints"/> property.
    /// </para>
    /// </summary>
    /// <see cref="Checkpoint"/>
    /// <see cref="CheckpointState"/>
    public class CheckpointsTimerBar : TimerBarBase
    {
        /// <summary>
        /// Gets the list containing the <see cref="Checkpoint"/>s of the timer bar.
        /// </summary>
        public IList<Checkpoint> Checkpoints { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckpointsTimerBar"/> class.
        /// </summary>
        /// <param name="label">A <see cref="string"/> that will appear at the left side of the timer bar.</param>
        /// <param name="numberOfCheckpoints">Determines the number of <see cref="Checkpoint"/>s created initially.</param>
        public CheckpointsTimerBar(string label, int numberOfCheckpoints) : base(label)
        {
            if (numberOfCheckpoints < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfCheckpoints), numberOfCheckpoints, $"{nameof(numberOfCheckpoints)} is negative");
            }

            Checkpoints = new List<Checkpoint>();
            for (int i = 0; i < numberOfCheckpoints; i++)
            {
                Checkpoints.Add(new Checkpoint());
            }
        }

        /// <inheritdoc/>
        public override void Draw(float x, float y)
        {
            base.Draw(x, y);

            if (Checkpoints.Count > 0)
            {
                x += TB.CheckpointXOffset;
                y += TB.CheckpointYOffset;

                bool requestedCrossTxd = false;
                foreach (Checkpoint cp in Checkpoints)
                {
                    Color circleColor = cp.State switch
                    {
                        CheckpointState.Completed => cp.Color,
                        CheckpointState.Failed => Color.Black,
                        _ => Color.FromArgb(51, 255, 255, 255),
                    };

                    N.DrawSprite(TB.CheckpointTextureDictionary, TB.CheckpointTextureName, x, y, TB.CheckpointWidth, TB.CheckpointHeight, 0.0f, circleColor.R, circleColor.G, circleColor.B, circleColor.A);

                    if (cp.IsCrossedOut)
                    {
                        if (!requestedCrossTxd)
                        {
                            N.RequestStreamedTextureDict(TB.CrossTextureDictionary);
                            if (!N.HasStreamedTextureDictLoaded(TB.CrossTextureDictionary))
                            {
                                break;
                            }

                            requestedCrossTxd = true;
                        }

                        int crossColor = cp.State == CheckpointState.Failed ? 255 : 0;
                        N.DrawSprite(TB.CrossTextureDictionary, TB.CrossTextureName, x, y, TB.CheckpointWidth, TB.CheckpointHeight, 0.0f, crossColor, crossColor, crossColor, 250);
                    }

                    x -= TB.CheckpointPadding;
                }
            }
        }

        /// <summary>
        /// Specifies the state of a <see cref="Checkpoint"/>.
        /// </summary>
        public enum CheckpointState
        {
            /// <summary>
            /// Represents an unfinished objective. Shown as a grey circle.
            /// </summary>
            InProgress = 0,
            /// <summary>
            /// Represents a successfully completed objective. Shown as a circle with color <see cref="Checkpoint.Color"/>.
            /// </summary>
            Completed,
            /// <summary>
            /// Represents a failed objective. Shown as a black circle.
            /// </summary>
            Failed,
        }

        /// <summary>
        /// Represents a checkpoint of a <see cref="CheckpointsTimerBar"/>.
        /// </summary>
        public class Checkpoint
        {
            /// <summary>
            /// Gets or sets the current state of this checkpoint.
            /// </summary>
            public CheckpointState State { get; set; }

            /// <summary>
            /// Gets or sets whether the checkpoint is crossed out.
            /// If <c>true</c>, a cross is drawn on top of the checkpoint circle, otherwise, nothing is drawn on top.
            /// </summary>
            public bool IsCrossedOut { get; set; }

            /// <summary>
            /// Gets or sets the color used to draw the checkpoint when <see cref="State"/> is set to <see cref="CheckpointState.Completed"/>.
            /// </summary>
            public Color Color { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Checkpoint"/> class with the given color.
            /// </summary>
            /// <param name="color">The color used to draw the checkpoint when <see cref="State"/> is set to <see cref="CheckpointState.Completed"/></param>
            public Checkpoint(Color color)
            {
                State = CheckpointState.InProgress;
                IsCrossedOut = false;
                Color = color;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Checkpoint"/> class with <see cref="Color"/> set to <see cref="Color.White"/>.
            /// </summary>
            public Checkpoint() : this(Color.White)
            {
            }
        }
    }
}
