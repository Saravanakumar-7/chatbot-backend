using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class LocationsDto
    {
        public int Id { get; set; }
        public string WarehouseId { get; set; }

        public string Warehouse { get; set; }

        [Required]
        public string LocationName { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public bool ActiveStatus { get; set; } = true;
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class LocationsDtoPost
    {
        public string WarehouseId { get; set; }
        [Required(ErrorMessage = "Warehouse is required")]
        public string Warehouse { get; set; }

        [Required(ErrorMessage = "LocationName is required")]
        public string LocationName { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public bool ActiveStatus { get; set; } = true;
       
    }
    public class LocationsDtoUpdate
    {
        public int Id { get; set; }
        public string WarehouseId { get; set; }
        [Required(ErrorMessage = "Warehouse is required")]
        public string Warehouse { get; set; }

        [Required(ErrorMessage = "LocationName is required")]
        public string LocationName { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public bool ActiveStatus { get; set; } = true;
        [Required(ErrorMessage = "Unit is required")]
        [StringLength(100, ErrorMessage = "Unit can't be longer than 100 characters")]
        public string Unit { get; set; }

    }

    public class GetListofLocationsByWarehouseDto
    {
        public long id { get; set; }
        public string WarehouseId { get; set; }
        public string Warehouse { get; set; }
        public string LocationName { get; set; }
    }
}
