using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Grin.Api.Entities.DTOs
{
    public class OpenGrinForGrinProjectNumberDto
    {
        [Key]
        public int Id { get; set; }
        public string? ReferenceSONumber { get; set; }

        [Precision(18, 3)]
        public decimal? ReferenceSOQty { get; set; }
    }
    public class OpenGrinForGrinProjectNumberPostDto
    {
        public string? ReferenceSONumber { get; set; }

        [Precision(18, 3)]
        public decimal? ReferenceSOQty { get; set; }
    }
    public class OpenGrinForGrinProjectNumberUpdateDto
    {
        public string? ReferenceSONumber { get; set; }

        [Precision(18, 3)]
        public decimal? ReferenceSOQty { get; set; }
    }
}
