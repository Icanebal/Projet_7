using AutoMapper;
using Projet_7.Core.Domain;
using Projet_7.Core.DTO;
using Projet_7.Core.Interfaces;
using Projet_7.Core.Utilities;

namespace Projet_7.Web.Services
{
    public class TradeService
    {
        private readonly ITradeRepository _tradeRepository;
        private readonly IMapper _mapper;

        public TradeService(ITradeRepository tradeRepository, IMapper mapper)
        {
            _tradeRepository = tradeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TradeDto>> GetAllAsync()
        {
            var entities = await _tradeRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<TradeDto>>(entities);
        }

        public async Task<TradeDto?> GetByIdAsync(int id)
        {
            var entity = await _tradeRepository.GetByIdAsync(id);
            if (entity == null)
                return null;
            return _mapper.Map<TradeDto>(entity);
        }

        public async Task<Result<TradeDto>> CreateAsync(TradeDto dto)
        {
            var entity = _mapper.Map<Trade>(dto);
            var created = await _tradeRepository.CreateAsync(entity);
            if (created == null || created.Id <= 0)
                return Result<TradeDto>.Failure("La création du trade n'a pas abouti.");
            return Result<TradeDto>.Success(_mapper.Map<TradeDto>(created));
        }

        public async Task<Result<TradeDto>> UpdateAsync(int id, TradeDto dto)
        {
            var existing = await _tradeRepository.GetByIdAsync(id);
            if (existing == null)
                return Result<TradeDto>.Failure("Le trade spécifié n'existe pas.");
            _mapper.Map(dto, existing);
            await _tradeRepository.UpdateAsync(existing);
            return Result<TradeDto>.Success(_mapper.Map<TradeDto>(existing));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _tradeRepository.GetByIdAsync(id);
            if (existing == null)
                return false;
            return await _tradeRepository.DeleteAsync(existing);
        }
    }
}
