using Entities;
using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class BinningDto
    {
        public int Id { get; set; }
        public string? GrinNumber { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorNumber { get; set; }

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
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? GateEntryNo { get; set; }
        public DateTime? GateEntryDate { get; set; }
        public List<BinningItemsDto>? BinningItems { get; set; }

    }


    public class BinningPostDto
    {

        public string? GrinNumber { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorNumber { get; set; }


        public List<BinningItemsPostDto>? BinningItems { get; set; }


    }

    public class BinningUpdateDto
    {

        public int Id { get; set; }
        public string? GrinNumber { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorNumber { get; set; }
        public List<BinningItemsUpdateDto>? BinningItems { get; set; }


    }
    public class BinningIdNameListDto
    {
        public int Id { get; set; }
        public string? GrinNumber { get; set; }
    }
    public class BinningSearchDto
    {
        public List<string>? GrinNumber { get; set; }
        public List<string>? VendorName { get; set; }
        public List<string>? VendorId { get; set; }
        public List<String>? InvoiceNumber { get; set; }
    }
    public class BinningInventoryDtoPost
    {
        [Required]
        public string PartNumber { get; set; }

        [Required]
        public string MftrPartNumber { get; set; }
        public string? LotNumber { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string ProjectNumber { get; set; }
        [Required]
        public decimal Balance_Quantity { get; set; }
        public bool IsStockAvailable { get; set; }
        public decimal? Max { get; set; }
        public decimal? Min { get; set; }
        [Required]
        public string? UOM { get; set; }

        [Required]
        public string? Warehouse { get; set; }
        [Required]
        public string? Location { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public PartType PartType { get; set; }
        public string? GrinMaterialType { get; set; }
        [Required]
        public string? ReferenceID { get; set; }
        [Required]
        public string? ReferenceIDFrom { get; set; }
        public string? ShopOrderNo { get; set; }

    }

    public class BinningInventoryTranctionDto
    {
        public string PartNumber { get; set; }
        public string LotNumber { get; set; }
        public string MftrPartNumber { get; set; }
        public string Description { get; set; }
        public PartType PartType { get; set; }
        public string ProjectNumber { get; set; }
        public InventoryType TransactionType { get; set; }
        [Precision(18, 2)]
        public decimal Issued_Quantity { get; set; }
        public string? UOM { get; set; }
        public string? ShopOrderId { get; set; }
        public string ReferenceID { get; set; }
        public string ReferenceIDFrom { get; set; }

        public decimal? BOM_Version_No { get; set; }

        public string? From_Location { get; set; }
        public string TO_Location { get; set; }

        public bool? ModifiedStatus { get; set; } = false;

        public string? Unit { get; set; }

        public string? GrinMaterialType { get; set; } = "Bought Out";

        public string? Remarks { get; set; }
        public bool IsStockAvailable { get; set; }
        public string Warehouse { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public string? shopOrderNo { get; set; }

    }

    public class BinningReportDto
    {
        public int Id { get; set; }
        public string? GrinNumber { get; set; }
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
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }


        public List<BinningItemsReportDto>? BinningItems { get; set; }

    }
    public class GrinAndBinningDetailsDto
    {
        public int Id { get; set; }
        public string? GrinNumber { get; set; }
        public string? VendorId { get; set; }
        public string? VendorName { get; set; }
        public string? VendorNumber { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }
    public class BinningSaveDto
    {
        public string? GrinNumber { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorNumber { get; set; }
        public BinningItemsSaveDto? BinningItems { get; set; }
    }
    public class BinningSPReportAviDto
    {
        public string? ponumber { get; set; }
        public string? grinnumber { get; set; }
        public string? itemnumber { get; set; }
        public string? projectnumber { get; set; }
    }
    public class BinningSPReportAvi
    {
        public string? PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public string? GrinNumber { get; set; }
        public DateTime? GrinDate { get; set; }
        public string? GateEntryNo { get; set; }
        public DateTime? GateEntryDate { get; set; }
        public string? VendorName { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? MftrItemNumber { get; set; }
        public string? ItemDescription { get; set; }
        public string? UOM { get; set; }
        public decimal? BinningQty { get; set; }
        public string? BinningLocation { get; set; }
        public string? BinningWarehouse { get; set; }
        public DateTime? BinningDate { get; set; }
    }


}
