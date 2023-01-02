namespace Tips.Purchase.Api.Entities.Dto
{
    public class PrAddDeliveryScheduleDto
    {
        public int Id { get; set; }
        public DateTime PrDeliveryDate { get; set; }
        public decimal PrDeliveryQty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class PrAddDeliveryScheduleDtoPost
    {
        public DateTime PrDeliveryDate { get; set; }
        public decimal PrDeliveryQty { get; set; }
       
    }

    public class PrAddDeliveryScheduleDtoUpdate
    {
        public int Id { get; set; }
        public DateTime PrDeliveryDate { get; set; }
        public decimal PrDeliveryQty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
