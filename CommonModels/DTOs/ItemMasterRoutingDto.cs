using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class ItemMasterRoutingDto
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
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class ItemMasterRoutingDtoPost
    {
        public string? ProcessStep { get; set; }
        public string? Process { get; set; }
        public string? RoutingDescription { get; set; }
        public string? MachineHours { get; set; }
        public string? LaborHours { get; set; }
        [DefaultValue(false)]
        public bool IsRoutingActive { get; set; }

    }

    public class ItemMasterRoutingDtoUpdate
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

    }

}
