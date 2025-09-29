namespace Tips.SalesService.Api.Entities
{
    public class LPCostingSPReport
    {
        public string? ProjectNumber { get; set; }
        public string? PPItemNumber { get; set; }
        public decimal? LandingPrice { get; set; }
        public decimal? MOQCost { get; set; }
    }

    public class LPCostingforFGSPReport
    {
        public string? ProjectNumber { get; set; }
        public string? FgItemnumber { get; set; }
        public decimal? LandingPrice { get; set; }
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

    public class CommoditySourcingSpReport
    {
        public string? ProjectNumber { get; set; }
        public string? Commodity { get; set; }
        public decimal? TotalLandingPrice { get; set; }
        public decimal? TotalMoqCost { get; set; }
    }

    public class VendorSourcingSpReport
    {
        public string? ProjectNumber { get; set; }
        public string? Vendor { get; set; }
        public decimal? TotalLandingPrice { get; set; }
        public decimal? TotalMoqCost { get; set; }
    }





    public class LPCostingandSummarySPReport
    {
        public List<LPCostingSPReport> LPCostingSPReport { get; set; }
        public List<LPCostingforFGSPReport> LPCostingforFGSPReport { get; set; }
        public List<LPCostingSummarySPReport> LPCostingSummarySPReport { get; set; }
        public List<CommoditySourcingSpReport> CommoditySourcingSPReport { get; set; }
        public List<VendorSourcingSpReport> VendorSourcingSpReport { get; set; }

    }
}
