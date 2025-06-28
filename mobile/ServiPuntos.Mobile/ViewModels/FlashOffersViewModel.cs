using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;

namespace ServiPuntos.Mobile.ViewModels
{
    public class FlashOffersViewModel : BindableObject
    {
        private readonly IOfferService _offerService;
        private bool _isBusy;
        private ObservableCollection<OfferDto> _offers;

        public ICommand RefreshCommand { get; }

        public ObservableCollection<OfferDto> Offers
        {
            get => _offers;
            set { _offers = value; OnPropertyChanged(); }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); ((Command)RefreshCommand).ChangeCanExecute(); }
        }

        public FlashOffersViewModel(IOfferService offerService)
        {
            _offerService = offerService;
            Offers = new ObservableCollection<OfferDto>();
            RefreshCommand = new Command(async () => await LoadOffersAsync(), () => !IsBusy);
        }

        private async Task LoadOffersAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var list = await _offerService.GetOffersAsync();
                Offers.Clear();
                foreach (var o in list)
                    Offers.Add(o);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar ofertas: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
