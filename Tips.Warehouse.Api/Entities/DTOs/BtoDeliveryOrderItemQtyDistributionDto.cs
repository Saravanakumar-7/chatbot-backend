namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class BtoDeliveryOrderItemQtyDistributionPostDto
    {
        public string PartNumber { get; set; }
        public string ProjectNumber { get; set; }
        public string Warehouse { get; set; }
        public string Location { get; set; }
        public decimal DistributingQty { get; set; }
    }
}
