using Microsoft.EntityFrameworkCore;
using Plannit.Data;
using Plannit.Enums;
using Plannit.Models;

namespace Plannit.Services
{
    /// <summary>
    /// Service for Meal Plan database operations
    /// </summary>
    public class MealPlanService(IDbContextFactory<AppDbContext> contextFactory)
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory = contextFactory;


        #region Get Methods
        /// <summary>
        /// Retrieve all Meal Plans
        /// </summary>
        /// <returns>A list of MealPlans</returns>
        public async Task<List<MealPlan>> GetAllMealPlansAsync()
        {
            await using AppDbContext context = await _contextFactory.CreateDbContextAsync();
            return await context.MealPlans
                .OrderByDescending(mp => mp.IsActive)
                .ThenByDescending(mp => mp.WeekStarting)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieve the active meal plan
        /// </summary>
        /// <returns>The first active meal plan found</returns>
        public async Task<MealPlan?> GetActiveMealPlanAsync()
        {
            await using AppDbContext context = await _contextFactory.CreateDbContextAsync();
            return await context.MealPlans
                .Include(mp => mp.Entries)
                .ThenInclude(e => e.Recipe)
                .FirstOrDefaultAsync(mp => mp.IsActive);
        }

        /// <summary>
        /// Retrieve a single meal plan based on a given Guid id
        /// </summary>
        /// <param name="id">The id of the meal plan</param>
        /// <returns>The first meal plan found matching the given id</returns>
        public async Task<MealPlan?> GetMealPlanByIdAsync(Guid id)
        {
            await using AppDbContext context = await _contextFactory.CreateDbContextAsync();
            return await context.MealPlans
                .Include(mp => mp.Entries)
                .ThenInclude(e => e.Recipe)
                .FirstOrDefaultAsync(mp => mp.Id == id);
        }

        /// <summary>
        /// Retrieve the active meal plan and ingredients details for shopping list generation
        /// </summary>
        /// <returns>The first active meal plan found</returns>
        public async Task<MealPlan?> GetActiveMealPlanWithIngredientsAsync()
        {
            await using AppDbContext context = await _contextFactory.CreateDbContextAsync();
            return await context.MealPlans
                .Include(mp => mp.Entries)
                .ThenInclude(e => e.Recipe)
                .ThenInclude(r => r.Ingredients)
                .FirstOrDefaultAsync(mp => mp.IsActive);
        }

        #endregion

        #region Command methods
        /// <summary>
        /// Adds a new meal plan to the database
        /// </summary>
        /// <param name="mealPlan">The meal plan to save</param>
        public async Task CreateMealPlanAsync(MealPlan mealPlan)
        {
            await using AppDbContext context = await _contextFactory.CreateDbContextAsync();
            context.MealPlans.Add(mealPlan);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Update data on an already existing meal plan
        /// </summary>
        /// <param name="mealPlan">The meal plan entity to update</param>
        public async Task UpdateMealPlanAsync(MealPlan mealPlan)
        {
            await using AppDbContext context = await _contextFactory.CreateDbContextAsync();
            context.MealPlans.Update(mealPlan);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Delete a meal plan from the database
        /// </summary>
        /// <param name="id">The Guid id of the meal plan to delete</param>
        public async Task DeleteMealPlanAsync(Guid id)
        {
            await using AppDbContext context = await _contextFactory.CreateDbContextAsync();
            MealPlan? mealPlan = await context.MealPlans.FindAsync(id);
            if (mealPlan is not null)
            {
                context.MealPlans.Remove(mealPlan);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Set the given meal plan as active, and set all others to inactive
        /// </summary>
        /// <param name="id">The meal plan id to set active</param>
        public async Task SetActivePlanAsync(Guid id)
        {
            await using AppDbContext context = await _contextFactory.CreateDbContextAsync();
            List<MealPlan> allPlans = await context.MealPlans.ToListAsync();
            foreach (MealPlan plan in allPlans)
            {
                plan.IsActive = plan.Id == id;
            }
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Add a recipe to a slot in a meal plan
        /// </summary>
        /// <param name="mealPlanId">The meal plan id to add to</param>
        /// <param name="day">The day for the entry</param>
        /// <param name="mealSlot">The meal slot for the entry</param>
        /// <param name="recipeId">The recipe id to add</param>
        public async Task AddEntryAsync(Guid mealPlanId, int day, MealSlot mealSlot, Guid recipeId)
        {
            await using AppDbContext context = await _contextFactory.CreateDbContextAsync();
            MealPlanEntry entry = new MealPlanEntry
            {
                MealPlanId = mealPlanId,
                Day = day,
                MealSlot = mealSlot,
                RecipeId = recipeId
            };
            context.MealPlanEntries.Add(entry);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Remove an entry from a meal plan
        /// </summary>
        /// <param name="entryId">The entry guid id to remove</param>
        public async Task RemoveEntryAsync(Guid entryId)
        {
            await using AppDbContext context = await _contextFactory.CreateDbContextAsync();
            MealPlanEntry? entry = await context.MealPlanEntries.FindAsync(entryId);
            if (entry is not null)
            {
                context.MealPlanEntries.Remove(entry);
                await context.SaveChangesAsync();
            }
        }
        #endregion
    }
}
