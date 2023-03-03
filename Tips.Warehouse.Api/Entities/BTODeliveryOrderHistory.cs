using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tips.Warehouse.Api.Entities
{
    public class BTODeliveryOrderHistory
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }
        public string? PONumber { get; set; }
        public string? IssuedTo { get; set; }
        public DateTime? DODate { get; set; }
        public string? FGItemNumber { get; set; }
        public int? SalesOrderId { get; set; }
        public string? BTONumber { get; set; }
        public string? Description { get; set; }
        public decimal? BalanceDoQty { get; set; } 
        public decimal? UnitPrice { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }
        public decimal? FGOrderQty { get; set; }
        public decimal? OrderBalanceQty { get; set; }
        public decimal? FGStock { get; set; }
        public decimal? Discount { get; set; }
        public decimal? NetValue { get; set; }
        public decimal? DispatchQty { get; set; }
        public decimal? InvoicedQty { get; set; }
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
