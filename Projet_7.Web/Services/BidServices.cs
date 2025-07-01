using AutoMapper;
using Projet_7.Core.Domain;
using Projet_7.Core.DTO;
using Projet_7.Core.Interfaces;
using Projet_7.Core.Utils;

namespace Projet_7.Web.Services
{
    public class BidService
    {
        private readonly IBidRepository _bidRepository;
        private readonly IMapper _mapper;

        public BidService(IBidRepository bidRepository, IMapper mapper)
        {
            _bidRepository = bidRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BidDto>> GetAllAsync()
        {
            var bidEntities = await _bidRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<BidDto>>(bidEntities);
        }

        public async Task<BidDto?> GetByIdAsync(int bidId)
        {
            var bidEntity = await _bidRepository.GetByIdAsync(bidId);
            if (bidEntity == null)
                return null;

            return _mapper.Map<BidDto>(bidEntity);
        }

        public async Task<Result<BidDto>> CreateAsync(BidDto bidDto)
        {
            if (bidDto.BidQuantity < 0)
                return Result<BidDto>.Failure("La quantité ne peut pas être négative.");

            var bidEntity = _mapper.Map<Bid>(bidDto);
            bidEntity.CreationName = "CurrentUser";

            var createdEntity = await _bidRepository.CreateAsync(bidEntity);
            if (createdEntity == null || createdEntity.Id <= 0)
                return Result<BidDto>.Failure("La création de l'offre n'a pas abouti.");

            var createdDto = _mapper.Map<BidDto>(createdEntity);
            return Result<BidDto>.Success(createdDto);
        }

        public async Task<Result<BidDto>> UpdateAsync(int bidId, BidDto bidDto)
        {
            var existingBid = await _bidRepository.GetByIdAsync(bidId);
            if (existingBid == null)
                return Result<BidDto>.Failure("L'offre spécifiée n'existe pas.");

            _mapper.Map(bidDto, existingBid);
            existingBid.RevisionDate = DateTime.UtcNow;
            existingBid.RevisionName = "CurrentUser";

            await _bidRepository.UpdateAsync(existingBid);

            var updatedBidDto = _mapper.Map<BidDto>(existingBid);
            return Result<BidDto>.Success(updatedBidDto);
        }

		public async Task<bool> DeleteAsync(int bidId)
		{
			var existingBid = await _bidRepository.GetByIdAsync(bidId);
			if (existingBid == null)
				return false;

			return await _bidRepository.DeleteAsync(existingBid);
		}

	}
}

