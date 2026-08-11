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
        /// The user-defined name of this meal plan
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The date of the start of the week for this meal plan
        /// </summary>
        [Required]
        public DateOnly WeekStarting { get; set; }

        /// <summary>
        /// The duration of this meal plan, defaulted to 7 days
        /// </summary>
        [Required]
        public int DurationDays { get; set; } = 7;

        /// <summary>
        /// Marker to indicate the currently active meal plan. Only one should be active at a time!
        /// </summary>
        [Required]
        public bool IsActive { get; set; } = false;

        /// <summary>
        /// The collection of recipes
        /// </summary>
        [Required]
        public ICollection<MealPlanEntry> Entries { get; set; } = new List<MealPlanEntry>();
    }
}
