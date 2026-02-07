using System;
using System.Collections.Generic;

namespace Financial_Management_Server.Models;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public int? UserId { get; set; }

    public int? CategoryId { get; set; }

    public decimal Amount { get; set; }

    public string? Note { get; set; }

    public DateOnly TransactionDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? WalletId { get; set; }

    public int? GoalId { get; set; }

    public bool IsDelete { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Savinggoal? Goal { get; set; }

    public virtual User? User { get; set; }

    public virtual Wallet? Wallet { get; set; }
}
