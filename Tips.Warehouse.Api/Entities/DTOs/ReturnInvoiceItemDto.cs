using Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class ReturnInvoiceItemDto
    {
        public int Id { get; set; }
        public string? DONumber { get; set; }
        public string? FGPartNumber { get; set; }
        public string? Description { get; set; }
        public string? CustomerItemNumber { get; set; }
        [Precision(13, 3)]
        public decimal? ActualQty { get; set; }

        [Precision(13, 3)]
        public decimal InvoicedQty { get; set; }
        public string? UOM { get; set; }

        public PartType PartType { get; set; }

        public int InvoicePartsId { get; set; }


        [Precision(13, 3)]
        public decimal UnitPrice { get; set; }
        public string UOC { get; set; }
        public decimal TotalValue { get; set; }
        public int? SalesOrderId { get; set; }
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
        [Precision(13, 3)]
        public decimal ReturnQty { get; set; }
        public string? SerialNumber { get; set; }
        public string? Remarks { get; set; }
        public List<ReturnInvoiceItemQtyDistributionDto>? QtyDistribution { get; set; }
    }

    public class ReturnInvoiceItemDtoPost
    {
        public string? DONumber { get; set; }
        public string? FGPartNumber { get; set; }
        public string? Description { get; set; }
        public string? CustomerItemNumber { get; set; }
        public int BtoDeliveryOrderPartsId { get; set; }

        [Precision(13, 3)]
        public decimal? ActualQty { get; set; }

        public string? SerialNumber { get; set; }

        [Precision(13, 3)]
        public decimal InvoicedQty { get; set; }
        public string? UOM { get; set; }
        public PartType PartType { get; set; }

        public int InvoicePartsId { get; set; }


        [Precision(13, 3)]
        public decimal UnitPrice { get; set; }
        public string  UOC { get; set; }
        public decimal TotalValue { get; set; }
        public int? SalesOrderId { get; set; }
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

        [Precision(13, 3)]
        public decimal ReturnQty { get; set; }
        public string? Remarks { get; set; }
        public List<ReturnInvoiceItemQtyDistributionPostDto>? QtyDistribution { get; set; }
    }

    public class ReturnInvoiceItemDtoUpdate
    {
       // public int Id { get; set; }
        public string? DONumber { get; set; }
        public string? FGPartNumber { get; set; }
        public string? Description { get; set; } 
        public int InvoicePartsId { get; set; }
        public string? CustomerItemNumber { get; set; }
        [Precision(13, 3)]
        public decimal InvoicedQty { get; set; }
        public string? UOM { get; set; }

        [Precision(13, 3)]
        public decimal UnitPrice { get; set; }
        public string UOC { get; set; }
        public decimal TotalValue { get; set; }
        public int? SalesOrderId { get; set; }
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

        [Precision(13, 3)]
        public decimal ReturnQty { get; set; }

        public string? SerialNumber { get; set; }

        public string? Remarks { get; set; }
    }
    public class BtoInvoiceReturnQtyDetailsDto
    {

        public string? FGPartNumber { get; set; }
        public int? SalesOrderId { get; set; }
        public decimal? ReturnQty { get; set; }

    }

}
