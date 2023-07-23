using Lottery.Domain;
using Lottery.Service;
using Lottery.View;
using Lottery.ViewModel;
using Microsoft.Extensions.Logging;

namespace Lottery;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddSingleton<IRestAPI, RestAPI>();

		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddSingleton<MainPageViewModel>();

		builder.Services.AddTransient<DrawPageChooser>();
		builder.Services.AddTransient<DrawPageChooserViewModel>();

		builder.Services.AddTransient<DrawPage>();
		builder.Services.AddTransient<DrawPageViewModel>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
