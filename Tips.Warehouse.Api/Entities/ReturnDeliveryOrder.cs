namespace Tips.Warehouse.Api.Entities
{
    public class ReturnDeliveryOrder
    {
        public int Id { get; set; }
        public string? ReturnDONumber { get; set; }
        public DateTime? ReturnDODate { get; set; }
        public string? CustomerName { get; set; }
         public string? CustomerId { get; set; }
        public string? PONumber { get; set; }
        public string? IssuedTo { get; set; }

        public string Unit { get; set; }
        public DateTime? CreatedOn { get; set; }

        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<ReturnDeliveryOrderItems>? ReturnDeliveryOrderItems { get; set; }
    }
}
