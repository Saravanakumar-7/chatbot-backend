using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class EngineeringCustomField
    {

        public int Id { get; set; }

        public string Weight { get; set; }

        public string Height { get; set; }
        
        public string Margin { get; set; }

        public string Thickness { get; set; }

        public string Length { get; set; }

        public string SheetHeight { get; set; }

        public DateTime WirePurchasedDate { get; set; }

        public string WireLength { get; set; }

        public string WireType { get; set; }
    }
}
