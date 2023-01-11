using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Tips.SalesService.Api.Entities
{
    public class FinalOqc
    {
        public int? Id { get; set; }
        public string? ProjectNumber { get; set; }
        public string? FGItemNumber { get; set; } //partnumber
        public string ItemType { get; set; }
        public string? ShopOrderNumber { get; set; }

        [Precision(18, 3)]
        public decimal? ShopOrderQty { get; set; }
        public string? SubAssemblyItemNumber { get; set; }
        public string? SAShopOrderNumber { get;}
        [Precision(18, 3)]
        public decimal? SAShopOrderQty { get; set; }
        [Precision(18, 3)]
        public decimal? PendingQty { get; set; }
        [Precision(18, 3)]
        public decimal? AcceptedQty { get; set; }
        [Precision(18, 3)]
        public decimal? RejectedQty { get; set; }

        public string Unit { get; set; }


        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
