using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities
{
    public class RfqNotes
    {
        public int Id { get; set; }
        public string? FromCSCategory { get; set; }

        public string? FromEnggCategory { get; set; }

        public string? FromEnggNotes { get; set; }
        public string? FromCSNotes { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int RfqId { get; set; }
        public Rfq? Rfq { get; set; }
    }
}
