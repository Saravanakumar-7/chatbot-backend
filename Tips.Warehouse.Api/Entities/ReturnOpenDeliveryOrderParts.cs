using Entities;
using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Warehouse.Api.Entities
{
    public class ReturnOpenDeliveryOrderParts
    {
        [Key]
        public int Id { get; set; }
        public string ItemNumber { get; set; }
        public string? ODONumber { get; set; }
        public string Description { get; set; }
        public PartType ItemType { get; set; }
        [Precision(13, 3)]
        public decimal UnitPrice { get; set; }
        [Precision(13, 3)]
        public decimal ReturnQty { get; set; }
        [Precision(13, 3)]
        public decimal AlreadyReturnQty { get; set; }
        public string UOC { get; set; }
        public string UOM { get; set; }
        [Precision(13, 3)]
        public decimal StockAvailable { get; set; }
        public string Warehouse { get; set; }
        public string Location { get; set; }
        [Precision(13, 3)]
        public decimal LocationStock { get; set; }
        [Precision(13, 3)]
        public decimal DispatchQty { get; set; }
        public string Remarks { get; set; }
        public string? SerialNo { get; set; }

        public int ReturnOpenDeliveryOrderId { get; set; }
        public ReturnOpenDeliveryOrder? ReturnOpenDeliveryOrder { get; set; }
    }
}
