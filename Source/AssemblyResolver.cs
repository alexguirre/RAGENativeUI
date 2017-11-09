namespace RAGENativeUI
{
    using System;
    using System.Reflection;

    using Rage;

    using RAGENativeUI.Properties;

    internal static class AssemblyResolver
    {
        public static void Initialize()
        {
            Game.LogTrivialDebug($"Initializing {nameof(AssemblyResolver)}");

            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            string assemblyName = args.Name.Substring(0, args.Name.IndexOf(','));

            Game.LogTrivialDebug($"Resolving '{assemblyName}' ({args.Name})");

            switch (assemblyName)
            {
                case "System.Runtime.CompilerServices.Unsafe": return Assembly.Load(Resources.System_Runtime_CompilerServices_Unsafe);
                default: return null;
            }
        }
    }
}

