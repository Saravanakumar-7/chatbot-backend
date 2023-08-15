using Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class ShopOrderMaterialIssueTrackerDto
    {
        public string ShopOrderNumber { get; set; }
        public string Description { get; set; }

        public string PartNumber { get; set; }
        [Precision(13, 3)]
        public decimal IssuedQty { get; set; }
        [Precision(13, 3)]
        public decimal ConvertedToFgQty { get; set; }
        [Precision(13, 3)]
        public decimal BalanceQty { get; set; }
        public string DataFrom { get; set; }
    }
    public class ShopOrderDtoForMaterialRequest
    {
        public string PartNumber { get; set; }
        public string ProjectNumber { get; set; }
        public decimal IssueQty { get; set; }
        public string? DataFrom { get; set; }
        public string ShopOrderNumber { get; set; }
        public string Description { get; set; }
        public string MftrPartNumber { get; set; }
        public PartType PartType { get; set; }

    }
}
