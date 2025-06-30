using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.ViewModels;
#if ANDROID
using Android.Util;
#endif

namespace ServiPuntos.Mobile.Views
{
    public partial class CanjesPage : ContentPage
    {
#if ANDROID
        const string TAG = "CanjesPage";
#endif

        public CanjesPage(CanjesViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
#if ANDROID
            Log.Debug(TAG, "Constructor");
#endif
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
#if ANDROID
            Log.Debug(TAG, "OnAppearing - executing RefreshCommand");
#endif
            ((CanjesViewModel)BindingContext).RefreshCommand.Execute(null);
        }
    }
}
