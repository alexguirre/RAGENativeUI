#nullable enable
namespace RAGENativeUI.Internals;

using Rage;

using System.Diagnostics;
using System.Globalization;

internal record struct ConfigValues(
    bool VersionCheckerEnabled)
{
    public bool IsDefault => this == Default;

    public static readonly ConfigValues Default = new(
        VersionCheckerEnabled: true);
}

internal class Config
{
    [Conditional("DEBUG")]
    internal static void Log(string str) => Game.LogTrivialDebug($"[RAGENativeUI::{nameof(Config)}] {str}");

    public InitializationFile Ini { get; }

    public Config(string iniFileName)
    {
        Ini = new(iniFileName, CultureInfo.InvariantCulture, autoLayout: true);
    }

    public ConfigValues Load()
        => new(
            VersionCheckerEnabled: Ini.ReadBoolean(General, nameof(ConfigValues.VersionCheckerEnabled), ConfigValues.Default.VersionCheckerEnabled));

    public void Save(ConfigValues config)
    {
        if (!Ini.Exists())
        {
            Log("Saving and the .ini file does not exist");
            if (config.IsDefault)
            {
                Log("Saving default config, don't need to create the .ini file");
                return;
            }

            Log("Creating the .ini file");
            Ini.Create();
        }

        Log($"Saving new config {config}");
        Ini.Write(General, nameof(ConfigValues.VersionCheckerEnabled), config.VersionCheckerEnabled);
    }

    private const string General = "General";

    public static Config Get() => new("RAGENativeUI.ini");
}
