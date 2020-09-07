using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FoodApp.Models
{
    public class Order
    {
        [Key] public int Id { get; set; }
        [Required] public string FullName { get; set; }
        [Required] public string UserId { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public DateTime? Date { get; set; }
    }
}