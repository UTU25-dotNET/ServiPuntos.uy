namespace ServiPuntos.Mobile.Services
{
    public interface IAuthService
    {
        Task<bool> LoginWithGoogleAsync();
        Task<SignInResponse?> SignInAsync(string email, string password);
        Task<bool> IsAuthenticatedAsync();
        Task<string?> GetTokenAsync();
        Task LogoutAsync();
        Task<UserInfo?> GetUserInfoAsync();
        Task SaveTokenAsync(string token);
    }
    public class SignInRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class SignInResponse
    {
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public bool IsMobile { get; set; }
    }

    public class UserInfo
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? TenantId { get; set; }
        public string? Role { get; set; }
    }
}