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

			// Suscripción a eventos de login/logout
			MessagingCenter.Subscribe<AuthService>(this, MessagingConstants.UserLoggedIn, sender => ShowMainTabs());
			MessagingCenter.Subscribe<AuthService>(this, MessagingConstants.UserLoggedOut, sender => ShowLoginTab());

			// Inicializar pestañas según estado de autenticación
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
				Icon = "icon_login.png",
				Route = nameof(LoginPage),
				ContentTemplate = new DataTemplate(typeof(LoginPage))
			});
		}

		void ShowMainTabs()
		{
			RootTabBar.Items.Clear();

			// Perfil / Cerrar sesión
			RootTabBar.Items.Add(new ShellContent
			{
				Title = "Cerrar sesión",
				Icon = "icon_logout.png",
				Route = nameof(ProfilePage),
				ContentTemplate = new DataTemplate(typeof(ProfilePage))
			});

			// Resto de secciones
			RootTabBar.Items.Add(new ShellContent
			{
				Title = "Saldo",
				Icon = "icon_points.png",
				Route = nameof(PointsPage),
				ContentTemplate = new DataTemplate(typeof(PointsPage))
			});
			RootTabBar.Items.Add(new ShellContent
			{
				Title = "Ofertas",
				Icon = "icon_offers.png",
				Route = nameof(OffersPage),
				ContentTemplate = new DataTemplate(typeof(OffersPage))
			});
			RootTabBar.Items.Add(new ShellContent
			{
				Title = "Alertas",
				Icon = "icon_alerts.png",
				Route = nameof(AlertsPage),
				ContentTemplate = new DataTemplate(typeof(AlertsPage))
			});
			RootTabBar.Items.Add(new ShellContent
			{
				Title = "Catálogo/Canjear",
				Icon = "icon_redeem.png",
				Route = nameof(RedemptionPage),
				ContentTemplate = new DataTemplate(typeof(RedemptionPage))
			});
			RootTabBar.Items.Add(new ShellContent
			{
				Title = "QR",
				Icon = "icon_qr.png",
				Route = nameof(QRCodePage),
				ContentTemplate = new DataTemplate(typeof(QRCodePage))
			});
			RootTabBar.Items.Add(new ShellContent
			{
				Title = "Mis Canjes",
				Icon = "icon_history.png",
				Route = nameof(CanjesPage),
				ContentTemplate = new DataTemplate(typeof(CanjesPage))
			});
			RootTabBar.Items.Add(new ShellContent
			{
				Title = "Historial",
				Icon = "icon_history_alt.png",
				Route = nameof(HistoryPage),
				ContentTemplate = new DataTemplate(typeof(HistoryPage))
			});
		}
	}
}
