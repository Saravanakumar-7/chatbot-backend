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
        public string? FGItemNumber { get; set; }

        public string? Description { get; set; }

        [Precision(13,3)]
        public decimal InvoicedQty { get; set; }
        public string? UOM { get; set; }
        public string? PartType { get; set; }
        public string? SerialNumber { get; set; }
        public int BtoDeliveryOrderPartsId { get; set; }


        [Precision(13, 3)]
        public decimal UnitPrice { get; set; }
        public string UOC { get; set; }

        [Precision(13, 3)]
        public decimal TotalValue { get; set; }
        public int? SalesOrderID { get; set; }

        [Precision(13, 3)]
        public decimal SGST { get; set; }

        [Precision(13, 3)]
        public decimal IGST { get; set; }

        [Precision(13, 3)]
        public decimal CGST { get; set; }

        [Precision(13, 3)]
        public decimal UTGST { get; set; }

        [Precision(13, 3)]
        public decimal TotalValueWithTax { get; set; }       


        public int InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }
    }
}
