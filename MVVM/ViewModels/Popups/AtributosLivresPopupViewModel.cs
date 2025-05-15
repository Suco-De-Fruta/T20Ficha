using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace T20FichaComDB.MVVM.ViewModels.Popups
{
    public partial class AtributosLivresPopupViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _mensagem;

        public event Action RequestClose;

        public AtributosLivresPopupViewModel(string mensagem)
        {
            Mensagem = mensagem;
        }

        [RelayCommand]
        private void FecharPopup()
        {
            RequestClose?.Invoke();
        }
    }
}
