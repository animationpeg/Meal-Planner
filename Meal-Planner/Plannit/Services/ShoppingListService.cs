using Plannit.Models;
using Plannit.Enums;
using Humanizer;
using Plannit.Models.ViewModels;

namespace Plannit.Services
{
    /// <summary>
    /// Service for shopping list generation
    /// </summary>
    /// <param name="mealPlanService">Meal plan service dependency</param>
    public class ShoppingListService(MealPlanService mealPlanService)
    {
        private readonly MealPlanService _mealPlanService = mealPlanService;

        /// <summary>
        /// Generate a shopping list based on the active meal plan
        /// </summary>
        /// <returns>A list of Shopping List Items</returns>
        public async Task<List<ShoppingListItem>> GenerateShoppingListAsync()
        {
            MealPlan? mealPlan = await _mealPlanService.GetActiveMealPlanWithIngredientsAsync();

            if (mealPlan == null)
            {
                return new List<ShoppingListItem>();
            }

            List<Ingredient> allIngredients = mealPlan.Entries
                .Select(e => e.Recipe)
                .Where(r => r != null)
                .SelectMany(r => r.Ingredients)
                .ToList();

            List<ShoppingListItem> shoppingList = allIngredients
                .GroupBy(i => new { Name = i.Name.ToLower().Trim(), i.Unit })
                .Select(group => new ShoppingListItem
                {
                    Name = group.First().Name,
                    Unit = group.Key.Unit,
                    Quantity = group.Any(i => i.Quantity.HasValue)
                        ? group.Where(i => i.Quantity.HasValue).Sum(i => i.Quantity!.Value)
                        : null
                })
                .OrderBy(i => i.Name)
                .ToList();

            return shoppingList;
        }
    }
}
