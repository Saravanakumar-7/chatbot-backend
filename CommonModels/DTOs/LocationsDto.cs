using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class LocationsDto
    {
        public int Id { get; set; }
        [Required]
        public string LocationName { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public bool ActiveStatus { get; set; } = true;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class LocationsDtoPost
    {
        [Required]
        public string LocationName { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public bool ActiveStatus { get; set; } = true;
       
    }
    public class LocationsDtoUpdate
    {
        public int Id { get; set; }
        [Required]
        public string LocationName { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public bool ActiveStatus { get; set; } = true;
      
    }
}
