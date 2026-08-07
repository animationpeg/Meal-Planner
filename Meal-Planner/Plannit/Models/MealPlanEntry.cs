using Plannit.Enums;
using System.ComponentModel.DataAnnotations;

namespace Plannit.Models
{
    /// <summary>
    /// A single meal entry in a meal plan
    /// </summary>
    public class MealPlanEntry
    {
        /// <summary>
        /// The unique id of this meal entry
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// The day of the meal plan this entry lands on, starting with 1
        /// </summary>
        [Required]
        [Range(1, 50)]
        public int Day { get; set; }
        /// <summary>
        /// Which meal of the day this entry belongs to
        /// </summary>
        [Required]
        public MealSlot MealSlot { get; set; }

        /// <summary>
        /// The unique id of the meal plan this entry belongs to
        /// </summary>
        public Guid MealPlanId { get; set; }
        /// <summary>
        /// The meal plan entity this entry belongs to
        /// </summary>
        public MealPlan MealPlan { get; set; } = null!;

        /// <summary>
        /// The unique id of the recipe for this meal entry
        /// </summary>
        public Guid RecipeId { get; set; }
        /// <summary>
        /// The recipe entity for this meal entry
        /// </summary>
        public Recipe Recipe { get; set; } = null!;
    }
}
