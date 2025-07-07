using AutoMapper;
using Projet_7.Core.Domain;
using Projet_7.Core.DTO;
using Projet_7.Core.Interfaces;
using Projet_7.Core.Utilities;

namespace Projet_7.Web.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return null;
            return _mapper.Map<UserDto>(user);
        }

        public async Task<Result<UserDto>> CreateAsync(UserDto dto)
        {
            var entity = _mapper.Map<User>(dto);
            var created = await _userRepository.CreateAsync(entity);
            if (created == null || created.Id <= 0)
                return Result<UserDto>.Failure("La création de l'utilisateur n'a pas abouti.");
            return Result<UserDto>.Success(_mapper.Map<UserDto>(created));
        }

        public async Task<Result<UserDto>> UpdateAsync(int id, UserDto dto)
        {
            var existing = await _userRepository.GetByIdAsync(id);
            if (existing == null)
                return Result<UserDto>.Failure("L'utilisateur spécifié n'existe pas.");
            _mapper.Map(dto, existing);
            await _userRepository.UpdateAsync(existing);
            return Result<UserDto>.Success(_mapper.Map<UserDto>(existing));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _userRepository.GetByIdAsync(id);
            if (existing == null)
                return false;
            return await _userRepository.DeleteAsync(existing);
        }
    }
}
