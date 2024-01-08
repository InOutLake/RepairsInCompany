namespace RepairsInCompany.Model.ViewModel
{
    public class EquipmentListVM
    {
        public Guid EquipmentId { get; set; }
        public string Name { get; set; }
        public DateTime? StartDateTime { get; set; }
        public bool WasBroken { get; set; }
        public bool IsInRepair { get; set; }

    }
}
