#nullable enable

namespace RAGENativeUI.Internals;

using Rage;

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

using Task = System.Threading.Tasks.Task;

internal enum VersionCheckerStatus
{
    None,
    LatestVersionAvailable,
    UpToDate,
    Outdated,
    Error,
}

internal class VersionChecker
{
    [Conditional("DEBUG")]
    internal static void LogDebug(string str) => Game.LogTrivialDebug($"[RAGENativeUI::{nameof(VersionChecker)}] {str}");
    internal static void Log(string str) => Game.LogTrivial($"[RAGENativeUI::{nameof(VersionChecker)}] {str}");

    public VersionCheckerStatus Status { get; private set; }
    public Version CurrentVersion { get; }
    public ILatestVersionProvider LatestVersionProvider { get; }
    public Version? LatestVersion { get; private set; }
    public string? HtmlUrl { get; private set; }
    public ProductInfoHeaderValue UserAgent => new("RAGENativeUI", CurrentVersion.ToString());

    public VersionChecker(Version currentVersion, ILatestVersionProvider latestVersionProvider)
    {
        Status = VersionCheckerStatus.None;
        CurrentVersion = NormalizeVersion(currentVersion)!;
        LatestVersionProvider = latestVersionProvider;
    }

    public Task RequestLatestVersionAsync()
    {
        return Task.Run(async () =>
        {
            try
            {
                var latestVersionResponse = await LatestVersionProvider.RequestLatestVersionAsync(this);
                LatestVersion = NormalizeVersion(latestVersionResponse?.LatestVersion);
                HtmlUrl = latestVersionResponse?.HtmlUrl;
                LogDebug($"Current version: {CurrentVersion}");
                LogDebug($"Latest version:  {LatestVersion}");
                Status = LatestVersion is null ? VersionCheckerStatus.LatestVersionAvailable : VersionCheckerStatus.Error;
            }
            catch (Exception e)
            {
                Log($"Version check failed (may not have internet connection): {e}");
                Status = VersionCheckerStatus.Error;
            }
        });
    }

    public void CheckForUpdates()
    {
        if (Status != VersionCheckerStatus.LatestVersionAvailable || LatestVersion is null)
        {
            return;
        }

        if (LatestVersion > CurrentVersion)
        {
            Status = VersionCheckerStatus.Outdated;
        }
        else if (LatestVersion == CurrentVersion)
        {
            Status = VersionCheckerStatus.UpToDate;
        }
        else
        {
            LogDebug("Latest version older than current version!");
            Status = VersionCheckerStatus.Error;
        }
    }

    public void NotifyUser()
    {
        if (Status != VersionCheckerStatus.Outdated)
        {
            return;
        }

        GameFiber.StartNew(() =>
        {
            Commands.DownloadPageUrl = HtmlUrl;
            Game.AddConsoleCommands(new[] { typeof(Commands) });

            Game.DisplayNotification(
                "txd", "tex",
                "RAGENativeUI",
                "~b~New version available!",
                $"<font size='14'>Version ~r~<font size='17'>{ToStringTrimZeroes(CurrentVersion)}</font>~s~ is outdated. The latest version is ~b~<font size='17'>{ToStringTrimZeroes(LatestVersion)}</font>~s~.</font>");
            Game.DisplayNotification(
                $"For more information, run ~b~{Commands.OpenDownloadPageName}~s~ in the console or visit ~b~{HtmlUrl}~s~.");

            Game.LogTrivial($"[RAGENativeUI] New version available!");
            Game.LogTrivial($"[RAGENativeUI] Version {ToStringTrimZeroes(CurrentVersion)} is outdated. The latest version is {ToStringTrimZeroes(LatestVersion)}.");
            Game.LogTrivial($"[RAGENativeUI] For more information, run `{Commands.OpenDownloadPageName}` in the console or visit {HtmlUrl}.");
        });
    }

    /// <summary>
    /// Sets undefined components to 0.
    /// </summary>
    private static Version? NormalizeVersion(Version? v)
        => v is null ? null :
            new(v.Major,
                v.Minor,
                v.Build == -1 ? 0 : v.Build,
                v.Revision == -1 ? 0 : v.Revision);

    private static string ToStringTrimZeroes(Version? v)
    {
        if (v is null) return "unknown";

        if (v.Revision == 0)
        {
            v = v.Build == 0 ? new(v.Major, v.Minor) :
                               new(v.Major, v.Minor, v.Build);
        }
        return v.ToString();
    }

    public static VersionChecker FromGithubReleases() => new(Assembly.GetExecutingAssembly().GetName().Version, new GithubReleasesLatestVersionProvider());

    public static async Task RunCheckAsync()
    {
        var checker = FromGithubReleases();
        await checker.RequestLatestVersionAsync();
        checker.CheckForUpdates();
        checker.NotifyUser();
    }

    private static class Commands
    {
        public const string OpenDownloadPageName = "RNUI_OpenDownloadPage";

        public static string? DownloadPageUrl { get; set; }

        [Rage.Attributes.ConsoleCommand(Name = OpenDownloadPageName, Description = "Opens the download page for the latest RAGENativeUI version in your browser.")]
        private static void OpenDownloadPage()
        {
            if (DownloadPageUrl != null)
            {
                Process.Start(DownloadPageUrl);
            }
        }

    }
}

internal interface ILatestVersionProvider
{
    Task<(Version LatestVersion, string HtmlUrl)?> RequestLatestVersionAsync(VersionChecker versionChecker);
}

internal sealed class GithubReleasesLatestVersionProvider : ILatestVersionProvider
{
    public async Task<(Version LatestVersion, string HtmlUrl)?> RequestLatestVersionAsync(VersionChecker versionChecker)
    {
        const string RequestUrl = "https://api.github.com/repos/alexguirre/RAGENativeUI/releases/latest";

        VersionChecker.LogDebug("HTTP request...");
        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.Add(versionChecker.UserAgent);
        using var response = await client.GetAsync(RequestUrl);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsByteArrayAsync();
            using var reader = JsonReaderWriterFactory.CreateJsonReader(content, new());
            var doc = XDocument.Load(reader);
            var root = doc.Element("root");
            var htmlUrl = root.Element("html_url").Value;
            var latestVersionStr = root.Element("tag_name").Value;
            if (Version.TryParse(latestVersionStr, out var latestVersion))
            {
                return (latestVersion, htmlUrl);
            }
            else
            {
                VersionChecker.LogDebug($"Failed to parse latest version (string '{latestVersionStr}')");
            }
        }
        else
        {
            VersionChecker.Log($"Request failed (HTTP {(int)response.StatusCode} {response.StatusCode})");
        }

        return null;
    }
}
