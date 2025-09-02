using Microsoft.EntityFrameworkCore;
using Tips.Purchase.Api.Entities.Dto;

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
        public List<PoAddKitProjectDto>? PoAddKitProjects { get; set; }

    }
    public class PoAddProjectPostDto
    {
        public string? ProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal ProjectQty { get; set; }
        public List<PoAddKitProjectPostDto>? PoAddKitProjects { get; set; }

    }
    public class PoAddProjectUpdateDto
    {
        public string? ProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal ProjectQty { get; set; }
        public List<PoAddKitProjectUpdateDto>? PoAddKitProjects { get; set; }

    }
    public class PoAddProjectApprovalUpdateDto
    {
        public int Id { get; set; }
        public string? ProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal ProjectQty { get; set; }
        public List<PoAddKitProjectApprovalUpdateDto>? PoAddKitProjects { get; set; }

    }
    public class PoAddProjectShortCloseDto
    {
        //public int Id { get; set; }
        public string? ProjectNumber { get; set; }

        [Precision(13, 3)]
        public decimal ProjectQty { get; set; }
        public decimal BalanceQty { get; set; }
        public decimal ReceivedQty { get; set; }

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
        public string ProjectNumber { get; set; }
        public decimal ProjectQty { get; set; }
        public int PoItemId { get; set; }

    }
}
