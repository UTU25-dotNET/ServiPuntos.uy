using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.Views;

namespace ServiPuntos.Mobile;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(TokenDisplayPage), typeof(TokenDisplayPage));
		Routing.RegisterRoute(nameof(QRCodePage), typeof(QRCodePage));
		Routing.RegisterRoute(nameof(LocationsPage), typeof(LocationsPage));
		Routing.RegisterRoute(nameof(CatalogPage), typeof(CatalogPage));
		Routing.RegisterRoute(nameof(MapPage), typeof(MapPage));
	}
}
