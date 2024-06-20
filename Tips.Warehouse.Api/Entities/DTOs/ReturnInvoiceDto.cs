namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class ReturnInvoiceDto
    {
        public int Id { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CompanyName { get; set; }
        public string? Remarks { get; set; }
        public string? ReturnPdfPrint { get; set; }
        public decimal? ReturnInvoiceValue { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<ReturnInvoiceItemDto>? ReturnInvoiceItems { get; set; }
    }

    public class ReturnInvoiceDtoPost
    {
        public string? InvoiceNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CompanyName { get; set; }
        public string? Remarks { get; set; }
        public decimal? ReturnInvoiceValue { get; set; }
        public string? ReturnPdfPrint { get; set; }

        public List<ReturnInvoiceItemDtoPost>? ReturnInvoiceItems { get; set; }
    }

   public class ReturnInvoiceDtoUpdate
    {
        public int Id { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CompanyName { get; set; }
        public string? Remarks { get; set; }
        public decimal? ReturnInvoiceValue { get; set; }
        public string? ReturnPdfPrint { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<ReturnInvoiceItemDtoUpdate>? ReturnInvoiceItems { get; set; }
    }
    public class ReturnInvoiceNumberListDto
    {
        public int Id { get; set; }
        public string? ReturnInvoiceNumber { get; set; }
    }
    public class ReturnInvoiceDoNoDto
    {
        public int Id { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? DONumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CompanyName { get; set; }
        public string? Remarks { get; set; }
        public string? ReturnPdfPrint { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<ReturnInvoiceItemDoNoDto>? ReturnInvoiceItems { get; set; }
    }
}
