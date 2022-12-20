namespace Tips.SalesService.Api.Entities.Dto
{
    public class SalesOrderItemsDto
    {
        public int Id { get; set; }
        public string? SAItemNo { get; set; }
        public string? Description { get; set; }
        public decimal UOM { get; set; }
        public decimal Currency { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal OrderQty { get; set; }
        public decimal SGST { get; set; }
        public decimal CGST { get; set; }
        public decimal UTGST { get; set; }
        public DateTime RequestedDate { get; set; }
        public string Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class SalesOrderItemsDtoPost
    {
        public string? SAItemNo { get; set; }
        public string? Description { get; set; }
        public decimal UOM { get; set; }
        public decimal Currency { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal OrderQty { get; set; }
        public decimal SGST { get; set; }
        public decimal CGST { get; set; }
        public decimal UTGST { get; set; }
        public DateTime RequestedDate { get; set; }
        public string Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
    public class SalesOrderItemsDtoUpdate
    {
        public int Id { get; set; }
        public string? SAItemNo { get; set; }
        public string? Description { get; set; }
        public decimal UOM { get; set; }
        public decimal Currency { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal OrderQty { get; set; }
        public decimal SGST { get; set; }
        public decimal CGST { get; set; }
        public decimal UTGST { get; set; }
        public DateTime RequestedDate { get; set; }
        public string Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

}
