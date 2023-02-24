using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ItemmasterAlternate
    {
        [Key]
        public int Id { get; set; }
        public string? ManufacturerPartNo { get; set; }
        public string? Manufacturer { get; set; }

        [DefaultValue(false)]
        public bool AlternateSource { get; set; }

        [DefaultValue(false)]
        public bool IsDefault { get; set; }

        public long ItemMasterId { get; set; }
        public ItemMaster? ItemMaster { get; set; }
    }
}
