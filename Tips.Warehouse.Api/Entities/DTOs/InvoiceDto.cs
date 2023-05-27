using System.ComponentModel.DataAnnotations;
using Tips.SalesService.Api.Entities.DTOs;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class InvoiceDto
    {
        
        public int Id { get; set; }
        public string? InvoiceNumber { get; set; }        
        public string? CustomerName { get; set; }
        public string? DOType { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }
        public string? CompanyName { get; set; }
        public string? Remarks { get; set; }
        public string? SerialNumber { get; set; }
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
       // public string? InvoiceNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }

        public string? CompanyName { get; set; }
         public string? Remarks { get; set; }
        public string? SerialNumber { get; set; }


        public List<InvoiceChildItemPostDto>? InvoiceChildItems { get; set; }
        public List<InvoiceAdditionalChargesPostDto>? InvoiceAdditionalCharges { get; set; }

    }

    public class InvoiceUpdateDto
    {
        public int Id { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }
        public string? CompanyName { get; set; }
        public string? DOType { get; set; }
        public string? Remarks { get; set; }
        public string? SerialNumber { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
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

}
