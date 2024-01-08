using System;
using System.Collections.Generic;

namespace RepairsInCompany.Model;

public partial class Registration
{
    public DateTime StartDateTime { get; set; }

    public int PeriodInDays { get; set; }

    public DateTime? EndDate { get; set; }

    public Guid EquipmentId { get; set; }

    public virtual Equipment Equipment { get; set; } = null!;
}
