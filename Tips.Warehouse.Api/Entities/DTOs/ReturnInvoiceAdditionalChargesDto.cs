using Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class ReturnInvoiceAdditionalChargesDto
    {
        public int Id { get; set; }
        public int SalesOrderId { get; set; }
        public string SalesOrderNumber { get; set; }

        public string AdditionalChargesLabelName { get; set; }
        public string AddtionalChargesValueType { get; set; }

        [Precision(18, 4)]
        public decimal AddtionalChargesValueAmount { get; set; }

        [Precision(18, 3)]
        public decimal IGST { get; set; }

        [Precision(18, 3)]
        public decimal CGST { get; set; }

        [Precision(18, 3)]
        public decimal UTGST { get; set; }

        [Precision(18, 3)]
        public decimal SGST { get; set; }

        [Precision(13, 3)]
        public decimal TotalValue { get; set; }
        [Precision(13, 3)]
        public decimal InvoicedValue { get; set; }
        [Precision(13, 3)]
        public decimal ReturnInvoicedValue { get; set; }
        public int SalesAdditionalChargeId { get; set; }
        public int InvoiceAdditionalChargeId { get; set; }
    }
    public class ReturnInvoiceAdditionalChargesPostDto
    {
        public int SalesOrderId { get; set; }
        public string SalesOrderNumber { get; set; }

        public string AdditionalChargesLabelName { get; set; }
        public string AddtionalChargesValueType { get; set; }

        [Precision(18, 4)]
        public decimal AddtionalChargesValueAmount { get; set; }

        [Precision(18, 3)]
        public decimal IGST { get; set; }

        [Precision(18, 3)]
        public decimal CGST { get; set; }

        [Precision(18, 3)]
        public decimal UTGST { get; set; }

        [Precision(18, 3)]
        public decimal SGST { get; set; }

        [Precision(13, 3)]
        public decimal TotalValue { get; set; }
        [Precision(13, 3)]
        public decimal InvoicedValue { get; set; }
        [Precision(13, 3)]
        public decimal ReturnInvoicedValue { get; set; }
        public int SalesAdditionalChargeId { get; set; }
        public int InvoiceAdditionalChargeId { get; set; }
        public Status InvoiceAdditionalStatus { get; set; }
    }
    public class ReturnInvoiceAdditionalChargesUpdateDto
    {
        public int SalesOrderId { get; set; }
        public string SalesOrderNumber { get; set; }

        public string AdditionalChargesLabelName { get; set; }
        public string AddtionalChargesValueType { get; set; }

        [Precision(18, 4)]
        public decimal AddtionalChargesValueAmount { get; set; }

        [Precision(18, 3)]
        public decimal IGST { get; set; }

        [Precision(18, 3)]
        public decimal CGST { get; set; }

        [Precision(18, 3)]
        public decimal UTGST { get; set; }

        [Precision(18, 3)]
        public decimal SGST { get; set; }

        [Precision(13, 3)]
        public decimal TotalValue { get; set; }
        [Precision(13, 3)]
        public decimal InvoicedValue { get; set; }
        [Precision(13, 3)]
        public decimal ReturnInvoicedValue { get; set; }
        public int SalesAdditionalChargeId { get; set; }
        public int InvoiceAdditionalChargeId { get; set; }
    }
}
