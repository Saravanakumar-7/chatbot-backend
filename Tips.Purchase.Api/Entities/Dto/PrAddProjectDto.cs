using Microsoft.EntityFrameworkCore;

namespace Tips.Purchase.Api.Entities.Dto
{
    public class PrAddProjectDto
    {
        public int Id { get; set; }
        public string? ItemNumber { get; set; }
        public string? PrNumber { get; set; }
        public string? ProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal ProjectQty { get; set; }

    }
    public class PrAddProjectPostDto
    {
        public string? ItemNumber { get; set; }
        public string? PrNumber { get; set; }
        public string? ProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal ProjectQty { get; set; }
    }

    public class PrAddProjectDtoUpdate
    {
        public string? ItemNumber { get; set; }
        public string? PrNumber { get; set; }
        public string? ProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal ProjectQty { get; set; }

    }
}
