namespace RAGENativeUI.Tests.Runner;

internal sealed class PluginRunnerLogger : IRunnerLogger
{
    public object LockObject { get; } = new();

    public void LogMessage(StackFrameInfo stackFrame, string message)
        => Game.LogTrivial(message);
    public void LogImportantMessage(StackFrameInfo stackFrame, string message)
        => Game.LogTrivial(message);
    public void LogWarning(StackFrameInfo stackFrame, string message)
        => Game.LogTrivial(message);
    public void LogError(StackFrameInfo stackFrame, string message)
        => Game.LogTrivial(message);
}
