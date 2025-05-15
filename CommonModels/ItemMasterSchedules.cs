using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ItemMasterSchedules
    {
        [Key]
        public int Id { get; set; }
        public DateTime SchedulesDate { get; set; }
        public string? ScheduleName {  get; set; }
        public List<ItemMasterScheduleParts>? ItemMasterScheduleParts { get; set; }
        public long ItemMasterId { get; set; }
        public ItemMaster? ItemMaster { get; set; }
    }
}
