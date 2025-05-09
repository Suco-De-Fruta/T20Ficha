using CommunityToolkit.Maui.Views;
using Microsoft.Maui.ApplicationModel;
using T20FichaComDB.MVVM.ViewModels;
using T20FichaComDB.MVVM.Views;
using CommunityToolkit.Maui.Extensions;

namespace T20FichaComDB.MVVM.Views.Popups
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