namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class ReturnBtoDeliveryOrderDto
    {

        public int Id { get; set; }
        public string? ReturnDONumber { get; set; }
        public DateTime? ReturnDODate { get; set; }
        public string? BTONumber { get; set; }
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


        public List<ReturnBtoDeliveryOrderItemsDto>? ReturnBtoDeliveryOrderItemsDtos { get; set; }
    }

    public class ReturnBtoDeliveryOrderPostDto
    {
        //public string? ReturnDeliveryOrderNumber { get; set; }
        public DateTime? ReturnDeliveryOrderDate { get; set; }
        public string? CustomerName { get; set; }       

        public string? BTONumber { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }
        public string? PONumber { get; set; }
        public string? IssuedTo { get; set; }

        public List<ReturnBtoDeliveryOrderItemsPostDto>? ReturnBtoDeliveryOrderItemsPostDtos { get; set; }

    }

    public class ReturnBtoDeliveryOrderUpdateDto
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

        public List<ReturnBtoDeliveryOrderItemsUpdateDto>? ReturnBtoDeliveryOrderItemsUpdateDtos { get; set; }
    }
}
