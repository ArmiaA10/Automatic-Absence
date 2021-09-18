using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace AutomaticAbsence.Dialogs
{
    public class AboutDialogViewModel : BindableBase, IDialogAware
    {
        private DelegateCommand _closeDialogCommand;

        public event Action<IDialogResult> RequestClose;

        public DelegateCommand CloseDialogCommand =>
            _closeDialogCommand ??= new DelegateCommand(ExecuteCloseDialogCommand);

        public string Title => "About Automatic Absence";

        private void ExecuteCloseDialogCommand()
        {
            const ButtonResult result = ButtonResult.OK;

            var p = new DialogParameters
            {
                { "myParam", "Close" }
            };

            RequestClose?.Invoke(new DialogResult(result, p));
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
