namespace Plannit.Models
{
    /// <summary>
    /// A shopping list generated from a meal plan
    /// </summary>
    public class ShoppingList
    {
        /// <summary>
        /// The unique id for this shopping list
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The Id of the meal plan this shopping list is generated from
        /// </summary>
        public Guid MealPlanId { get; set; }

        /// <summary>
        /// The MealPlan entity this shopping list is generated from
        /// </summary>
        public MealPlan? MealPlan { get; set; } = null;

        /// <summary>
        /// The collection of items in this shopping list
        /// </summary>
        public ICollection<ShoppingListItem> Items { get; set; } = new List<ShoppingListItem>();
    }
}
