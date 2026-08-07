using System.ComponentModel.DataAnnotations;

namespace Plannit.Models
{
    /// <summary>
    /// A meal plan for the week
    /// </summary>
    public class MealPlan
    {
        /// <summary>
        /// The unique id of this meal plan
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// The date of the start of the week for this meal plan
        /// </summary>
        [Required]
        public DateOnly StartingWeek { get; set; }

        /// <summary>
        /// The collection of recipes
        /// </summary>
        [Required]
        public ICollection<MealPlanEntry> Entries { get; set; } = new List<MealPlanEntry>();
    }
}
