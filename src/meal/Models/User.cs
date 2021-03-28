#nullable enable
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Meal.Models {
    public class User {
        [Required] public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Picture { get; set; }
        public string? SlackId { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
