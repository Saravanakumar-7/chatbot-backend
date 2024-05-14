using System.ComponentModel.DataAnnotations;

namespace Tips.Warehouse.Api.Entities
{
    public class ReturnInvoice
    {
        [Key]
        public int Id { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CompanyName { get; set; }
        public string? Remarks { get; set; }
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<ReturnInvoiceItem>? ReturnInvoiceItems { get; set; }
    }
}
