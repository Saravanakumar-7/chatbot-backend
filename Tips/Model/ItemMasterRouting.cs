using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Tips.Model
{
    public class ItemMasterRouting
    {
        [Key]
        public int Id { get; set; }
        public string? ProcessStep { get; set; }
        public string? Process { get; set; }
        public string? RoutingDescription { get; set; }
        public string? MachineHours { get; set; }
        public string? LaborHours { get; set; }
        [DefaultValue(false)]
        public bool IsRoutingActive { get; set; }

        [ForeignKey(nameof(ItemMaster))]
        public long ItemMasterId { get; set; }
        public ItemMaster? ItemMaster { get; set; }


    }
}
}
