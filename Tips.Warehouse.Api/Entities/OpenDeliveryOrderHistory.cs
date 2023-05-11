using Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tips.Warehouse.Api.Entities
{
    public class OpenDeliveryOrderHistory
    {
        public int Id { get; set; }
        public string? ODONumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }
        public string? Description { get; set; }
        public string? ResponsiblePerson { get; set; }
        public string? ReasonForIssuingStock { get; set; }
        public string? IssuedTo { get; set; }
        public string? ODOType { get; set; }
        public DateTime? ODODate { get; set; }
        public string? ItemNumber { get; set; } 
        public string? ItemDescription { get; set; }
        public PartTypes ItemType { get; set; } 
        public decimal? UnitPrice { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }
        public decimal? StockAvailable { get; set; }
        public string? Location { get; set; }
        public decimal? LocationStock { get; set; } 
        public decimal? DispatchQty { get; set; } 
        public string? SerialNo { get; set; }
        public string? Unit { get; set; }
        public string? Remark { get; set; }
        public string? UniqeId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
