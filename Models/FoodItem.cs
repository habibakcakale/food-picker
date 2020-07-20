using System;
using System.ComponentModel.DataAnnotations;

namespace FoodApp.Models
{
    public class FoodItem
    {
        [Key] public int Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string Type { get; set; }
    }

    public class TodaySelection
    {
        [Key] public int Id { get; set; }
        [Required] public string FullName { get; set; }
        [Required] public string Mains { get; set; }
        [Required] public string SideOrders { get; set; }
        [Required] public string Salad { get; set; }
        [Required] public string Soup { get; set; }
        public DateTime? Date { get; set; }
    }
}