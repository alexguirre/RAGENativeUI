namespace Examples
{
    using Rage;
    using Rage.Attributes;

    using RAGENativeUI;

#if DEBUG
    internal static class Misc
    {
        [ConsoleCommand(Name = "GenerateHudColorsDocs")]
        private static void Command_GenerateHudColorsDocs()
        {
            using (System.IO.StreamWriter w = new System.IO.StreamWriter("output.txt"))
            {
                foreach (HudColor item in System.Enum.GetValues(typeof(HudColor)))
                {
                    System.Drawing.Color c = item.GetColor();

                    string s = $@"<row>
<entry>
    <para>{item.GetName()}</para>
</entry>
<entry>
    <para>{(int)item}</para>
</entry>
<entry>
    <para>{c.R}</para>
</entry>
<entry>
    <para>{c.G}</para>
</entry>
<entry>
    <para>{c.B}</para>
</entry>
<entry>
    <para>{c.A}</para>
</entry>
<entry>
    <markup>
     <div style=""width: 20px; height: 20px; background: url('../media/colorbackground.png') no-repeat; background-size: cover; border: 1px solid rgb(219, 219, 219);""><div style=""width: 100%; height: 100%; background: rgba({c.R}, {c.G}, {c.B}, {(c.A / 255.0f).ToString(System.Globalization.CultureInfo.InvariantCulture)});""/></div>
    </markup>
</entry>
</row>";
                    w.WriteLine(s);
                }
            }
        }
    }
#endif
}

