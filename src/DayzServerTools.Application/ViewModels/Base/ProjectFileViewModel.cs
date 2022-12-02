﻿using CommunityToolkit.Mvvm.ComponentModel;
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
        public IRelayCommand SaveCommand { get; }
        public IRelayCommand SaveAsCommand { get; }
        public IRelayCommand CloseCommand { get; }
        public IRelayCommand ValidateCommand { get; }

        public event EventHandler CloseRequested;

        public ProjectFileViewModel(string fileName, T model, IValidator<T> validator, IDialogFactory dialogService)
        {
            _model = model;
            _fileName = fileName;
            _dialogFactory = dialogService;
            _validator = validator;

            SaveCommand = new RelayCommand(Save);
            SaveAsCommand = new RelayCommand(SaveAs);
            CloseCommand = new RelayCommand(Close);
            ValidateCommand = new RelayCommand(() => Validate());
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
                    //OnLoad(input, filename);
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

        protected abstract IFileDialog CreateOpenFileDialog();
        protected abstract bool Validate();
        
        protected virtual bool CanSave() => Validate();
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
