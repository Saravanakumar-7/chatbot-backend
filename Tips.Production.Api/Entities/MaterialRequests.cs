using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Tips.Production.Api.Entities.Enums;
using Tips.Production.Api.Entities.DTOs;
using Entities.Enums;

namespace Tips.Production.Api.Entities
{
    public class MaterialRequests
    {
        [Key]
        public int Id { get; set; }

        public string? MRNumber { get; set; }
        public string? ProjectNumber { get; set; }
        public string? FGItemNumber { get; set; }

        public PartType ItemType { get; set; }
        public string? ShopOrderNumber { get; set; }
        public string? IssuedBy { get; set; }
        public DateTime? IssuedOn { get; set; }
        public IssuedStatus IssuedStatus { get; set; }

        [DefaultValue(0)]
        public ShortStatus StatusOfShort { get; set; }
        public MaterialStatus MrStatus { get; set; }
        public string? IssuedTo {  get; set; }
        public string Unit { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public ShopOrderType ShopOrderType { get; set; }

        public List<MaterialRequestItems>? MaterialRequestItems { get; set; }

    }
}
