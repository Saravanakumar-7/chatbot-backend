using System;

using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;

using System.Linq;

using System.Text;

using System.Threading.Tasks;



namespace Entities.DTOs

{

    public class VendorRelatedVendorDto

    {

        [Key]
        public int Id { get; set; }

        public string? RelatedVendorName { get; set; }

        public string? RelatedVendorAlias { get; set; }
        public string? NatureOfRelationship { get; set; }

    }

    public class VendorRelatedVendorPostDto

    {
        public string? RelatedVendorName { get; set; }

        public string? RelatedVendorAlias { get; set; }

        public string? NatureOfRelationship { get; set; }

    }

    public class VendorRelatedVendorUpdateDto

    {

        public int Id { get; set; }

        public string? RelatedVendorName { get; set; }

        public string? RelatedVendorAlias { get; set; }

        public string? NatureOfRelationship { get; set; }

    }



}