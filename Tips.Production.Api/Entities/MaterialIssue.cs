using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tips.Production.Api.Entities.Enums;

namespace Tips.Production.Api.Entities
{
    public class MaterialIssue
    {
        public int Id { get; set; }
        public string? ShopOrderNumber { get; set; }
        public ShopOrderType ShopOrderType { get; set; }
        public DateTime? ShopOrderDate { get; set; }
        public string? ItemNumber { get; set; }
        public string? ProjectType { get; set; }
        public decimal? BomRevisionNo { get; set; }
        public PartType ItemType { get; set; }
        [Precision(13, 3)]
        public decimal? ShopOrderQty { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public IssuedStatus MaterialIssuedStatus { get; set; } = IssuedStatus.Open;
        public bool IsShortClosed { get; set; }
        public List<MaterialIssueItem> materialIssueItems { get; set; }
    }
}
