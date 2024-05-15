using Microsoft.EntityFrameworkCore;

namespace Tips.Purchase.Api.Entities.DTOs
{
    public class PoAddProjectDto
    {
        public int Id { get; set; }
        public string? ProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal ProjectQty { get; set; }
        public decimal BalanceQty { get; set; }
        public decimal ReceivedQty { get; set; }

    }
    public class PoAddProjectPostDto
    {
        public string? ProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal ProjectQty { get; set; }

    }
    public class PoAddProjectUpdateDto
    {
        public string? ProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal ProjectQty { get; set; }

    }
    public class PoAddProjectReportDto
    {
        public int Id { get; set; }
        public string? PONumber { get; set; }
        public string? ItemNumber { get; set; }
        public string? ProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal ProjectQty { get; set; }
        public decimal BalanceQty { get; set; }
        public decimal ReceivedQty { get; set; }

    }
    public class PoProjectNoUpdateQtyDetailsDto
    {
        public string ItemNumber { get; set; }
        public string PONumber { get; set; }
        public string ProjectNumber { get; set; }
        public decimal ProjectQty { get; set; }

    }
}
