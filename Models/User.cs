using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Meal.Models
{
    public class User
    {
        [Required] public string UserId { get; set; }
        [Required] public string FullName { get; set; }
        public string SlackId { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}