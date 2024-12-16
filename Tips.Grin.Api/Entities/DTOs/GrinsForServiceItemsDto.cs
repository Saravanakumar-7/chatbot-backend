using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tips.Purchase.Api.Entities;
namespace Tips.Grin.Api.Entities.DTOs
{
    public class GrinsForServiceItemsDto
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
        public List<GrinsForServiceItemsPartsDto>? GrinsForServiceItemsParts { get; set; }
        public List<GrinsForServiceItemsOtherChargesDto>? GrinsForServiceItemsOtherCharges { get; set; }
    }
    public class GrinsForServiceItemsPostDto
    {
        [Required(ErrorMessage = "VendorName is required")]
        [StringLength(100, ErrorMessage = "ItemDescription can't be longer than 100 characters")]
        public string VendorName { get; set; }
        [Required(ErrorMessage = "VendorId is required")]
        [StringLength(100, ErrorMessage = "ItemDescription can't be longer than 100 characters")]
        public string VendorId { get; set; }
        public string? VendorNumber { get; set; }
        [Required(ErrorMessage = "InvoiceNumber is required")]
        [StringLength(100, ErrorMessage = "ItemDescription can't be longer than 100 characters")]
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
        public string? GrinsForServiceItemsDocuments { get; set; }
        public List<GrinsForServiceItemsPartsPostDto>? GrinsForServiceItemsParts { get; set; }
        public List<GrinsForServiceItemsOtherChargesPostDto>? GrinsForServiceItemsOtherCharges { get; set; }
    }
    public class GrinsForServiceItemsUpdateDto
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
        public string? GrinsForServiceItemsDocuments { get; set; }
        public string? GateEntryNo { get; set; }
        public decimal? TotalInvoiceValue { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<GrinsForServiceItemsPartsUpdateDto>? GrinsForServiceItemsParts { get; set; }
        public List<GrinsForServiceItemsOtherChargesUpdateDto>? GrinsForServiceItemsOtherCharges { get; set; }
    }
    public class GrinsForServiceItemsItemMasterEnggDto
    {
        public int Id { get; set; }
        public string? GrinsForServiceItemsNumber { get; set; }
        [Required]
        public string VendorName { get; set; }
        [Required]
        public string VendorId { get; set; }
        public string? VendorNumber { get; set; }
        [Required]
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
        public string? GrinsForServiceItemsDocuments { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<GrinsForServiceItemsPartsItemMasterEnggDto>? GrinsForServiceItemsParts { get; set; }
        public List<GrinsForServiceItemsOtherChargesDto>? GrinsForServiceItemsOtherCharges { get; set; }
    }
    public class GrinsForServiceItemsIQCForServiceItemsSaveDto
    {
        public string? GrinsForServiceItemsNumber { get; set; }
        public int GrinsForServiceItemsId { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorNumber { get; set; }

        public GrinsForServiceItemsIQCForServiceItems_ItemsSaveDto GrinsForServiceItemsIQCForServiceItems_ItemsSaveDto { get; set; }

    }
    public class GrinForServiceItemsNoForIqcForServiceItems
    {
        public string? GrinsForServiceItemsNumber { get; set; }
        public int GrinsForServiceItemsId { get; set; }

    }
    public class GrinForServiceItemsReportWithParamDto
    {
        public string GrinsForServiceItemsNumber { get; set; }
        public string VendorName { get; set; }
        public string PONumber { get; set; }
        public string KPN { get; set; }
        public string MPN { get; set; }
        public string Warehouse { get; set; }
        public string Location { get; set; }
    }
    public class GrinForServiceItemsReportWithParamForTransDto
    {
        public string GrinsForServiceItemsNumber { get; set; }
        public string VendorName { get; set; }
        public string PONumber { get; set; }
        public string KPN { get; set; }
        public string MPN { get; set; }
        public string Warehouse { get; set; }
        public string Location { get; set; }
        public string ProjectNumber { get; set; }
    }
    public class GrinForServiceItemsSPReport
    {
        public string? GrinsForServiceItemsNumber { get; set; }
        public string? VendorName { get; set; }
        public string? VendorId { get; set; }
        public string? VendorAddress { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? PONumber { get; set; }
        public string? KPN { get; set; }
        public string? MPN { get; set; }
        public string? ItemDescription { get; set; }
        public string? ManufactureBatchNumber { get; set; }

        [Precision(18, 3)]
        public decimal? UnitPrice { get; set; }

        [Precision(18, 3)]
        public decimal? Qty { get; set; }

        [Precision(13, 3)]
        public decimal? AcceptedQty { get; set; }
        public string? UOM { get; set; }

        [Precision(13, 3)]
        public decimal? SGST { get; set; }

        [Precision(13, 3)]
        public decimal? CGST { get; set; }

        [Precision(13, 3)]
        public decimal? IGST { get; set; }

        [Precision(13, 3)]
        public decimal? UTGST { get; set; }
        public decimal? totalvalue { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? Remarks { get; set; }
        public DateTime? GrinDate { get; set; }
        public string? ProjectNumber { get; set; }
        public string? UOC { get; set; }
    }
    public class GrinForServiceItemsandIqcForServiceItemsDetail
    {
        public List<GrinsForServiceItems> grinsForServiceItems { get; set; }
        public List<IQCForServiceItems>? iqcsForServiceItems { get; set; }
    }
}
