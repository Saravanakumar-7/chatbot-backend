namespace Tips.Model
{
    public class ItemMaster
    {
        public string id { get; set; }
        public string itemNumber { get; set; }
        public string description { get; set; }
        public bool activeStatus { get; set; }
        public bool obsolete { get; set; }
        public string itemType { get; set; }
        public string uom { get; set; }
        public string commodity { get; set; }
        public string hsn { get; set; }
        public string materialGroup { get; set; }
        public DateTime validFrom { get; set; }
        public string purchaseGroup { get; set; }
        public DateTime validTo { get; set; }
        public string department { get; set; }
        public string drawingNo { get; set; }
        public string docRet { get; set; }
        public string revNo { get; set; }
        public bool coc { get; set; }
        public bool rohs { get; set; }
        public bool shelfLife { get; set; }
        public bool reach { get; set; }
        public string uploadFile { get; set; }
        public string netWeight { get; set; }
        public string netUom { get; set; }
        public string grossWeight { get; set; }
        public string grossUom { get; set; }
        public string volume { get; set; }
        public string volumeUom { get; set; }
        public string size { get; set; }
        public string footPrint { get; set; }
        public string min { get; set; }
        public string max { get; set; }
        public string leadtime { get; set; }
        public string reorder { get; set; }
        public string toBin { get; set; }
        public bool kanban { get; set; }
        public bool esd { get; set; }
        public bool fifo { get; set; }
        public bool lifo { get; set; }
        public bool cycleCount { get; set; }
        public bool hazardousMaterial { get; set; }
        public string expiry { get; set; }
        public string inspectionInt { get; set; }
        public string specialInstructions { get; set; }
        public string shippingInstruction { get; set; }
        public bool iqc { get; set; }
        public string grProcessing { get; set; }
        public string batchSize { get; set; }
        public string costCenter { get; set; }
        public string stdCost { get; set; }
        public string costingMethod { get; set; }
        public bool valuation { get; set; }
        public bool depreciation { get; set; }
        public bool pfo { get; set; }
        public string createdBy { get; set; }
        public DateTime createdOn { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedOn { get; set; }

    }

}