using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.Grin.Api.Entities
{
         public class DocumentUpload
        {
            [Key]
            public int Id { get; set; }

            public string FileName { get; set; }

            public string FileExtension { get; set; }

            public string FilePath { get; set; }

            public string DocumentFrom { get; set; }

            public string ParentId { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public Grins? Grins { get; set; }


    }
}
 