using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace ServiPuntos.Mobile.ViewModels
{
    public class TenantConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
    }

    public class TenantSelectorViewModel : BindableObject
    {
        public ObservableCollection<TenantConfig> Tenants { get; set; }
        private TenantConfig _selectedTenant;
        public TenantConfig SelectedTenant
        {
            get => _selectedTenant;
            set
            {
                _selectedTenant = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanContinue));
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

        private void OnContinue()
        {
            // Acción al pulsar Continuar
        }

        public bool CanContinue()
        {
            return SelectedTenant != null;
        }
    }
}
