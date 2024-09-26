using Microsoft.EntityFrameworkCore;

namespace Tips.Purchase.Api.Entities.Dto
{
    public class PrAddDeliveryScheduleDto
    {
        public int Id { get; set; }
        public DateTime PrDeliveryDate { get; set; }

        [Precision(13, 3)]
        public decimal PrDeliveryQty { get; set; }

    }

    public class PrAddDeliverySchedulePostDto
    {
        public DateTime PrDeliveryDate { get; set; }

        [Precision(13, 3)]
        public decimal PrDeliveryQty { get; set; }

    }

    public class PrAddDeliveryScheduleUpdateDto
    {
        public DateTime PrDeliveryDate { get; set; }

        [Precision(13, 3)]
        public decimal PrDeliveryQty { get; set; }
    }
    public class PrAddDeliveryScheduleReportDto
    {
        public int Id { get; set; }
        public string? PrNumber { get; set; }
        public string? ItemNumber { get; set; }
        public DateTime PrDeliveryDate { get; set; }

        [Precision(13, 3)]
        public decimal PrDeliveryQty { get; set; }

    }
}
