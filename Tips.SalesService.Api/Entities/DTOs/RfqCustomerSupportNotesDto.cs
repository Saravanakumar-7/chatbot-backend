using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class RfqCustomerSupportNotesDto
    {
        public int Id { get; set; }
        public string? CustomerSupportCategory { get; set; }
        public string? CustomerSupportNotes { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class RfqCustomerSupportNotesPostDto
    {
        [StringLength(500, ErrorMessage = "CustomerSupportCategory can't be longer than 500 characters")]

        public string? CustomerSupportCategory { get; set; }
        
        [StringLength(500, ErrorMessage = "CustomerSupportNotes can't be longer than 500 characters")]
        public string? CustomerSupportNotes { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class RfqCustomerSupportNotesUpdateDto
    {
        //public int Id { get; set; }

        [StringLength(500, ErrorMessage = "CustomerSupportCategory can't be longer than 500 characters")]
        public string? CustomerSupportCategory { get; set; }

        [StringLength(500, ErrorMessage = "CustomerSupportNotes can't be longer than 500 characters")]
        public string? CustomerSupportNotes { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
