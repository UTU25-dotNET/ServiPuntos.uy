using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;

namespace ServiPuntos.Mobile.ViewModels
{
    public class HistoryViewModel : BindableObject
    {
        private readonly IHistoryService _historyService;
        private bool _isBusy;
        private Guid? _cursor;
        private bool _hasMore;

        public ObservableCollection<TransaccionDto> Transactions { get; } = new ObservableCollection<TransaccionDto>();
        public ICommand LoadMoreCommand { get; }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy == value) return;
                _isBusy = value;
                OnPropertyChanged();
                ((Command)LoadMoreCommand).ChangeCanExecute();
            }
        }

        public bool HasAny => Transactions.Count > 0;
        public bool HasMore
        {
            get => _hasMore;
            set
            {
                if (_hasMore == value) return;
                _hasMore = value;
                OnPropertyChanged();
            }
        }

        public HistoryViewModel(IHistoryService historyService)
        {
            _historyService = historyService;
            LoadMoreCommand = new Command(async () => await LoadMoreAsync(), () => !IsBusy && HasMore);
            ResetHistory();
        }

        public void ResetHistory()
        {
            Transactions.Clear();
            _cursor = null;
            HasMore = true;
            OnPropertyChanged(nameof(HasAny));
            OnPropertyChanged(nameof(HasMore));
            ((Command)LoadMoreCommand).ChangeCanExecute();
        }

        private async Task LoadMoreAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            var resp = await _historyService.GetHistoryAsync(_cursor);
            foreach (var tx in resp.Items)
                Transactions.Add(tx);

            _cursor = resp.NextCursor;
            HasMore = resp.NextCursor != null;

            OnPropertyChanged(nameof(HasAny));
            OnPropertyChanged(nameof(HasMore));
            ((Command)LoadMoreCommand).ChangeCanExecute();
            IsBusy = false;
        }
    }
}
