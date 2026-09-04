using Plannit.Enums;
using System.ComponentModel.DataAnnotations;

namespace Plannit.Models
{
    /// <summary>
    /// An individual item on a shopping list
    /// </summary>
    public class ShoppingListItem
    {
        /// <summary>
        /// The unique id for this shopping list item
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The name of the shopping list item
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The quantity of the shopping list item
        /// </summary>
        public decimal? Quantity { get; set; }

        /// <summary>
        /// The unit of measurement of the shopping list item
        /// </summary>
        public UnitMeasurement Unit { get; set; } = UnitMeasurement.None;

        /// <summary>
        /// Whether the shopping list item is marked as purchased
        /// </summary>
        public bool IsChecked { get; set; } = false;

        /// <summary>
        /// The order position of this item in the shopping list
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// The unique Id of the shopping list this item belongs to
        /// </summary>
        public Guid ShoppingListId { get; set; }

        /// <summary>
        /// The shopping list entity this item belongs to
        /// </summary>
        public ShoppingList ShoppingList { get; set; } = null!;
    }
}
