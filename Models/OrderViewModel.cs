using System;

namespace Meal.Models
{
    public class OrderViewModel
    {
        public DateTime Date { get; set; }
        public OrderItem[] OrderItems { get; set; }
    }
}