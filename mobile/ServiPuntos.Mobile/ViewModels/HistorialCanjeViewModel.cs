using System.Collections.ObjectModel;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;
using System.Windows.Input;
using Microsoft.Maui.Controls;

public class HistorialCanjeViewModel : BindableObject
{
    private readonly CanjeService _canjeService;
    private readonly string _usuarioId;

    public ObservableCollection<CanjeHistorialItem> Historial { get; } = new();

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set { _isBusy = value; OnPropertyChanged(); }
    }

    public ICommand LoadCommand { get; }

    public HistorialCanjeViewModel(CanjeService canjeService, string usuarioId)
    {
        _canjeService = canjeService;
        _usuarioId = usuarioId;
        LoadCommand = new Command(async () => await LoadHistorialAsync());
        LoadCommand.Execute(null);
    }

    public async Task LoadHistorialAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            Historial.Clear();
            var items = await _canjeService.GetHistorialUsuarioAsync(_usuarioId);
            foreach (var item in items)
                Historial.Add(item);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
