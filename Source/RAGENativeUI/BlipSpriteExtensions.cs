namespace RAGENativeUI
{
    using Rage;

    public static class BlipSpriteExtensions
    {
        /// <summary>
        /// Gets the real name of the specified blip sprite. For example, for <see cref="BlipSprite.Police"/> it will return "BLIP_POLICE_PED".
        /// </summary>
        /// <remarks>
        /// One of the uses of the returned name is displaying the blip sprite in help messages.
        /// <code language="C#" title="Usage example in a notification">
        /// Game.DisplayHelp($"The police peds have this blip sprite: ~{BlipSprite.Police.GetName()}~");
        /// </code>
        /// </remarks>
        /// <param name="sprite">The blip sprite.</param>
        public static string GetName(this BlipSprite sprite)
        {
            int i = (int)sprite;
            Throw.IfOutOfRange(i, 0, KnownNames.BlipSpriteNames.Array.Length - 1, nameof(sprite));

            return KnownNames.BlipSpriteNames.Array[i];
        }
    }
}

