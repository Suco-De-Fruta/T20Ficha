using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using T20FichaComDB.Data.Entities;
using System;

namespace T20FichaComDB.MVVM.ViewModels.Popups
{
    public partial class DetalhesPoderesPopupViewModel : ObservableObject
    {
        [ObservableProperty]
        private PoderesData _poder;

        public event Action RequestClose;

        public DetalhesPoderesPopupViewModel(PoderesData poder)
        {
            Poder = poder;
        }

        [RelayCommand]
        private void FecharPopup()
        {
            RequestClose?.Invoke(); 
        }
    }
}
