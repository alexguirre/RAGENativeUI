namespace RAGENativeUI
{
    using Rage;
    using Rage.Exceptions;

    public static class BlipExtensions
    {
        /// <summary>
        /// Gets the token used to display the sprite of <paramref name="blip"/> in formatted scaleform text (i.e. the scaleform uses the game function `SET_FORMATTED_TEXT_WITH_ICONS`).
        /// <para>
        /// Example:
        /// <code>
        /// Blip myBlip = ...;<br />
        /// Game.DisplayHelp($"Go to ~{myBlip.GetIconToken()}~.");<br />
        /// Game.DisplayHelp($"Go to ~{HudColor.Red.GetName()}~~{myBlip.GetIconToken()}~~s~."); // with a different color
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="blip">The blip to get the sprite from.</param>
        /// <returns>The <see cref="string"/> with the icon token for the sprite of the given blip.</returns>
        public static string GetIconToken(this Blip blip) => blip ? blip.Sprite.GetIconToken() : throw new InvalidHandleableException(blip);

        /// <summary>
        /// Gets the token used to display <paramref name="blipSprite"/> in formatted scaleform text (i.e. the scaleform uses the game function `SET_FORMATTED_TEXT_WITH_ICONS`).
        /// <para>
        /// Example:
        /// <code>
        /// Game.DisplayHelp($"Go to ~{BlipSprite.Waypoint.GetIconToken()}~.");<br />
        /// Game.DisplayHelp($"Go to ~{HudColor.Red.GetName()}~~{BlipSprite.Waypoint.GetIconToken()}~~s~."); // with a different color
        /// </code>
        /// </para>
        /// </summary>
        /// <param name="blipSprite">The blip sprite.</param>
        /// <returns>The <see cref="string"/> with the icon token for the given blip sprite.</returns>
        public static string GetIconToken(this BlipSprite blipSprite) => "BLIP_" + (int)blipSprite;
    }
}
