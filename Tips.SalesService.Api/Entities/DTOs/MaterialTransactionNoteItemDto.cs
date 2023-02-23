using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tips.SalesService.Api.Entities.DTOs
{
    public class MaterialTransactionNoteItemDto
    {
        public int? Id { get; set; }
        public string? FromPartNumber { get; set; }
        public string? FromLocation { get; set; }
        public int? Stock { get; set; }
        public string? ToProject { get; set; }
        public string? ToPartNumber { get; set; }

        [Precision(13, 3)]
        public decimal? TransferQty { get; set; }
        public string Unit { get; set; }
        public bool IsApproved { get; set; }
        public bool IsClosed { get; set; }
    }

    public class MaterialTransactionNoteItemPostDto
    {
        public string? FromPartNumber { get; set; }
        public string? FromLocation { get; set; }
        public int? Stock { get; set; }
        public string? ToProject { get; set; }
        public string? ToPartNumber { get; set; }

        [Precision(13, 3)]
        public decimal? TransferQty { get; set; }
        public string Unit { get; set; }
        public bool IsApproved { get; set; }
        public bool IsClosed { get; set; }
    }

    public class MaterialTransactionNoteItemUpdateDto
    {
        public string? FromPartNumber { get; set; }
        public string? FromLocation { get; set; }
        public int? Stock { get; set; }
        public string? ToProject { get; set; }
        public string? ToPartNumber { get; set; }

        [Precision(13, 3)]
        public decimal? TransferQty { get; set; }
        public string Unit { get; set; }
        public bool IsApproved { get; set; }
        public bool IsClosed { get; set; }
    }
}