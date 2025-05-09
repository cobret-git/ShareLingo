using NetForge.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace ShareLingo.WinUI.Services
{
    public class EventAggregator : EventAggregatorBase
    {
        public override void InvokeActionOnUIThread(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            if (dispatcherQueue != null)
            {
                if (!dispatcherQueue.HasThreadAccess)
                {
                    dispatcherQueue.TryEnqueue(() => action());
                }
                else
                {
                    action();
                }
            }
            else
            {
                action();
            }
        }
    }
}
