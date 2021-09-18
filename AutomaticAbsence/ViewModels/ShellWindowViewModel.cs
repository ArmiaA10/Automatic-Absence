using AutomaticAbsence.Core.Bases;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace AutomaticAbsence.ViewModels
{
    public class ShellWindowViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;

        public ShellWindowViewModel(IDialogService dialogService)
        {
            Title = "Automtaic Absence Aplication";
            _dialogService = dialogService;
        }

        private DelegateCommand _showAboutDialog;

        public DelegateCommand ShowAboutDialog =>
            _showAboutDialog ??= new DelegateCommand(ExecuteShowAboutDialog);

        private void ExecuteShowAboutDialog()
        {
            var p = new DialogParameters
            {
                { "message", "about page is about page" }
            };

            _dialogService.ShowDialog("AboutDialog", p, _ => { });
        }
    }
}
