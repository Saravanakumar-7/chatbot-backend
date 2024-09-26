using System.ComponentModel;
using Tips.Purchase.Api.Entities.Dto;

namespace Tips.Purchase.Api.Entities.DTOs
{

    public class POBreakDownDto
    {
        public int Id { get; set; }
        public string VendorId { get; set; }

        public string PONumber { get; set; }
        public decimal TotalValue { get; set; }
        public decimal PendingValue { get; set; }
        public decimal AmountAgainstPO { get; set; }
    }
    public class POBreakDownPostDto
    {
        public string VendorId { get; set; }

        public string PONumber { get; set; }
        public decimal TotalValue { get; set; }
        public decimal PendingValue { get; set; }
        public decimal AmountAgainstPO { get; set; }
    }
    public class POBreakDownUpdateDto 
    {
        public int Id { get; set; }
        public string VendorId { get; set; }

        public string PONumber { get; set; }
        public decimal TotalValue { get; set; }
        public decimal PendingValue { get; set; }
        public decimal AmountAgainstPO { get; set; }
    }
}
