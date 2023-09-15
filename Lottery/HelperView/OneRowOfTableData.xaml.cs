namespace Lottery.HelperView;

public partial class OneRowOfTableData : ContentView
{
    public static readonly BindableProperty PrizeProperty = BindableProperty.Create(nameof(Prize), typeof(string), typeof(OneRowOfTableData), string.Empty);
    public static readonly BindableProperty NumberOfWinnersProperty = BindableProperty.Create(nameof(NumberOfWinners), typeof(string), typeof(OneRowOfTableData), string.Empty);
    public static readonly BindableProperty WinnerTypeProperty = BindableProperty.Create(nameof(WinnerType), typeof(string), typeof(OneRowOfTableData), string.Empty);

    public string Prize
    {
        get => (string)GetValue(OneRowOfTableData.PrizeProperty);
        set => SetValue(OneRowOfTableData.PrizeProperty, value);
    }
    public string NumberOfWinners
    {
        get => (string)GetValue(OneRowOfTableData.NumberOfWinnersProperty);
        set => SetValue(OneRowOfTableData.NumberOfWinnersProperty, value);
    }
    public string WinnerType
    {
        get => (string)GetValue(OneRowOfTableData.WinnerTypeProperty);
        set => SetValue(OneRowOfTableData.WinnerTypeProperty, value);
    }
    public OneRowOfTableData()
	{
		InitializeComponent();
	}
}