using System.ComponentModel.DataAnnotations;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class InvoiceDto
    {
        
        public int Id { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? DOType { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }
        public string? CompanyName { get; set; }
        public string? Remarks { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<InvoiceChildItemDto>? invoiceChildItems { get; set; }
        public List<InvoiceAdditionalChargesDto>? InvoiceAdditionalCharges { get; set; }

    }

    public class InvoicePostDto 
    {
        public string? DOType { get; set; }
        public string? ProjectNumber { get; set; }
        // public string? InvoiceNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }

        public string? CompanyName { get; set; }
         public string? Remarks { get; set; }


        public List<InvoiceChildItemPostDto>? InvoiceChildItems { get; set; }
        public List<InvoiceAdditionalChargesPostDto>? InvoiceAdditionalCharges { get; set; }

    }

    public class InvoiceUpdateDto
    {
        public int Id { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }
        public string? CompanyName { get; set; }
        public string? DOType { get; set; }
        public string? Remarks { get; set; }
        public string? Unit { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<InvoiceAdditionalCharges>? InvoiceAdditionalCharges { get; set; }

        public List<InvoiceAdditionalChargesUpdateDto>? InvoiceChildItems { get; set; }
    }

    public class InvoiceIdNameList
    {
        public int Id { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? CustomerName { get; set; }
    }
    public class InvoiceSearchDto
    {
        public List<string> InvoiceNumber { get; set; }
        public List<string> CustomerName { get; set; }
        public List<string> CompanyName { get; set; }
 
    }

    public class SalesOrderAdditionalChargesUpdate
    {
        public int SalesOrderId { get; set; }
        public decimal InvoicedValue { get; set; }
        public int SalesAdditionalChargeId { get; set; }

    }
    public class InvoiceReportDto
    {

        public int Id { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? DOType { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }
        public string? CompanyName { get; set; }
        public string? Remarks { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<InvoiceChildItemReportDto>? invoiceChildItems { get; set; }
        public List<InvoiceAdditionalChargesReportDto>? InvoiceAdditionalCharges { get; set; }

    }
    public class InvoiceSPReportDTO
    {
        public string? InvoiceNumber { get; set; }
        public DateTime? Invoice_Date { get; set; }
        public string? DONumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? LeadId { get; set; }
        public string? SalesOrderNumber { get; set; }
        public decimal? RevisionNumber { get; set; }
        public DateTime? DODate { get; set; }
        public string? ProductType { get; set; }
        public string? TypeOfSolution { get; set; }
        public string? OrderType { get; set; }
        public string? Location { get; set; }
        public string? Warehouse { get; set; }
        public string? IssuedTo { get; set; }
        public string? IssuedBy { get; set; }
        public string? KPN { get; set; }
        public string? MPN { get; set; }
        public string? Description { get; set; }
        public decimal? AvaliableQty { get; set; }
        public decimal? OrderQty { get; set; }
        public decimal? DispatchQty { get; set; }
        public string? SerialNumber { get; set; }
        public string? Remarks { get; set; }

    }
    public class InvoiceSPReportWithParamDTO
    {
        public string? InvoiceNumber { get; set; }
        public string? DONumber { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? SalesOrderNumber { get; set; }
        public string? Location { get; set; }
        public string? Warehouse { get; set; }
        public string? KPN { get; set; }
        public string? MPN { get; set; }
        public string? IssuedTo { get; set; }
    }
}
