using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class KIT_GRINPostDto
    {
        [Required(ErrorMessage = "VendorName is required")]
        public string VendorName { get; set; }

        [Required(ErrorMessage = "VendorId is required")]
        public string VendorId { get; set; }
        public string? VendorNumber { get; set; }
        [Required(ErrorMessage = "InvoiceNumber is required")]
        public string InvoiceNumber { get; set; }
        public decimal? InvoiceValue { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string? AWBNumber1 { get; set; }

        public DateTime? AWBDate1 { get; set; }

        public string? AWBNumber2 { get; set; }

        public DateTime? AWBDate2 { get; set; }

        public string? BENumber { get; set; }

        public DateTime? BEDate { get; set; }

        public decimal? TotalInvoiceValue { get; set; }
        [Precision(13, 3)]
        public decimal? Freight { get; set; }
        [Precision(13, 3)]
        public decimal? Insurance { get; set; }
        [Precision(13, 3)]
        public decimal? LoadingorUnLoading { get; set; }
        public DateTime? GateEntryDate { get; set; }
        [Precision(13, 3)]
        public decimal? CurrencyConversion { get; set; }
        [Precision(13, 3)]
        public decimal? Transport { get; set; }
        [Precision(13, 3)]
        public decimal? BECurrencyValue { get; set; }
        public string? GateEntryNo { get; set; }
        public string? GrinDocuments { get; set; }
        public bool TallyStatus { get; set; } = false;
        public List<KIT_GRINPartsPostDto> KIT_GRINParts { get; set; }
        public List<KIT_GRIN_OtherChargesPostDto>? KIT_GRIN_OtherCharges { get; set; }
    }
}
