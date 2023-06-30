using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class VendorRelatedVendor

    {
        [Key]
        public int Id { get; set; }

        public string? RelatedVendorName { get; set; }
        public string? RelatedVendorAlias { get; set; }
        public string? NatureOfRelationship { get; set; }
        public int VendorMasterId { get; set; }
        public VendorMaster? VendorMaster { get; set; }

    }


}
