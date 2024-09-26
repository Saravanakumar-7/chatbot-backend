using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class ConvertionrateDto
    {
        public int Id { get; set; }
        public decimal ConvertionRate { get; set; }
        public string UOC { get; set; }
        public DateTime Date { get; set; }
        public bool ActiveStatus { get; set; } = true;
        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class ConvertionratePostDto
    {
        public decimal ConvertionRate { get; set; }
        public string UOC { get; set; }
        public DateTime Date { get; set; }
        public bool ActiveStatus { get; set; } = true;
    }
    public class ConvertionrateUpdateDto
    {
        public int Id { get; set; }
        public decimal ConvertionRate { get; set; }
        public string UOC { get; set; }
        public DateTime Date { get; set; }
        public bool ActiveStatus { get; set; } = true;
        public string Unit { get; set; }
    }
}
