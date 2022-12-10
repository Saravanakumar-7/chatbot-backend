namespace Tips.Warehouse.Api.Entities.Dto
{
    public class DeliveryOrderItemsDto
    {
        public int Id { get; set; }
        public string FGItemNo { get; set; }
        public string ItemDescription { get; set; }
        public decimal ShopOrderNo { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UOC { get; set; }
        public decimal UOM { get; set; }
        public decimal FGOrderQty { get; set; }
        public decimal FGStock { get; set; }
        public decimal DispatchQty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class DeliveryOrderItemsDtoPost
    {
        public string FGItemNo { get; set; }
        public string ItemDescription { get; set; }
        public decimal ShopOrderNo { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UOC { get; set; }
        public decimal UOM { get; set; }
        public decimal FGOrderQty { get; set; }
        public decimal FGStock { get; set; }
        public decimal DispatchQty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class DeliveryOrderItemsDtoUpdate
    {
        public int Id { get; set; }
        public string FGItemNo { get; set; }
        public string ItemDescription { get; set; }
        public decimal ShopOrderNo { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UOC { get; set; }
        public decimal UOM { get; set; }
        public decimal FGOrderQty { get; set; }
        public decimal FGStock { get; set; }
        public decimal DispatchQty { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
