using System.ComponentModel.DataAnnotations;

namespace Plannit.Models
{
    /// <summary>
    /// A single operational step in a recipe
    /// </summary>
    public class MethodStep
    {
        /// <summary>
        /// The unique id of this method step
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// The order position in the recipe
        /// </summary>
        [Required]
        public int StepNumber { get; set; }
        /// <summary>
        /// The instructions to follow at this step in the recipe
        /// </summary>
        [Required]
        public string Instruction { get; set; } = string.Empty;

        /// <summary>
        /// The unique id of the recipe this method step belongs to
        /// </summary>
        public Guid RecipeId { get; set; }
        /// <summary>
        /// The recipe entity that this method step belongs to
        /// </summary>
        public Recipe Recipe { get; set; } = null!;
    }
}
