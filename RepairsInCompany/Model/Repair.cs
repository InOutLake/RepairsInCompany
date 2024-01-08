using System;
using System.Collections.Generic;

namespace RepairsInCompany.Model;

public partial class Repair
{
    public DateTime StartDateTime { get; set; }

    public DateTime? EndDateTime { get; set; }

    public bool Planned { get; set; }

    public Guid EquipmentId { get; set; }

    public virtual Equipment Equipment { get; set; } = null!;
}
