using WebApi.Models;

namespace WebApi.Interfaces
{
    public interface IUserRepository
    {
        Task<User> Authenticate(string userName, string password);
    }
}
