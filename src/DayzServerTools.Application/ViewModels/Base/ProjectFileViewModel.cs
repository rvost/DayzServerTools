using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DayzServerTools.Application.Models;
using DayzServerTools.Application.Services;
using DayzServerTools.Library.Common;

namespace DayzServerTools.Application.ViewModels.Base
{
    public abstract partial class ProjectFileViewModel<T> : ObservableObject, IProjectFileTab
        where T : IProjectFile
    {
        protected readonly IDialogFactory _dialogFactory;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Name))]
        protected string fileName = "";
        [ObservableProperty]
        protected T model;

        public string Name => Path.GetFileName(FileName);
        public IRelayCommand SaveCommand { get; }
        public IRelayCommand SaveAsCommand { get; }
        public IRelayCommand CloseCommand { get; }

        public event EventHandler CloseRequested;

        public ProjectFileViewModel(IDialogFactory dialogService)
        {
            _dialogFactory = dialogService;

            SaveCommand = new RelayCommand(Save);
            SaveAsCommand = new RelayCommand(SaveAs);
            CloseCommand = new RelayCommand(Close);

        }

        public void Load()
        {
            var dialog = CreateOpenFileDialog();

            dialog.ShowDialog();

            var filename = dialog.FileName;
            if (File.Exists(filename))
            {
                using var input = File.OpenRead(filename);
                try
                {
                    OnLoad(input, filename);
                    FileName = filename;
                }
                catch (InvalidOperationException e)
                {
                    var errorDialog = _dialogFactory.CreateMessageDialog();
                    errorDialog.Title = "File format error";
                    errorDialog.Message = e.InnerException?.Message ?? e.Message;
                    errorDialog.Image = MessageDialogImage.Error;
                    errorDialog.Show();
                    CloseCommand.Execute(null);
                }
            }
            else
            {
                CloseCommand.Execute(null);
            }
        }
        public void Save()
        {
            if (CanSave())
            {
                SaveTo(FileName);
            }
        }
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
        public void Close()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        protected abstract void OnLoad(Stream input, string filename);
        protected abstract IFileDialog CreateOpenFileDialog();
        protected abstract bool CanSave();

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
