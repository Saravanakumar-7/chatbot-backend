namespace Tips.Grin.Api.Entities.DTOs
{
    public class KIT_GRIN_OtherChargesPostDto
    {
        public string? OtherChargesName { get; set; }
        public string? OtherChargesValue { get; set; }
    }
    public class KIT_GRIN_OtherChargesDto
    {
        public int Id { get; set; }
        public string? OtherChargesName { get; set; }
        public string? OtherChargesValue { get; set; }
        public int KIT_GRINId { get; set; }
    }
    public class KIT_GRIN_OtherChargesUpdateDto
    {
        public int Id { get; set; }
        public string? OtherChargesName { get; set; }
        public string? OtherChargesValue { get; set; }
    }
}
