using System.ComponentModel.DataAnnotations;

namespace Tips.Purchase.Api.Entities.Dto
{
    public class PoIncoTermDto
    {
        [Key]
        public int Id { get; set; }
        public string? IncoTermName { get; set; }
    }
    public class PoIncoTermPostDto
    {
        public string? IncoTermName { get; set; }
    }
    public class PoIncoTermUpdateDto
    {
        public string? IncoTermName { get; set; }
    }
    public class PoIncoTermShortCloseDto
    {
        [Key]
        public int Id { get; set; }
        public string? IncoTermName { get; set; }
    }
    public class PoIncoTermReportDto
    {
        public int Id { get; set; }
        public string? IncoTermName { get; set; }
        public string? PONumber { get; set; }
    }
}
