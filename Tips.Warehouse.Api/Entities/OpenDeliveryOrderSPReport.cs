namespace Tips.Warehouse.Api.Entities
{
    public class OpenDeliveryOrderSPReport
    {
        public string OpenDoNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? LeadId { get; set; }
        public string? IssuedTo { get; set; }
        public string? IssuedBy { get; set; }
        public string? KPNNo { get; set; }
        public string? MPN { get; set; }
        public string? ItemDescription { get; set; }
        public string? Location { get; set; }
        public string? Warehouse { get; set; }
        public string? ODOType { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }
        public decimal? AvaliableQty { get; set; }
        public decimal? DispatchQty { get; set; }
        public string? SerialNo { get; set; }
        public string? Remarks { get; set; }
    }
}
