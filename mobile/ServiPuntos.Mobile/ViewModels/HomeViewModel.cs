
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace ServiPuntos.Mobile.ViewModels
{
    public class HomeViewModel : BindableObject
    {
        private readonly IUserService _userService;

        public HomeViewModel(IUserService userService)
        {
            _userService = userService;
            Transactions = new ObservableCollection<TransactionSummary>();
            LoadDataCommand = new Command(async () => await LoadDataAsync());

            LoadDataCommand.Execute(null);
        }

        private int _saldo;
        public int Saldo
        {
            get => _saldo;
            set { _saldo = value; OnPropertyChanged(); }
        }

        private int _puntosAcumuladosMes;
        public int PuntosAcumuladosMes
        {
            get => _puntosAcumuladosMes;
            set { _puntosAcumuladosMes = value; OnPropertyChanged(); }
        }

        public ObservableCollection<TransactionSummary> Transactions { get; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public ICommand LoadDataCommand { get; }

        public async Task LoadDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var balance = await _userService.GetBalanceAsync();
                if (balance != null)
                {
                    Saldo = balance.Saldo;
                    PuntosAcumuladosMes = balance.PuntosAcumuladosMes;
                }

                Transactions.Clear();
                var list = await _userService.GetRecentTransactionsAsync();
                if (list != null)
                {
                    foreach (var item in list)
                        Transactions.Add(item);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
