using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities
{
    public class ProjectNumbers
    {
        [Key]
        public int Id { get; set; }
        public string? ProjectNumber { get; set; } 
        [Precision(18, 3)]
        public decimal? ProjectQty { get; set; }         
        //[Precision(18, 3)]
        //public decimal RejectReturnQty { get; set; } = 0;
        public int GrinPartsId { get; set; }
        public GrinParts? GrinParts { get; set; }

    }
}
