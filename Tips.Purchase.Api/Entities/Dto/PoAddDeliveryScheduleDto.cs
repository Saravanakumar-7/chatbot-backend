namespace Tips.Purchase.Api.Entities.DTOs
{
    public class PoAddDeliveryScheduleDto
    {
        public int Id { get; set; }
        public DateTime PODeliveryDate { get; set; }
        public decimal PODeliveryQty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class PoAddDeliveryScheduleDtoPost
    {
        public DateTime PODeliveryDate { get; set; }
        public decimal PODeliveryQty { get; set; }
        
    }
    public class PoAddDeliveryScheduleDtoUpdate
    {
        public int Id { get; set; }
        public DateTime PODeliveryDate { get; set; }
        public decimal PODeliveryQty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
