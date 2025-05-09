using NetForge.Core;
using ShareLingo.Core.Services;

namespace ShareLingo.Core.ViewModel
{
    public partial class ModuleEditorViewModel : PageViewModelBase
    {
        #region Fields
        private readonly IDataManager dataManager;
        #endregion

        #region Constructors
        public ModuleEditorViewModel(IEventAggregator eventAggregator, IDataManager dataManager)
            : base(eventAggregator)
        {
            this.dataManager = dataManager;
        }
        #endregion

        #region Properties
        public override IViewModelDataParameter? DataParameter { get; set; }
        #endregion

        #region Methods
        public override void Dispose()
        {
        }
        #endregion
    }
}
