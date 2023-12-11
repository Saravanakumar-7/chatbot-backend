using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tips.Production.Api.Entities.Enums;
namespace Tips.Production.Api.Entities
{
    public class MaterialIssueSPReport
    {
        public string? ShopOrderNumber { get; set; }
        public DateTime? ShopOrderDate { get; set; }
        public string? ItemNumber { get; set; }
        public ProjectType ProjectType { get; set; }

        public PartType ItemType { get; set; }
        public decimal BomVersion { get; set; }
        public string? ProjectNumber { get; set; }
        public decimal OrderQty { get; set; }

        [Precision(13, 3)]
        public decimal BalanceQty { get; set; }
        public string? MPN { get; set; }
        public string? Description { get; set; }
        public decimal IssuedQty { get; set; }
        public string? UOM { get; set; }
        public decimal Quantity { get; set; }
        public string? Remarks { get; set; }
    }
}
