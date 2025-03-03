using Microsoft.UI.Dispatching;
using NetForge.Core;
using System;

namespace ShareLingo.WinUI.Services
{
    public class EventAggregator : EventAggregatorBase
    {
        public override void InvokeActionOnUIThread(Action action)
        {
            DispatcherQueue.GetForCurrentThread().TryEnqueue(action.Invoke);
        }
    }
}
