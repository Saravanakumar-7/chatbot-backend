using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class ItemMasterApprovedVendorDto
    {
        public int Id { get; set; }
        public string? VendorCode { get; set; }
        public string? VendorName { get; set; }
        public string? ShareOfBusiness { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class ItemMasterApprovedVendorDtoPost
    {
        public string? VendorCode { get; set; }
        public string? VendorName { get; set; }
        public string? ShareOfBusiness { get; set; }
    }

    public class ItemMasterApprovedVendorDtoUpdate
    {
        
        public int Id { get; set; }
        public string? VendorCode { get; set; }
        public string? VendorName { get; set; }
        public string? ShareOfBusiness { get; set; }

    }

}
