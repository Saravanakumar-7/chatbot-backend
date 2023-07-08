namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class BTODeliveryOrderItemsDto
    {
        public int Id { get; set; }
        public string? FGItemNumber { get; set; }        
        public int? SalesOrderId { get; set; }
        public string? Description { get; set; }
        public decimal? BalanceDoQty { get; set; }
        public decimal InvoicedQty { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }
        public decimal? FGOrderQty { get; set; }
        public decimal? OrderBalanceQty { get; set; }
        public decimal? FGStock { get; set; }
        public decimal? Discount { get; set; }
        public decimal? NetValue { get; set; }
        public decimal DispatchQty { get; set; }
        public string? SerialNo { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
       // public List<BTOSerialNumberDto> BTOSerialNumberDto { get; set; }

    }
    public class BTODeliveryOrderItemsDtoPost
    {
        public string? FGItemNumber { get; set; }
        public int? SalesOrderId { get; set; }
        public string? Description { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }
        public decimal? FGOrderQty { get; set; }
        public decimal? OrderBalanceQty { get; set; }
        public decimal? FGStock { get; set; }
        public decimal? Discount { get; set; }
        public decimal? NetValue { get; set; }
        public decimal DispatchQty { get; set; }
        public string? SerialNo { get; set; }
        
       // public List<BTOSerialNumberDtoPost> BTOSerialNumberDtoPost { get; set; }

    }
    public class BTODeliveryOrderItemsDtoUpdate
    {
        
        public string? FGItemNumber { get; set; }
        public string? Description { get; set; }
        public int? SalesOrderId { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }
        public decimal? FGOrderQty { get; set; }
        public decimal? OrderBalanceQty { get; set; }
        public decimal? FGStock { get; set; }
        public decimal? Discount { get; set; }
        public decimal? NetValue { get; set; }
        public decimal DispatchQty { get; set; }
        public string? SerialNo { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
      //  public List<BTOSerialNumberDtoUpdate> BTOSerialNumberDtoUpdate { get; set; }

    }

    public class BtoDeliveryOrderDispatchQtyDetailsDto
    { 

        public string? FGItemNumber { get; set; }
        public int? SalesOrderId { get; set; } //salesorderno
        public decimal? DispatchQty { get; set; } 


    } 
    

}
