namespace RAGENativeUI.Tests;

using Moq;

using RAGENativeUI.Internals;

public class VersionCheckerTests
{
    [Fact]
    public void SameVersion()
    {
        var current = new Version(1, 0, 0, 0);
        var latest = new Version(1, 0, 0, 0);

        var versionChecker = new VersionChecker(current, GetLatestVersionProviderMock(latest, "url", new DateTime(2022, 5, 28)));
        Assert.Equal(current, versionChecker.CurrentVersion);

        versionChecker.RequestLatestVersionAsync().Wait();
        Assert.Equal(VersionCheckerStatus.LatestVersionAvailable, versionChecker.Status);
        Assert.Equal(latest, versionChecker.LatestVersion);
        Assert.Equal("url", versionChecker.HtmlUrl);
        Assert.Equal(new DateTime(2022, 5, 28), versionChecker.ReleaseDate);

        versionChecker.CheckForUpdates();
        Assert.Equal(VersionCheckerStatus.UpToDate, versionChecker.Status);
    }

    [Theory]
    [InlineData("1.0.0.0", "1.0.1.0")]
    [InlineData("1.0.0.0", "1.1.0.0")]
    [InlineData("1.0.0.0", "2.0.0.0")]
    public void NewerVersionAvailable(string currentVersion, string latestVersion)
    {
        var current = Version.Parse(currentVersion);
        var latest = Version.Parse(latestVersion);

        var versionChecker = new VersionChecker(current, GetLatestVersionProviderMock(latest, "url", new DateTime(2022, 5, 28)));
        Assert.Equal(current, versionChecker.CurrentVersion);

        versionChecker.RequestLatestVersionAsync().Wait();
        Assert.Equal(VersionCheckerStatus.LatestVersionAvailable, versionChecker.Status);
        Assert.Equal(latest, versionChecker.LatestVersion);
        Assert.Equal("url", versionChecker.HtmlUrl);
        Assert.Equal(new DateTime(2022, 5, 28), versionChecker.ReleaseDate);

        versionChecker.CheckForUpdates();
        Assert.Equal(VersionCheckerStatus.Outdated, versionChecker.Status);
    }

    /// <summary>
    /// The Assembly version and Github tag version may use different number of components (e.g. X.Y.Z.W vs X.Y.Z vs X.Y).
    /// Components not provided should be considered zeroes.
    /// </summary>
    [Theory]
    [InlineData("1.9.0.0", "1.9")]
    [InlineData("1.9", "1.9.0.0")]
    [InlineData("1.9.0", "1.9")]
    [InlineData("1.9", "1.9.0")]
    [InlineData("1.9.0.0", "1.9.0")]
    [InlineData("1.9.0", "1.9.0.0")]
    public void SameVersionWithDifferentNumberOfComponents(string currentVersion, string latestVersion)
    {
        var current = Version.Parse(currentVersion);
        var latest = Version.Parse(latestVersion);
        var expected = Version.Parse("1.9.0.0");

        var versionChecker = new VersionChecker(current, GetLatestVersionProviderMock(latest, "url", new DateTime(2022, 5, 28)));
        Assert.Equal(expected, versionChecker.CurrentVersion);

        versionChecker.RequestLatestVersionAsync().Wait();
        Assert.Equal(VersionCheckerStatus.LatestVersionAvailable, versionChecker.Status);
        Assert.Equal(expected, versionChecker.LatestVersion);
        Assert.Equal("url", versionChecker.HtmlUrl);
        Assert.Equal(new DateTime(2022, 5, 28), versionChecker.ReleaseDate);

        versionChecker.CheckForUpdates();
        Assert.Equal(VersionCheckerStatus.UpToDate, versionChecker.Status);
    }

    [Fact]
    public void ViewOutdatedVersionUserNotification()
    {
        var current = new Version(1, 0, 0, 0);
        var latest = new Version(1, 9, 1, 0);

        var versionChecker = new VersionChecker(current, GetLatestVersionProviderMock(latest, "https://github.com/alexguirre/RAGENativeUI/releases/tag/1.9", new DateTime(2022, 5, 28)));
        versionChecker.RequestLatestVersionAsync().Wait();
        versionChecker.CheckForUpdates();
        versionChecker.NotifyUser();
        Assert.True(UserConfirmation.Confirm("Version checker notification."));
    }

    [Fact]
    public void VersionRequestFailed()
    {
        var versionChecker = new VersionChecker(new(1, 0, 0, 0), GetFailingLatestVersionProviderMock());

        versionChecker.RequestLatestVersionAsync().Wait();
        Assert.Equal(VersionCheckerStatus.Error, versionChecker.Status);
        Assert.Null(versionChecker.LatestVersion);
        Assert.Null(versionChecker.HtmlUrl);
    }

    private static ILatestVersionProvider GetLatestVersionProviderMock(Version version, string htmlUrl, DateTime releaseDate)
    {
        var mock = new Mock<ILatestVersionProvider>();
        mock.Setup(x => x.RequestLatestVersionAsync(It.IsAny<VersionChecker>()).Result).Returns(new LatestVersionInfo(version, htmlUrl, releaseDate));
        return mock.Object;
    }

    private static ILatestVersionProvider GetFailingLatestVersionProviderMock()
    {
        var mock = new Mock<ILatestVersionProvider>();
        mock.Setup(x => x.RequestLatestVersionAsync(It.IsAny<VersionChecker>()).Result).Returns((LatestVersionInfo?)null);
        return mock.Object;
    }
}
