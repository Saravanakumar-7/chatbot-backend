using Microsoft.EntityFrameworkCore;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class ReturnInvoiceItemDto
    {
        public int Id { get; set; }
        public string? DONumber { get; set; }
        public string? FGPartNumber { get; set; }
        public string? Description { get; set; }

        [Precision(13, 3)]
        public decimal? ActualQty { get; set; }

        [Precision(13, 3)]
        public decimal? Qty { get; set; }
        public string? UOM { get; set; }

        [Precision(13, 3)]
        public decimal? UnitPrice { get; set; }
        public string? UnitOfCurrency { get; set; }
        public string? TotalValue { get; set; }
        public string? SalesOrderId { get; set; }
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

        [Precision(13, 3)]
        public decimal? ReturnQty { get; set; }

        public string? Remarks { get; set; }
    }

    public class ReturnInvoiceItemDtoPost
    {
        public string? DONumber { get; set; }
        public string? FGPartNumber { get; set; }
        public string? Description { get; set; }

        [Precision(13, 3)]
        public decimal? ActualQty { get; set; }

        [Precision(13, 3)]
        public decimal? Qty { get; set; }
        public string? UOM { get; set; }

        [Precision(13, 3)]
        public decimal? UnitPrice { get; set; }
        public string? UnitOfCurrency { get; set; }
        public string? TotalValue { get; set; }
        public string? SalesOrderId { get; set; }
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

        [Precision(13, 3)]
        public decimal? ReturnQty { get; set; }

        public string? Remarks { get; set; }
    }

    public class ReturnInvoiceItemDtoUpdate
    {
       // public int Id { get; set; }
        public string? DONumber { get; set; }
        public string? FGPartNumber { get; set; }
        public string? Description { get; set; }

        [Precision(13, 3)]
        public decimal? ActualQty { get; set; }

        [Precision(13, 3)]
        public decimal? Qty { get; set; }
        public string? UOM { get; set; }

        [Precision(13, 3)]
        public decimal? UnitPrice { get; set; }
        public string? UnitOfCurrency { get; set; }
        public string? TotalValue { get; set; }
        public string? SalesOrderId { get; set; }
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

        [Precision(13, 3)]
        public decimal? ReturnQty { get; set; }

        public string? Remarks { get; set; }
    }
    public class BtoInvoiceReturnQtyDetailsDto
    {

        public string? FGPartNumber { get; set; }
        public int? SalesOrderId { get; set; }
        public decimal? ReturnQty { get; set; }

    }

}
