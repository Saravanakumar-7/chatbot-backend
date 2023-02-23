using System.ComponentModel.DataAnnotations;

namespace Tips.Warehouse.Api.Entities
{
    public class DoSerialNumber
    {
        [Key]
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int DeliveryOrderItemsId { get; set; }
        public DeliveryOrderItems? DeliveryOrderItems { get; set; }
    }
}
