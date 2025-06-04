using NetForge.Core;

namespace ShareLingo.WPF.Services;

public class EventAggregator : EventAggregatorBase
{
    public override void InvokeActionOnUIThread(Action action)
    {
        App.Current.Dispatcher.Invoke(action);
    }
}