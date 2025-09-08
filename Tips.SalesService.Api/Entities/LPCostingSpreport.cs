namespace Tips.SalesService.Api.Entities
{
    public class LPCostingSPReport
    {
        public string? ProjectNumber { get; set; }
        public string? FGItemNumber { get; set; }
        public string? PPItemNumber { get; set; }
        public decimal? LandedPrice { get; set; }
        public decimal? MOQCost { get; set; }
    }

    public class LPCostingSummarySPReport
    {
        public string? RfqNumber { get; set; }
        public string? FGPartNumber { get; set; }
        public decimal? LandedPrice { get; set; }
        public decimal? MOQCost { get; set; }
        public double? LabourHrs { get; set; }
        public decimal? LabourHrsCost { get; set; }
        public decimal? CostOfLable { get; set; }
    }

    public class LPCostingandSummarySPReport
    {
        public List<LPCostingSPReport> LPCostingSPReport { get; set; }
        public List<LPCostingSummarySPReport> LPCostingSummarySPReport { get; set; }

    }
}
