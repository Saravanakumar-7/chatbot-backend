using Entities;
using System.ComponentModel.DataAnnotations;

namespace Tips.Warehouse.Api.Entities
{
    public class BTODeliveryOrderItems
    {
        [Key]
        public int Id { get; set; }
        public string FGItemNumber { get; set; }
        public string Description { get; set; } 
        public decimal UnitPrice { get; set; }
        public decimal UOC { get; set; }
        public decimal UOM { get; set; }
        public decimal FGOrderQty { get; set; }
        public decimal OrderBalanceQty { get; set; }
        public decimal FGStock { get; set; }
        public decimal Discount { get; set; }
        public decimal NetValue { get; set; }
        public decimal DispatchQty { get; set; }
        public string SerialNo { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int BTODeliveryOrderId { get; set; }
        public BTODeliveryOrder? BTODeliveryOrder { get; set; }
        public List<BTOSerialNumber> BTOSerialNumbers { get; set; }   
    }
}
