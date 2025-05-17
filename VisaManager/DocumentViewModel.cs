using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows;
using System;
using System.IO;
using CommunityToolkit.Mvvm.Input;

public class DocumentViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private string _name;
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
        }
    }

    private string _filePath;
    public string FilePath
    {
        get => _filePath;
        set
        {
            _filePath = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FilePath)));
        }
    }

    public ICommand ViewCommand { get; }
    public ICommand RemoveCommand { get; }

    private Action<DocumentViewModel> _removeAction;

    public DocumentViewModel(Action<DocumentViewModel> removeAction)
    {
        _removeAction = removeAction;
        ViewCommand = new RelayCommand(ViewDocument);
        RemoveCommand = new RelayCommand(RemoveDocument);
    }

    private void ViewDocument()
    {
        if (!string.IsNullOrEmpty(FilePath) && File.Exists(FilePath))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = FilePath,
                UseShellExecute = true
            });
        }
        else
        {
            MessageBox.Show("No document file available");
        }
    }

    private void RemoveDocument()
    {
        _removeAction?.Invoke(this);
    }
}