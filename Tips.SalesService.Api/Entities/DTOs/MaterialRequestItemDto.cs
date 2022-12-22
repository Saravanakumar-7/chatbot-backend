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
        public string MRNo { get; set; }
        public string? PartNo { get; set; }
        public string? PartDescription { get; set; }
        public string? MftrPartNo { get; set; }
        public string? PartType { get; set; }
        public string? Stock { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? LocationStock { get; set; }
        public string? RequiredQty { get; set; }
        public string Unit { get; set; }
    }
    public class MaterialRequestItemDtoPost
    {
        public string MRNo { get; set; }
        public string? PartNo { get; set; }
        public string? PartDescription { get; set; }
        public string? MftrPartNo { get; set; }
        public string? PartType { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? LocationStock { get; set; }
        public string? RequiredQty { get; set; }
        public string Unit { get; set; }
    }
    public class MaterialRequestItemDtoUpdate
    {
        public int Id { get; set; }
        public string MRNo { get; set; }
        public string? PartNo { get; set; }
        public string? PartDescription { get; set; }
        public string? MftrPartNo { get; set; }
        public string? PartType { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? LocationStock { get; set; }
        public string? RequiredQty { get; set; }
        public string Unit { get; set; }
    }
}
