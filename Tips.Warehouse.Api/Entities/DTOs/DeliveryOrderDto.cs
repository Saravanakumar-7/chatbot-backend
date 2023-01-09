namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class DeliveryOrderDto
    {
        public int Id { get; set; }
        public string ProjectNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerId { get; set; }
        public string DeliveryOrderNumber { get; set; }
        public string PONumber { get; set; }
        public DateTime DODate { get; set; }

        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<DeliveryOrderItemsDto>? DeliveryOrderItemsDto { get; set; }
    }
    public class DeliveryOrderDtoPost
    {
        public string ProjectNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerId { get; set; }
        public string DeliveryOrderNumber { get; set; }
        public string PONumber { get; set; }
        public DateTime DODate { get; set; }
        
        public List<DeliveryOrderItemsDtoPost>? DeliveryOrderItemsDtoPost { get; set; }
    }
    public class DeliveryOrderDtoUpdate
    {
        public int Id { get; set; }
        public string ProjectNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerId { get; set; }
        public string DeliveryOrderNumber { get; set; }
        public string PONumber { get; set; }
        public DateTime DODate { get; set; }

        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<DeliveryOrderItemsDtoUpdate>? DeliveryOrderItemsDtoUpdate { get; set; }
    }
}
