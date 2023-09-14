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
    private Chart chartWinners5 = new BarChart();
    [ObservableProperty]
    private Chart chartWinners6 = new BarChart();

    [ObservableProperty]
    private bool isLoading = false;
    
    [ObservableProperty]
    private ObservableCollection<LotteryWinnersDataPOCO> latestWinnersData5Collection = new ObservableCollection<LotteryWinnersDataPOCO>();
    [ObservableProperty]
    private ObservableCollection<LotteryWinnersDataPOCO> latestWinnersData6Collection = new ObservableCollection<LotteryWinnersDataPOCO>();

    public DataPageViewModel(IRestAPI restApi, IKeyCloakService keyCloakService)
    {
        this.restApi = restApi;
        this.keyCloakService = keyCloakService;
    }

    public async void LoadData()
    {
        IsLoading = true;
       
        List<PrizesEntity> lastYearsPrizes5 = await restApi.getLastYearPrizes("5");
        List<PrizesEntity> lastYearsPrizes6 = await restApi.getLastYearPrizes("6");

        List<LotteryWinnersDataPOCO> latestWinnersData5 = await restApi.getLatestWinnersData(5);
        List<LotteryWinnersDataPOCO> latestWinnersData6 = await restApi.getLatestWinnersData(6);

        LoadPrizesData(lastYearsPrizes5, lastYearsPrizes6);

        LoadWinnersTableData(latestWinnersData5, latestWinnersData6);

        LoadWinnersDataForChart(latestWinnersData5, latestWinnersData6);

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

    private void LoadPrizesData(List<PrizesEntity> lastYearsPrizes5, List<PrizesEntity> lastYearsPrizes6)
    {
        SKPaint paint = new SKPaint();
        paint.Color = SKColor.Parse("#1E7836");

        List<ChartEntry> chartEntries5 = new List<ChartEntry>();
        List<ChartEntry> chartEntries6 = new List<ChartEntry>();

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
            ValueLabelTextSize = 15,
            ValueLabelOrientation = Orientation.Horizontal,
            LabelColor = SKColor.Parse("#ffffff"),
            ValueLabelOption = ValueLabelOption.TopOfElement,
            Typeface = SKTypeface.FromFamilyName("Times New Roman", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
            YAxisLinesPaint = paint,
            LabelOrientation = Orientation.Horizontal,
            LabelTextSize = 15,
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
            YAxisLinesPaint = paint,
            LabelOrientation = Orientation.Horizontal,
            LabelTextSize = 15,
        };
    }

    private void LoadWinnersDataForChart(List<LotteryWinnersDataPOCO> latestWinnersData5, List<LotteryWinnersDataPOCO> latestWinnersData6)
    {
        List<ChartEntry> chartEntries5 = new List<ChartEntry>();
        List<ChartEntry> chartEntries6 = new List<ChartEntry>();

        for (int i = 0; i < latestWinnersData5.Count; i++)
        {
            ChartEntry newChartEntry = new ChartEntry(latestWinnersData5[i].numberOfWinners);
            if(i == 0)
            {
                newChartEntry.Color = SKColor.Parse("#35DF59");
            }
            else if(i == 1) 
            {
                newChartEntry.Color = SKColor.Parse("#00FFFF");
            }
            else if (i == 2)
            {
                newChartEntry.Color = SKColor.Parse("#00008B");
            }
            else if (i == 3)
            {
                newChartEntry.Color = SKColor.Parse("#00A36C");
            }
            else if (i == 4)
            {
                newChartEntry.Color = SKColor.Parse("#1F51FF");
            }

            newChartEntry.Label = latestWinnersData5[i].prize.ToString();
            newChartEntry.ValueLabel = latestWinnersData5[i].winnerType.ToString();
            newChartEntry.ValueLabelColor = SKColor.Parse("#35DF59");
            chartEntries5.Add(newChartEntry);
        }

        ChartWinners5 = new DonutChart
        {
            Entries = chartEntries5,
            BackgroundColor = SKColor.Parse("#001220"),
            LabelTextSize = 30
        };

        for (int i = 0; i < latestWinnersData6.Count; i++)
        {
            ChartEntry newChartEntry = new ChartEntry(latestWinnersData6[i].numberOfWinners);
            if (i == 0)
            {
                newChartEntry.Color = SKColor.Parse("#35DF59");
            }
            else if (i == 1)
            {
                newChartEntry.Color = SKColor.Parse("#00FFFF");
            }
            else if (i == 2)
            {
                newChartEntry.Color = SKColor.Parse("#00008B");
            }
            else if (i == 3)
            {
                newChartEntry.Color = SKColor.Parse("#00A36C");
            }
            else if (i == 4)
            {
                newChartEntry.Color = SKColor.Parse("#1F51FF");
            }

            newChartEntry.Label = latestWinnersData6[i].prize.ToString();
            newChartEntry.ValueLabel = latestWinnersData6[i].winnerType.ToString();
            newChartEntry.ValueLabelColor = SKColor.Parse("#35DF59");
            chartEntries6.Add(newChartEntry);
        }

        ChartWinners6 = new DonutChart
        {
            Entries = chartEntries6,
            BackgroundColor = SKColor.Parse("#001220"),
            LabelTextSize = 30
        };
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
