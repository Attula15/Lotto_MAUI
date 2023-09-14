using Lottery.Domain;
using Lottery.Service;
using Lottery.View;
using Lottery.ViewModel;
using Microsoft.Extensions.Logging;
using Microcharts.Maui;

namespace Lottery;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMicrocharts()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddSingleton<IRestAPI, RestAPI>();

		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddSingleton<MainPageViewModel>();

		builder.Services.AddTransient<DrawPage>();
		builder.Services.AddTransient<DrawPageViewModel>();

		builder.Services.AddTransient<SavedPage>();
		builder.Services.AddTransient<SavedPageViewModel>();

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<LoginViewModel>();

		builder.Services.AddTransient<DataPage>();
		builder.Services.AddTransient<DataPageViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
