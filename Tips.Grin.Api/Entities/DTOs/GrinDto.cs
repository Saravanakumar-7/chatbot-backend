using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class GrinDto
    {
        public int Id { get; set; }
        public string? GrinNumber { get; set; }

        [Required]
        public string PONumber { get; set; }

        [Required]
        public string VendorName { get; set; }

        [Required]
        public string VendorId { get; set; }

        [Required]
        public string InvoiceNumber { get; set; }

        public string? InvoiceValue { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string? AWBNumber1 { get; set; }

        public DateTime? AWBDate1 { get; set; }

        public string? AWBNumber2 { get; set; }

        public DateTime? AWBDate2 { get; set; }

        public string? BENumber { get; set; }

        public DateTime? BEDate { get; set; }

        public int? TotalInvoice { get; set; }

     
        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public List<GrinPartsDto>? GrinParts { get; set; }


    }
    public class GrinPostDto
    {
        public string? GrinNumber { get; set; }


        [Required(ErrorMessage = "PONumber is required")]
        [StringLength(100, ErrorMessage = "ItemDescription can't be longer than 100 characters")]
        public string PONumber { get; set; }

        [Required(ErrorMessage = "VendorName is required")]
        [StringLength(100, ErrorMessage = "ItemDescription can't be longer than 100 characters")]
        public string VendorName { get; set; }

        [Required(ErrorMessage = "VendorId is required")]
        [StringLength(100, ErrorMessage = "ItemDescription can't be longer than 100 characters")]

        public string VendorId { get; set; }

        [Required(ErrorMessage = "InvoiceNumber is required")]
        [StringLength(100, ErrorMessage = "ItemDescription can't be longer than 100 characters")]
        public string InvoiceNumber { get; set; }

        public string? InvoiceValue { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string? AWBNumber1 { get; set; }

        public DateTime? AWBDate1 { get; set; }

        public string? AWBNumber2 { get; set; }

        public DateTime? AWBDate2 { get; set; }

        public string? BENumber { get; set; }

        public DateTime? BEDate { get; set; }

        public int? TotalInvoice { get; set; }

    
        

        public List<GrinPartsPostDto>? GrinParts { get; set; }


    }
    public class GrinUpdateDto
    {
        public int Id { get; set; }
        public string? GrinNumber { get; set; }

        [Required(ErrorMessage = "PONumber is required")]
        public string PONumber { get; set; }

        [Required(ErrorMessage = "VendorName is required")]
        public string VendorName { get; set; }

        [Required(ErrorMessage = "VendorId is required")]
        public string VendorId { get; set; }

        [Required(ErrorMessage = "InvoiceNumber is required")]

        public string InvoiceNumber { get; set; }

        public string? InvoiceValue { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string? AWBNumber1 { get; set; }

        public DateTime? AWBDate1 { get; set; }

        public string? AWBNumber2 { get; set; }

        public DateTime? AWBDate2 { get; set; }

        public string? BENumber { get; set; }

        public DateTime? BEDate { get; set; }

        public int? TotalInvoice { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<GrinPartsUpdateDto>? GrinParts { get; set; }


    }
    public class GrinNumberListDto
    {
        public int Id { get; set; }
        public string? GrinNumber { get; set; }
        
    }
}
