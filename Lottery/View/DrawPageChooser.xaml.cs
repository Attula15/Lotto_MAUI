using Lottery.ViewModel;
using System.Diagnostics;

namespace Lottery.View;

public partial class DrawPageChooser : ContentPage
{
	public DrawPageChooser(DrawPageChooserViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}