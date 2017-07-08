namespace Examples
{
    using Rage;

    internal static class EntryPoint
    {
        private static void Main()
        {
            GameFiber.Hibernate();
        }

        private static void OnUnload(bool isTerminating)
        {

        }
    }
}

