using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.SalesService.Api.Entities.DTOs
{
    public class LocationTransferDto
    {


        public int Id { get; set; }
        public string FromPartNo { get; set; }
        public string ToPartNo { get; set; }
        public string FromLocation { get; set; }
        public string? FromDescription { get; set; }
        public string? ToDescription { get; set; }
        [Precision(13, 3)]
        public decimal AvailableStockInLocation { get; set; }
        public string? FromUOM { get; set; }
        public string? ToUOM { get; set; }
        public string ToLocation { get; set; }
        public string? FromPartType { get; set; }
        public string? ToPartType { get; set; }
        [Precision(13, 3)]
        public decimal TransferQty { get; set; }
        public string? Remarks { get; set; }
        public string Unit { get; set; }
    }

    public class LocationTransferDtoPost
    {
        [Required(ErrorMessage = "FromPartNo is required")]
        [StringLength(500, ErrorMessage = "FromPartNo can't be longer than 500 characters")]
        public string FromPartNo { get; set; }

        [Required(ErrorMessage = "ToPartNo is required")]
        [StringLength(500, ErrorMessage = "ToPartNo can't be longer than 500 characters")]
        public string ToPartNo { get; set; }

        [Required(ErrorMessage = "FromLocation is required")]
        [StringLength(500, ErrorMessage = "FromLocation can't be longer than 500 characters")]
        public string FromLocation { get; set; }
        public string? FromDescription { get; set; }
        public string? ToDescription { get; set; }      

        [Precision(13, 3)]
        public decimal AvailableStockInLocation { get; set; }
        public string? FromUOM { get; set; }
        public string? ToUOM { get; set; }
        public string ToLocation { get; set; }
        public string? FromPartType { get; set; }
        public string? ToPartType { get; set; }

        [Precision(13, 3)]       
        public decimal TransferQty { get; set; }
        public string? Remarks { get; set; }

       
    }

    public class LocationTransferDtoUpdate
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "FromPartNo is required")]
        [StringLength(500, ErrorMessage = "FromPartNo can't be longer than 500 characters")]
        public string FromPartNo { get; set; }

        [Required(ErrorMessage = "ToPartNo is required")]
        [StringLength(500, ErrorMessage = "ToPartNo can't be longer than 500 characters")]
        public string ToPartNo { get; set; }

        [Required(ErrorMessage = "FromLocation is required")]
        [StringLength(500, ErrorMessage = "FromLocation can't be longer than 500 characters")]
        public string FromLocation { get; set; }
        public string? FromDescription { get; set; }
        public string? ToDescription { get; set; }

        [Precision(13, 3)]
        public decimal AvailableStockInLocation { get; set; }
        public string? FromUOM { get; set; }
        public string? ToUOM { get; set; }
        public string ToLocation { get; set; }
        public string? FromPartType { get; set; }
        public string? ToPartType { get; set; }

        [Precision(13, 3)]
        public decimal TransferQty { get; set; }
        public string? Remarks { get; set; }
        public string Unit { get; set; }
    }
}