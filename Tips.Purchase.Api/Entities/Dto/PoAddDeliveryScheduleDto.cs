using Microsoft.EntityFrameworkCore;

namespace Tips.Purchase.Api.Entities.DTOs
{
    public class PoAddDeliveryScheduleDto
    {
        public int Id { get; set; }
        public DateTime PODeliveryDate { get; set; }

        [Precision(13, 3)]
        public decimal PODeliveryQty { get; set; }
   
    }
    public class PoAddDeliverySchedulePostDto
    {
        public DateTime PODeliveryDate { get; set; }

        [Precision(13, 3)]
        public decimal PODeliveryQty { get; set; }
        
    }
    public class PoAddDeliveryScheduleUpdateDto
    {
        public DateTime PODeliveryDate { get; set; }

        [Precision(13, 3)]
        public decimal PODeliveryQty { get; set; }
       
    }
    public class PoAddDeliveryScheduleReportDto
    {
        public int Id { get; set; }
        public string? PONumber { get; set; }
        public string? ItemNumber { get; set; }
        public DateTime PODeliveryDate { get; set; }

        [Precision(13, 3)]
        public decimal PODeliveryQty { get; set; }

    }
}
