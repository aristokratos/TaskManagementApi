namespace TaskManagementApi.Application.Interfaces
{
    using TaskManagementApi.Model;
    public interface IUserAuthServices
    {
        Task<bool> RegisterUserAsync(UserModel user);
        Task<string?> LoginUserAsync(string username, string password);
    }
}
