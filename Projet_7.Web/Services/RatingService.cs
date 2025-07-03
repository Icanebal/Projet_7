using AutoMapper;
using Projet_7.Core.Domain;
using Projet_7.Core.DTO;
using Projet_7.Core.Interfaces;
using Projet_7.Core.Utilities;

namespace Projet_7.Web.Services
{
    public class RatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IMapper _mapper;

        public RatingService(IRatingRepository ratingRepository, IMapper mapper)
        {
            _ratingRepository = ratingRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RatingDto>> GetAllAsync()
        {
            var entities = await _ratingRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<RatingDto>>(entities);
        }

        public async Task<RatingDto?> GetByIdAsync(int id)
        {
            var entity = await _ratingRepository.GetByIdAsync(id);
            if (entity == null)
                return null;
            return _mapper.Map<RatingDto>(entity);
        }

        public async Task<Result<RatingDto>> CreateAsync(RatingDto dto)
        {
            var entity = _mapper.Map<Rating>(dto);
            var created = await _ratingRepository.CreateAsync(entity);
            if (created == null || created.Id <= 0)
                return Result<RatingDto>.Failure("La création de la note n'a pas abouti.");
            return Result<RatingDto>.Success(_mapper.Map<RatingDto>(created));
        }

        public async Task<Result<RatingDto>> UpdateAsync(int id, RatingDto dto)
        {
            var existing = await _ratingRepository.GetByIdAsync(id);
            if (existing == null)
                return Result<RatingDto>.Failure("La note spécifiée n'existe pas.");
            _mapper.Map(dto, existing);
            await _ratingRepository.UpdateAsync(existing);
            return Result<RatingDto>.Success(_mapper.Map<RatingDto>(existing));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _ratingRepository.GetByIdAsync(id);
            if (existing == null)
                return false;
            return await _ratingRepository.DeleteAsync(existing);
        }
    }
}
