namespace RAGENativeUI.Internals
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Security;

    using Rage;

    internal static class Functions
    {
        [SuppressUnmanagedCodeSecurity] private delegate float RetFloat();

        private static readonly RetFloat getActualScreenWidth, getActualScreenHeight;

        static Functions()
        {
            // TODO: move this to Module Initializer
            IntPtr[] addresses = Game.FindAllOccurrencesOfPattern("48 83 EC 38 0F 29 74 24 ?? 66 0F 6E 35 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ??");
            getActualScreenHeight = Marshal.GetDelegateForFunctionPointer<RetFloat>(addresses[0]);
            getActualScreenWidth = Marshal.GetDelegateForFunctionPointer<RetFloat>(addresses[1]);
        }

        /// <summary>
        /// Gets the resolution of the main screen, the one containing the UI in case multiple screens are used.
        /// </summary>
        public static SizeF ActualScreenResolution
        {
            get
            {
                using var tls = UsingTls.Scope();

                return new SizeF(getActualScreenWidth(), getActualScreenHeight());
            }
        }
    }
}
