using Entities;
using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tips.Warehouse.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Entities
{
    public class BTODeliveryOrderItems
    {
        [Key]
        public int Id { get; set; }
        public string? FGItemNumber { get; set; }       
        public int? SalesOrderId { get; set; }
        public string? BTONumber { get; set; }
        public string? Description { get; set; }
        public decimal? BalanceDoQty { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }
        public string? CustomerItemNumber { get; set; }
        public PartType? PartType { get; set; }

        public decimal? FGOrderQty { get; set; }
        public decimal? OrderBalanceQty { get; set; }
         
        public decimal? FGStock { get; set; }
        public decimal? Discount { get; set; }
        public decimal? NetValue { get; set; }
        public decimal? DispatchQty { get; set; }
        public decimal InvoicedQty { get; set; }       
        public string? SerialNo { get; set; }
        //public int MyProperty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int BTODeliveryOrderId { get; set; }
        public Status DoStatus { get; set; } = Status.Open;
        public List<BtoDeliveryOrderItemQtyDistribution>? QtyDistribution { get; set; }
        public BTODeliveryOrder? BTODeliveryOrder { get; set; }
       // public List<BtoDeliveryOrderItemQtyDistribution>? QtyDistridution { get; set; }

        // public List<BTOSerialNumber> BTOSerialNumbers { get; set; }   
    }
}
