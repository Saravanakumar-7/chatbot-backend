using Entities;
using System.ComponentModel.DataAnnotations;

namespace Tips.Warehouse.Api.Entities
{
    public class DeliveryOrderItems
    {
        [Key]
        public int Id { get; set; }
        public string FGItemNumber { get; set; }
        public string ItemDescription { get; set; }
        public decimal ShopOrderNumber { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UOC { get; set; }
        public decimal UOM { get; set; }
        public decimal FGOrderQty { get; set; }
        public decimal FGStock { get; set; }
        public decimal DispatchQty { get; set; }
        public string SerialNo { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int DeliveryOrderId { get; set; }
        public DeliveryOrder? DeliveryOrder { get; set; }

        public List<DoSerialNumber> DoSerialNumbers { get; set; }
    }
}
