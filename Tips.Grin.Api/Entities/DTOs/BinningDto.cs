using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class BinningDto
    {
        public int Id { get; set; }
        public string? GrinNumber { get; set; }
        [Required]
        public string VendorName { get; set; }

        [Required]
        public string VendorId { get; set; }

        [Required]
        public string InvoiceNumber { get; set; }

        public decimal? InvoiceValue { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string? AWBNumber1 { get; set; }

        public DateTime? AWBDate1 { get; set; }

        public string? AWBNumber2 { get; set; }

        public DateTime? AWBDate2 { get; set; }

        public string? BENumber { get; set; }

        public DateTime? BEDate { get; set; }

        public decimal? TotalInvoiceValue { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
      

        public List<BinningItemsDto>? BinningItems { get; set; }
       
    }


    public class BinningPostDto
    {

        public string? GrinNumber { get; set; }
        //public string? InvoiceNumber { get; set; }
        //public string? PONumber { get; set; }
        //public string? VendorName { get; set; }
        //public DateTime? InvoiceDate { get; set; }
        

        public List<BinningItemsPostDto>? BinningItems { get; set; }


    }

    public class BinningUpdateDto
    {

        public int Id { get; set; }
        public string? GrinNumber { get; set; }
        //public string? InvoiceNumber { get; set; }
        //public string? PONumber { get; set; }
        //public string? VendorName { get; set; }
        //public DateTime? InvoiceDate { get; set; }
        //public string Unit { get; set; }
        //public string? CreatedBy { get; set; }
        //public DateTime? CreatedOn { get; set; }
        //public string? LastModifiedBy { get; set; }
        //public DateTime? LastModifiedOn { get; set; }
        public List<BinningItemsUpdateDto>? BinningItems { get; set; }
        

    }
    public class BinningIdNameListDto
    {
        public int Id { get; set; }
        public string? GrinNumber { get; set; }
    }
    public class BinningSearchDto
    {
        public List<string>? GrinNumber { get; set; }
        public List<string>? VendorName { get; set; }
        public List<string>? VendorId { get; set; }
        public List<String>? InvoiceNumber { get; set; }
    }
}