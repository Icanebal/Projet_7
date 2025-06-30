using Projet_7.Core.Domain;

namespace Projet_7.Core.Interfaces
{
    public interface IBidRepository
    {
        Task<IEnumerable<Bid>> GetAllAsync();
        Task<Bid?> GetByIdAsync(int bidId);
        Task<Bid?> CreateAsync(Bid bid);
        Task UpdateAsync(Bid bid);
        Task<bool> DeleteAsync(Bid bid);
    }
}

