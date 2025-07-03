using Projet_7.Core.Domain;

namespace Projet_7.Core.Interfaces
{
    public interface IRatingRepository
    {
        Task<IEnumerable<Rating>> GetAllAsync();
        Task<Rating?> GetByIdAsync(int ratingId);
        Task<Rating?> CreateAsync(Rating rating);
        Task UpdateAsync(Rating rating);
        Task<bool> DeleteAsync(Rating rating);
    }
}
