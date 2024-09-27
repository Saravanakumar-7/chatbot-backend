namespace Tips.Warehouse.Api.Entities
{
    public class ReturnInvoiceItemQtyDistribution
    {
        public int Id { get; set; }
        public string PartNumber { get; set; }
        public string LotNumber { get; set; }
        public string ProjectNumber { get; set; }
        public string Warehouse { get; set; }
        public string Location { get; set; }
        public decimal DistributingQty { get; set; }
        public int ReturnInvoiceItemsId { get; set; }
        public ReturnInvoiceItem? ReturnInvoiceItems { get; set; }
    }
}
