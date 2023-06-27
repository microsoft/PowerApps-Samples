using Microsoft.Identity.Client;

namespace DataverseMauiApp;

public partial class WelcomePage : ContentPage
{
	public WelcomePage()
	{
		InitializeComponent();
	}

	private async void OnLoadEnvironmentsClicked(object sender, EventArgs e)
	{
    await Navigation.PushAsync(Handler.MauiContext.Services.GetService<EnvironmentsPage>());

    SemanticScreenReader.Announce(LoadEnvironmentsBtn.Text);
	}
}

