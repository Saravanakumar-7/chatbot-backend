using System.ComponentModel.DataAnnotations;

namespace Tips.Warehouse.Api.Entities
{
    public class ReturnOpenDeliveryOrder
    {
        [Key]
        public int Id { get; set; }
        public string? ODONumber { get; set; }
        public DateTime? ODODate { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }
        public string? ResponsiblePerson { get; set; }
        public string? Description { get; set; }
        public string? ODOType { get; set; }
        public string? IssuedTo { get; set; }
        public string? ReasonforIssuingStock { get; set; }
        public string? CreatedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<ReturnOpenDeliveryOrderParts>? ReturnOpenDeliveryOrderParts { get; set; }
    }
}
