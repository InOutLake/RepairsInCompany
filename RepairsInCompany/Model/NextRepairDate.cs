using System;
using System.Collections.Generic;

namespace RepairsInCompany.Model;

public partial class NextRepairDate
{
    public string Name { get; set; } = null!;

    public DateTime? NextRepairDate1 { get; set; }
}
