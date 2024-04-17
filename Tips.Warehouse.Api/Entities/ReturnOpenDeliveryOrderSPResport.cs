using Entities;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace Tips.Warehouse.Api.Entities
{
    public class ReturnOpenDeliveryOrderSPResport
    {
        public string? ODONumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? customerLeadId { get; set; }
        public DateTime? ReturnODOdate { get; set; }
        public string? IssuedTo { get; set; }
        public string? issuedby { get; set; }
        public string? KPN { get; set; }
        public string? MPN { get; set; }
        public string? Description { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? ODOType { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }
        public decimal? DispatchedStk { get; set; }
        public string? SerialNo { get; set; }
        public decimal? ReturnQty { get; set; }
        public string? Remarks { get; set; }

    }
}
