using Microsoft.EntityFrameworkCore;

namespace Tips.Purchase.Api.Entities.DTOs
{
    public class PoAddProjectDto
    {
        public int Id { get; set; }
        public string? POProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal POProjectQty { get; set; }
 
    }
    public class PoAddProjectPostDto
    { 
        public string? POProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal POProjectQty { get; set; }

    }
    public class PoAddProjectUpdateDto
    {
        public string? POProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal POProjectQty { get; set; }
   
    }
}
