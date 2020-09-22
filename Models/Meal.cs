using System.ComponentModel.DataAnnotations;

namespace Meal.Models
{
    public class Meal
    {
        [Key] public int Id { get; set; }
        [Required] [MaxLength(200)] public string Name { get; set; }
        [Required] public MealType MealType { get; set; }
    }
}