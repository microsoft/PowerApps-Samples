using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Broker;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace DataverseMauiApp;


public static class MauiProgram
{
  public static MauiApp CreateMauiApp()
  {
    var builder = MauiApp.CreateBuilder();
    builder
      .UseMauiApp<App>()
      .RegisterAuthentication()
      .RegisterHttpClients()
      .RegisterViews()
      .RegisterViewModels()
      .ConfigureFonts(fonts =>
      {
        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
      });

#if DEBUG
    builder.Logging.AddDebug();
#endif

    return builder.Build();
  }

  private static MauiAppBuilder RegisterHttpClients(this MauiAppBuilder mauiAppBuilder)
  {
    mauiAppBuilder.Services.AddHttpClient<EnvironmentsViewModel>();

    return mauiAppBuilder;
  }

  private static MauiAppBuilder RegisterAuthentication(this MauiAppBuilder mauiAppBuilder)
  {
    mauiAppBuilder.Services.AddSingleton(c =>
    {
      // TODO: Replace "Dynamics 365 Example Client Application" id with your own
      return PublicClientApplicationBuilder.Create("51f81489-12ee-4a9e-aaae-a2591f45987d")
                      .WithDefaultRedirectUri()
                      .WithAuthority(AadAuthorityAudience.AzureAdMultipleOrgs)
                      .WithBroker(new BrokerOptions(BrokerOptions.OperatingSystems.Windows))
                      .Build();
    });

    return mauiAppBuilder;
  }
  public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
  {
    mauiAppBuilder.Services.AddTransient<WelcomePage>();
    mauiAppBuilder.Services.AddTransient<EnvironmentsPage>();

    return mauiAppBuilder;
  }

  public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
  {
    mauiAppBuilder.Services.AddTransient<EnvironmentsViewModel>();

    return mauiAppBuilder;
  }
}
