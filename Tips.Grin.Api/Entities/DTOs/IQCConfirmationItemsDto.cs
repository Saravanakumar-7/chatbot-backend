using Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class IQCConfirmationItemsDto
    {
        public int? Id { get; set; }
        public string? PONumber { get; set; }
        public int GrinPartId { get; set; }
        public string? ItemNumber { get; set; }
        public string ItemDescription { get; set; }
        public string MftrItemNumber { get; set; }
        public List<ProjectNumbersDto>? ProjectNumbers { get; set; }
        public string ManufactureBatchNumber { get; set; }

        [Precision(18, 3)]
        public decimal UnitPrice { get; set; }

        [Precision(18, 3)]
        public decimal POOrderedQty { get; set; }

        [Precision(18, 3)]
        public decimal POBalancedQty { get; set; }

        [Precision(18, 3)]
        public decimal POUnitPrice { get; set; }
        public string UOM { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public decimal? ReceivedQty { get; set; }
        public decimal AcceptedQty { get; set; }
        public decimal RejectedQty { get; set; }
        public decimal RejectReturnQty { get; set; }
        public string? Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class IQCConfirmationItemsPostDto
    {
        public string? ItemNumber { get; set; }
        public int GrinPartId { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal AcceptedQty { get; set; }
        public decimal RejectedQty { get; set; }
        public string? Remarks { get; set; }
    }
    public class IQCConfirmationItemsUpdateDto
    {
        public int? Id { get; set; }
        public string? ItemNumber { get; set; }
        public int GrinPartId { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal AcceptedQty { get; set; }
        public decimal RejectedQty { get; set; }
        public string? Remarks { get; set; }
    }
    public class IQCConfirmationItemsReportDto
    {
        public int? Id { get; set; }
        public string? PONumber { get; set; }
        public string? GrinNumber { get; set; }
        public int GrinPartId { get; set; }
        public string? ItemNumber { get; set; }
        public string ItemDescription { get; set; }
        public string MftrItemNumber { get; set; }
        public List<ProjectNumbersReportDto>? ProjectNumbers { get; set; }
        public string ManufactureBatchNumber { get; set; }

        [Precision(18, 3)]
        public decimal UnitPrice { get; set; }

        [Precision(18, 3)]
        public decimal POOrderedQty { get; set; }

        [Precision(18, 3)]
        public decimal POBalancedQty { get; set; }

        [Precision(18, 3)]
        public decimal POUnitPrice { get; set; }
        public string UOM { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public decimal? ReceivedQty { get; set; }
        public decimal AcceptedQty { get; set; }
        public decimal RejectedQty { get; set; }
        public string? Remarks { get; set; }

    }
    public class IQCConfirmationItemsSaveDto
    {
        public string? ItemNumber { get; set; }
        public int GrinPartId { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal AcceptedQty { get; set; }
        public decimal RejectedQty { get; set; }
        public string? Remarks { get; set; }
    }
    public class IQCInventoryDto
    {
        public string PartNumber { get; set; }
        public string? LotNumber { get; set; }

        public string MftrPartNumber { get; set; }


        public string Description { get; set; }

        public bool IsStockAvailable { get; set; }
        public string ProjectNumber { get; set; }
        public decimal Balance_Quantity { get; set; }
        public decimal? Max { get; set; }
        public decimal? Min { get; set; }
        public string? UOM { get; set; }

        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? GrinNo { get; set; }
        public int? GrinPartId { get; set; }
        public PartType PartType { get; set; }
        public string? GrinMaterialType { get; set; }
        public string? ReferenceID { get; set; }
        public string? ReferenceIDFrom { get; set; }
        public string? ShopOrderNo { get; set; }

    }
}
