using ModuleCapture.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ModuleCapture
{
    public class ModuleCaptureModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public ModuleCaptureModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion("ContentRegion", typeof(ViewCapture));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
