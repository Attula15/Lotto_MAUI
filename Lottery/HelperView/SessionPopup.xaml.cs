using Lottery.ViewModel;

namespace Lottery.HelperView;

public partial class SessionPopup
{
	public SessionPopup(SessionPopuViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}