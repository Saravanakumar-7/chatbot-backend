using Microsoft.EntityFrameworkCore;

namespace Tips.Purchase.Api.Entities.DTOs
{
    public class PoAddProjectDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? PONumber { get; set; }
        public string? ProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal ProjectQty { get; set; }
 
    }
    public class PoAddProjectPostDto
    {
        public string? ItemNumber { get; set; }
        public string? PONumber { get; set; }
        public string? ProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal ProjectQty { get; set; }

    }
    public class PoAddProjectUpdateDto
    {
        public string? ItemNumber { get; set; }
        public string? PONumber { get; set; }
        public string? ProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal ProjectQty { get; set; }
   
    }
}
