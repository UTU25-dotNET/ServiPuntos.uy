namespace ServiPuntos.Mobile.Services
{
    public interface IAuthService
    {
        Task<bool> LoginWithGoogleAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<string?> GetTokenAsync();
        Task LogoutAsync();
        Task<UserInfo?> GetUserInfoAsync();
    }

    public class UserInfo
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? TenantId { get; set; }
        public string? Role { get; set; }
    }
}