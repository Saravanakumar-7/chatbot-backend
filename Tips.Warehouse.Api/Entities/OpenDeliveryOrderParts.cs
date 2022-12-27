using System;

namespace Tips.Warehouse.Api.Entities
{
    public class OpenDeliveryOrderParts
    {
        public int Id { get; set; }
        public string ItemNo { get; set; }
        public string ItemType { get; set; }
        public Decimal UnitPrice { get; set; }
        public string UOC { get; set; }
        public string UOM { get; set; }
        public int StockAvailable { get; set; }
        public string Location { get; set; }
        public string LocationStock { get; set; }
        public decimal DispatchQty { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; } = true;
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public int OpenDeliveryOrderId { get; set; }

        public OpenDeliveryOrder? OpenDeliveryOrder { get; set; }
    }
}
