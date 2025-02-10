using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class PrioritizeDTO
    {
        public int Id { get; set; }

        public string PrioritizeName { get; set; }

        public string Description { get; set; }

        public decimal FromRange { get; set; }

        public decimal ToRange { get; set; } = 0;

        public string Unit { get; set; }

        public string Remarks { get; set; }

        public bool ActiveStatus { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? LastModifiedBy { get; set; }

        public DateTime? LastModifiedOn { get; set; }

    }

    public class PrioritizeDtoPost
    {
        [Required]

        public string PrioritizeName { get; set; }

        public string Description { get; set; }

        public decimal FromRange { get; set; }

        public double ToRange { get; set; }

        public string Remarks { get; set; }

        public bool ActiveStatus { get; set; } = true;

    }

    public class PrioritizeDtoUpdate
    {
        public int Id { get; set;}
        [Required]
        public string PrioritizeName { get; set; }

        public string Description { get; set; }

        public decimal FromRange { get; set; }

        public double ToRange { get; set; }

        public string Remarks { get; set; }

        public bool ActiveStatus { get; set; } = true;

        [Required(ErrorMessage ="unit is required")]
        [StringLength(100,ErrorMessage = "Unit can't be longer than 100 characters")]
        public string Unit { get; set; }

    }

}
