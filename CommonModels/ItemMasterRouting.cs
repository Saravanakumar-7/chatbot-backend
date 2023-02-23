using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
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

        public long ItemMasterId { get; set; }
        public ItemMaster? ItemMaster { get; set; }

    }
}
