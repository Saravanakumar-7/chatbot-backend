using Microsoft.EntityFrameworkCore;

namespace Tips.Production.Api.Entities.DTOs
{
    public class MRStockDetailsDto
    {

        public int Id { get; set; }
        //public string? LotNumber { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }

        [Precision(13,3)]
        public decimal LocationStock { get; set; }
        public bool IsMRIssueDone { get; set; }
        public string? SerialNo { get; set; }

    }
    public class MRStockDetailsPostDto
    {

        //public string? LotNumber { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }

        [Precision(13, 3)]
        public decimal LocationStock { get; set; }
        public bool IsMRIssueDone { get; set; } 

    }
    public class MRStockDetailsUpdateDto
    {
       // public string? LotNumber { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }

        [Precision(13, 3)]
        public decimal LocationStock { get; set; }
        public bool IsMRIssueDone { get; set; } 
    }
    public class IssueMRStockDetailsUpdateDto
    {
        //public string LotNumber { get; set; }
        public string? Warehouse { get; set; }
        public string? Location { get; set; }

        [Precision(13, 3)]
        public decimal Qty { get; set; }

        [Precision(13, 3)]
        public decimal LocationStock { get; set; }
        public bool IsMRIssueDone { get; set; }
        public string? SerialNo { get; set; }
    }


}
