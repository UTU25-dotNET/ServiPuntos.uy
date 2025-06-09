namespace ServiPuntos.Mobile.Services
{
    public interface ICanjeService
    {
        Task<CanjeResponse> GenerarCanjeAsync(CanjeRequest body);
        Task<IEnumerable<CanjeHistorialItem>> GetHistorialAsync(string userId);
    }

    public class CanjeService : ICanjeService
    {
        private readonly HttpClient _httpClient;

        public CanjeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CanjeResponse> GenerarCanjeAsync(CanjeRequest body)
        {


            var response = await _httpClient.PostAsJsonAsync("generar-canje", body);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CanjeResponse>();
        }

        public async Task<IEnumerable<CanjeHistorialItem>> GetHistorialAsync(string userId)
        {

            var response = await _httpClient.GetAsync($"historial/{userId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<CanjeHistorialItem>>();
        }
    }
}
