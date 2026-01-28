using System;
using System.Collections.Generic;

namespace Financial_Management_Server.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Fullname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Savinggoal> Savinggoals { get; set; } = new List<Savinggoal>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual Usertaxprofile? Usertaxprofile { get; set; }

    public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
}
