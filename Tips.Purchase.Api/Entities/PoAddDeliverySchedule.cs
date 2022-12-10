namespace Tips.Purchase.Api.Entities
{
    public class PoAddDeliverySchedule
    {
        public int Id { get; set; }
        public DateTime PoDeliveryDate { get; set; }
        public decimal PoDeliveryQuantity { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int PoItemsId { get; set; }
        public PoItems? PoItems { get; set; }
    }
}
