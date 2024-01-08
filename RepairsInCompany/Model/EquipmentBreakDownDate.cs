using System;
using System.Collections.Generic;

namespace RepairsInCompany.Model;

public partial class EquipmentBreakDownDate
{
    public string Name { get; set; } = null!;

    public DateTime StartDateTime { get; set; }

    public DateTime? EndDateTime { get; set; }
}
