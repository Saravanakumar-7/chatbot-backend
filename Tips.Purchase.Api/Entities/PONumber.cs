using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tips.Purchase.Api.Entities.DTOs;

namespace Tips.Purchase.Api.Entities
{
    public class PONumber
    {
        public int Id { get; set; }
        public int CurrentValue { get; set; }
    }
}
