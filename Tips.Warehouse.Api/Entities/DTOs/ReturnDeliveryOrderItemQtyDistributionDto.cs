namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class ReturnDeliveryOrderItemQtyDistributionPostDto
    {
        public string PartNumber { get; set; }
        public string LotNumber { get; set; }
        public string ProjectNumber { get; set; }
        public string Warehouse { get; set; }
        public string Location { get; set; }
        public decimal DistributingQty { get; set; }
    }
    public class ReturnDeliveryOrderItemQtyDistributionDto
    {
        public int Id { get; set; }
        public string PartNumber { get; set; }
        public string LotNumber { get; set; }
        public string ProjectNumber { get; set; }
        public string Warehouse { get; set; }
        public string Location { get; set; }
        public decimal DistributingQty { get; set; }
        public int ReturnDeliveryOrderItemsId { get; set; }
    }
}
