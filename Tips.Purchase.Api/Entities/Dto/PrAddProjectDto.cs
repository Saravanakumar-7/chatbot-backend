using Microsoft.EntityFrameworkCore;

namespace Tips.Purchase.Api.Entities.Dto
{
    public class PrAddProjectDto
    {
        public int Id { get; set; }
        public string? PRProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal PRProjectQty { get; set; }

    }
    public class PrAddProjectPostDto
    {
        public string? PRProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal PRProjectQty { get; set; }
    }

    public class PrAddProjectDtoUpdate
    {
        public string? PRProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal PRProjectQty { get; set; }

    }
}
