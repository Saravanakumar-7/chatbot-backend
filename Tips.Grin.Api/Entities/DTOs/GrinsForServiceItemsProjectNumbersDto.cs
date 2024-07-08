using Microsoft.EntityFrameworkCore;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class GrinsForServiceItemsProjectNumbersDto
    {
        public int Id { get; set; }
        public string? ProjectNumber { get; set; }
        [Precision(18, 3)]
        public decimal? ProjectQty { get; set; }        
    }
    public class GrinsForServiceItemsProjectNumbersPostDto
    {
        public string? ProjectNumber { get; set; }
        [Precision(18, 3)]
        public decimal? ProjectQty { get; set; }
    }
    public class GrinsForServiceItemsProjectNumbersUpdateDto
    {
        public string? ProjectNumber { get; set; }
        [Precision(18, 3)]
        public decimal? ProjectQty { get; set; }
    }
    }
