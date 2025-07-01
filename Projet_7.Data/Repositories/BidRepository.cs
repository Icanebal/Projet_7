using Microsoft.EntityFrameworkCore;
using Projet_7.Core.Domain;
using Projet_7.Core.Interfaces;
using Projet_7.Data.Data;

namespace Projet_7.Data.Repositories
{
    public class BidRepository : IBidRepository
    {
        private readonly LocalDbContext _databaseContext;

        public BidRepository(LocalDbContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<IEnumerable<Bid>> GetAllAsync()
        {
            return await _databaseContext.Bids.ToListAsync();
        }

        public async Task<Bid?> GetByIdAsync(int bidId)
        {
            return await _databaseContext.Bids.FindAsync(bidId);
        }

        public async Task<Bid?> CreateAsync(Bid bid)
        {
            _databaseContext.Bids.Add(bid);
            var newBid = await _databaseContext.SaveChangesAsync();
            return newBid > 0 ? bid : null;
        }
        public async Task UpdateAsync(Bid bid)
        {
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Bid bid)
        {
            _databaseContext.Bids.Remove(bid);
            await _databaseContext.SaveChangesAsync();
            return true;
        }
    }
}

