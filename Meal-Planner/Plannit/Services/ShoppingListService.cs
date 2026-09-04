using Microsoft.EntityFrameworkCore;
using Plannit.Data;
using Plannit.Enums;
using Plannit.Models;

namespace Plannit.Services
{
    /// <summary>
    /// Service for shopping list generation
    /// </summary>
    /// <param name="contextFactory">The AppDbContext factory dependency</param>
    public class ShoppingListService(IDbContextFactory<AppDbContext> contextFactory)
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory = contextFactory;

        /// <summary>
        /// Generates a shopping list based on the active meal plan
        /// </summary>
        public async Task<ShoppingList?> GetShoppingListForActivePlanAsync()
        {
            await using AppDbContext context = await _contextFactory.CreateDbContextAsync();
            return await context.ShoppingLists
                .Include(sl => sl.Items.OrderBy(i => i.Order))
                .FirstOrDefaultAsync(sl => sl.MealPlan.IsActive);
        }
        /// <summary>
        /// Generates a shopping list, or merges with preexisting shopping list
        /// </summary>
        public async Task<ShoppingList> GenerateOrMergeShoppingListAsync()
        {
            await using AppDbContext context = await _contextFactory.CreateDbContextAsync();

            MealPlan? mealPlan = await context.MealPlans
                .Include(mp => mp.Entries)
                    .ThenInclude(e => e.Recipe)
                        .ThenInclude(r => r.Ingredients)
                .Include(mp => mp.ShoppingList)
                    .ThenInclude(sl => sl!.Items)
                .FirstOrDefaultAsync(mp => mp.IsActive);

            if (mealPlan is null)
            {
                return new ShoppingList();
            }

            List<Ingredient> allIngredients = mealPlan.Entries
                .Select(e => e.Recipe)
                .Where(r => r is not null)
                .SelectMany(r => r.Ingredients)
                .ToList();

            List<(string Key, string Name, decimal? Quantity, UnitMeasurement Unit)> generated = allIngredients
                .GroupBy(i => new { Name = i.Name.ToLower().Trim(), i.Unit })
                .Select(group => (
                    Key: $"{group.Key.Name}|{group.Key.Unit}",
                    Name: group.First().Name,
                    Quantity: group.Any(i => i.Quantity.HasValue)
                        ? (decimal?)group.Where(i => i.Quantity.HasValue).Sum(i => i.Quantity!.Value)
                        : null,
                    Unit: group.Key.Unit
                ))
                .ToList();

            if (mealPlan.ShoppingList is null)
            {
                ShoppingList newList = new ShoppingList
                {
                    MealPlanId = mealPlan.Id,
                    Items = generated.Select((item, index) => new ShoppingListItem
                    {
                        Name = item.Name,
                        Quantity = item.Quantity,
                        Unit = item.Unit,
                        Order = index
                    }).ToList()
                };

                context.ShoppingLists.Add(newList);
                await context.SaveChangesAsync();
                return newList;
            }

            ShoppingList existingList = mealPlan.ShoppingList;
            List<ShoppingListItem> existingItems = existingList.Items.ToList();

            // Remove items no longer in the generated list
            foreach (ShoppingListItem existingItem in existingItems)
            {
                string existingKey = $"{existingItem.Name.ToLower().Trim()}|{existingItem.Unit}";
                if (!generated.Any(g => g.Key == existingKey))
                {
                    context.ShoppingListItems.Remove(existingItem);
                }
            }

            // Update quantities on existing items and add new ones
            int maxOrder = existingItems.Count > 0 ? existingItems.Max(i => i.Order) : -1;

            foreach ((string Key, string Name, decimal? Quantity, UnitMeasurement Unit) item in generated)
            {
                ShoppingListItem? existingItem = existingItems
                    .FirstOrDefault(i => $"{i.Name.ToLower().Trim()}|{i.Unit}" == item.Key);

                if (existingItem is null)
                {
                    context.ShoppingListItems.Add(new ShoppingListItem
                    {
                        Name = item.Name,
                        Quantity = item.Quantity,
                        Unit = item.Unit,
                        Order = ++maxOrder,
                        ShoppingListId = existingList.Id
                    });
                }
                else
                {
                    existingItem.Quantity = item.Quantity;
                }
            }

            await context.SaveChangesAsync();

            return await context.ShoppingLists
                .Include(sl => sl.Items.OrderBy(i => i.Order))
                .FirstAsync(sl => sl.Id == existingList.Id);
        }

        /// <summary>
        /// Updates the order of items in the shopping list
        /// </summary>
        /// <param name="itemId">The item id</param>
        /// <param name="newOrder">The new order position for the item</param>
        public async Task UpdateItemOrderAsync(Guid itemId, int newOrder)
        {
            await using AppDbContext context = await _contextFactory.CreateDbContextAsync();
            ShoppingListItem? item = await context.ShoppingListItems.FindAsync(itemId);
            if (item is not null)
            {
                item.Order = newOrder;
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Updates the IsChecked property of an item
        /// </summary>
        /// <param name="itemId">The item id</param>
        /// <param name="isChecked">Boolean to set whether the item is checked or not</param>
        public async Task UpdateItemCheckedAsync(Guid itemId, bool isChecked)
        {
            await using AppDbContext context = await _contextFactory.CreateDbContextAsync();
            ShoppingListItem? item = await context.ShoppingListItems.FindAsync(itemId);
            if (item is not null)
            {
                item.IsChecked = isChecked;
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Updates the order numbers for all items in the shopping list whenever they are reordered
        /// </summary>
        /// <param name="reorderedItems">The reordered list of items</param>
        /// <returns>The full list of items with updated orders</returns>
        public async Task ReorderItemsAsync(List<ShoppingListItem> reorderedItems)
        {
            await using AppDbContext context = await _contextFactory.CreateDbContextAsync();

            for (int i = 0; i < reorderedItems.Count; i++)
            {
                ShoppingListItem? item = await context.ShoppingListItems.FindAsync(reorderedItems[i].Id);
                if (item is not null)
                {
                    item.Order = i;
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
