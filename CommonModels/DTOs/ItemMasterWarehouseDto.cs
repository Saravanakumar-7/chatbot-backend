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
    public class ItemMasterWarehouseDto
    {
        [Key]
        public int Id { get; set; }
        public string? WareHouse { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class ItemMasterWarehouseDtoPost
    {
        public string? WareHouse { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }
     
    }
    public class ItemMasterWarehouseDtoUpdate
    {
        [Key]
        public int Id { get; set; }
        public string? WareHouse { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }

    }
}
