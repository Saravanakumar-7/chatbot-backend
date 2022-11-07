using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Tips.Model
{
    public class ItemMasterApprovedVendor
    {
        [Key]
        public int Id { get; set; }
        public string? VendorCode { get; set; }
        public string? VendorName { get; set; }
        public string? ShareOfBusiness { get; set; }

        [ForeignKey(nameof(ItemMaster))]
        public long ItemMasterId { get; set; }
        public ItemMaster? ItemMaster { get; set; }
    }
}
