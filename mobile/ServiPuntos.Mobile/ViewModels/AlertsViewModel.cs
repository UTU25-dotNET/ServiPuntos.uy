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
        public ObservableCollection<NotificationDto> Notifications { get; } = new ObservableCollection<NotificationDto>();

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
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

        private async Task LoadNotificationsAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            Notifications.Clear();
            var items = await _notificationService.GetMyNotificationsAsync();
            foreach (var item in items)
            {
                Notifications.Add(item);
            }
            IsBusy = false;
        }
    }
}
