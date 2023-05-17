using Entities;
using System.ComponentModel.DataAnnotations;

namespace Tips.Warehouse.Api.Entities
{
    public class ReturnOpenDeliveryOrderParts
    {
        [Key]
        public int Id { get; set; }
        public string ItemNumber { get; set; }
        public string Description { get; set; }
        public PartTypes ItemType { get; set; }
        public decimal UnitPrice { get; set; }
        public string UOC { get; set; }
        public string UOM { get; set; }
        public decimal StockAvailable { get; set; }
        public string Location { get; set; }
        public decimal LocationStock { get; set; }
        public decimal DispatchQty { get; set; }
        public string Remarks { get; set; }
        public string? SerialNo { get; set; }

        public int ReturnOpenDeliveryOrderId { get; set; }
        public ReturnOpenDeliveryOrder? ReturnOpenDeliveryOrder { get; set; }
    }
}
