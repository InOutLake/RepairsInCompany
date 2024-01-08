using System;
using System.Collections.Generic;

namespace RepairsInCompany.Model ;

public partial class RepairFrequency
{
    public string Name { get; set; } = null!;

    public int? PeriodInDays { get; set; }
}
