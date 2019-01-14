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
            // disabled
            return;

            Common.LogDebug($"Initializing {nameof(AssemblyResolver)}");

            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            string assemblyName = args.Name.Substring(0, args.Name.IndexOf(','));

            Common.LogDebug($"Resolving '{assemblyName}' ({args.Name})");

            switch (assemblyName)
            {
                default: return null;
            }
        }
    }
}

