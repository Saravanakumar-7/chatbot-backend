using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class MaterialRequestItemDto
    {
        public int Id { get; set; }
        public string MRNumber { get; set; }
        public string? PartNumber { get; set; }
        public string? PartDescription { get; set; }
        public string? MftrPartNumber { get; set; }
        public string? PartType { get; set; }
        public string? Stock { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? LocationStock { get; set; }
        public string? RequiredQty { get; set; }
        
    }
    public class MaterialRequestItemDtoPost
    {
        [Required(ErrorMessage = "MRNumber is required")]
        public string MRNumber { get; set; }
        public string? PartNumber { get; set; }
        public string? PartDescription { get; set; }
        public string? MftrPartNumber { get; set; }
        public string? PartType { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? LocationStock { get; set; }
        public string? RequiredQty { get; set; }
       
    }
    public class MaterialRequestItemDtoUpdate
    {
        

        [Required(ErrorMessage = "MRNumber is required")]
        public string MRNumber { get; set; }
        public string? PartNumber { get; set; }
        public string? PartDescription { get; set; }
        public string? MftrPartNumber { get; set; }
        public string? PartType { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? LocationStock { get; set; }
        public string? RequiredQty { get; set; }
        
    }
}
