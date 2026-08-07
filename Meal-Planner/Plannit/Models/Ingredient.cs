using Plannit.Enums;
using System.ComponentModel.DataAnnotations;

namespace Plannit.Models
{
    /// <summary>
    /// An ingredient for use in a recipe
    /// </summary>
    public class Ingredient
    {
        /// <summary>
        /// The unique id of this ingredient
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// The name of this ingredient
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// The quantity of this ingredient required
        /// </summary>
        [Range(0.1, double.MaxValue)]
        public decimal? Quantity { get; set; }
        /// <summary>
        /// The unit of measurement for this ingredient
        /// </summary>
        public UnitMeasurement Unit { get; set; } = UnitMeasurement.None;

        /// <summary>
        /// The unique id of the recipe this ingredient belongs to
        /// </summary>
        public Guid RecipeId { get; set; }
        /// <summary>
        /// The recipe entity that this ingredient belongs to
        /// </summary>
        public Recipe Recipe { get; set; } = null!;
    }
}
