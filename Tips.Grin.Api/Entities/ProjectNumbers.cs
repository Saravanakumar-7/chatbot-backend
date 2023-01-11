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

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public int GrinPartsId { get; set; }
        public GrinParts? GrinParts { get; set; }





    }
}
