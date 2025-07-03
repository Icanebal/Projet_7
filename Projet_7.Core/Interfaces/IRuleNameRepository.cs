using Projet_7.Core.Domain;

namespace Projet_7.Core.Interfaces
{
    public interface IRuleNameRepository
    {
        Task<IEnumerable<RuleName>> GetAllAsync();
        Task<RuleName?> GetByIdAsync(int ruleNameId);
        Task<RuleName?> CreateAsync(RuleName ruleName);
        Task UpdateAsync(RuleName ruleName);
        Task<bool> DeleteAsync(RuleName ruleName);
    }
}
