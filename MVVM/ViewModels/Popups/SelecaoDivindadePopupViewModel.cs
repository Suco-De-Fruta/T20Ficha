using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using T20FichaComDB.Data.Entities; 
using static T20FichaComDB.MVVM.ViewModels.PersonagemViewModel;

namespace T20FichaComDB.MVVM.ViewModels.Popups
{
    public partial class SelecaoDivindadePopupViewModel : ObservableObject
    {
        private readonly PersonagemViewModel _personagemViewModel;

        public ObservableCollection<GrupoDivindade> DivindadesAgrupadasNoPopup { get; }

        public event Action RequestClose;

        public SelecaoDivindadePopupViewModel (PersonagemViewModel personagemViewModel)
        {
            _personagemViewModel = personagemViewModel;
            DivindadesAgrupadasNoPopup = _personagemViewModel.DivindadesAgrupadas;
        }

        [RelayCommand]
        private void SelecionarDivindade(DivindadesData divindadeSelecionada)
        {
            if (divindadeSelecionada != null)
            {
                _personagemViewModel.DivindadeSelecionadaObj = divindadeSelecionada;
            }
            RequestClose?.Invoke();
        }

        [RelayCommand]
        private void FecharPopup()
        {
            RequestClose?.Invoke();
        }
    }
}
