using Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities
{
    public class ShopOrderNumberSPReport
    {
        public string? ShopOrderNumber { get; set; }
        public DateTime ShopOrder_date { get; set; }
        public string? CreatedBy { get; set; }
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
        public decimal? OqcQty { get; set; }
        public decimal? WipQty { get; set; }
        public decimal? ShopOrderConfirmationQty { get; set; }
        public decimal? RequiredQty { get; set; }
        public string? UOM { get; set; }
        public string? Remarks { get; set; }
    }

    public class ShopOrderScrapQtySPReport
    {
        public string? MRNNumber { get; set; }

        public string? ProjectNumber { get; set; }

        public string? ShopOrderNumber { get; set; }

        public string? ItemNumber { get; set; }

        public string? ItemDescription { get; set; }

        public string? MftrPartNumber { get; set; }

        public int? PartType { get; set; }

        public decimal? ScrapQty { get; set; }

        public string? Warehouse { get; set; }

        public string? Location { get; set; }

        public decimal? Qty { get; set; }

        public decimal? inventory_balanceqty { get; set; }

        public string? UOM { get; set; }

        public string? LotNumber { get; set; }

        public decimal? Max { get; set; }

        public decimal? Min { get; set; }

        public string? MaterialGroup { get; set; }
    }


}
