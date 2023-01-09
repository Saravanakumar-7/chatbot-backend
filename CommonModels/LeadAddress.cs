using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class LeadAddress
    {
        public int Id { get; set; }

        public string? ProjectName { get; set; }

        public string? Addresses { get; set; }

        public string? Villa_House_Flat { get; set; }

        public string? Street { get; set; }

        public string? Area { get; set; }

        public string? Zone { get; set; }

        public string? LandMark { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Country { get; set; }
        public string? ZIP { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }


        public int LeadId { get; set; }
        public Lead? Lead { get; set; }
    }
}
