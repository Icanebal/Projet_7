using Projet_7.Core.Domain;

namespace Projet_7.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int userId);
        Task<User?> CreateAsync(User user);
        Task UpdateAsync(User user);
        Task<bool> DeleteAsync(User user);
        Task<User?> FindByUserNameAsync(string userName);
    }
}
