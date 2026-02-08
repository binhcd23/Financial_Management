using Financial_Management_Server.Models;

namespace Financial_Management_Server.DTOs.Finances
{
    public class SavingDto
    {
        public int UserId { get; set; }
        public int? WalletId { get; set; }
        public string? WalletName { get; set; }
        public decimal? SavingRate { get; set; }
        public SavingDto() { }
        public SavingDto(Usertaxprofile usertaxprofile)
        {
            UserId = usertaxprofile.UserId;
            WalletId = usertaxprofile.WalletId;
            WalletName = usertaxprofile.Wallet?.WalletName;
            SavingRate = usertaxprofile.SavingRate;
        }

        public Usertaxprofile ToSavings()
        {
            return new Usertaxprofile
            {
                UserId = UserId,
                WalletId = WalletId,
                SavingRate = SavingRate
            };
        }
    }
}
