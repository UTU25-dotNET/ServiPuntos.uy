using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.Services;
using ServiPuntos.Mobile.ViewModels;

namespace ServiPuntos.Mobile.Views
{
    public partial class CatalogoPage : ContentPage
    {
        public CatalogoPage()
        {
            InitializeComponent();


            var services = Application.Current.Handler.MauiContext.Services;
            var productoService = services.GetService<IProductoService>()!;
            var ubicacionService = services.GetService<IUbicacionService>()!;

            BindingContext = new CatalogoViewModel(productoService, ubicacionService);
        }
    }
}
