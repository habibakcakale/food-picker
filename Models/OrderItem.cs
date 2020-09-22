using System.ComponentModel.DataAnnotations;

namespace Meal.Models
{
    public class OrderItem
    {
        [Key] public int Id { get; set; }

        [Required] public MealType MealType { get; set; }
        public int OrderId { get; set; }
        [Required] [MaxLength(200)] public string Name { get; set; }
    }
}