using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lottery.Domain;
using Lottery.Domain.Entity;
using Lottery.Domain.ResponseBody;
using Lottery.View;
using Microcharts;
using SkiaSharp;
using System.Collections.ObjectModel;
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
    private bool isScrollViewVisible = true;

    [ObservableProperty]
    private bool isChart5ExtentededVisible = false;
    [ObservableProperty]
    private bool isChart6ExtentededVisible = false;

    [ObservableProperty]
    private bool isLoading = false;
    
    [ObservableProperty]
    private ObservableCollection<LotteryWinnersDataPOCO> latestWinnersData5Collection = new ObservableCollection<LotteryWinnersDataPOCO>();
    [ObservableProperty]
    private ObservableCollection<LotteryWinnersDataPOCO> latestWinnersData6Collection = new ObservableCollection<LotteryWinnersDataPOCO>();

    private List<ChartEntry> chartEntries5;
    private List<ChartEntry> chartEntries6;

    public DataPageViewModel(IRestAPI restApi, IKeyCloakService keyCloakService)
    {
        this.restApi = restApi;
        this.keyCloakService = keyCloakService;
    }

    [RelayCommand]
    private void Chart5ExtendedTapped()
    {
        if (DeviceInfo.Current.Platform == DevicePlatform.Android)
        {
            SKPaint paint = new SKPaint();
            paint.Color = SKColor.Parse("#1E7836");

            if (IsChart5ExtentededVisible)
            {
                Chart5 = new LineChart
                {
                    Entries = chartEntries5,
                    BackgroundColor = SKColor.Parse("#001220"),
                    ShowYAxisLines = true,
                    ValueLabelTextSize = 25,
                    ValueLabelOrientation = Orientation.Horizontal,
                    LabelColor = SKColor.Parse("#ffffff"),
                    ValueLabelOption = ValueLabelOption.TopOfElement,
                    Typeface = SKTypeface.FromFamilyName("Times New Roman", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
                    YAxisLinesPaint = paint,
                    LabelOrientation = Orientation.Horizontal,
                    LabelTextSize = 0
                };
            }
            else
            {
                Chart5 = new LineChart
                {
                    Entries = chartEntries5,
                    BackgroundColor = SKColor.Parse("#001220"),
                    ShowYAxisLines = true,
                    ValueLabelTextSize = 25,
                    ValueLabelOrientation = Orientation.Horizontal,
                    LabelColor = SKColor.Parse("#ffffff"),
                    ValueLabelOption = ValueLabelOption.TopOfElement,
                    Typeface = SKTypeface.FromFamilyName("Times New Roman", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
                    YAxisLinesPaint = paint,
                    LabelOrientation = Orientation.Horizontal,
                    LabelTextSize = 25,
                };
            }

            IsScrollViewVisible = !IsScrollViewVisible;
            IsChart5ExtentededVisible = !IsChart5ExtentededVisible;
        }
    }
    
    [RelayCommand]
    private void Chart6ExtendedTapped()
    {
        if (DeviceInfo.Current.Platform == DevicePlatform.Android)
        {
            SKPaint paint = new SKPaint();
            paint.Color = SKColor.Parse("#1E7836");

            if (IsChart6ExtentededVisible)
            {
                Chart6 = new LineChart
                {
                    Entries = chartEntries6,
                    BackgroundColor = SKColor.Parse("#001220"),
                    ShowYAxisLines = true,
                    ValueLabelTextSize = 25,
                    ValueLabelOrientation = Orientation.Horizontal,
                    LabelColor = SKColor.Parse("#ffffff"),
                    ValueLabelOption = ValueLabelOption.TopOfElement,
                    Typeface = SKTypeface.FromFamilyName("Times New Roman", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
                    YAxisLinesPaint = paint,
                    LabelOrientation = Orientation.Horizontal,
                    LabelTextSize = 0,
                };
            }
            else
            {
                Chart6 = new LineChart
                {
                    Entries = chartEntries6,
                    BackgroundColor = SKColor.Parse("#001220"),
                    ShowYAxisLines = true,
                    ValueLabelTextSize = 25,
                    ValueLabelOrientation = Orientation.Horizontal,
                    LabelColor = SKColor.Parse("#ffffff"),
                    ValueLabelOption = ValueLabelOption.TopOfElement,
                    Typeface = SKTypeface.FromFamilyName("Times New Roman", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
                    YAxisLinesPaint = paint,
                    LabelOrientation = Orientation.Horizontal,
                    LabelTextSize = 25,
                };
            }
            
            IsScrollViewVisible = !IsScrollViewVisible;
            IsChart6ExtentededVisible = !IsChart6ExtentededVisible;
        }
    }

    public async void LoadData()
    {
        IsLoading = true;
       
        List<PrizesPOCO> lastYearsPrizes5 = await restApi.getLastYearPrizes("5");
        List<PrizesPOCO> lastYearsPrizes6 = await restApi.getLastYearPrizes("6");

        List<LotteryWinnersDataPOCO> latestWinnersData5 = await restApi.getLatestWinnersData(5);
        List<LotteryWinnersDataPOCO> latestWinnersData6 = await restApi.getLatestWinnersData(6);

        LoadPrizesData(lastYearsPrizes5, lastYearsPrizes6);

        LoadWinnersTableData(latestWinnersData5, latestWinnersData6);

        IsLoading = false;
    }

    private void LoadWinnersTableData(List<LotteryWinnersDataPOCO> latestWinnersData5, List<LotteryWinnersDataPOCO> latestWinnersData6)
    {
        ObservableCollection<LotteryWinnersDataPOCO> temp5 = new ObservableCollection<LotteryWinnersDataPOCO>();
        ObservableCollection<LotteryWinnersDataPOCO> temp6 = new ObservableCollection<LotteryWinnersDataPOCO>();

        for (int i = 0; i < latestWinnersData5.Count; i++)
        {
            Debug.WriteLine("Added prize: " + latestWinnersData5[i].prize);
            temp5.Add(latestWinnersData5[i]);
            temp6.Add(latestWinnersData6[i]);
        }

        LatestWinnersData5Collection = temp5;
        LatestWinnersData6Collection = temp6;
    }

    private void LoadPrizesData(List<PrizesPOCO> lastYearsPrizes5, List<PrizesPOCO> lastYearsPrizes6)
    {
        if(lastYearsPrizes5 == null || lastYearsPrizes6 == null)
        {
            return;
        }

        SKPaint paint = new SKPaint();
        paint.Color = SKColor.Parse("#1E7836");

        chartEntries5 = new List<ChartEntry>();
        chartEntries6 = new List<ChartEntry>();

        for (int i = 0; i < lastYearsPrizes5.Count; i++)
        {
            ChartEntry newChartEntry = new ChartEntry(lastYearsPrizes5[i].prize);
            newChartEntry.Color = SKColor.Parse("#35DF59");
            newChartEntry.ValueLabelColor = SKColor.Parse("#35DF59");
            newChartEntry.Label = lastYearsPrizes5[i].date.ToShortDateString();
            newChartEntry.ValueLabel = lastYearsPrizes5[i].prize.ToString();
            chartEntries5.Add(newChartEntry);
        }

        for (int i = 0; i < lastYearsPrizes6.Count; i++)
        {
            ChartEntry newChartEntry = new ChartEntry(lastYearsPrizes6[i].prize);
            newChartEntry.Color = SKColor.Parse("#35DF59");
            newChartEntry.ValueLabelColor = SKColor.Parse("#35DF59");
            newChartEntry.Label = lastYearsPrizes6[i].date.ToShortDateString();
            newChartEntry.ValueLabel = lastYearsPrizes6[i].prize.ToString();
            chartEntries6.Add(newChartEntry);
        }

        Chart5 = new LineChart
        {
            Entries = chartEntries5,
            BackgroundColor = SKColor.Parse("#001220"),
            ShowYAxisLines = true,
            ValueLabelTextSize = 25,
            ValueLabelOrientation = Orientation.Horizontal,
            LabelColor = SKColor.Parse("#ffffff"),
            ValueLabelOption = ValueLabelOption.TopOfElement,
            Typeface = SKTypeface.FromFamilyName("Times New Roman", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
            YAxisLinesPaint = paint,
            LabelOrientation = Orientation.Horizontal,
            LabelTextSize = 0
        };

        Chart6 = new LineChart
        {
            Entries = chartEntries6,
            BackgroundColor = SKColor.Parse("#001220"),
            ShowYAxisLines = true,
            ValueLabelTextSize = 25,
            ValueLabelOrientation = Orientation.Horizontal,
            LabelColor = SKColor.Parse("#ffffff"),
            ValueLabelOption = ValueLabelOption.TopOfElement,
            Typeface = SKTypeface.FromFamilyName("Times New Roman", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
            YAxisLinesPaint = paint,
            LabelOrientation = Orientation.Horizontal,
            LabelTextSize = 0,
        };
        
        if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
        {
            Chart5.LabelTextSize = 20;
            Chart6.LabelTextSize = 20;
        }
    }

    [RelayCommand]
    private async Task Logout()
    {
        bool success = await keyCloakService.Logout();

        if (success)
        {
            Debug.WriteLine("Logged out");
            App.Current.MainPage = new LoginPage(new LoginViewModel(keyCloakService));
        }
    }
}
