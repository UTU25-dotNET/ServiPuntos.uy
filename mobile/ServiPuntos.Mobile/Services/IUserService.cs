namespace ServiPuntos.Mobile.Services
{
    public interface IUserService
    {
        Task UpdateFcmTokenAsync(string token);
    }
}