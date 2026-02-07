using Financial_Management_Server.DTOs.Finances;

namespace Financial_Management_Server.Interfaces.Finances
{
    public interface IBankService
    {
        Task<List<BankListDto>> GetVietQRBanksAsync();
    }
}
