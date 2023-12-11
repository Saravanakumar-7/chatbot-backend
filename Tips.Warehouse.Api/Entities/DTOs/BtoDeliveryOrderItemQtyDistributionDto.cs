namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class BtoDeliveryOrderItemQtyDistributionPostDto
    {
        public string PartNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string Warehouse { get; set; }
        public string Location { get; set; }
        public decimal DistributingQty { get; set; }
    }
    public class BtoDeliveryOrderItemQtyDistributionDto
    {
        public int Id { get; set; }
        public string PartNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string Warehouse { get; set; }
        public string Location { get; set; }
        public decimal DistributingQty { get; set; }
        public int BTODeliveryOrderItemsId { get; set; }
        //public BTODeliveryOrderItems? BTODeliveryOrderItems { get; set; }

    }
}
