using CommunityToolkit.Mvvm.ComponentModel;

namespace Lottery.HelperView;

public partial class MyNumber : ContentView
{
    public static readonly BindableProperty NumberTitleProperty = BindableProperty.Create(nameof(NumberTitle), typeof(string), typeof(MyNumber), string.Empty);
    public static readonly BindableProperty NumberDrawnProperty = BindableProperty.Create(nameof(NumberDrawn), typeof(bool), typeof(MyNumber), false);
    public static readonly BindableProperty NumberDrawnNotProperty = BindableProperty.Create(nameof(NumberDrawnNot), typeof(bool), typeof(MyNumber), true);
	
    
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
    
    public bool NumberDrawnNot
    {
	    get => (bool)GetValue(MyNumber.NumberDrawnNotProperty);
	    set => SetValue(MyNumber.NumberDrawnNotProperty, value);
    }
    
    public MyNumber()
	{
		InitializeComponent();
	}
}