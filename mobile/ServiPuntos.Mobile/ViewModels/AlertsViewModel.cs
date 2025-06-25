using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;

namespace ServiPuntos.Mobile.ViewModels
{
    public class AlertsViewModel : BindableObject
    {
        private readonly INotificationService _notificationService;
        public ObservableCollection<NotificationDto> Notifications { get; } = new();

        bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy == value) return;
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand { get; }

        public AlertsViewModel(INotificationService notificationService)
        {
            _notificationService = notificationService;
            RefreshCommand = new Command(async () => await LoadNotificationsAsync());
        }

        async Task LoadNotificationsAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                Notifications.Clear();
                var items = await _notificationService.GetMyNotificationsAsync();

                foreach (var item in items)
                    Notifications.Add(item);
            }
            catch (HttpRequestException)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error de red",
                    "No se pudieron cargar las alertas. Verifica tu conexión e inténtalo de nuevo.",
                    "OK");
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error inesperado",
                    "Ocurrió un error al cargar las alertas.",
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
