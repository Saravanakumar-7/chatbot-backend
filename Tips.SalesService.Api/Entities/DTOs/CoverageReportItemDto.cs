namespace Tips.SalesService.Api.Entities.DTOs
{
    //create a CoverageReportItemDto class that matches the CoverageReportItem class
    public class CoverageReportItemDto
    {
        public string Id { get; set; }
        public string ItemNumber { get; set; }
        public string Description { get; set; }
        public decimal TotalRequiredQty { get; set; }
        public decimal TotalStockAvailable { get; set; }
        public decimal OpenPOQty { get; set; }
        public decimal BalanceQtyToOrder { get; set; }
        public string Status { get; set; }
    }

    //create one more Dto class with the same properties as the CoverageReportItemDto class and the name is CoverageReportItemDtoPost
    public class CoverageReportItemDtoPost
    {
    
        public string ItemNumber { get; set; }
        public string Description { get; set; }
        public decimal TotalRequiredQty { get; set; }
        public decimal TotalStockAvailable { get; set; }
        public decimal OpenPOQty { get; set; }
        public decimal BalanceQtyToOrder { get; set; }
    }

    
    public class CoverageReportItemDtoUpdate
    {
        public string Id { get; set; }
        public string ItemNumber { get; set; }
        public string Description { get; set; }
        public decimal TotalRequiredQty { get; set; }
        public decimal TotalStockAvailable { get; set; }
        public decimal OpenPOQty { get; set; }
        public decimal BalanceQtyToOrder { get; set; }
        public string Status { get; set; }
    }
}
