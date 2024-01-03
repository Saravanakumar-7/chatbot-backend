using Microsoft.EntityFrameworkCore;

namespace Tips.Warehouse.Api.Entities
{
    public class ReturnDOSPReport
    {
        public string? DoNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? LeadId { get; set; }
        public string? SalesOrderNumber { get; set; }
        [Precision(13, 1)]
        public decimal? RevisionNumber { get; set; }
        public DateTime? DODate { get; set; }
        public string? ProductType { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? OrderType { get; set; }
        public string? Location { get; set; }
        public string? Warehouse { get; set; }
        //public string? IssuedTo { get; set; }
        public string? IssuedBy { get; set; }
        public string? KPN { get; set; }
        //  public string? MPN { get; set; }
        public string? Description { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }
        public decimal? BalanceDoQty { get; set; }
        public decimal? OrderQnty { get; set; }
        public decimal? ReturnedQty { get; set; }
        public string? SerialNo { get; set; }
        public string? Remark { get; set; }
    }
}
