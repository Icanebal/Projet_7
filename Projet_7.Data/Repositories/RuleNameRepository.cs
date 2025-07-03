using Microsoft.EntityFrameworkCore;
using Projet_7.Core.Domain;
using Projet_7.Core.Interfaces;
using Projet_7.Data.Data;

namespace Projet_7.Data.Repositories
{
    public class RuleNameRepository : IRuleNameRepository
    {
        private readonly LocalDbContext _databaseContext;

        public RuleNameRepository(LocalDbContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<IEnumerable<RuleName>> GetAllAsync()
        {
            return await _databaseContext.RuleNames.ToListAsync();
        }

        public async Task<RuleName?> GetByIdAsync(int ruleNameId)
        {
            return await _databaseContext.RuleNames.FindAsync(ruleNameId);
        }

        public async Task<RuleName?> CreateAsync(RuleName ruleName)
        {
            _databaseContext.RuleNames.Add(ruleName);
            var newRuleName = await _databaseContext.SaveChangesAsync();
            return newRuleName > 0 ? ruleName : null;
        }
        public async Task UpdateAsync(RuleName ruleName)
        {
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(RuleName ruleName)
        {
            _databaseContext.RuleNames.Remove(ruleName);
            await _databaseContext.SaveChangesAsync();
            return true;
        }
    }
}
