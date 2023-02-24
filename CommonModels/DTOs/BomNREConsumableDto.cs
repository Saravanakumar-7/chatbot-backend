using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class BomNREConsumableDto
    {
        public int Id { get; set; }
        public string? NREItemNumber { get; set; }

        [Precision(13, 3)]
        public decimal? NREQuantity { get; set; }
        
        [Precision(18, 3)]
        public decimal? NRECost { get; set; }
        public string? Description { get; set; }

        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class BomNREConsumablePostDto
    {
    public string? NREItemNumber { get; set; }

    [Precision(13, 3)]
    public decimal? NREQuantity { get; set; }

    [Precision(18, 3)]
    public decimal? NRECost { get; set; }

    [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
    public string? Description { get; set; }
   
}
    public class BomNREConsumableUpdateDto
    { 
        public string? NREItemNumber { get; set; }

        [Precision(13, 3)]
        public int? NREQuantity { get; set; }

        [Precision(18, 3)]
        public int? NRECost { get; set; }

        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string? Description { get; set; }

        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

}
