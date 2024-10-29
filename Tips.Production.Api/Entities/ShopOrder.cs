using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Tips.Production.Api.Entities.Enums;

namespace Tips.Production.Api.Entities
{
    public class ShopOrder
    {
        [Key]      
        public int Id { get; set; }       
        public string? ShopOrderNumber { get; set; }
        [Required]
        public string? ProjectType { get; set; }
        [Required]
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
        [Required]
        public PartType  ItemType { get; set; }
        
        [Precision(13,3)]
        public decimal TotalSOReleaseQty { get; set; }
        [Required]
        public DateTime SOCloseDate { get; set; }
        [Required]
        public decimal BomRevisionNo { get; set; }

        [Precision(13,3)]
        public decimal? CanCreateQty { get; set; }
        [Precision(13, 3)]
        public decimal WipQty { get; set; }
        [Precision(13, 3)]
        public decimal OqcQty { get; set; }
        [Precision(13, 3)]
        public decimal ScrapQty { get; set; }
        
        [DefaultValue(0)]
        public OrderStatus FGDoneStatus { get; set; } = OrderStatus.Open;
        public bool IsDeleted { get; set; } = false;
        [DefaultValue(0)]
        public OrderStatus Status { get; set; } = OrderStatus.Open;
        public ShopOrderConformationStatus ShopOrderConfirmationStatus { get; set; }
        public ShopOrderConformationStatus OQCStatus { get; set; }
        public ShopOrderConformationStatus OQCBinningStatus { get; set; }
        [DefaultValue(false)]
        public bool IsShortClosed { get; set; } = false;
        public string? ShortClosedBy { get; set; }
        public DateTime? ShortClosedOn { get; set; }
        public string? Remarks { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public ShopOrderType ShopOrderType { get; set; }
        public List<ShopOrderItem>? ShopOrderItems { get; set; }

    }
}
