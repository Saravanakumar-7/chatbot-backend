using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Tips.Purchase.Api.Entities;

namespace Tips.Grin.Api.Entities
{
    public class GrinsForServiceItems
    {
        [Key]
        public int Id { get; set; }
        public string? GrinsForServiceItemsNumber { get; set; }
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
        public bool IsGrinsForServiceItemsCompleted { get; set; }
        public bool IsIqcForServiceItemsCompleted { get; set; }
        public string? GrinsForServiceItemsDocuments { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<GrinsForServiceItemsParts>? GrinsForServiceItemsParts { get; set; }
        public List<GrinsForServiceItemsOtherCharges>? GrinsForServiceItemsOtherCharges { get; set; }
    }
}
