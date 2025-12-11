namespace Tips.Warehouse.Api.Entities
{
    public class StockMovementHistorySPReport
    {
        public string? ItemNumber { get; set; }
        public decimal? GrinQty { get; set; }
        public decimal? OpenGrinQty { get; set; }
        public decimal? RDoQty { get; set; }
        public decimal? RIvQty { get; set; }
        public decimal? RODOQTY { get; set; }
        public decimal? MaterialReturnQty { get; set; }
        public decimal? InwardsLocationTransfer { get; set; }
        public decimal? oqcqty { get; set; }
        public decimal? InwardsTotalOpeningStock { get; set; }
        public decimal? DoQty { get; set; }
        public decimal? ODOQTY { get; set; }
        public decimal? MaterailissueAgainstShop { get; set; }
        public decimal? MaterailissueAgainstMaterialrequest { get; set; }
        public decimal? TransferLocationTransfer { get; set; }
        public decimal? TransfersTotal { get; set; }
        public decimal? Stock_closing { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
