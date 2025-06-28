using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using Velopack;

namespace ShadowPet.Desktop.ViewModels
{
    public partial class UpdateViewModel : ViewModelBase
    {
        private readonly Window _window;

        public string NewVersion { get; }

        public string ReleaseNotes { get; }

        public ICommand UpdateCommand { get; }

        public ICommand SkipCommand { get; }

        public UpdateViewModel(Window window, UpdateInfo updateInfo)
        {
            _window = window;
            NewVersion = updateInfo.TargetFullRelease.Version.ToString();

            ReleaseNotes = string.IsNullOrWhiteSpace(updateInfo.TargetFullRelease.NotesMarkdown)
                ? "No hay notas para esta version."
                : updateInfo.TargetFullRelease.NotesMarkdown;

            UpdateCommand = new RelayCommand(() => _window.Close(true));
            SkipCommand = new RelayCommand(() => _window.Close(false));
        }
    }
}
