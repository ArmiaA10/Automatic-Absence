using AutomaticAbsence.Dialogs;
using AutomaticAbsence.Views;
using ModuleCapture;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace AutomaticAbsence
{
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<ShellWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<AboutDialog, AboutDialogViewModel>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ModuleCaptureModule>();
        }
    }
}
