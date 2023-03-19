using System.ComponentModel.DataAnnotations;

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

        public List<InvoiceChildItemDto>? InvoiceChildItems { get; set; }

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
        public List<InvoiceChildItemUpdateDto>? InvoiceChildItems { get; set; }
    }

    public class InvoiceIdNameList
    {
        public int Id { get; set; }
        public string? InvoiceNumber { get; set; }
    }

}
