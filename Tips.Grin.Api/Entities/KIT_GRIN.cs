using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Tips.Purchase.Api.Entities;

namespace Tips.Grin.Api.Entities
{
    public class KIT_GRIN
    {
        [Key]
        public int Id { get; set; }
        public string KIT_GrinNumber { get; set; }

        [Required]
        public string VendorName { get; set; }

        [Required]
        public string VendorId { get; set; }
        public string? VendorNumber { get; set; }

        [Required]
        public string InvoiceNumber { get; set; }

        [Precision(13, 3)]
        public decimal? InvoiceValue { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string? AWBNumber1 { get; set; }

        public DateTime? AWBDate1 { get; set; }

        public string? AWBNumber2 { get; set; }

        public DateTime? AWBDate2 { get; set; }

        public string? BENumber { get; set; }

        public DateTime? BEDate { get; set; }

        [Precision(13, 3)]
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
        public bool IsKIT_IqcCompleted { get; set; } = false;
        public bool IsKIT_BinningCompleted { get; set; } = false;
        public string? KIT_GrinDocuments { get; set; }
        public bool TallyStatus { get; set; } = false;
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<KIT_GRINParts> KIT_GRINParts { get; set; }
        public List<KIT_GRIN_OtherCharges>? KIT_GRIN_OtherCharges { get; set; }
    }
}
