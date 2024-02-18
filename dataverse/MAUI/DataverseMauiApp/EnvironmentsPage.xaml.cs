using System.Diagnostics;
using System.Windows.Input;

namespace DataverseMauiApp;

public partial class EnvironmentsPage : ContentPage
{
  EnvironmentsViewModel _viewModel;

  public EnvironmentsPage(EnvironmentsViewModel viewModel)
	{
		InitializeComponent();
    _viewModel = viewModel;
	}

  protected override async void OnNavigatedTo(NavigatedToEventArgs args)
  {
    base.OnNavigatedTo(args);

    await _viewModel.RefreshAsync().ConfigureAwait(false);

    await Application.Current.Dispatcher.DispatchAsync(() => { _environments.ItemsSource = _viewModel; }).ConfigureAwait(false);

    Debug.WriteLine("Binding complete");
  }

  protected EnvironmentsViewModel ViewModel => BindingContext as EnvironmentsViewModel;
}
