using Entities.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class ForeCastCustomerSupportNotesDto
    {
        public int Id { get; set; }
        public string? CustomerSupportCategory { get; set; }
        public string? CustomerSupportNotes { get; set; }       
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class ForeCastCustomerSupportNotesPostDto
    {
        [StringLength(500, ErrorMessage = "CustomerSupportCategory can't be longer than 500 characters")]

        public string? CustomerSupportCategory { get; set; }

        [StringLength(500, ErrorMessage = "CustomerSupportNotes can't be longer than 500 characters")]
        public string? CustomerSupportNotes { get; set; }
      
    }
    public class ForeCastCustomerSupportNotesUpdateDto
    {
        public int Id { get; set; }
        [StringLength(500, ErrorMessage = "CustomerSupportCategory can't be longer than 500 characters")]

        public string? CustomerSupportCategory { get; set; }

        [StringLength(500, ErrorMessage = "CustomerSupportNotes can't be longer than 500 characters")]
        public string? CustomerSupportNotes { get; set; }      
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
