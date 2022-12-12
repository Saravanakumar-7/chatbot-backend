namespace Tips.Warehouse.Api.Entities.Dto
{
    public class BTODeliveryOrderDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAliasName { get; set; }
        public string CustomerLeadId { get; set; }
        public string BTONumber { get; set; }
        public string PONumber { get; set; }
        public string IssuedTo { get; set; }
        public DateTime DODate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<BTODeliveryOrderItemsDto>? bTODeliveryOrderItemsDto { get; set; }
    }
    public class BTODeliveryOrderDtoPost
    {
       
        public string CustomerName { get; set; }
        public string CustomerAliasName { get; set; }
        public string CustomerLeadId { get; set; }
        public string BTONumber { get; set; }
        public string PONumber { get; set; }
        public string IssuedTo { get; set; }
        public DateTime DODate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<BTODeliveryOrderItemsDtoPost>? bTODeliveryOrderItemsDtoPost { get; set; }
    }
    public class BTODeliveryOrderDtoUpdate
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAliasName { get; set; }
        public string CustomerLeadId { get; set; }
        public string BTONumber { get; set; }
        public string PONumber { get; set; }
        public string IssuedTo { get; set; }
        public DateTime DODate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<BTODeliveryOrderItemsDtoUpdate>? bTODeliveryOrderItemsDtoUpdate { get; set; }

    }

}
