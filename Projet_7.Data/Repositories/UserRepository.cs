using Projet_7.Data.Data;
using Projet_7.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Projet_7.Data.Repositories
{
    public class UserRepository
    {
        private readonly LocalDbContext DbContext;

        public UserRepository(LocalDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<User?> FindByUserNameAsync(string userName)
        {
            return await DbContext.Users.FirstOrDefaultAsync(user => user.UserName == userName);
        }


        public async Task<List<User>> FindAllUsersAsync()
        {
            return await DbContext.Users.ToListAsync();
        }

        public async void AddUserAsync(User user)
        {
        }

        public async Task<User?> FindByIdAsync(int id)
        {
            var user = await DbContext.Users.FindAsync(id);
            return user;
        }
    }
}