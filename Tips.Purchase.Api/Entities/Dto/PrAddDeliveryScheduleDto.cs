using Microsoft.EntityFrameworkCore;

namespace Tips.Purchase.Api.Entities.Dto
{
    public class PrAddDeliveryScheduleDto
    {
        public int Id { get; set; }
        public DateTime PRDeliveryDate { get; set; }

        [Precision(13, 3)]
        public decimal PRDeliveryQty { get; set; }

    }

    public class PrAddDeliverySchedulePostDto
    {
        public DateTime PRDeliveryDate { get; set; }

        [Precision(13, 3)]
        public decimal PRDeliveryQty { get; set; }

    }

    public class PrAddDeliveryScheduleUpdateDto
    {
        public DateTime PRDeliveryDate { get; set; }

        [Precision(13, 3)]
        public decimal PRDeliveryQty { get; set; }
    }
}
