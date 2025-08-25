using Entities;
using System.Drawing.Printing;

namespace Tips.Warehouse.Api.Entities
{
    public class OpenDeliveryOrderSPReport
    {
        public string? OpenDONumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerLeadId { get; set; }
        public string? Description { get; set; }
        public string? ReasonforIssuingStock { get; set; }
        public string?  City { get; set; }
        public string? Address { get; set; }
        public string? State { get; set; }
        public string? IssuedTo { get; set; }
        public string? IssuedBy { get; set; }
        public string? KPNno { get; set; }
        public string? MPN { get; set; }
        public string? ItemDescription { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? ODOtype { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }
        public decimal? Avg_cost { get; set; }
        public decimal? Avaliablestk { get; set; }
        public decimal? OverallDispatchedQty { get; set; }
        public decimal? OverallReturnDispatchedQty { get; set; }
        public string? SerialNo { get; set; }
        public string? Remarks { get; set; }
        public DateTime? ODODate {  get; set; }
        public string? ProjectNumber { get; set; }
        public decimal? LocationwiseDispatchedQty { get; set; }

    }
}
