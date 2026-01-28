using System;
using System.Collections.Generic;

namespace Financial_Management_Server.Models;

public partial class Taxbracket
{
    public int BracketId { get; set; }

    public decimal ThresholdFrom { get; set; }

    public decimal? ThresholdTo { get; set; }

    public decimal TaxRate { get; set; }

    public decimal? DeductionAmount { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
