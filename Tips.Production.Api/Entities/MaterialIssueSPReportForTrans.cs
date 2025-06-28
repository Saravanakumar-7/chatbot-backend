namespace Tips.Production.Api.Entities
{
    public class MaterialIssueSPReportForTrans
    {
        public string? WorkorderNo { get; set; }
        public DateTime? WorkOrderDate { get; set; }
        public decimal? WOreleaseqty { get; set; }
        public string? ProjectType { get; set; }
        public int? ItemType { get; set; }
        public string? ProjectNumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? ItemNumberchildlevel { get; set; }
        public string? latestMPN { get; set; }
        public decimal? BOMversion { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? Description { get; set; }
        public string? UOM { get; set; }
        public decimal? RequiredQty { get; set; }
        public decimal? IssuedQty { get; set; }
        public decimal? BalanceIssueQnty { get; set; }
        public string? UOC { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? lotnumber { get; set; }
        public string? BENumber { get; set; }
        
    }
}
