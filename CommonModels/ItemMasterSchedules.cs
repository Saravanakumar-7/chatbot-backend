using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ItemMasterSchedules
    {
        public int Id { get; set; }
        public DateTime SchedulesDate { get; set; }
        public List<ItemMasterScheduleParts>? ItemMasterScheduleParts { get; set; }
        public int ItemMasterId { get; set; }
        public ItemMaster? ItemMaster { get; set; }
    }
}
