using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class QuoteTermsDto
    {
        public int Id { get; set; }
        [Required]
        public string QuoteTermsName { get; set; }
        public string Description { get; set; }
        public bool ActiveStatus { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class QuoteTermsDtoPost
    {
        [Required]
        public string QuoteTermsName { get; set; }
        public string Description { get; set; }
        public bool ActiveStatus { get; set; }
    }
    public class QuoteTermsDtoUpdate
    {
        public int Id { get; set; }
        [Required]
        public string QuoteTermsName { get; set; }
        public string Description { get; set; }
        public bool ActiveStatus { get; set; }
    }
}