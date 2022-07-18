[assembly: Rage.Attributes.Plugin("RAGENativeUI.Tests", Author = "alexguirre", PrefersSingleInstance = true, SupportUrl = "https://github.com/alexguirre/ragenativeui", EntryPoint = "RAGENativeUI.Tests.Runner.Plugin.Main")]

namespace RAGENativeUI.Tests.Runner;

using Rage;

using Xunit.Runners;
using System.Reflection;
using System;

internal static class Plugin
{
    private static int totalTests = 0, passedTests = 0, finishedTests = 0;

    private static void Main()
    {
        Game.LogTrivial("-" + Thread.CurrentThread.ManagedThreadId);
        using var runner = new PluginRunner("Plugins\\RAGENativeUI.Tests.dll");
        runner.DiscoveryCompleted += OnDiscoveryCompleted;
        runner.TestFinished += OnTestFinished;
        runner.TestPassed += OnTestPassed;
        runner.TestFailed += OnTestFailed;

        Game.DisplaySubtitle("Discovering test cases...");
        runner.Run();

        var msg = $"Tests finished: {passedTests} / {totalTests} passed";
        Game.LogTrivial(msg);
        if (passedTests == totalTests)
        {
            Game.DisplaySubtitle($"~g~{msg}");
        }
        else
        {
            Game.DisplaySubtitle($"~r~{msg}");
        }
    }

    private static void ReportProgress()
    {
        var success = finishedTests == passedTests;
        Game.DisplaySubtitle($"Running tests: {finishedTests} / {totalTests} (passed {(success ? "~g~" : "~r~")}{passedTests} / {totalTests}~s~)", 240_000);
    }

    private static void OnDiscoveryCompleted(object sender, DiscoveryCompleteInfo e)
    {
        totalTests = e.TestCasesToRun;
        ReportProgress();
    }

    private static void OnTestFinished(object sender, TestFinishedInfo e)
    {
        finishedTests++;
        ReportProgress();
    }

    private static void OnTestPassed(object sender, TestPassedInfo e)
    {
        passedTests++;
    }

    private static void OnTestFailed(object sender, TestFailedInfo e)
    {
        Game.DisplayNotification(
            "some_txd", "some_tex", // TODO: look for an appropriate icon
            $"~r~{e.TestDisplayName}",
            $"~r~TEST FAILED",
            $"{e.ExceptionMessage}~n~~s~Stack Trace:  {e.ExceptionStackTrace}".Replace(Environment.NewLine, "~n~").Replace("Expected:", "~b~Expected:").Replace("Actual:", "~r~Actual:"));
    }
}
