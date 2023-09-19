using CommunityToolkit.Mvvm.ComponentModel;

namespace Lottery.HelperView;

public partial class MyNumber : ContentView
{
    public static readonly BindableProperty NumberTitleProperty = BindableProperty.Create(nameof(NumberTitle), typeof(string), typeof(MyNumber), string.Empty);
    public static readonly BindableProperty NumberDrawnProperty = BindableProperty.Create(nameof(NumberDrawn), typeof(bool), typeof(MyNumber), false);
    
    public string NumberTitle
    {
        get => (string)GetValue(MyNumber.NumberTitleProperty);
        set => SetValue(MyNumber.NumberTitleProperty, value);
    }

    public bool NumberDrawn
    {
	    get => (bool)GetValue(MyNumber.NumberDrawnProperty);
	    set => SetValue(MyNumber.NumberDrawnProperty, value);
    }
    
    public MyNumber()
	{
		InitializeComponent();
	}
}