using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.Grin.Api.Entities
{
    public class Grins
    {
        public int Id { get; set; }
        public string? GrinNumber { get; set; }

        [Required]
        public string PONumber { get; set; }

        [Required]
        public string VendorName { get; set; } 

        [Required]
        public string VendorId { get; set; }

        [Required]
        public string InvoiceNumber { get; set; }

        public string? InvoiceValue { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string? AWBNumber1 { get; set; }

        public DateTime? AWBDate1 { get; set; }

        public string? AWBNumber2 { get; set; }

        public DateTime? AWBDate2 { get; set; }

        public string? BENumber { get; set; }

        public DateTime? BEDate { get; set; }

        public int? TotalInvoice { get; set; }
        public string? GrinDocuments { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }


        public List<GrinParts>? GrinParts { get; set; }
        public List<ProjectNumbers>? ProjectNumbers { get; set; }


    }
}
