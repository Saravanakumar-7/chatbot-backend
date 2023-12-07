namespace Tips.Warehouse.Api.Entities
{
    public class ReturnOpenDeliveryOrderItemQtyDistribution
    {
        public int Id { get; set; }
        public string PartNumber { get; set; }
        public string ProjectNumber { get; set; }
        public string Warehouse { get; set; }
        public string Location { get; set; }
        public decimal DistributingQty { get; set; }
        public int ReturnOpenDeliveryOrderItemsId { get; set; }
        public ReturnOpenDeliveryOrderParts? ReturnOpenDeliveryOrderItems { get; set; }
    }
}
