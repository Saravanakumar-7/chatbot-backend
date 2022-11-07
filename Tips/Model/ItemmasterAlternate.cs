using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tips.Model
{
    public class ItemmasterAlternate
    {
        [Key]
        public int Id { get; set; }
        public string? ManufacturerPartNo { get; set; }
        public string? Manufacturer { get; set; }
        [DefaultValue(false)]
        public bool IsDefault { get; set; }

        [ForeignKey(nameof(ItemMaster))]
        public long ItemMasterId { get; set; }
        public ItemMaster? ItemMaster { get; set; }
    }
}
