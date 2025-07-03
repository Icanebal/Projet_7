using Projet_7.Core.Domain;

namespace Projet_7.Core.Interfaces
{
    public interface ICurvePointRepository
    {
        Task<IEnumerable<CurvePoint>> GetAllAsync();
        Task<CurvePoint?> GetByIdAsync(int curvePointId);
        Task<CurvePoint?> CreateAsync(CurvePoint curvePoint);
        Task UpdateAsync(CurvePoint curvePoint);
        Task<bool> DeleteAsync(CurvePoint curvePoint);
    }
}
