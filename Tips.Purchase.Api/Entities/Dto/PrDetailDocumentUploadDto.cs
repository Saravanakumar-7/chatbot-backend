using System.ComponentModel.DataAnnotations;

namespace Tips.Purchase.Api.Entities.Dto
{
    public class PrDetailDocumentUploadDto
    { 

            public string FileName { get; set; }

            public string FileExtension { get; set; }

            public string FilePath { get; set; } 
        }
    public class PrDetailDocumentUploadPostDto
    { 
        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public string FilePath { get; set; } 
    }
    public class PrDetailDocumentUploadUpdateDto
    {

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public string FilePath { get; set; } 
    }

}
