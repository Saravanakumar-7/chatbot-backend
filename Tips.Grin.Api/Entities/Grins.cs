using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tips.Purchase.Api.Entities;

namespace Tips.Grin.Api.Entities
{
    public class Grins
    {
        [Key]
        public int Id { get; set; }
        public string? GrinNumber { get; set; }         

        [Required]
        public string VendorName { get; set; } 

        [Required]
        public string VendorId { get; set; }

        [Required]
        public string InvoiceNumber { get; set; }

        [Precision(13,3)]
        public decimal? InvoiceValue { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string? AWBNumber1 { get; set; }

        public DateTime? AWBDate1 { get; set; }

        public string? AWBNumber2 { get; set; }

        public DateTime? AWBDate2 { get; set; }

        public string? BENumber { get; set; }

        public DateTime? BEDate { get; set; }

        [Precision(13,3)]
        public decimal? TotalInvoiceValue { get; set; }
        [Precision(13, 3)]
        public decimal? Freight { get; set; }
        [Precision(13, 3)]
        public decimal? Insurance { get; set; }
        [Precision(13, 3)]
        public decimal? LoadingorUnLoading { get; set; }
        public DateTime? GateEntryDate { get; set; }
        public string? GateEntryNo { get; set; }
        [Precision(13, 3)]
        public decimal? CurrencyConversion { get; set; }
        [Precision(13, 3)]
        public decimal? Transport { get; set; }
        [Precision(13, 3)]
        public decimal? BECurrencyValue { get; set; }

        [DefaultValue(0)]
        public Status Status { get; set; }
        public bool IsGrinCompleted { get; set; }
        public bool IsIqcCompleted { get; set; }
        public bool IsBinningCompleted { get; set; }

        //public string? GrinDocuments { get; set; }

        public List<DocumentUpload>? GrinDocuments { get; set; }


        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }


        public List<GrinParts>? GrinParts { get; set; }
        public List<OtherCharges>? OtherCharges { get; set; }

    }
}
