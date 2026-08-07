using System.ComponentModel.DataAnnotations;

namespace Plannit.Models
{
    /// <summary>
    /// A recipe that contains a list of ingredients and instructions for a meal
    /// </summary>
    public class Recipe
    {
        /// <summary>
        /// The unique id for this recipe
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// The name of this recipe
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// A short description of this recipe
        /// </summary>
        public string? Description { get; set; } = string.Empty;
        /// <summary>
        /// The number of servings the recipe caters for
        /// </summary>
        public int? Servings { get; set; }

        /// <summary>
        /// A list of ingredients required for this meal
        /// </summary>
        public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        /// <summary>
        /// The instructional steps for cooking this meal
        /// </summary>
        public ICollection<MethodStep> MethodSteps { get; set; } = new List<MethodStep>();
    }
}
