using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using Entities.Enums;

namespace Tips.Warehouse.Api.Entities.DTOs
{
    public class OpenDeliveryOrderDto
    {

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
        public string Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public bool AllowReturnODO { get; set; }
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
        public List<OpenDeliveryOrderPartsDtoPost> OpenDeliveryOrderParts { get; set; }
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
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<OpenDeliveryOrderPartsDtoUpdate>? OpenDeliveryOrderParts { get; set; }

    }
    public class OpenDeliveryOrderIdNameList
    {
        public int Id { get; set; }
        public string? ODONumber { get; set; }
        public string? ODOType { get; set; }
        //public string? CustomerName { get; set; }
    }
    public class OpenDeliveryOrderSearchDto
    {
        public List<string> ODONumber { get; set; }
        public List<string> CustomerName { get; set; }
        public List<string> DOType { get; set; }
        public List<string> Description { get; set; }
        public List<String>? IssuedTo { get; set; }
    }
    public class ODODetailsDto
    {
        public string ItemNumber { get; set; }
        public PartType? ItemType { get; set; }
        public string? UOM { get; set; }
        public decimal? StockAvailable { get; set; }


        public List<WarehouseDetailsDto>? WarehouseDetails { get; set; }
    }
    public class WarehouseDetailsDto
    {
        public string WarehouseName { get; set; }
        public List<LocationDetailsDto>? LocationDetails { get; set; }
    }
    public class LocationDetailsDto
    {
        public string? LocationName { get; set; }
        public decimal LocationStock { get; set; }

    }

    public class OpenDeliveryOrderSPReportDto
    {
        public string OpenDONumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? LeadId { get; set; }
        public string? IssuedTo { get; set; }
        public string? IssuedBy { get; set; }
        public string? KPNno { get; set; }
        public string? MPN { get; set; }
        public string? ItemDescription { get; set; }
        public string? Location { get; set; }
        public string? Warehouse { get; set; }
        public string? ODOtype { get; set; }
        public string? UOC { get; set; }
        public string? UOM { get; set; }
        public decimal? Avaliablestk { get; set; }
        public decimal? Dispatchstk { get; set; }
        public string? SerialNo { get; set; }
        public string? Remarks { get; set; }

    }
    public class OpenDeliveryOrderSPReportWithParamDto
    {
        public string? OpenDoNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAliasName { get; set; }
        public string? LeadId { get; set; }
        public string? IssuedTo { get; set; }
        public string? KPN { get; set; }
        public string? MPN { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? ODOtype { get; set; }
    }
    public class OpenDeliveryOrderSPReportWithParamForTransDto
    {
        public string? OpenDoNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? IssuedTo { get; set; }
        public string? ItemNumber { get; set; }
        public string? MPN { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? ODOtype { get; set; }
        public string? ProjectNumber { get; set; }
    }
    public class OpenDeliveryOrderSPReportForTrans
    {
        public string? OpenDONumber { get; set; }
        public DateTime? ODODate { get; set; }
        public string? CustomerName { get; set; }
        public string? IssuedTo { get; set; }
        public string? IssuedBy { get; set; }
        public string? ProjectNumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? MPN { get; set; }
        public string? ItemDescription { get; set; }
        public string? UOM { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? ODOtype { get; set; }
        public int? Avaliablestk { get; set; }
        public decimal? ODOqty { get; set; }
        public decimal? ReturnQty { get; set; }
        public string? SerialNo { get; set; }
        public string? Remarks { get; set; }
    }

    public class ODOMonthlyConsumptionDto
    {
        public string? CustomerId { get; set; }
    }
    public class odoLotNumberListDto
    {
        public string LotNumber { get; set; }
    }
    public class ODOQuantityDto
    {
        public string? ItemNumber { get; set; }
        public decimal? ODOQty { get; set; }

    }

}
