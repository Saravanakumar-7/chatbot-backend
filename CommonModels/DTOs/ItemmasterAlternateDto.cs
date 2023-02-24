using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class ItemmasterAlternateDto
    {
        [Key]
        public int Id { get; set; }
        public string? ManufacturerPartNo { get; set; }
        public string? Manufacturer { get; set; }

        [DefaultValue(false)]
        public bool AlternateSource { get; set; }

        [DefaultValue(false)]
        public bool IsDefault { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class ItemmasterAlternateDtoPost

    {
        public string? ManufacturerPartNo { get; set; }

        [DefaultValue(false)]
        public bool AlternateSource { get; set; }

        public string? Manufacturer { get; set; }
        [DefaultValue(false)]
        public bool IsDefault { get; set; }
       
    }

    public class ItemmasterAlternateDtoUpdate
    {
        [Key]
        public int Id { get; set; }
        public string? ManufacturerPartNo { get; set; }

        [DefaultValue(false)]
        public bool AlternateSource { get; set; }
        public string? Manufacturer { get; set; }
        [DefaultValue(false)]
        public bool IsDefault { get; set; }
          
    }
}
