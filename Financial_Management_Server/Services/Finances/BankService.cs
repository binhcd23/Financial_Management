using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Interfaces.Finances;
using Microsoft.Extensions.Caching.Memory;

namespace Financial_Management_Server.Services.Finances
{
    public class BankService : IBankService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private const string BankCacheKey = "VietQR_Banks_Cache";

        public BankService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<List<BankListDto>> GetVietQRBanksAsync()
        {
            if (_cache.TryGetValue(BankCacheKey, out List<BankListDto>? cachedBanks))
            {
                return cachedBanks ?? new List<BankListDto>();
            }

            try
            {
                var response = await _httpClient.GetFromJsonAsync<BanksDto>("https://api.vietqr.io/v2/banks");

                if (response != null && response.Code == "00")
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromHours(24))
                        .SetAbsoluteExpiration(TimeSpan.FromHours(48));

                    _cache.Set(BankCacheKey, response.Data, cacheEntryOptions);

                    return response.Data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi gọi API VietQR: {ex.Message}");
            }

            return new List<BankListDto>();
        }
    }
}