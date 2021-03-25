using System;

namespace Meal.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public OrderItem[] OrderItems { get; set; }
        public User User { get; set; }
    }
}