using Dot.Net.WebApi.Data;
using Dot.Net.WebApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace Dot.Net.WebApi.Repositories
{
    public class UserRepository
    {
        public LocalDbContext DbContext { get; }

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