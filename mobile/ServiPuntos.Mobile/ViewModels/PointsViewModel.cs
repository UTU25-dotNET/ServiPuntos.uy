using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;

namespace ServiPuntos.Mobile.ViewModels
{
    public class PointsViewModel : BindableObject
    {
        private readonly IPointsService _pointsService;
        private bool _isBusy;
        private int _currentPoints;

        public PointsViewModel(IPointsService pointsService)
        {
            _pointsService = pointsService;
            RefreshCommand = new Command(async () => await LoadPointsAsync(), () => !IsBusy);
        }

        public ICommand RefreshCommand { get; }

        public int CurrentPoints
        {
            get => _currentPoints;
            set
            {
                if (_currentPoints == value) return;
                _currentPoints = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayPoints));
            }
        }

        public string DisplayPoints =>
            CurrentPoints > 0
                ? CurrentPoints.ToString()
                : "Aún no tienes puntos";

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy == value) return;
                _isBusy = value;
                OnPropertyChanged();
                ((Command)RefreshCommand).ChangeCanExecute();
            }
        }

        private async Task LoadPointsAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var dto = await _pointsService.GetCurrentPointsAsync();
                CurrentPoints = dto.Points;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar puntos: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
