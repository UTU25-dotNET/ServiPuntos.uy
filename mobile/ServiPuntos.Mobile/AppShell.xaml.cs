using ServiPuntos.Mobile.Views;
using Microsoft.Maui.Graphics;
using static ServiPuntos.Mobile.Services.AppLogger;

namespace ServiPuntos.Mobile;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(QRCodePage), typeof(QRCodePage));
		Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
		Routing.RegisterRoute(nameof(CanjesPage), typeof(CanjesPage));
		if (Application.Current.Resources.TryGetValue("PrimaryColor", out var obj)
			&& obj is Color c)
			LogInfo($"[AppShell] PrimaryColor resource is {c}");
		else
			LogInfo("[AppShell] PrimaryColor resource not found or invalid");
	}
}
