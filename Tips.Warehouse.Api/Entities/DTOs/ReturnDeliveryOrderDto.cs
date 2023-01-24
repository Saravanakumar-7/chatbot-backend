namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class ReturnDeliveryOrderDto
    {
        public int Id { get; set; }
        public string? ReturnDeliveryOrderNumber { get; set; }
        public DateTime? ReturnDeliveryOrderDate { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }
        public string? PONumber { get; set; }
        public string? IssuedTo { get; set; }

        public string Unit { get; set; }
        public DateTime? CreatedOn { get; set; }

        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }


        public List<ReturnDeliveryOrderItemsDto>? ReturnDeliveryOrderItems { get; set; }
    }

    public class ReturnDeliveryOrderDtoPost
    {
        public string? ReturnDeliveryOrderNumber { get; set; }
        public DateTime? ReturnDeliveryOrderDate { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }
        public string? PONumber { get; set; }
        public string? IssuedTo { get; set; }

        public List<ReturnDeliveryOrderItemsDtoPost>? ReturnDeliveryOrderItems { get; set; }

    }

    public class ReturnDeliveryOrderDtoUpdate
    {
        public int Id { get; set; }
        public string? ReturnDeliveryOrderNumber { get; set; }
        public DateTime? ReturnDeliveryOrderDate { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }
        public string? PONumber { get; set; }
        public string? IssuedTo { get; set; }

        public string Unit { get; set; }
        public DateTime? CreatedOn { get; set; }

        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<ReturnDeliveryOrderItemsDtoUpdate>? ReturnDeliveryOrderItems { get; set; }
    }
}
