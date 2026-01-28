using System;
using System.Collections.Generic;

namespace Financial_Management_Server.Models;

public partial class Savinggoal
{
    public int GoalId { get; set; }

    public int? UserId { get; set; }

    public string GoalName { get; set; } = null!;

    public decimal TargetAmount { get; set; }

    public decimal? CurrentAmount { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? TargetDate { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual User? User { get; set; }
}
