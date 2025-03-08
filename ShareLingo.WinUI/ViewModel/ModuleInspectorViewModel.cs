using NetForge.Core;
using NetForge.Core.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareLingo.WinUI.ViewModel
{
    public partial class ModuleInspectorViewModel : PageViewModelBase
    {
        public ModuleInspectorViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        protected override void OnPageClosed(PageClosedEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OnPageClosing(PageClosingEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
