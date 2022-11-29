using System.ComponentModel.DataAnnotations;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class InvoiceDto
    {
        [Key]
        public int Id { get; set; }
        public string? InvoiceNo { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CompanyName { get; set; }
        string? Remarks { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<InvoiceChildItemDto>? InvoiceChildItems { get; set; }

    }

    public class InvoicePostDto 
    {
        public string? InvoiceNo { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CompanyName { get; set; }
        string? Remarks { get; set; }
        public List<InvoiceChildItemPostDto>? InvoiceChildItems { get; set; }
    }

    public class InvoiceUpdateDto
    {
        public int Id { get; set; }
        public string? InvoiceNo { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CompanyName { get; set; }
        string? Remarks { get; set; }
        public List<InvoiceChildItemUpdateDto>? InvoiceChildItems { get; set; }
    }

}
