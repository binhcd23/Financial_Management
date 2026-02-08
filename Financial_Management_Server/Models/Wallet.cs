using System;
using System.Collections.Generic;

namespace Financial_Management_Server.Models;

public partial class Wallet
{
    public int WalletId { get; set; }

    public int? UserId { get; set; }

    public string WalletName { get; set; } = null!;

    public decimal? Balance { get; set; }

    public string? WalletType { get; set; }

    public string? CardNumber { get; set; }

    public string? CardHolderName { get; set; }

    public int? BankId { get; set; }

    public bool IsDefault { get; set; }

    public bool IsDelete { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual User? User { get; set; }

    public virtual Usertaxprofile? Usertaxprofile { get; set; }
}
