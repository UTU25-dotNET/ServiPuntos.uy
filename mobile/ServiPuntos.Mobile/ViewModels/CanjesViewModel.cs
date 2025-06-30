using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
#if ANDROID
using Android.Util;
#endif
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;

namespace ServiPuntos.Mobile.ViewModels
{
    public class CanjesViewModel : BindableObject
    {
        readonly ICanjeService _canjeService;
        Guid? _cursor;
        bool _isBusy;

#if ANDROID
        const string TAG = "CanjesViewModel";
#endif

        public ObservableCollection<CanjeSummaryDto> Canjes { get; } = new();
        public ICommand RefreshCommand { get; }
        public ICommand LoadMoreCommand { get; }
        public ICommand SelectCommand { get; }

        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public CanjesViewModel(ICanjeService canjeService)
        {
            _canjeService = canjeService;
            RefreshCommand = new Command(async () => await LoadAsync(true), () => !IsBusy);
            LoadMoreCommand = new Command(async () => await LoadAsync(false), () => !IsBusy && _cursor.HasValue);
            SelectCommand = new Command<CanjeSummaryDto>(async c =>
            {
#if ANDROID
                Log.Debug(TAG, $"Select {c.Id}");
#endif
                var code = Uri.EscapeDataString(c.CodigoQR);
                await Shell.Current.GoToAsync($"QRCodePage?code={code}");
            });
        }

        async Task LoadAsync(bool clear)
        {
            if (IsBusy) return;

#if ANDROID
            Log.Debug(TAG, $"LoadAsync clear={clear}, cursor={_cursor}");
#endif
            try
            {
                IsBusy = true;
                if (clear)
                {
                    Canjes.Clear();
                    _cursor = null;
                }

                var userId = Guid.Parse(await SecureStorage.GetAsync("userId"));
                var resp = await _canjeService.GetCanjesByUsuarioAsync(userId, _cursor, 20);

                foreach (var c in resp.Items)
                    Canjes.Add(c);

                _cursor = resp.NextCursor;
#if ANDROID
                Log.Debug(TAG, $"Loaded {resp.Items.Count} items; nextCursor={_cursor}");
#endif
            }
            catch (Exception ex)
            {
#if ANDROID
                Log.Error(TAG, ex.ToString());
#endif
                throw;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
