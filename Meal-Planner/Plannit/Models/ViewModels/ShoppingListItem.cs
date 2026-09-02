using Plannit.Enums;

namespace Plannit.Models.ViewModels
{
    /// <summary>
    /// View model for a shopping list item
    /// </summary>
    public class ShoppingListItem
    {
        /// <summary>
        /// The name of the item
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The sum total quantity of the item
        /// </summary>
        public decimal? Quantity { get; set; }

        /// <summary>
        /// The unit of measurement for the item
        /// </summary>
        public UnitMeasurement Unit { get; set; } = UnitMeasurement.None;

        /// <summary>
        /// Whether the item is marked as purchased
        /// </summary>
        public bool IsChecked { get; set; } = false;
    }
}
