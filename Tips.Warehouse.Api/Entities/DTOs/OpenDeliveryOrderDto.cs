using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class OpenDeliveryOrderDto
    {

        public int Id { get; set; }
        public DateTime? OpenDODate { get; set; } 
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }
        public string? ResponsiblePerson { get; set; }
        public string? Description { get; set; }
        public string? DOType { get; set; }
        public string? IssuedTo { get; set; }
        public string? ReasonforIssuingStock { get; set; }
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<OpenDeliveryOrderPartsDto>? OpenDeliveryOrderParts { get; set; }
    }
    public class OpenDeliveryOrderDtoPost
    {

        public DateTime? OpenDODate { get; set; } 
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }
        public string? ResponsiblePerson { get; set; }
        public string? Description { get; set; }
        public string? DOType { get; set; }
        public string? IssuedTo { get; set; }
        public string? ReasonforIssuingStock { get; set; }

     
        public List<OpenDeliveryOrderPartsDtoPost>? OpenDeliveryOrderParts { get; set; }

    }
    public class OpenDeliveryOrderDtoUpdate
    {
        public int Id { get; set; }
        public DateTime? OpenDODate { get; set; }         
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? CustomerId { get; set; }
        public string? ResponsiblePerson { get; set; }
        public string? Description { get; set; }
        public string? DOType { get; set; }
        public string? IssuedTo { get; set; }
        public string? ReasonforIssuingStock { get; set; }

        [Required(ErrorMessage = "Unit is required")]
        [StringLength(100, ErrorMessage = "Unit can't be longer than 100 characters")]
        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<OpenDeliveryOrderPartsDtoUpdate>? OpenDeliveryOrderParts { get; set; }

    }
    public class OpenDeliveryOrderIdNameList
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
    }
}
