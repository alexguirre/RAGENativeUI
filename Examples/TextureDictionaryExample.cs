namespace Examples
{
    using System.Diagnostics;

    using Rage;
    using Rage.Attributes;

    using RAGENativeUI;

    internal static class TextureDictionaryExample
    {
        [ConsoleCommand(Name = "TextureDictionaryExample", Description = "Example showing the TextureDictionary struct and TextureAsset class.")]
        private static void Command()
        {
            GameFiber.StartNew(() =>
            {
                while (true)
                {
                    GameFiber.Yield();

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                    {
                        TextureDictionary dict = new TextureDictionary("commonmenu");
                        Stopwatch sw = Stopwatch.StartNew();
                        TextureProperties texture = dict["arrowleft"];
                        sw.Stop();
                        Game.LogTrivial($"Indexer: {sw.Elapsed} | {sw.ElapsedMilliseconds} | {sw.ElapsedTicks}");
                        Game.LogTrivial($"Name: {texture.Name}");
                        Game.LogTrivial($"W: {texture.Width}");
                        Game.LogTrivial($"H: {texture.Height}");
                        Game.LogTrivial($"Depth: {texture.Depth}");
                        Game.LogTrivial($"Format: {texture.Format}");

                        Game.LogTrivial("");

                        sw.Restart();
                        texture = dict["arrowleft"];
                        sw.Stop();
                        Game.LogTrivial($"Indexer: {sw.Elapsed} | {sw.ElapsedMilliseconds} | {sw.ElapsedTicks}");

                        Game.LogTrivial("");

                        sw.Restart();
                        texture = dict["gradient_bgd"];
                        sw.Stop();
                        Game.LogTrivial($"Indexer: {sw.Elapsed} | {sw.ElapsedMilliseconds} | {sw.ElapsedTicks}");
                        Game.LogTrivial($"Name: {texture.Name}");
                        Game.LogTrivial($"W: {texture.Width}");
                        Game.LogTrivial($"H: {texture.Height}");
                        Game.LogTrivial($"Depth: {texture.Depth}");
                        Game.LogTrivial($"Format: {texture.Format}");

                        Game.LogTrivial("");

                        sw.Restart();
                        TextureProperties[] textures = dict.ToArray();
                        sw.Stop();
                        Game.LogTrivial($"ToArray: {sw.Elapsed} | {sw.ElapsedMilliseconds} | {sw.ElapsedTicks}");

                        sw.Restart();
                        foreach (TextureProperties t in dict)
                        {
                            Game.LogTrivial($"  Name: {t.Name}");
                        }
                        sw.Stop();
                        Game.LogTrivial($"ForEach: {sw.Elapsed} | {sw.ElapsedMilliseconds} | {sw.ElapsedTicks}");
                    }
                }
            });
        }
    }
}

