using AutoMapper;
using Projet_7.Core.Domain;
using Projet_7.Core.DTO;
using Projet_7.Core.Interfaces;
using Projet_7.Core.Utilities;

namespace Projet_7.Web.Services
{
    public class CurvePointService
    {
        private readonly ICurvePointRepository _curvePointRepository;
        private readonly IMapper _mapper;

        public CurvePointService(ICurvePointRepository curvePointRepository, IMapper mapper)
        {
            _curvePointRepository = curvePointRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CurvePointDto>> GetAllAsync()
        {
            var entities = await _curvePointRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CurvePointDto>>(entities);
        }

        public async Task<CurvePointDto?> GetByIdAsync(int id)
        {
            var entity = await _curvePointRepository.GetByIdAsync(id);
            if (entity == null)
                return null;

            return _mapper.Map<CurvePointDto>(entity);
        }

        public async Task<Result<CurvePointDto>> CreateAsync(CurvePointDto dto)
        {
            var entity = _mapper.Map<CurvePoint>(dto);

            var created = await _curvePointRepository.CreateAsync(entity);
            if (created == null || created.Id <= 0)
                return Result<CurvePointDto>.Failure("La création du point n'a pas abouti.");

            return Result<CurvePointDto>.Success(_mapper.Map<CurvePointDto>(created));
        }

        public async Task<Result<CurvePointDto>> UpdateAsync(int id, CurvePointDto dto)
        {
            var existing = await _curvePointRepository.GetByIdAsync(id);
            if (existing == null)
                return Result<CurvePointDto>.Failure("Le point spécifié n'existe pas.");

            _mapper.Map(dto, existing);

            await _curvePointRepository.UpdateAsync(existing);

            return Result<CurvePointDto>.Success(_mapper.Map<CurvePointDto>(existing));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _curvePointRepository.GetByIdAsync(id);
            if (existing == null)
                return false;

            return await _curvePointRepository.DeleteAsync(existing);
        }
    }
}

