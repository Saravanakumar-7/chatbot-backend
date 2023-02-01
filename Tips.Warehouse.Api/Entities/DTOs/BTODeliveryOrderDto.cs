using System.ComponentModel;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class BTODeliveryOrderDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAliasName { get; set; }
        public string? ReturnQty { get; set; }
        public string CustomerLeadId { get; set; }
        public string BTONumber { get; set; }       
        public int SalesOrderId { get; set; }
        public string PONumber { get; set; }
        public string IssuedTo { get; set; }
        public DateTime DODate { get; set; }

        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<BTODeliveryOrderItemsDto>? BTODeliveryOrderItemsDto { get; set; }
    }
    public class BTODeliveryOrderDtoPost
    {

        public string CustomerName { get; set; }
        public string CustomerAliasName { get; set; }
        public string? ReturnQty { get; set; }

        public string CustomerLeadId { get; set; }       
        public int SalesOrderId { get; set; }
        public string PONumber { get; set; }
        public string IssuedTo { get; set; }
        public DateTime DODate { get; set; }
     
        public List<BTODeliveryOrderItemsDtoPost>? BTODeliveryOrderItemsDtoPost { get; set; }
    }
    public class BTODeliveryOrderDtoUpdate
    {

        public string CustomerName { get; set; }
        public string CustomerAliasName { get; set; }
        public string? ReturnQty { get; set; }

        public string CustomerLeadId { get; set; }
        public string PONumber { get; set; }
        public string IssuedTo { get; set; }
        public DateTime DODate { get; set; }

        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<BTODeliveryOrderItemsDtoUpdate>? BTODeliveryOrderItemsDtoUpdate { get; set; }

    }
    public class ListofBtoDeliveryOrderDetails
    {
        public int BtoDeliveryOrderId { get; set; }
        public string BTONumber { get; set; }
    }

}
