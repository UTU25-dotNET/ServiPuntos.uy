using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;

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

                Saldo = await _userService.GetBalanceAsync();


                Transactions.Clear();
                var list = await _userService.GetRecentTransactionsAsync()
                           ?? new List<TransactionSummary>();
                foreach (var item in list)
                    Transactions.Add(item);


                var hoy = DateTime.Today;
                PuntosAcumuladosMes = list
                    .Where(t => t.Fecha.Month == hoy.Month && t.Fecha.Year == hoy.Year)
                    .Sum(t => t.Puntos);
            }
            finally
            {
                IsBusy = false;
            }
        }

    }
}
