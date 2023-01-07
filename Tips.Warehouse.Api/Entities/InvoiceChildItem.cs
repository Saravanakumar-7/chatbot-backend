using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Tips.Warehouse.Api.Entities;

namespace Tips.Warehouse.Api.Entities
{
    public class InvoiceChildItem
    {
        [Key]
        public int Id { get; set; }
        public string? DONumber { get; set; }
        public string? FGItemNoumber { get; set; }
        public string? Qty { get; set; }
        public string? UOM { get; set; }

        [Precision(13, 3)]
        public decimal UnitPrice { get; set; }
        public string? UOC { get; set; }

        [Precision(13, 3)]
        public decimal TotalValue { get; set; }
        public string? SalesOrderID { get; set; }

        [Precision(13, 3)]
        public decimal SGST { get; set; }

        [Precision(13, 3)]
        public decimal IGST { get; set; }

        [Precision(13, 3)]
        public decimal CGST { get; set; }

        [Precision(13, 3)]
        public decimal GST { get; set; }

        [Precision(13, 3)]
        public decimal TotalValueWithTax { get; set; }
        


        public int InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }
    }
}
