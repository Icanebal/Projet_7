using Microsoft.EntityFrameworkCore;
using Projet_7.Core.Domain;
using Projet_7.Core.Interfaces;
using Projet_7.Data.Data;

namespace Projet_7.Data.Repositories
{
    public class TradeRepository : ITradeRepository
    {
        private readonly LocalDbContext _databaseContext;

        public TradeRepository(LocalDbContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<IEnumerable<Trade>> GetAllAsync()
        {
            return await _databaseContext.Trades.ToListAsync();
        }

        public async Task<Trade?> GetByIdAsync(int tradeId)
        {
            return await _databaseContext.Trades.FindAsync(tradeId);
        }

        public async Task<Trade?> CreateAsync(Trade trade)
        {
            _databaseContext.Trades.Add(trade);
            var newTrade = await _databaseContext.SaveChangesAsync();
            return newTrade > 0 ? trade : null;
        }
        public async Task UpdateAsync(Trade trade)
        {
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Trade trade)
        {
            _databaseContext.Trades.Remove(trade);
            await _databaseContext.SaveChangesAsync();
            return true;
        }
    }
}
