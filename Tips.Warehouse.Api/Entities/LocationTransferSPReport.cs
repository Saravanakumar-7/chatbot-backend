using Entities.Enums;

namespace Tips.Warehouse.Api.Entities
{
    public class LocationTransferSPReport
    {
        public string? FromKpn { get; set; }
        public string? FromPartType { get; set; }
        public string? FromUOM { get; set; }
        public string? FromDescription { get; set; }
        public decimal? FromQty { get; set; }
        public string? FromLocation { get; set; }
        public string? FromWarehouse { get; set; }
        public string? FromProjectNumber { get; set; }
        public string? ToKPN { get; set; }
        public string? ToPartType { get; set; }
        public string? ToUOM { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? ToDescription { get; set; }
        public decimal? ToQty { get; set; }
        public string? ToLocation { get; set; }
        public string? ToWarehouse { get; set; }
        public string? ToProjectNumber { get; set; }
        public string? Remarks { get; set; }

    }
    public class LocationTransferSpReportForTras
    {
        public string? FromKpn { get; set; }
        public string? FromPartType { get; set; }
        public string? FromUOM { get; set; }
        public string? FromDescription { get; set; }
        public decimal? FromQty { get; set; }
        public string? FromLocation { get; set; }
        public string? FromWarehouse { get; set; }
        public string? FromProjectNumber { get; set; }
        public string? FromLotNumber { get; set; }
        public string? ToKPN { get; set; }
        public string? ToPartType { get; set; }
        public string? ToUOM { get; set; }
        public string? ToDescription { get; set; }
        public decimal? ToQty { get; set; }
        public string? ToLocation { get; set; }
        public string? ToWarehouse { get; set; }
        public string? ToProjectNumber { get; set; }
        public string? ToLotNumber { get; set; }
        public string? Remarks { get; set; }

    }
    public class LocationTransferSpReportForAvi
    {
        public string? FromKpn { get; set; }
        public string? FromPartType { get; set; }
        public string? FromUOM { get; set; }
        public string? FromDescription { get; set; }
        public decimal? FromQty { get; set; }
        public string? FromLocation { get; set; }
        public string? FromWarehouse { get; set; }
        public string? FromProjectNumber { get; set; }
        public string? ToKPN { get; set; }
        public string? ToPartType { get; set; }
        public string? ToUOM { get; set; }
        public string? ToDescription { get; set; }
        public decimal? ToQty { get; set; }
        public string? ToLocation { get; set; }
        public string? ToWarehouse { get; set; }
        public string? ToProjectNumber { get; set; }
        public string? Remarks { get; set; }

    }

    public class InvLocationtransferUnitPriceDateSpForAvi
    {
       
            public string? PartNumber { get; set; }
            public string? MftrPartNumber { get; set; }
            public string? Description { get; set; }
            public int? PartType { get; set; }
            public string? ProjectNumber { get; set; }
            public string? UOM { get; set; }
            public string? LotNumber { get; set; }
            public decimal? Balance_Quantity { get; set; }
            public string? Warehouse { get; set; }
            public string? Location { get; set; }
            public decimal? Max { get; set; }
            public decimal? Min { get; set; }
            public DateTime? CreatedOn { get; set; }
            public string? MaterialGroup { get; set; }
            public decimal? UnitPrice { get; set; }
        

    }
    public class InvLocationtransferUnitPriceSpWithParamForAvi
    {

        public string? PartNumber { get; set; }
        public string? MftrPartNumber { get; set; }
        public string? Description { get; set; }
        public int? PartType { get; set; }
        public string? ProjectNumber { get; set; }
        public string? UOM { get; set; }
        public string? LotNumber { get; set; }
        public decimal? Balance_Quantity { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public decimal? Max { get; set; }
        public decimal? Min { get; set; }
        public string? MaterialGroup { get; set; }
        public decimal? UnitPrice { get; set; }
    }
    public class InventoryFilterParams
    {
        public string? PartNumber { get; set; }
        public string? Description { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }
        public string? ProjectNumber { get; set; }
    }


}
