namespace Tips.Warehouse.Api.Entities
{
    public class BTOSerialNumber
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int BTODeliveryOrderItemId { get; set; }
        public BTODeliveryOrderItems? BTODeliveryOrderItems { get; set; }

    }
}
