using static Tips.Production.Api.Entities.ShopOrder;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Tips.Production.Api.Entities.Enums;
using Entities.Enums;

namespace Tips.Production.Api.Entities.DTOs
{
    public class ShopOrderDto
    {
        public int Id { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? Description { get; set; }
        public ProjectType ProjectType { get; set; }
        public PartType ItemType { get; set; }
        public string? ItemNumber { get; set; }
        public decimal TotalSOReleaseQty { get; set; }
        public DateTime SOCloseDate { get; set; }
        public decimal? CanCreateQty { get; set; }

        public decimal WipQty { get; set; }

        public decimal OqcQty { get; set; }

        public decimal ScrapQty { get; set; }
        public OrderStatus FGDoneStatus { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ShopOrderType ShopOrderType { get; set; }
        public OrderStatus Status { get; set; }
        public ShopOrderConformationStatus ShopOrderConfirmationStatus { get; set; }
        public ShopOrderConformationStatus OQCStatus { get; set; }
        public ShopOrderConformationStatus OQCBinningStatus { get; set; }
        public bool IsShortClosed { get; set; }
        public string? ShortClosedBy { get; set; }
        public DateTime? ShortClosedOn { get; set; }
        public bool ShopOrderApproval { get; set; } = false;
        public string? ShopOrderApprovedBy { get; set; }
        public DateTime? ShopOrderApprovedDate { get; set; }
        [Required]
        public decimal BomRevisionNo { get; set; }
        public string? Remarks { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<ShopOrderItemDto>? ShopOrderItems { get; set; }

    }

    public class ShopOrderPostDto
    {
        [Required]
        public ProjectType ProjectType { get; set; }
        [Required]
        public PartType ItemType { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        [Precision(13, 3)]
        public decimal TotalSOReleaseQty { get; set; }
        [Required]
        public DateTime SOCloseDate { get; set; }
        [Required]
        public decimal BomRevisionNo { get; set; }
        [Precision(13, 3)]
        public decimal? CanCreateQty { get; set; }
        public string? Remarks { get; set; }
        public ShopOrderType ShopOrderType { get; set; }
        public List<ShopOrderItemPostDto>? ShopOrderItems { get; set; }
    }

    public class ShopOrderUpdateDto
    {
        public int Id { get; set; }
        [Required]
        public string? ShopOrderNumber { get; set; }
        [Required]
        public ProjectType ProjectType { get; set; }
        [Required]
        public PartType ItemType { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        [Precision(13, 3)]
        public decimal TotalSOReleaseQty { get; set; }
        [Required]
        public DateTime SOCloseDate { get; set; }
        [Required]
        public decimal BomRevisionNo { get; set; }
        [Precision(13, 3)]
        public decimal? CanCreateQty { get; set; }
        public ShopOrderType ShopOrderType { get; set; }
        public string? Remarks { get; set; }
        public string? Unit { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<ShopOrderItemUpdateDto>? ShopOrderItems { get; set; }
    }
    public class ListOfShopOrderDto
    {
        public int Id { get; set; }
        public string? ShopOrderNumber { get; set; }
        public decimal TotalSOReleaseQty { get; set; }
    }
    public class ListOfShopOrderNumberwithTypeDto
    {
        public int Id { get; set; }
        public string? ShopOrderNumber { get; set; }
        public ShopOrderType ShopOrderType { get; set; }
        public decimal TotalSOReleaseQty { get; set; }
    }
    public class ShopOrderSearchDto
    {
        public List<string>? ShopOrderNumber { get; set; }
        public List<string>? FGItemNumber { get; set; }
        public List<string>? SAItemNumber { get; set; }
        public List<decimal>? TotalSOReleaseQty { get; set; }

    }
    public class ShopOrderReportDto
    {
        public int Id { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? Description { get; set; }
        public ProjectType ProjectType { get; set; }
        public PartType ItemType { get; set; }
        public string? ItemNumber { get; set; }
        public decimal TotalSOReleaseQty { get; set; }
        public DateTime SOCloseDate { get; set; }
        public decimal? CanCreateQty { get; set; }

        public decimal WipQty { get; set; }

        public decimal OqcQty { get; set; }

        public decimal ScrapQty { get; set; }

        public OrderStatus FGDoneStatus { get; set; }
        public bool IsDeleted { get; set; } = false;

        public OrderStatus Status { get; set; }

        public bool IsShortClosed { get; set; }
        public string? ShortClosedBy { get; set; }
        public DateTime? ShortClosedOn { get; set; }
        [Required]
        public decimal BomRevisionNo { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<ShopOrderItemReportDto>? ShopOrderItems { get; set; }

    }
    public class ShopOrderReportWithParamDto
    {
        public string? ShopOrderNumber { get; set; }
        public string? ProjectType { get; set; }
        public string? ProjectNumber { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? KPN { get; set; }
        public string? MPN { get; set; }
    }
    public class ShopOrderReportWithParamDtoForTrans
    {
        public string? WorkOrderNumber { get; set; }
        public string? ProjectType { get; set; }
        public string? ProjectNumber { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? KPN { get; set; }
        public string? MPN { get; set; }
        public string? Status { get; set; }
    }

    public class ShopOrderReportWithDateDtoForTrans
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Status { get; set; }
    }

    public class ShopOrderSPReportForTrans
    {
        public string? WorkOrderNumber { get; set; }
        public DateTime? WorkOrderDate { get; set; }
        public string? ProjectType { get; set; }
        public int? ItemType { get; set; }
        public string? ItemNumber { get; set; }
        public string? CustomerName { get; set; }
        public decimal? BOMversion { get; set; }
        public string? ProjectNumber { get; set; }
        public string? SalesOrderNumber { get; set; }
        public decimal? salesorederQty { get; set; }
        public decimal? OpenSalesOrderQty { get; set; }
        public decimal? ReleaseQty { get; set; }
        public string? KPN { get; set; }
        public string? MPN { get; set; }
        public string? Description { get; set; }
        public decimal? ShopOrder_qty { get; set; }
        public string? UOM { get; set; }
        public decimal? WO_confirmed_Qty { get; set; }
        public decimal? WO_Balance_Qty { get; set; }
        public string? Status { get; set; }
    }
    public class ShopOrderShortCloseDto
    {
        public decimal? SOReleaseQty { get; set; }
        public decimal WipQty { get; set; }
        public OrderStatus Status { get; set; }

    }
    public class ShopOrderWipQtyDto
    {
        public string ItemNumber { get; set; }
        public decimal WipQuantity { get; set; }
    }
    public class ShopOrderNumberSPReportForAvi
    {
        public string? ShopOrderNumber { get; set; }
        public DateTime ShopOrder_date { get; set; }
        public string? ProjectType { get; set; }
        public PartType ItemType { get; set; }
        public string? ItemNumber { get; set; }
        public decimal? BOMversion { get; set; }
        public string? ProjectNumber { get; set; }
        public string? SalesOrderNumber { get; set; }
        public decimal? salesorederQty { get; set; }
        [Precision(13, 3)]
        public decimal? OpenSalesOrderQty { get; set; }

        [Precision(13, 3)]
        public decimal? ReleaseQty { get; set; }
        public string? KPN { get; set; }
        public string? MPN { get; set; }
        public string? Description { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? UOC { get; set; }
        public decimal? ShopOrder_qty { get; set; }
        public decimal? RequiredQty { get; set; }
        public string? UOM { get; set; }
        public string? Remarks { get; set; }
        public decimal? ShopOrder_ConfirmationQty { get; set; }
        public decimal? ShopOrder_OQCConfirmationQty { get; set; }
        public DateTime? ShopOrder_CompletionDate { get; set; }
        public decimal? ShopOrder_WIP_Pending_Qty { get; set; }
        public string? Status { get; set; }
    }

    public class ShopOrderComsumpDetailsDto
    {
        public string? ShopOrderNumber { get; set; }
        public string? ItemNumber { get; set; }
        public decimal ReleaseQty { get; set; }
        public decimal WipQty { get; set; }
    }
}
