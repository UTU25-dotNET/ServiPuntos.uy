using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.ViewModels;
using ServiPuntos.Mobile.Services;

namespace ServiPuntos.Mobile.Views
{
    public partial class CatalogoPage : ContentPage
    {
        public CatalogoPage()
        {
            InitializeComponent();


            var productoService = Application.Current?.Handler?.MauiContext?.Services?.GetService<ProductoService>();
            var ubicacionService = Application.Current?.Handler?.MauiContext?.Services?.GetService<UbicacionService>();


            string tenantId = "MI_TENANT_ID";

            this.BindingContext = new CatalogoViewModel(productoService, ubicacionService, tenantId);
        }
    }
}
