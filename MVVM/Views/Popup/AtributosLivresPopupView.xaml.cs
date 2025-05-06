using CommunityToolkit.Maui.Views;
using Microsoft.Maui.ApplicationModel;
using T20FichaComDB.MVVM.ViewModels;
using T20FichaComDB.MVVM.Views;

namespace T20FichaComDB.MVVM.Views.Popup
{
	public partial class AtributosLivresPopupView : CommunityToolkit.Maui.Views.Popup
	{
		public AtributosLivresPopupView(AtributosLivresPopupViewModel viewModel)
		{
			InitializeComponent();
			BindingContext = viewModel;

			if (viewModel != null)
			{
				viewModel.RequestClose += OnRequestClose;
			}
			this.Closed += (s, e) =>
			{
				if (viewModel != null)
				{
					viewModel.RequestClose -= OnRequestClose;
				}
			};
		}

		private void OnRequestClose()
		{
			MainThread.BeginInvokeOnMainThread(() => Close());
		}
	}
}