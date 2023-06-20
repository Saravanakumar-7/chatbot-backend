using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace Tips.Warehouse.Api.Entities
{
    public class OpenDeliveryOrder
    {
        [Key]
        public int Id { get; set; }
        public DateTime? OpenDODate { get; set; }
        public string OpenDONumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }
        public string? ResponsiblePerson { get; set; }
        public string? Description { get; set; }
        public string? DOType { get; set; }
        public string? IssuedTo { get; set; }
        public string? ReasonforIssuingStock { get; set; }
        public bool ModifiedStatus { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public string Unit { get; set; }

        public string? CreatedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<OpenDeliveryOrderParts>? OpenDeliveryOrderParts { get; set; }
    }
}
