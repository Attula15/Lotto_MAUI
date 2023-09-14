using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lottery.Domain;
using Lottery.Domain.Entity;
using Lottery.View;
using Microcharts;
using SkiaSharp;
using System.Diagnostics;

namespace Lottery.ViewModel;
public partial class DataPageViewModel : ObservableObject
{
    private readonly IRestAPI restApi;

    private readonly IKeyCloakService keyCloakService;

    [ObservableProperty]
    private Chart chart5 = new BarChart();
    
    [ObservableProperty]
    private Chart chart6 = new BarChart();

    [ObservableProperty]
    private bool isLoading = false;

    public DataPageViewModel(IRestAPI restApi, IKeyCloakService keyCloakService)
    {
        this.restApi = restApi;
        this.keyCloakService = keyCloakService;
    }

    public async void LoadData()
    {
        IsLoading = true;
        SKPaint paint = new SKPaint();
        paint.Color = SKColor.Parse("#1E7836");

        List<PrizesEntity> lastYearsPrizes5 = await restApi.getLastYearPrizes("5");
        List<PrizesEntity> lastYearsPrizes6 = await restApi.getLastYearPrizes("6");

        List<ChartEntry> chartEntries5 = new List<ChartEntry>();
        List<ChartEntry> chartEntries6 = new List<ChartEntry>();

        for(int i = 0; i < lastYearsPrizes5.Count; i++)
        {
            ChartEntry newChartEntry = new ChartEntry(lastYearsPrizes5[i].prize);
            newChartEntry.Color = SKColor.Parse("#35DF59");
            newChartEntry.ValueLabelColor = SKColor.Parse("#35DF59");
            //Debug.WriteLine(lastYearsPrizes5[i].date);
            //newChartEntry.Label = lastYearsPrizes5[i].date.ToShortDateString();
            newChartEntry.ValueLabel = lastYearsPrizes5[i].prize.ToString();
            chartEntries5.Add(newChartEntry);
        }

        for (int i = 0; i < lastYearsPrizes6.Count; i++)
        {
            ChartEntry newChartEntry = new ChartEntry(lastYearsPrizes6[i].prize);
            newChartEntry.Color = SKColor.Parse("#35DF59");
            newChartEntry.ValueLabelColor = SKColor.Parse("#35DF59");
            newChartEntry.ValueLabel = lastYearsPrizes6[i].prize.ToString();
            chartEntries6.Add(newChartEntry);
        }

        Chart5 = new LineChart
        {
            Entries = chartEntries5,
            BackgroundColor = SKColor.Parse("#001220"),
            ShowYAxisLines = true,
            ValueLabelTextSize = 15,
            ValueLabelOrientation = Orientation.Horizontal,
            LabelColor = SKColor.Parse("#ffffff"),
            ValueLabelOption = ValueLabelOption.TopOfElement,
            Typeface = SKTypeface.FromFamilyName("Times New Roman", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
            YAxisLinesPaint = paint
        };

        Chart6 = new LineChart
        {
            Entries = chartEntries6,
            BackgroundColor = SKColor.Parse("#001220"),
            ShowYAxisLines = true,
            ValueLabelTextSize = 15,
            ValueLabelOrientation = Orientation.Horizontal,
            LabelColor = SKColor.Parse("#ffffff"),
            ValueLabelOption = ValueLabelOption.TopOfElement,
            Typeface = SKTypeface.FromFamilyName("Times New Roman", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
            YAxisLinesPaint = paint
        };

        IsLoading = false;
    }

    [RelayCommand]
    private async void Logout()
    {
        bool success = await keyCloakService.Logout();

        if (success)
        {
            Debug.WriteLine("Logged out");
            App.Current.MainPage = new LoginPage(new LoginViewModel(keyCloakService));
        }
    }
}
