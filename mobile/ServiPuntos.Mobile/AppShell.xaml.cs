using System;
using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.Helpers;
using ServiPuntos.Mobile.Services;
using ServiPuntos.Mobile.Views;

namespace ServiPuntos.Mobile
{
	public partial class AppShell : Shell
	{
		readonly IAuthService _authService;

		public AppShell(IAuthService authService)
		{
			InitializeComponent();
			_authService = authService ?? throw new ArgumentNullException(nameof(authService));

			// Rutas
			Routing.RegisterRoute(nameof(QRCodePage), typeof(QRCodePage));
			Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
			Routing.RegisterRoute(nameof(CanjesPage), typeof(CanjesPage));
			Routing.RegisterRoute(nameof(CatalogPage), typeof(CatalogPage));    // <-- nueva

			// Eventos de login/logout
			MessagingCenter.Subscribe<AuthService>(this, MessagingConstants.UserLoggedIn, sender => ShowMainTabs());
			MessagingCenter.Subscribe<AuthService>(this, MessagingConstants.UserLoggedOut, sender => ShowLoginTab());

			// Estado inicial
			if (_authService.IsLoggedIn)
				ShowMainTabs();
			else
				ShowLoginTab();
		}

		void ShowLoginTab()
		{
			RootTabBar.Items.Clear();
			RootTabBar.Items.Add(new ShellContent
			{
				Title = "Iniciar sesión",
				Route = nameof(LoginPage),
				ContentTemplate = new DataTemplate(typeof(LoginPage))
			});
		}

		void ShowMainTabs()
		{
			RootTabBar.Items.Clear();

			// Cerrar sesión
			RootTabBar.Items.Add(new ShellContent
			{
				Title = "Cerrar sesión",
				Route = nameof(ProfilePage),
				ContentTemplate = new DataTemplate(typeof(ProfilePage))
			});

			// Saldo
			RootTabBar.Items.Add(new ShellContent
			{
				Title = "Saldo",
				Route = nameof(PointsPage),
				ContentTemplate = new DataTemplate(typeof(PointsPage))
			});

			// Ofertas
			RootTabBar.Items.Add(new ShellContent
			{
				Title = "Ofertas",
				Route = nameof(OffersPage),
				ContentTemplate = new DataTemplate(typeof(OffersPage))
			});

			// Alertas
			RootTabBar.Items.Add(new ShellContent
			{
				Title = "Alertas",
				Route = nameof(AlertsPage),
				ContentTemplate = new DataTemplate(typeof(AlertsPage))
			});

			// Catálogo
			RootTabBar.Items.Add(new ShellContent
			{
				Title = "Catálogo",
				Route = nameof(CatalogPage),
				ContentTemplate = new DataTemplate(typeof(CatalogPage))
			});

			// Canjear / Catálogo Legacy
			RootTabBar.Items.Add(new ShellContent
			{
				Title = "Canjear",
				Route = nameof(RedemptionPage),
				ContentTemplate = new DataTemplate(typeof(RedemptionPage))
			});

			// QR
			RootTabBar.Items.Add(new ShellContent
			{
				Title = "QR",
				Route = nameof(QRCodePage),
				ContentTemplate = new DataTemplate(typeof(QRCodePage))
			});

			// Mis Canjes
			RootTabBar.Items.Add(new ShellContent
			{
				Title = "Mis Canjes",
				Route = nameof(CanjesPage),
				ContentTemplate = new DataTemplate(typeof(CanjesPage))
			});

			// Historial
			RootTabBar.Items.Add(new ShellContent
			{
				Title = "Historial",
				Route = nameof(HistoryPage),
				ContentTemplate = new DataTemplate(typeof(HistoryPage))
			});
		}
	}
}
