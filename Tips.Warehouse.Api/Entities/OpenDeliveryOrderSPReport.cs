using System.Drawing.Printing;

namespace Tips.Warehouse.Api.Entities
{
    public class OpenDeliveryOrderSPReport
    {
        public string OpenDONumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerLeadId { get; set; }
        public string? IssuedTo { get; set; }
        public string? IssuedBy { get; set; }
        public string? KPNno { get; set; }
        public string? MPN { get; set; }
        public string? ItemDescription { get; set; }
        public string? Location { get; set; }
        public string? Warehouse { get; set; }
        public string? ODOtype { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }
        public decimal? Avg_cost { get; set; }
        public decimal? Avaliablestk { get; set; }
        public decimal? Dispatchstk { get; set; }
        public decimal? ReturnQty { get; set; }
        public string? SerialNo { get; set; }
        public string? Remarks { get; set; }
        public DateTime ODODate {  get; set; }

    }
}
