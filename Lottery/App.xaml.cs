using Lottery.Domain;
using Lottery.View;
using Lottery.ViewModel;

namespace Lottery;

public partial class App : Application
{
    public App(IKeyCloakService key)
	{
		InitializeComponent();
		LoginViewModel vm = new LoginViewModel(key);
		MainPage = new LoginPage(vm);
	}
}
