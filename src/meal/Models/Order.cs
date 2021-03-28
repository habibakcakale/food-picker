using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Meal.Models
{
    public class Order
    {
        [Key] public int Id { get; set; }
        [Required] public string UserId { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public DateTime? Date { get; set; }
        public User User { get; set; }
    }
}