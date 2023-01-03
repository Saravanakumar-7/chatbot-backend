using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs
{
    public class EngineeringCustomFieldDto
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

    public class EngineeringCustomFieldPostDto
    {
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

    public class EngineeringCustomFieldUpdateDto
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
