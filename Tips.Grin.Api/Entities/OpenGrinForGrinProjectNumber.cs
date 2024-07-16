using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities
{
    public class OpenGrinForGrinProjectNumber
    {
        [Key]
        public int Id { get; set; }
        public string? ReferenceSONumber { get; set; }

        [Precision(18, 3)]
        public decimal? ReferenceSOQty { get; set; }

        public int OpenGrinForGrinItemsId { get; set; }
        public OpenGrinForGrinItems? OpenGrinForGrinItems { get; set; }
    }
}
