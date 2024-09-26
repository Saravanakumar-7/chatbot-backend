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
        public ProjectType? ProjectType { get; set; }
        public PartType? ItemType { get; set; }
        public string? ItemNumber { get; set; }
        public string? latestMPN { get; set; }
        public decimal? BOMVersion { get; set; }
        public string? ProjectNumber { get; set; }
        public string? SalesOrderNumber { get; set; }
        public decimal? SalesorderQnty { get; set; }
        public decimal? ReleaseQty { get; set; }
        public string? KPN { get; set; }
        public string? Description { get; set; }
        public decimal? AvailableQnty { get; set; }
        public decimal? IssuedQty { get; set; }
        public string? UOM { get; set; }
        public string? Remarks { get; set; }
        public decimal? RequiredQty { get; set; }
        public decimal? BalanceIssueQnty { get; set; }
    }
}
