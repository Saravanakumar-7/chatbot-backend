using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class IQCForServiceItemsDto
    {
        public int Id { get; set; }
        public string? IQCForServiceItemsNumber { get; set; }
        public string? GrinsForServiceItemsNumber { get; set; }
        [Required]
        public string VendorName { get; set; }
        [Required]
        public string VendorId { get; set; }
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
        public int GrinsForServiceItemsId { get; set; }
        public string Unit { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? GateEntryNo { get; set; }
        public DateTime? GateEntryDate { get; set; }
        public List<IQCForServiceItems_ItemsDto>? IQCForServiceItems_Items { get; set; }

    }
    public class IQCForServiceItemsPostDto
    {
        public string GrinsForServiceItemsNumber { get; set; }
        public int GrinsForServiceItemsId { get; set; }
        public List<IQCForServiceItems_ItemsPostDto>? IQCForServiceItems_Items { get; set; }
    }
    public class IQCForServiceItemsSaveDto
    {
        public string GrinsForServiceItemsNumber { get; set; }
        public int GrinsForServiceItemsId { get; set; }
        public IQCForServiceItems_ItemsSaveDto? IQCForServiceItems_Items { get; set; }

    }
    public class IQCForServiceItemsReportWithParamDto
    {
        public string? GrinsForServiceItemsNumber { get; set; }
        public string? ItemNumber { get; set; }
    }
    public class IQCForServiceItemsReportWithParamForTransDto
    {
        public string? GrinsForServiceItemsNumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? ProjectNumber { get; set; }
    }
    public class IQCForServiceItemsSPReport
    {
        public string? GrinsForServiceItemsNumber { get; set; }
        public string? VendorName { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal? InvoiceValue { get; set; }
        public string? PONumber { get; set; }
        public string? ItemDescription { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? ManufactureBatchNumber { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? ProjectNumber { get; set; }
        public decimal? ProjectQty { get; set; }
        public string? UOM { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public decimal? ReceivedQty { get; set; }
        public decimal? AcceptedQty { get; set; }
        public decimal? RejectedQty { get; set; }
        public string? Remarks { get; set; }
        public decimal? TotalInvoiceValue { get; set; }
        public string? AWBNumber1 { get; set; }
        public DateTime? AWBDate1 { get; set; }


    }

}
