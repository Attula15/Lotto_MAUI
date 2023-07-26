using CommunityToolkit.Mvvm.ComponentModel;

namespace Lottery.HelperView;

public partial class MyNumber : ContentView
{
    public static readonly BindableProperty NumberTitleProperty = BindableProperty.Create(nameof(NumberTitle), typeof(string), typeof(MyNumber), string.Empty);

    public string NumberTitle
    {
        get => (string)GetValue(MyNumber.NumberTitleProperty);
        set => SetValue(MyNumber.NumberTitleProperty, value);
    }
    public MyNumber()
	{
		InitializeComponent();
	}
}