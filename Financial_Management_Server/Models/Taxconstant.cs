using System;
using System.Collections.Generic;

namespace Financial_Management_Server.Models;

public partial class Taxconstant
{
    public string ConstantKey { get; set; } = null!;

    public decimal ConstantValue { get; set; }

    public string? Description { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
