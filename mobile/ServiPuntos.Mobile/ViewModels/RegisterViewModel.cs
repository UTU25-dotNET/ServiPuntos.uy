using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;

namespace ServiPuntos.Mobile.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private readonly IAuthService _authService;
        private readonly PushNotificationService _pushService;
        private bool _isLoading;
        private bool _isRegisterEnabled = true;
        private TenantResponse? _selectedTenant;
        private string _nombre = string.Empty;
        private string _ci = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _errorMessage = string.Empty;

        public RegisterViewModel(IAuthService authService, PushNotificationService pushService)
        {
            _authService = authService;
            _pushService = pushService;
            Tenants = new ObservableCollection<TenantResponse>();
            RegisterCommand = new Command(async () => await OnRegister(), () => IsRegisterEnabled);
            LoadTenantsCommand = new Command(async () => await LoadTenants());
            
            // Cargar tenants al inicializar
            _ = Task.Run(async () => await LoadTenants());
        }

        public ObservableCollection<TenantResponse> Tenants { get; }
        public ICommand RegisterCommand { get; }
        public ICommand LoadTenantsCommand { get; }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                IsRegisterEnabled = !value;
            }
        }

        public bool IsRegisterEnabled
        {
            get => _isRegisterEnabled;
            set
            {
                _isRegisterEnabled = value;
                OnPropertyChanged();
                ((Command)RegisterCommand).ChangeCanExecute();
            }
        }

        public TenantResponse? SelectedTenant
        {
            get => _selectedTenant;
            set
            {
                _selectedTenant = value;
                OnPropertyChanged();
                ValidateForm();
            }
        }

        public string Nombre
        {
            get => _nombre;
            set
            {
                _nombre = value;
                OnPropertyChanged();
                ValidateForm();
            }
        }

        public string Ci
        {
            get => _ci;
            set
            {
                _ci = value;
                OnPropertyChanged();
                ValidateForm();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
                ValidateForm();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                ValidateForm();
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged();
                ValidateForm();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasError));
            }
        }

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        private async Task LoadTenants()
        {
            try
            {
                Console.WriteLine("[RegisterViewModel] Cargando tenants...");
                IsLoading = true;
                ErrorMessage = string.Empty;

                var tenants = await _authService.GetTenantsAsync();
                
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Tenants.Clear();
                    foreach (var tenant in tenants)
                    {
                        Tenants.Add(tenant);
                    }
                });

                Console.WriteLine($"[RegisterViewModel] {tenants.Count} tenants cargados");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RegisterViewModel] Error cargando tenants: {ex.Message}");
                ErrorMessage = "Error al cargar la lista de organizaciones";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task OnRegister()
        {
            try
            {
                Console.WriteLine("[RegisterViewModel] === INICIO REGISTRO ===");
                IsLoading = true;
                ErrorMessage = string.Empty;

                if (!ValidateForm())
                {
                    return;
                }

                var request = new RegisterRequest
                {
                    Nombre = Nombre.Trim(),
                    Email = Email.Trim().ToLower(),
                    Password = Password,
                    Ci = Ci.Trim(),
                    TenantId = Guid.Parse(SelectedTenant!.Id)
                };

                var success = await _authService.RegisterAsync(request);

                if (success)
                {
                    await _pushService.RegisterAndRetrieveTokenAsync();
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            "Registro Exitoso",
                            "Tu cuenta ha sido creada exitosamente. Ahora puedes iniciar sesión.",
                            "OK");
                        
                        // Navegar de vuelta al login
                        await Shell.Current.GoToAsync("..");
                    });
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"[RegisterViewModel] Error de registro: {ex.Message}");
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RegisterViewModel] Error: {ex.Message}");
                ErrorMessage = "Error durante el registro. Intenta nuevamente.";
            }
            finally
            {
                IsLoading = false;
                Console.WriteLine("[RegisterViewModel] === FIN REGISTRO ===");
            }
        }

        private bool ValidateForm()
        {
            // Validar que todos los campos estén completos
            if (SelectedTenant == null ||
                string.IsNullOrWhiteSpace(Nombre) ||
                string.IsNullOrWhiteSpace(Ci) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                IsRegisterEnabled = false;
                return false;
            }

            // Validar formato de CI (7-8 dígitos)
            if (!System.Text.RegularExpressions.Regex.IsMatch(Ci, @"^\d{7,8}$"))
            {
                ErrorMessage = "La Cédula de Identidad debe tener entre 7 y 8 dígitos";
                IsRegisterEnabled = false;
                return false;
            }

            // Validar formato de email básico
            if (!Email.Contains("@") || !Email.Contains("."))
            {
                ErrorMessage = "El formato del email no es válido";
                IsRegisterEnabled = false;
                return false;
            }

            // Validar longitud de contraseña
            if (Password.Length < 6)
            {
                ErrorMessage = "La contraseña debe tener al menos 6 caracteres";
                IsRegisterEnabled = false;
                return false;
            }

            // Validar que las contraseñas coincidan
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Las contraseñas no coinciden";
                IsRegisterEnabled = false;
                return false;
            }

            ErrorMessage = string.Empty;
            IsRegisterEnabled = !IsLoading;
            return true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}