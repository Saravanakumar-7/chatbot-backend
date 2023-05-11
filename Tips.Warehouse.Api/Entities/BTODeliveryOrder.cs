using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tips.Warehouse.Api.Entities
{
    public class BTODeliveryOrder
    {
        [Key]
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }              
        public string SalesOrderNumber { get; set; }

        [Precision(13, 1)]
        public int? SalesOrderRevisionNumber { get; set; }
        public string? CustomerId { get; set; }
        public string? BTONumber { get; set; }        
        public int? SalesOrderId { get; set; }
        public string? PONumber { get; set; }
        public string? IssuedTo { get; set; }
        public DateTime? DODate { get; set; }

        public string? Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<BTODeliveryOrderItems>? bTODeliveryOrderItems { get; set; }

    }
}
