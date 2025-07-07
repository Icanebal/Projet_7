using Microsoft.EntityFrameworkCore;
using Projet_7.Core.Domain;
using Projet_7.Core.Interfaces;
using Projet_7.Data.Data;

namespace Projet_7.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly LocalDbContext _databaseContext;

        public UserRepository(LocalDbContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _databaseContext.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            return await _databaseContext.Users.FindAsync(userId);
        }

        public async Task<User?> CreateAsync(User user)
        {
            _databaseContext.Users.Add(user);
            var newUser = await _databaseContext.SaveChangesAsync();
            return newUser > 0 ? user : null;
        }

        public async Task UpdateAsync(User user)
        {
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(User user)
        {
            _databaseContext.Users.Remove(user);
            await _databaseContext.SaveChangesAsync();
            return true;
        }

        public async Task<User?> FindByUserNameAsync(string userName)
        {
            return await _databaseContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        }
    }
}