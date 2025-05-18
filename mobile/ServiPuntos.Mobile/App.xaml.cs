using Microsoft.Maui.Controls;

namespace ServiPuntos.Mobile
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();
			// Esto te asegura que lo primero que se ve es tu TenantSelectorPage
			MainPage = new NavigationPage(new Views.TenantSelectorPage());
		}
	}
}
