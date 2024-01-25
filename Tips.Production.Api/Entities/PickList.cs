using Microsoft.VisualBasic;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace Tips.Production.Api.Entities
{
    public class PickList
    {
        public string? SO_ItemNumber {  get; set; }
        public string? ShopOrderNumber {  get; set; }
        public decimal? BomRevisionNo {  get; set; }
        public DateTime? lastDate {  get; set; }
        public DateTime? SODate {  get; set; }
        public string? descd {  get; set; }
        public string? SO_Child_ItemNumber {  get; set; }
        public string? INPUT_ProjectNumber {  get; set; }
        public string? BOM_CHILD_INPUT_ItemNumber {  get; set; }
        public string? PartType {  get; set; }
        public decimal? ReleaseQty {  get; set; }
        public decimal? LotQuantity {  get; set; }
        public DateTime? soClosedate {  get; set; }
        public DateTime? shortclosedate {  get; set; }
        public decimal? INPUT_RequiredQty {  get; set; }
        public string? UOM {  get; set; }
        public decimal? BalanceToIssue {  get; set; }
        public decimal? IssuedQty {  get; set; }

        // inventory PickList fields
        //public string? PartNumber {  get; set; }
        //public string? ProjectNumber {  get; set; }
        //public string? Location {  get; set; }
        //public string? Warehouse {  get; set; }
        //public decimal? Balance_Quantity {  get; set; }



    }
         
}
