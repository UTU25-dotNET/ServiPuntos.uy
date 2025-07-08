using Android.App;
using Android.Runtime;
using Plugin.FirebasePushNotification;

namespace ServiPuntos.Mobile;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

	protected override MauiApp CreateMauiApp()
        {
            var app = MauiProgram.CreateMauiApp();
#if DEBUG
            FirebasePushNotificationManager.Initialize(this, true);
#else
            FirebasePushNotificationManager.Initialize(this, false);
#endif
            return app;
        }
}
