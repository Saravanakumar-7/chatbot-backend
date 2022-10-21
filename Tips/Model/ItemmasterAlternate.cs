using System.ComponentModel.DataAnnotations;

namespace Tips.Model
{
    public class ItemmasterAlternate
    {
        [Key]
        public int Id { get; set; }
        public string manufacturerPartNo { get; set; }
        public string manufacturer { get; set; }
        public bool default_manufacturer { get; set; }
    }
}
