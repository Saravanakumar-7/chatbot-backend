using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class ProjectNumbersDto
    {
        public int Id { get; set; }
        public string? ProjectNumber { get; set; }
        

        [Precision(18, 3)]
        public decimal? ProjectQty { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }

    public class ProjectNumbersDtoPost
    {
        public string? ProjectNumber { get; set; }
     

        [Precision(18, 3)]
     
        public decimal? ProjectQty { get; set; }
       
    }

    public class ProjectNumbersDtoUpdate
    {
        
        public string? ProjectNumber { get; set; }


        [Precision(18, 3)]
        public decimal? ProjectQty { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
