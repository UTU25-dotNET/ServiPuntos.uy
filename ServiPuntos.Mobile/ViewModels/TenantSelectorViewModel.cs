using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.ViewModels
{
    public class TenantSelectorViewModel : BindableObject
    {
        public ObservableCollection<TenantConfig> Tenants { get; set; }

        private TenantConfig _selectedTenant;
        public TenantConfig SelectedTenant
        {
            get => _selectedTenant;
            set
            {
                if (_selectedTenant != value)
                {
                    _selectedTenant = value;
                    OnPropertyChanged();
                    (ContinueCommand as Command)?.ChangeCanExecute();
                }
            }
        }

        public ICommand ContinueCommand { get; }

        public TenantSelectorViewModel()
        {
            Tenants = new ObservableCollection<TenantConfig>
            {
                new TenantConfig { Id = "1", Name = "Ancap", LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/4/45/Logo_Ancap.png", PrimaryColor = "#FFD600", SecondaryColor = "#000000" },
                new TenantConfig { Id = "2", Name = "DUCSA", LogoUrl = "https://www.ducsa.com.uy/img/logo-ducsa.svg", PrimaryColor = "#004987", SecondaryColor = "#FFFFFF" },
                new TenantConfig { Id = "3", Name = "Petrobras", LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/4/44/Petrobras_logo.svg", PrimaryColor = "#006341", SecondaryColor = "#F6DF4A" }
            };

            ContinueCommand = new Command(OnContinue, CanContinue);
        }

        private async void OnContinue()
        {
            if (SelectedTenant != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new Views.LoginPage(SelectedTenant));
            }
        }

        private bool CanContinue() => SelectedTenant != null;
    }
}
