using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class ItemMasterSchedulesDto
    {
        public int Id { get; set; }
        public DateTime SchedulesDate { get; set; }
        public string? ScheduleName { get; set; }
        public List<ItemMasterSchedulePartsDto>? ItemMasterScheduleParts { get; set; }
    }
    public class ItemMasterSchedulesDtoPost
    {
        public DateTime SchedulesDate { get; set; }
        public string? ScheduleName { get; set; }
        public List<ItemMasterSchedulePartsDtoPost>? ItemMasterScheduleParts { get; set; }
    }
    public class ItemMasterSchedulesDtoUpdate
    {
        public int Id { get; set; }
        public DateTime SchedulesDate { get; set; }
        public string? ScheduleName { get; set; }
        public List<ItemMasterSchedulePartsDtoUpdate>? ItemMasterScheduleParts { get; set; }
    }
}
