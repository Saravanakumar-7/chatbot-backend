using Entities.Enums;

namespace Tips.Warehouse.Api.Entities
{
    public class LocationTransferSPReport
    {
        public string? FromKpn { get; set; }
        public string? FromUOM { get; set; }
        public string? FromDescription { get; set; }
        public decimal? FromQty { get; set; }
        public string? FromLocation { get; set; }
        public string? FromWarehouse { get; set; }
        public string? FromProjectNumber { get; set; }
        public string? ToKPN { get; set; }
        public PartType ToPartType { get; set; }
        public string? ToUOM { get; set; }
        public string? ToDescription { get; set; }
        public decimal? ToQty { get; set; }
        public string? ToLocation { get; set; }
        public string? ToWarehouse { get; set; }
        public string? ToProjectNumber { get; set; }
        public string? Remarks { get; set; }

    }
}
