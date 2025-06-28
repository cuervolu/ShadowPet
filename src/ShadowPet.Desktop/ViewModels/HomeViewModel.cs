using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace ShadowPet.Desktop.ViewModels
{
    public partial class HomeViewModel : ViewModelBase
    {
        public event Action? StartPetRequested;

        [ObservableProperty]
        private bool _startWithSystem;

        [RelayCommand]
        private void StartPet()
        {
            StartPetRequested?.Invoke();
        }

        [RelayCommand]
        private void ToggleStartWithSystem(bool isChecked)
        {
            Console.WriteLine($"Iniciar con el sistema: {isChecked}");
        }
    }
}
