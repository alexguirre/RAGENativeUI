namespace RAGENativeUI.Internals
{
    using System;
    using System.Drawing;

    using Rage;

    using static RAGENativeUI.IL.Invoker;

    internal static class Functions
    {
        private static readonly IntPtr getActualScreenWidth, getActualScreenHeight;

        static Functions()
        {
            // TODO: move this to Module Initializer
            IntPtr[] addresses = Game.FindAllOccurrencesOfPattern("48 83 EC 38 0F 29 74 24 ?? 66 0F 6E 35 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ??");

            getActualScreenHeight = addresses[0];
            getActualScreenWidth = addresses[1];
        }

        /// <summary>
        /// Gets the resolution of the main screen, the one containing the UI in case multiple screens are used.
        /// </summary>
        public static SizeF ActualScreenResolution
        {
            get
            {
                using var tls = UsingTls.Scope();

                return new SizeF(InvokeRetFloat(getActualScreenWidth), InvokeRetFloat(getActualScreenHeight));
            }
        }
    }
}
