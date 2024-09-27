namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class ReturnInvoiceItemQtyDistributionPostDto
    {
        public string PartNumber { get; set; }
        public string LotNumber { get; set; }
        public string ProjectNumber { get; set; }
        public string Warehouse { get; set; }
        public string Location { get; set; }
        public decimal DistributingQty { get; set; }
    }
    public class ReturnInvoiceItemQtyDistributionDto
    {
        public int Id { get; set; }
        public string PartNumber { get; set; }
        public string LotNumber { get; set; }
        public string ProjectNumber { get; set; }
        public string Warehouse { get; set; }
        public string Location { get; set; }
        public decimal DistributingQty { get; set; }
        public int ReturnInvoiceItemsId { get; set; }
    }
    public class ReturnInvoiceItemQtyDistributionDoNoDto
    {
        public int Id { get; set; }
        public string PartNumber { get; set; }
        public string LotNumber { get; set; }
        public string ProjectNumber { get; set; }
        public string Warehouse { get; set; }
        public string Location { get; set; }
        public decimal DistributingQty { get; set; }
        public int ReturnInvoiceItemsId { get; set; }
    }
}
