using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Tips.Purchase.Api.Entities;

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
        public string? KIT_GrinDocuments { get; set; }
        public bool TallyStatus { get; set; } = false;
        public List<KIT_GRINPartsPostDto> KIT_GRINParts { get; set; }
        public List<KIT_GRIN_OtherChargesPostDto>? KIT_GRIN_OtherCharges { get; set; }
    }
    public class KIT_GRIN_POUpdate
    {
        public string PONumber { get; set; }
        public List<KIT_GRIN_POItemsUpdate> POItems { get; set; }
    }
    public class KIT_GRIN_POItemsUpdate
    {
        public string ItemNumber { get; set; }
        public decimal? Qty { get; set; }
        public List<KIT_GRIN_POProjectUpdate> POProjects { get; set; }
    }
    public class KIT_GRIN_POProjectUpdate
    {
        public string? ProjectNumber { get; set; }
        public decimal? ProjectQty { get; set; }
        public List<KIT_GRIN_POComponentsUpdate> POComponents { get; set; }
    }
    public class KIT_GRIN_POComponentsUpdate
    {
        public string? PartNumber { get; set; }
        public decimal KitComponentQty { get; set; }
    }
    public class KIT_GrinNoForKIT_IqcAndKIT_Binning
    {
        public string? KIT_GrinNumber { get; set; }
        public int KIT_GrinId { get; set; }

    }
    public class KIT_GRINDto
    {
        public int Id { get; set; }
        public string? KIT_GrinNumber { get; set; }
        public string VendorName { get; set; }
        public string VendorId { get; set; }
        public string? VendorNumber { get; set; }
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
        public bool IsKIT_GrinCompleted { get; set; }
        public bool IsKIT_IqcCompleted { get; set; }
        public bool IsKIT_BinningCompleted { get; set; }
        public string? KIT_GrinDocuments { get; set; }
        public bool TallyStatus { get; set; } = false;
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<KIT_GRINPartsDto> KIT_GRINParts { get; set; }
        public List<KIT_GRIN_OtherChargesDto>? KIT_GRIN_OtherCharges { get; set; }
    }
    public class KIT_GRINUpdateDto
    {
        public int Id { get; set; }
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
        public string? KIT_GrinDocuments { get; set; }
        public bool TallyStatus { get; set; } = false;
        public List<KIT_GRINPartsUpdateDto> KIT_GRINParts { get; set; }
        public List<KIT_GRIN_OtherChargesUpdateDto>? KIT_GRIN_OtherCharges { get; set; }
    }
}
