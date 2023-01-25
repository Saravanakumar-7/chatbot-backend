namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class DeliveryOrderItemsDto
    {
        public int Id { get; set; }
        public string FGItemNumber { get; set; }
        public string ItemDescription { get; set; }
        public decimal ShopOrderNumber { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UOC { get; set; }
        public decimal UOM { get; set; }
        public decimal FGOrderQty { get; set; }
        public decimal FGStock { get; set; }
        public decimal DispatchQty { get; set; }
        public string SerialNo { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<DoSerialNumberDto> DoSerialNumberDto { get; set; }public List<BTOSerialNumberDto> BTOSerialNumberDto { get; set; }
    }
    public class DeliveryOrderItemsDtoPost
    {
        public string FGItemNumber { get; set; }
        public string ItemDescription { get; set; }
        public decimal ShopOrderNumber { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UOC { get; set; }
        public decimal UOM { get; set; }
        public decimal FGOrderQty { get; set; }
        public decimal FGStock { get; set; }
        public decimal DispatchQty { get; set; }
        public string SerialNo { get; set; }
        public List<DoSerialNumberDtoPost> DoSerialNumberDtoPost { get; set; }
    }
    public class DeliveryOrderItemsDtoUpdate
    {
        public int Id { get; set; }
        public string FGItemNumber { get; set; }
        public string ItemDescription { get; set; }
        public decimal ShopOrderNumber { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal UOC { get; set; }
        public decimal UOM { get; set; }
        public decimal FGOrderQty { get; set; }
        public decimal FGStock { get; set; }
        public decimal DispatchQty { get; set; }
        public string SerialNo { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<DoSerialNumberDtoUpdate> DoSerialNumberDtoUpdate { get; set; }

    }
}
