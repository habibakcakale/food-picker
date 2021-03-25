using System;

namespace Meal.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public OrderItem[] OrderItems { get; set; }
    }
}