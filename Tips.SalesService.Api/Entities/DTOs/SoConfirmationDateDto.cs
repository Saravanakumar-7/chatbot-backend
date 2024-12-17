using Microsoft.EntityFrameworkCore;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class SoConfirmationDateDto
    {
        public DateTime ConfirmationDate { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }
    }
}
