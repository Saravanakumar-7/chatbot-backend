using Microsoft.EntityFrameworkCore;

namespace Tips.Purchase.Api.Entities.Dto
{
    public class PoConfirmationDateDto
    {
        public int Id { get; set; }
        public DateTime ConfirmationDate { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }
    }
    public class PoConfirmationDatePostDto
    {
        public DateTime ConfirmationDate { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }
    }
    public class PoConfirmationDateUpdateDto
    {
        public DateTime ConfirmationDate { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }
    }
    public class PoConfirmationDateShortCloseDto
    {
        //public int Id { get; set; }
        public DateTime ConfirmationDate { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }
    }
    public class PoConfirmationDateReportDto
    {
        public int Id { get; set; }
        public DateTime ConfirmationDate { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }
    }
}
