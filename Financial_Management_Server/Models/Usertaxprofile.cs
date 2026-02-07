using System;
using System.Collections.Generic;

namespace Financial_Management_Server.Models;

public partial class Usertaxprofile
{
    public int UserId { get; set; }

    public decimal? SavingRate { get; set; }

    public virtual User User { get; set; } = null!;
}
