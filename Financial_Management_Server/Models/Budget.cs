using System;
using System.Collections.Generic;

namespace Financial_Management_Server.Models;

public partial class Budget
{
    public int BudgetId { get; set; }

    public int? UserId { get; set; }

    public int? CategoryId { get; set; }

    public decimal AmountLimit { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public virtual Category? Category { get; set; }

    public virtual User? User { get; set; }
}
