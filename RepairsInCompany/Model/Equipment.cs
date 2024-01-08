using System;
using System.Collections.Generic;

namespace RepairsInCompany.Model;

public partial class Equipment
{
    public Guid EquipmentId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();

    public virtual ICollection<Repair> Repairs { get; set; } = new List<Repair>();
}
