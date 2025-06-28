namespace ServiPuntos.Core.Interfaces
{
    public interface IFcmService
    {
        Task SendAsync(string token, string title, string body);
    }
}