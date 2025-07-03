using Microsoft.EntityFrameworkCore;
using Projet_7.Core.Domain;
using Projet_7.Core.Interfaces;
using Projet_7.Data.Data;

namespace Projet_7.Data.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly LocalDbContext _databaseContext;

        public RatingRepository(LocalDbContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<IEnumerable<Rating>> GetAllAsync()
        {
            return await _databaseContext.Ratings.ToListAsync();
        }

        public async Task<Rating?> GetByIdAsync(int ratingId)
        {
            return await _databaseContext.Ratings.FindAsync(ratingId);
        }

        public async Task<Rating?> CreateAsync(Rating rating)
        {
            _databaseContext.Ratings.Add(rating);
            var newRating = await _databaseContext.SaveChangesAsync();
            return newRating > 0 ? rating : null;
        }
        public async Task UpdateAsync(Rating rating)
        {
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Rating rating)
        {
            _databaseContext.Ratings.Remove(rating);
            await _databaseContext.SaveChangesAsync();
            return true;
        }
    }
}
