using Projet_7.Core.Domain;

namespace Projet_7.Core.Interfaces
{
    public interface ITradeRepository
    {
        Task<IEnumerable<Trade>> GetAllAsync();
        Task<Trade?> GetByIdAsync(int tradeId);
        Task<Trade?> CreateAsync(Trade trade);
        Task UpdateAsync(Trade trade);
        Task<bool> DeleteAsync(Trade trade);
    }
}
