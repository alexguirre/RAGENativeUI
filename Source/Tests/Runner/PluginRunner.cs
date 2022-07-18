namespace RAGENativeUI.Tests.Runner;

using Xunit.Abstractions;
using Xunit.Runners;

internal sealed class PluginRunner : IDisposable, IMessageSinkWithTypes
{
    private bool disposedValue;
    private readonly IFrontController controller;
    private readonly IMessageSink reporterMessageHandler;

    public object LockObject { get; } = new();

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

    private bool DispatchMessage<TMessage>(IMessageSinkMessage message, HashSet<string> messageTypes, Action<TMessage> handler) where TMessage : class, IMessageSinkMessage
    {
        if (messageTypes == null || !messageTypes.Contains(typeof(TMessage).FullName))
        {
            return false;
        }

        handler((TMessage)message);
        return true;
    }

    public bool OnMessageWithTypes(IMessageSinkMessage message, HashSet<string> messageTypes)
    {
        reporterMessageHandler.OnMessage(message);

        DispatchMessage<ITestFinished>(message, messageTypes, RaiseTestFinished);
        DispatchMessage<ITestPassed>(message, messageTypes, RaiseTestPassed);
        DispatchMessage<ITestFailed>(message, messageTypes, RaiseTestFailed);
        return true;
    }

    public void Run()
    {
        var discoveryOptions = GetDiscoveryOptions();
        var executionOptions = GetExecutionOptions();
        controller.RunAll(this, discoveryOptions, executionOptions);
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

    private void RaiseTestFinished(ITestFinished m) => TestFinished?.Invoke(this, new(m.TestClass.Class.Name, m.TestMethod.Method.Name, m.TestCase.Traits, m.Test.DisplayName, m.TestCollection.DisplayName, m.ExecutionTime, m.Output));
    private void RaiseTestPassed(ITestPassed m) => TestPassed?.Invoke(this, new(m.TestClass.Class.Name, m.TestMethod.Method.Name, m.TestCase.Traits, m.Test.DisplayName, m.TestCollection.DisplayName, m.ExecutionTime, m.Output));
    private void RaiseTestFailed(ITestFailed m) => TestFailed?.Invoke(this, new(m.TestClass.Class.Name, m.TestMethod.Method.Name, m.TestCase.Traits, m.Test.DisplayName, m.TestCollection.DisplayName, m.ExecutionTime, m.Output, m.ExceptionTypes.FirstOrDefault(), m.Messages.FirstOrDefault(), m.StackTraces.FirstOrDefault()));
}
