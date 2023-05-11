namespace Tips.SalesService.Api.Entities.DTOs
{
    public class SOBreakDownDto
    {
        public int Id { get; set; }
        public string SalesOrderNumber { get; set; }
        public decimal TotalValue { get; set; }
        public decimal PendingValue { get; set; }
        public decimal AmountAgainstSO { get; set; }
    }
    public class SOBreakDownPostDto
    {
        public string SalesOrderNumber { get; set; }
        public decimal TotalValue { get; set; }
        public decimal PendingValue { get; set; }
        public decimal AmountAgainstSO { get; set; }
    }
    public class SOBreakDownUpdateDto
    {
        public int Id { get; set; }
        public string SalesOrderNumber { get; set; }
        public decimal TotalValue { get; set; }
        public decimal PendingValue { get; set; }
        public decimal AmountAgainstSO { get; set; }
    }
}
