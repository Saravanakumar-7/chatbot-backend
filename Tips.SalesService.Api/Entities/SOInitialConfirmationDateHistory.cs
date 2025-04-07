namespace Tips.SalesService.Api.Entities
{
    public class SOInitialConfirmationDateHistory
    {
        public int Id {  get; set; }
        public int SalesOrderId { get; set; }
        public int SalesOrderItemsId { get; set; }
        public string? SalesOrderNumber { get; set; }
        public DateTime InitialConfirmationDate { get; set; }
        public decimal InitialQty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
