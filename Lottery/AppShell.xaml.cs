using Lottery.View;

namespace Lottery;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(DrawPage), typeof(DrawPage));
	}
}
