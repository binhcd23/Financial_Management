using System;
using System.Collections.Generic;

namespace Financial_Management_Server.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string Type { get; set; } = null!;

    public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
