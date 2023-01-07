using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class ItemMasterApprovedVendor
    {
        [Key]
        public int Id { get; set; }
        public string? VendorCode { get; set; }
        public string? VendorName { get; set; }
        public string? ShareOfBusiness { get; set; }

        public long ItemMasterId { get; set; }
        public ItemMaster? ItemMaster { get; set; }
    }
}
