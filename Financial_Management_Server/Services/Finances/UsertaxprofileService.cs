using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Interfaces.Finances;
using Financial_Management_Server.Models;

namespace Financial_Management_Server.Services.Finances
{
    public class UsertaxprofileService : IUsertaxprofileService
    {
        private readonly IUsertaxprofileRepository _usertaxprofileRepository;
        private readonly IWalletRepository _walletRepository;

        public UsertaxprofileService(IUsertaxprofileRepository usertaxprofileRepository, IWalletRepository walletRepository)
        {
            _usertaxprofileRepository = usertaxprofileRepository;
            _walletRepository = walletRepository;
        }

        public async Task<bool> AddAsync(SavingDto dto)
        {
            var existing = await _usertaxprofileRepository.GetUsertaxprofileByUserId(dto.UserId);
            if (existing != null) return false;

            var profile = dto.ToSavings();

            return await _usertaxprofileRepository.AddAsync(profile);
        }

        public async Task<SavingDto?> GetUsertaxprofileAsync(int userId)
        {
            var profile = await _usertaxprofileRepository.GetUsertaxprofileByUserId(userId);
            if (profile == null) return null;
            return new SavingDto(profile);
        }

        public async Task<bool> UpdateAsync(SavingDto dto)
        {
            var profile = await _usertaxprofileRepository.GetUsertaxprofileByUserId(dto.UserId);
            if (profile == null) return false;
            if(profile.SavingRate == dto.SavingRate && 
                profile.WalletId == dto.WalletId) 
                return false;
            profile.WalletId = dto.WalletId;
            profile.SavingRate = dto.SavingRate;

            return await _usertaxprofileRepository.UpdateAsync(profile);
        }
    }
}