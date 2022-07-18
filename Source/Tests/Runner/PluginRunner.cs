namespace RAGENativeUI.Tests.Runner;

using Xunit.Abstractions;
using Xunit.Runners;

internal sealed class PluginRunner : IDisposable, IMessageSinkWithTypes
{
    private bool disposedValue;
    private readonly IFrontController controller;
    private readonly IMessageSink reporterMessageHandler;
    private readonly List<ITestCase> testCasesToRun = new();
    private bool discoveryCompletedFlag = false;

    public event EventHandler<ITestCase>? TestCaseDiscovered;
    public event EventHandler<DiscoveryCompleteInfo>? DiscoveryCompleted;
    public event EventHandler<TestFinishedInfo>? TestFinished;
    public event EventHandler<TestPassedInfo>? TestPassed;
    public event EventHandler<TestFailedInfo>? TestFailed;

    public PluginRunner(string assemblyFileName)
    {
        reporterMessageHandler = new DefaultRunnerReporterWithTypes().CreateMessageHandler(new PluginRunnerLogger());
        controller = new XunitFrontController(AppDomainSupport.Denied, assemblyFileName, diagnosticMessageSink: MessageSinkAdapter.Wrap(this));
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                controller.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public void Run()
    {
        if (!GameFiber.CanSleepNow)
        {
            throw new InvalidOperationException("Tests must be run within a game fiber!");
        }

        var discoveryOptions = GetDiscoveryOptions();
        var executionOptions = GetExecutionOptions();
        controller.Find(includeSourceInformation: false, this, discoveryOptions);
        GameFiber.SleepUntil(() => discoveryCompletedFlag, 240_000);
        controller.RunTests(testCasesToRun, this, executionOptions);
    }

    private ITestFrameworkDiscoveryOptions GetDiscoveryOptions()
    {
        var opt = TestFrameworkOptions.ForDiscovery();
        return opt;
    }

    private ITestFrameworkExecutionOptions GetExecutionOptions()
    {
        var opt = TestFrameworkOptions.ForExecution();
        opt.SetDisableParallelization(true);
        return opt;
    }

    private bool DispatchMessage<TMessage>(IMessageSinkMessage message, HashSet<string> messageTypes, Action<TMessage> handler) where TMessage : class, IMessageSinkMessage
    {
        if (messageTypes == null || !messageTypes.Contains(typeof(TMessage).FullName))
        {
            return false;
        }

        handler((TMessage)message);
        return true;
    }

    bool IMessageSinkWithTypes.OnMessageWithTypes(IMessageSinkMessage message, HashSet<string> messageTypes)
    {
        reporterMessageHandler.OnMessage(message);

        DispatchMessage<ITestCaseDiscoveryMessage>(message, messageTypes, RaiseTestCaseDiscovered);
        DispatchMessage<IDiscoveryCompleteMessage>(message, messageTypes, RaiseDiscoveryCompleted);
        DispatchMessage<ITestFinished>(message, messageTypes, RaiseTestFinished);
        DispatchMessage<ITestPassed>(message, messageTypes, RaiseTestPassed);
        DispatchMessage<ITestFailed>(message, messageTypes, RaiseTestFailed);
        return true;
    }

    private void RaiseTestCaseDiscovered(ITestCaseDiscoveryMessage m)
    {
        testCasesToRun.Add(m.TestCase);
        TestCaseDiscovered?.Invoke(this, m.TestCase);
    }
    private void RaiseDiscoveryCompleted(IDiscoveryCompleteMessage m)
    {
        DiscoveryCompleted?.Invoke(this, new(testCasesToRun.Count, testCasesToRun.Count));
        discoveryCompletedFlag = true;
    }
    private void RaiseTestFinished(ITestFinished m) => TestFinished?.Invoke(this, new(m.TestClass.Class.Name, m.TestMethod.Method.Name, m.TestCase.Traits, m.Test.DisplayName, m.TestCollection.DisplayName, m.ExecutionTime, m.Output));
    private void RaiseTestPassed(ITestPassed m) => TestPassed?.Invoke(this, new(m.TestClass.Class.Name, m.TestMethod.Method.Name, m.TestCase.Traits, m.Test.DisplayName, m.TestCollection.DisplayName, m.ExecutionTime, m.Output));
    private void RaiseTestFailed(ITestFailed m) => TestFailed?.Invoke(this, new(m.TestClass.Class.Name, m.TestMethod.Method.Name, m.TestCase.Traits, m.Test.DisplayName, m.TestCollection.DisplayName, m.ExecutionTime, m.Output, m.ExceptionTypes.FirstOrDefault(), m.Messages.FirstOrDefault(), m.StackTraces.FirstOrDefault()));
}
