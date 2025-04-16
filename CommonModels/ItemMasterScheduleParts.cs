using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ItemMasterScheduleParts
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        public PartType ItemType { get; set; }
        public decimal Qty { get; set; }
        public int ItemMasterSchedulesId { get; set; }
        public ItemMasterSchedules? ItemMasterSchedules { get; set; }

    }
}

