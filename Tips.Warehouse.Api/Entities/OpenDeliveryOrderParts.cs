using Entities;
using Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Tips.Warehouse.Api.Entities
{
    public class OpenDeliveryOrderParts
    {
        [Key]
        public int Id { get; set; }
        public string ItemNumber { get; set; }
        public string ODONumber { get; set; }
        public string ItemDescription { get; set; }
        public PartType ItemType { get; set; }
        public decimal UnitPrice { get; set; }
        public string UOC { get; set; }
        public string UOM { get; set; }
        public decimal StockAvailable { get; set; }
        public string Warehouse { get; set; }
        public string Location { get; set; }
        public decimal LocationStock { get; set; }
        public decimal DispatchQty { get; set; }
        public string? Remarks { get; set; }
        public string? SerialNo { get; set; }
        public bool IsActive { get; set; } = true;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public int OpenDeliveryOrderId { get; set; }

        public OpenDeliveryOrder? OpenDeliveryOrder { get; set; }
    }
}
