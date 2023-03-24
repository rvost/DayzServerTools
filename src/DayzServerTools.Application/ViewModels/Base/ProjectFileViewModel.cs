using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DayzServerTools.Application.Models;
using DayzServerTools.Application.Services;
using DayzServerTools.Library.Common;
using FluentValidation;

namespace DayzServerTools.Application.ViewModels.Base
{
    public abstract partial class ProjectFileViewModel<T> : ObservableObject, IProjectFileTab
        where T : IProjectFile
    {
        protected readonly IDialogFactory _dialogFactory;
        protected readonly IValidator<T> _validator;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Name))]
        protected string _fileName = "";
        [ObservableProperty]
        protected T _model;

        public string Name => Path.GetFileName(FileName);

        public IRelayCommand ValidateCommand { get; }

        public event EventHandler CloseRequested;

        public ProjectFileViewModel(string fileName, T model, IValidator<T> validator, IDialogFactory dialogService)
        {
            _model = model;
            _fileName = fileName;
            _dialogFactory = dialogService;
            _validator = validator;

            ValidateCommand = new RelayCommand(() => Validate());
        }

        [RelayCommand]
        public void Save()
        {
            if (CanSave())
            {
                SaveTo(FileName);
            }
        }
        
        [RelayCommand]
        public void SaveAs()
        {
            if (CanSave())
            {
                var dialog = _dialogFactory.CreateSaveFileDialog();
                dialog.FileName = Name;
                if (dialog.ShowDialog() ?? false)
                {
                    var filename = dialog.FileName;
                    SaveTo(filename);
                    FileName = filename;
                }
            }
        }
        
        [RelayCommand]
        public void Close()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        protected abstract bool Validate();

        protected virtual bool CanSave()
        {
            var isValid =  Validate();
            
            if (!isValid)
            {
                var confirmationDialog = _dialogFactory.CreateConfirmationDialog();
                confirmationDialog.Title = "Validation Errors";
                confirmationDialog.Message = "The file contains validation errors! Do you want to save it?";
                confirmationDialog.Button = ConfirmationDialogButton.YesNo;
                confirmationDialog.Image = MessageDialogImage.Warning;

                var res = confirmationDialog.Show();
                return res == ConfirmationDialogResult.Yes;
            }
            
            return true;
        }

        protected void SaveTo(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                using var output = File.Create(filePath);
                Model.WriteToStream(output);
            }
        }
    }
}
