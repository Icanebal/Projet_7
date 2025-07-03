using Microsoft.EntityFrameworkCore;
using Projet_7.Core.Domain;
using Projet_7.Core.Interfaces;
using Projet_7.Data.Data;

namespace Projet_7.Data.Repositories
{
    public class CurvePointRepository : ICurvePointRepository
    {
        private readonly LocalDbContext _databaseContext;

        public CurvePointRepository(LocalDbContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<IEnumerable<CurvePoint>> GetAllAsync()
        {
            return await _databaseContext.CurvePoints.ToListAsync();
        }

        public async Task<CurvePoint?> GetByIdAsync(int curvePointId)
        {
            return await _databaseContext.CurvePoints.FindAsync(curvePointId);
        }

        public async Task<CurvePoint?> CreateAsync(CurvePoint curvePoint)
        {
            _databaseContext.CurvePoints.Add(curvePoint);
            var result = await _databaseContext.SaveChangesAsync();
            return result > 0 ? curvePoint : null;
        }

        public async Task UpdateAsync(CurvePoint curvePoint)
        {
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(CurvePoint curvePoint)
        {
            _databaseContext.CurvePoints.Remove(curvePoint);
            await _databaseContext.SaveChangesAsync();
            return true;
        }
    }
}

