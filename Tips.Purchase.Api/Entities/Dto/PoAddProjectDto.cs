using Microsoft.EntityFrameworkCore;

namespace Tips.Purchase.Api.Entities.DTOs
{
    public class PoAddProjectDto
    {
        public int Id { get; set; }
        public string? ProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal ProjectQty { get; set; }
 
    }
    public class PoAddProjectPostDto
    {
        public string? ProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal ProjectQty { get; set; }

    }
    public class PoAddProjectUpdateDto
    {
        public string? ProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal ProjectQty { get; set; }
   
    }
}
