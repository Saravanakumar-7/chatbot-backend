using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tips.Production.Api.Entities.Enums;

namespace Tips.Production.Api.Entities
{
    public class MaterialIssueHistory
    {
        public int Id { get; set; }
        public string? PartNumber { get; set; }
        public string? ShopOrderNumber { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        public string? ProjectNumber { get; set; }
        public PartType PartType { get; set; }
        public string? UOM { get; set; }
        [Precision(13, 3)]
        public decimal RequiredQty { get; set; }

        [Precision(13, 3)]
        public decimal IssuedQty { get; set; }
        public IssuedStatus MaterialIssuedStatus { get; set; } = IssuedStatus.Open;
        [Precision(13, 3)]
        public string? Unit { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
