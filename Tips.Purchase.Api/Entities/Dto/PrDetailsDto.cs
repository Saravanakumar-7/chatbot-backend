namespace Tips.Purchase.Api.Entities.Dto
{
    public class PrDetailsDto
    {
        public int Id { get; set; }
        public string? PRNumber { get; set; }
        public decimal? Qty { get; set; }
    }
    public class PrDetailsPostDto
    {
        public string? PRNumber { get; set; }
        public decimal? Qty { get; set; }
    }
    public class PrDetailsUpdateDto
    {
        public string? PRNumber { get; set; }
        public decimal? Qty { get; set; }
    }
}
