using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class GrinDto
    {
        public int Id { get; set; }

        public string? VendorName { get; set; }

        public string? VendorId { get; set; }

        public string? InvoiceNumber { get; set; }

        public string? InvoiceValue { get; set; }

        public string? PoNumber { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<GrinPartsDto>? grinParts { get; set; }

    }
    public class GrinPostDto
    {
        public string? VendorName { get; set; }

        public string? VendorId { get; set; }

        public string? InvoiceNumber { get; set; }

        public string? InvoiceValue { get; set; }

        public string? PoNumber { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<GrinPostDto>? grinParts { get; set; }

    }

    public class GrinUpdateDto
    {
        public int Id { get; set; }

        public string? VendorName { get; set; }

        public string? VendorId { get; set; }

        public string? InvoiceNumber { get; set; }

        public string? InvoiceValue { get; set; }

        public string? PoNumber { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<GrinUpdateDto>? grinParts { get; set; }

    }

}
