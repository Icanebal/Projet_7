using AutoMapper;
using Projet_7.Core.Domain;
using Projet_7.Core.DTO;
using Projet_7.Core.Interfaces;
using Projet_7.Core.Utilities;

namespace Projet_7.Web.Services
{
    public class RuleNameService
    {
        private readonly IRuleNameRepository _ruleNameRepository;
        private readonly IMapper _mapper;

        public RuleNameService(IRuleNameRepository ruleNameRepository, IMapper mapper)
        {
            _ruleNameRepository = ruleNameRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RuleNameDto>> GetAllAsync()
        {
            var entities = await _ruleNameRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<RuleNameDto>>(entities);
        }

        public async Task<RuleNameDto?> GetByIdAsync(int id)
        {
            var entity = await _ruleNameRepository.GetByIdAsync(id);
            if (entity == null)
                return null;
            return _mapper.Map<RuleNameDto>(entity);
        }

        public async Task<Result<RuleNameDto>> CreateAsync(RuleNameDto dto)
        {
            var entity = _mapper.Map<RuleName>(dto);
            var created = await _ruleNameRepository.CreateAsync(entity);
            if (created == null || created.Id <= 0)
                return Result<RuleNameDto>.Failure("La création de la règle n'a pas abouti.");
            return Result<RuleNameDto>.Success(_mapper.Map<RuleNameDto>(created));
        }

        public async Task<Result<RuleNameDto>> UpdateAsync(int id, RuleNameDto dto)
        {
            var existing = await _ruleNameRepository.GetByIdAsync(id);
            if (existing == null)
                return Result<RuleNameDto>.Failure("La règle spécifiée n'existe pas.");
            _mapper.Map(dto, existing);
            await _ruleNameRepository.UpdateAsync(existing);
            return Result<RuleNameDto>.Success(_mapper.Map<RuleNameDto>(existing));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _ruleNameRepository.GetByIdAsync(id);
            if (existing == null)
                return false;
            return await _ruleNameRepository.DeleteAsync(existing);
        }
    }
}
