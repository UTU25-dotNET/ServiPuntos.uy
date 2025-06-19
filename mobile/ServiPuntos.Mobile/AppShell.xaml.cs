using ServiPuntos.Mobile.Views;

namespace ServiPuntos.Mobile;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(TokenDisplayPage), typeof(TokenDisplayPage));
	}
}
