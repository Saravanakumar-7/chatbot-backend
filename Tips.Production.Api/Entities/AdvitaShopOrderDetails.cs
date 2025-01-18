using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tips.Production.Api.Entities
{
    public class AdvitaShopOrderDetails
    {
        
            public string Shop_Order_No { get; set; } 

            public string? Shop_Order_Date { get; set; }

            public string Shop_Order_Type { get; set; }

            public string Sales_Order_No { get; set; } 

            public string Item_Number { get; set; } 
            public string Project_Name { get; set; } 

            public string Item_Description { get; set; } 

            public long Shop_Order_Release_Qty { get; set; }

            public string? Shop_Order_Completion_Date { get; set; } 

            public string Customer_Name { get; set; } 

            public string Remarks { get; set; } 

            public string? Created_By { get; set; } 

            public string? Created_On { get; set; }
            [Key]
            public int Trans_Unique_Id { get; set; } 
            public DateTime? Trans_Uploaded_On { get; set; } 

            public int Trans_Uploaded_By_Id { get; set; } 

            public long? Trans_Uploaded_Id { get; set; } 
        


    }

}
