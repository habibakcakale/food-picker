namespace FoodApp.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Meal
    {
        [Key] public int Id { get; set; }
        [Required] [MaxLength(200)] public string Name { get; set; }
        [Required] public MealType MealType { get; set; }
    }
}