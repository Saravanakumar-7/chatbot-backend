using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class InvoiceChildItemDto
    {
       
        public int Id { get; set; }
        public string? DONumber { get; set; }
        public string? FGItemNumber { get; set; }        

        [Precision(13, 3)]
        public decimal? Qty { get; set; }
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
    }

    public class InvoiceChildItemPostDto 
    {
        public string? DONumber { get; set; }
        public string? FGItemNumber { get; set; }

        [Precision(13, 3)]
        public decimal? Qty { get; set; }
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


    }
    public class InvoiceChildItemUpdateDto
    {
        
        public string? DONumber { get; set; }
        public string? FGItemNumber { get; set; }

        [Precision(13, 3)]
        public decimal? Qty { get; set; }
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


    }    
}
