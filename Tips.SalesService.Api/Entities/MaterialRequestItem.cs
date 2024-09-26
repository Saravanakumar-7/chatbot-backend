using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities
{
    public class MaterialRequestItem
    {
        [Key]
        public int Id { get; set; }       
        public string? PartNumber { get; set; }
        public string? PartDescription { get; set; }
        public string? MftrPartNumber { get; set; }
        public string? PartType { get; set; }
        public string? Stock { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? LocationStock { get; set; }
        public bool IssueStatus { get; set; }
        [Precision(13,3)]
        public decimal? RequiredQty { get; set; }
       
        public int? MaterialRequestId { get; set; }
        public MaterialRequest? MaterialRequest { get; set; }


    }
}
