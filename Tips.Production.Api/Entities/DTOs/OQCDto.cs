using Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities.DTOs
{
    public class OQCDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }

        public PartType ItemType { get; set; }
        public string? ShopOrderNumber { get; set; }
        [Precision(13, 3)]
        public decimal ShopOrderQty { get; set; }
        [Precision(13, 3)]
        public decimal PendingQty { get; set; }
        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; }
        [Precision(13, 3)]
        public decimal RejectedQty { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class OQCPostDto
    {
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }

        public PartType ItemType { get; set; }
        public string? ShopOrderNumber { get; set; }
        [Precision(13, 3)]
        public decimal ShopOrderQty { get; set; }
        [Precision(13, 3)]
        public decimal PendingQty { get; set; }
        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; }
        [Precision(13, 3)]
        public decimal RejectedQty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class OQCUpdateDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }

        public PartType ItemType { get; set; }
        public string? ShopOrderNumber { get; set; }
        [Precision(13, 3)]
        public decimal ShopOrderQty { get; set; }
        [Precision(13, 3)]
        public decimal PendingQty { get; set; }
        [Precision(13, 3)]
        public decimal AcceptedQty { get; set; }
        [Precision(13, 3)]
        public decimal RejectedQty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
     public class ShopOrderConfirmationItemNoListDto
    {
        public string? ItemNumber { get; set; }
        public string? Description { get; set; }
    }
    public class ShopOrderConfirmationDetailsDto
    {
        public string? ShopOrderNumber { get; set; }
        public decimal? ShopOrderQty { get; set; }
        public decimal? PendingQty { get; set; }

    }

    public class OQCIdNameList
    {
        public int Id { get; set; }

        public string? ShopOrderNumber { get; set; }
    }
}
