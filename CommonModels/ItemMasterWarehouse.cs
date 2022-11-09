using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class ItemMasterWarehouse
    {
        [Key]
        public int Id { get; set; }
        public string? WareHouse { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }

        [ForeignKey(nameof(ItemMaster))]
        public long ItemMasterId { get; set; }
        public ItemMaster? ItemMaster { get; set; }
    }
}
