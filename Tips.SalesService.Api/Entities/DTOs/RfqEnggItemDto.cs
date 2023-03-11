using Entities.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class RfqEnggItemDto
    {
        public int Id { get; set; }
        public string? CustomerItemNumber { get; set; }
        public string Description { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }
        public bool ReleaseStatus { get; set; }

        [Precision(13, 3)]
        public decimal? CostingBomVersionNo { get; set; }
        public string? ItemNumber { get; set; }
       

    }
    public class RfqEnggItemDtoPost
    {
       
        public string? CustomerItemNumber { get; set; }

        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string Description { get; set; }
        
        [Precision(13,3)]
        public decimal Qty { get; set; }

        [Precision(13, 3)]
        public decimal? CostingBomVersionNo { get; set; }
        public string? ItemNumber { get; set; }
        public string? CustomFields { get; set; }

    }
    public class RfqEnggItemDtoUpdate
    {
        public string? CustomerItemNumber { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string Description { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }

        [Precision(13, 3)]
        public decimal? CostingBomVersionNo { get; set; }
        public string? ItemNumber { get; set; }
        public string? CustomFields { get; set; }


    }
}
