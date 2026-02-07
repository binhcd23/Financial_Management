
using Financial_Management_Server.Models;

namespace Financial_Management_Server.DTOs.Finances
{
    public class TransactionDto
    {
        public int TransactionId { get; set; }

        public int? UserId { get; set; }

        public int? CategoryId { get; set; }

        public string? CategoryName { get; set; }    

        public decimal Amount { get; set; }

        public string? Note { get; set; }

        public DateOnly TransactionDate { get; set; }

        public DateTime? CreatedAt { get; set; }

        public bool IsDelete { get; set; }

        public TransactionDto() { }
        public TransactionDto(Transaction transaction)
        {
            TransactionId = transaction.TransactionId;
            UserId = transaction.UserId;
            CategoryId = transaction.CategoryId;
            CategoryName = transaction.Category?.CategoryName ?? "Không xác định";
            Amount = transaction.Amount;
            Note = transaction.Note;
            TransactionDate = transaction.TransactionDate;
            CreatedAt = transaction.CreatedAt;
            IsDelete = transaction.IsDelete;
        }
        public Transaction ToTransaction()
        {
            return new Transaction
            {
                UserId = UserId,
                CategoryId = CategoryId,
                Amount = Amount,
                Note = Note,
                TransactionDate = TransactionDate == default ? DateOnly.FromDateTime(DateTime.Now) : TransactionDate,
                CreatedAt = DateTime.Now,
                IsDelete = false,
            };
        }
    }
    public class TransactionRequest()
    {
        public int? userId { get; set; }
        public int? categoryId { get; set; }
        public string? timeRange { get; set; }
        public string? search { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
    }
    public class WalletDto
    {
        public int WalletId { get; set; }

        public int? UserId { get; set; }

        public string WalletName { get; set; } = null!;

        public decimal? Balance { get; set; }

        public string? WalletType { get; set; }

        public string? CardNumber { get; set; }

        public string? CardHolderName { get; set; }

        public int? BankId { get; set; }

        public string? BankName { get; set; }

        public string? BankLogo { get; set; }

        public bool IsDefault { get; set; } = false;

        public bool IsDelete { get; set; }

        public WalletDto() { }
        public WalletDto(Wallet wallet)
        {
            WalletId = wallet.WalletId;
            UserId = wallet.UserId;
            WalletName = wallet.WalletName;
            Balance = wallet.Balance;
            WalletType = wallet.WalletType;
            CardNumber = wallet.CardNumber;
            CardHolderName = wallet.CardHolderName;
            BankId = wallet.BankId;
            IsDefault = wallet.IsDefault;
            IsDelete = wallet.IsDelete;
        }
        public Wallet ToWallet()
        {
            return new Wallet
            {
                UserId = UserId,
                WalletName = WalletName,
                Balance = Balance,
                WalletType = WalletType,
                CardNumber = CardNumber,
                CardHolderName = CardHolderName,
                BankId = BankId,
                IsDefault = IsDefault,
                IsDelete = false,
            };
        }
    }

    public class WalletResponses
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
    public class WalletSummaryDto
    {
        public decimal? TotalBalance { get; set; }
        public int? WalletCount { get; set; }
        public string? WalletType { get; set; }
    }
    public class CategoriesDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string Type { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }
        public CategoriesDto() { }
        public CategoriesDto(Category category)
        {
            CategoryId = category.CategoryId;
            CategoryName = category.CategoryName;
            Type = category.Type;
        }
    }

    public class BillingDto
    {
        public List<WalletDto> Wallets { get; set; } = new List<WalletDto>();
        public List<WalletSummaryDto> WalletSummaries { get; set; } = new List<WalletSummaryDto> { };
        public WalletDto? WalletDefault { get; set; }
        public PagedResult<TransactionDto> Transactions { get; set; } = new PagedResult<TransactionDto>();
        public List<CategoriesDto> Categories { get; set; } = new List<CategoriesDto>();
        public List<BankListDto> Banks { get; set; } = new List<BankListDto>();
    }
}
